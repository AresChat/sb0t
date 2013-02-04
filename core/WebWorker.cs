using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace core
{
    class WebWorker
    {
        private List<byte> data_in = new List<byte>();
        private List<byte[]> data_out = new List<byte[]>();
        private WWItem current_item = new WWItem();
        public String UserName { get; set; }

        public Socket Sock { get; set; }
        public bool Dead { get; set; }

        public WebWorker(AresClient client)
        {
            this.Sock = client.Sock;
            this.data_in.AddRange(client.ReceiveDump);
            this.UserName = String.Empty;
        }

        private int socket_health = 0;

        public void DoSocketTasks()
        {
            while (this.data_out.Count > 0)
            {
                try
                {
                    this.Sock.Send(this.data_out[0]);
                    this.data_out.RemoveAt(0);
                }
                catch { break; }
            }

            byte[] buf = new byte[8192];
            int size = 0;
            SocketError se = SocketError.Success;

            try { size = this.Sock.Receive(buf, 0, buf.Length, SocketFlags.None, out se); }
            catch { }

            if (size == 0)
            {
                if (se == SocketError.WouldBlock)
                    this.socket_health = 0;
                else if (this.socket_health++ > 5)
                {
                    this.Disconnect();
                    this.Dead = true;
                    return;
                }
            }
            else
            {
                this.socket_health = 0;
                this.data_in.AddRange(buf.Take(size));
            }

            if (this.data_in.Count > 0)
            {
                if (this.current_item.GotHeader)
                {
                    if (this.data_in.Count >= this.current_item.ContentLength)
                    {
                        this.data_in.RemoveRange(0, this.current_item.ContentLength);
                        this.GetFile(this.current_item.FileName);
                        this.current_item = new WWItem();
                    }
                }
                else
                {
                    while (this.data_in.CanTakeLine())
                    {
                        String line = this.data_in.TakeLine();

                        if (line.ToUpper().StartsWith("GET /"))
                        {
                            this.current_item.FileName = line.Substring(5);

                            if (this.current_item.FileName.IndexOf(" ") > -1)
                                this.current_item.FileName = this.current_item.FileName.Substring(0, this.current_item.FileName.IndexOf(" "));

                            if (this.current_item.FileName.IndexOf("?") > -1)
                            {
                                this.current_item.QueryString = new QueryStringCollection(this.current_item.FileName.Substring(this.current_item.FileName.IndexOf("?") + 1));                                
                                this.current_item.FileName = this.current_item.FileName.Substring(0, this.current_item.FileName.IndexOf("?"));

                                if (this.current_item.QueryString["user"] != null)
                                    this.UserName = this.current_item.QueryString["user"];
                            }
                        }
                        else if (line.ToUpper().StartsWith("CONTENT-LENGTH:"))
                        {
                            line = line.Substring(15).Trim();
                            int.TryParse(line, out this.current_item.ContentLength);
                        }
                        else if (line == String.Empty)
                        {
                            this.current_item.GotHeader = true;

                            if (this.current_item.ContentLength == 0)
                            {
                                this.GetFile(this.current_item.FileName);
                                this.current_item = new WWItem();
                            }

                            break;
                        }
                    }
                }
            }
        }

        private void GetFile(String filename)
        {
            if (filename != null)
            {
                if (File.Exists(Settings.WebPath + filename) && !this.IsBadName(filename))
                {
                    try
                    {
                        byte[] content = File.ReadAllBytes(Settings.WebPath + filename);

                        if (filename == "template.htm")
                        {
                            String html = Encoding.UTF8.GetString(content).Replace("\"sb0t.js\"", "\"sb0t.js?r=" + Helpers.UnixTime + "\"");
                            int index = html.ToUpper().IndexOf("<HEAD>");

                            if (index > -1)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine();
                                sb.AppendLine("<script>");
                                sb.AppendLine("<!--");
                                sb.AppendLine("    var my_username = \"" + this.UserName + "\";");
                                sb.AppendLine("-->");
                                sb.Append("</script>");
                                content = Encoding.UTF8.GetBytes(html.Insert((index + 6), sb.ToString()));
                            }
                        }

                        content = Zip.Compress(content);
                        String mime = this.GetMIME(filename.ToLower());
                        byte[] header = this.BuildHeader(mime, content.Length);
                        this.data_out.Add(header);
                        this.data_out.Add(content);
                    }
                    catch
                    {
                        this.data_out.Add(this.Build404());
                    }
                }
                else
                {
                    if (filename == "sb0t.js") // dynamic
                    {
                        byte[] content = Resource1.sb0t;
                        content = Zip.Compress(content);
                        String mime = this.GetMIME(filename.ToLower());
                        byte[] header = this.BuildHeader(mime, content.Length);
                        this.data_out.Add(header);
                        this.data_out.Add(content);
                    }
                    else if (filename == "font.htm") // dynamic
                    {
                        this.ParseFont();
                        byte[] content = new byte[] { 79, 75 };
                        content = Zip.Compress(content);
                        String mime = this.GetMIME(filename.ToLower());
                        byte[] header = this.BuildHeader(mime, content.Length);
                        this.data_out.Add(header);
                        this.data_out.Add(content);
                    }
                    else this.data_out.Add(this.Build404());
                }
            }
        }

        private void ParseFont()
        {
            String _font = this.current_item.QueryString["f"];
            String _name = this.current_item.QueryString["n"];

            if (!String.IsNullOrEmpty(_font) && !String.IsNullOrEmpty(_name))
            {
                try
                {
                    String[] split = _font.Split(new String[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length >= 2)
                    {
                        String font_family = split[0];
                        String font_color = split[1];
                        String name = Encoding.UTF8.GetString(Convert.FromBase64String(_name));
                        name = Uri.UnescapeDataString(name);
                        IPAddress ip = ((IPEndPoint)this.Sock.RemoteEndPoint).Address;
                        AresClient target = UserPool.AUsers.Find(x => x.LoggedIn && x.Name.Equals(name) && x.SocketAddr.Equals(ip));

                        if (target != null)
                        {
                            target.Font.Enabled = true;
                            target.Font.FontName = font_family;
                            target.Font.TextColor = font_color;

                            if (split.Length >= 3)
                                target.Font.NameColor = split[2];
                        }
                    }
                }
                catch { }
            }
        }

        private String[] bad_chars_script = new String[]
        {
            "..",
            "\\"
        };

        private byte[] BuildHeader(String mime, int len)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK");
            sb.AppendLine("Date: " + this.BuildTimestamp(DateTime.Now.ToUniversalTime()));
            sb.AppendLine("Server: sb0t style server");
            sb.AppendLine("Connection: keep-alive");
            sb.AppendLine("Accept-Ranges: bytes");
            sb.AppendLine("Content-Length: " + len);
            sb.AppendLine("Content-Encoding: gzip");
            sb.AppendLine("Content-Type: " + mime);
            sb.AppendLine("");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] Build404()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 404 Not Found");
            sb.AppendLine("Date: " + this.BuildTimestamp(DateTime.Now.ToUniversalTime()));
            sb.AppendLine("Server: sb0t style server");
            sb.AppendLine("Content-Length: 17");
            sb.AppendLine("Connection: keep-alive");
            sb.AppendLine("Content-Type: text/html; charset=iso-8859-1");
            sb.AppendLine("");
            sb.AppendLine("file not found :(");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private bool IsBadName(String filename)
        {
            int i = this.bad_chars_script.Count<String>(x => filename.Contains(x));
            return i > 0;
        }

        private String BuildTimestamp(DateTime d)
        {
            String[] months = new String[]
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
            };

            StringBuilder sb = new StringBuilder();
            sb.Append(d.DayOfWeek.ToString().Substring(0, 3) + ", ");
            sb.Append(d.Day + " ");
            sb.Append(months[d.Month] + " ");
            sb.Append(d.Year + " ");
            sb.Append((d.Hour > 10 ? d.Hour.ToString() : ("0" + d.Hour)) + ":");
            sb.Append((d.Minute > 10 ? d.Minute.ToString() : ("0" + d.Minute)) + ":");
            sb.Append((d.Second > 10 ? d.Second.ToString() : ("0" + d.Second)) + " GMT");

            return sb.ToString();
        }

        private String GetMIME(String path)
        {
            if (path.EndsWith(".gif"))
                return "image/gif";

            if (path.EndsWith(".jpg"))
                return "image/jpg";

            if (path.EndsWith(".bmp"))
                return "image/bmp";

            if (path.EndsWith(".png"))
                return "image/png";

            if (path.EndsWith(".html"))
                return "text/html; charset=utf-8";

            if (path.EndsWith(".htm"))
                return "text/html; charset=utf-8";

            if (path.EndsWith(".txt"))
                return "text/plain; charset=utf-8";

            if (path.EndsWith(".css"))
                return "text/css";

            if (path.EndsWith(".js"))
                return "text/javascript; charset=utf-8";

            if (path.EndsWith(".wav"))
                return "audio/vnd.wave";

            if (path.EndsWith(".mpeg") || path.EndsWith(".mp3"))
                return "audio/mpeg";

            if (path.EndsWith(".ogg"))
                return "video/ogg";

            if (path.EndsWith(".oga"))
                return "audio/ogg";

            if (path.EndsWith(".mp4") || path.EndsWith(".m4v"))
                return "video/mp4";

            if (path.EndsWith(".m4a"))
                return "audio/mp4";

            return "text/plain; charset=utf-8";
        }

        public void Disconnect()
        {
            try { this.Sock.Disconnect(false); }
            catch { }
            try { this.Sock.Shutdown(SocketShutdown.Both); }
            catch { }
            try { this.Sock.Close(); }
            catch { }
            try { this.Sock.Dispose(); }
            catch { }

            this.Sock = null;
        }
    }

    class WWItem
    {
        public String FileName { get; set; }
        public int ContentLength = 0;
        public bool GotHeader { get; set; }
        public QueryStringCollection QueryString { get; set; }

        public WWItem()
        {
            this.ContentLength = 0;
            this.GotHeader = false;
            this.FileName = null;
            this.QueryString = null;
        }
    }

    class QueryStringCollection
    {
        private List<QueryStringItem> list { get; set; }

        public QueryStringCollection(String raw)
        {
            this.list = new List<QueryStringItem>();
            String[] items = raw.Split(new String[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (String str in items)
            {
                String[] kv = str.Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (kv.Length > 0)
                {
                    String key = Uri.UnescapeDataString(kv[0]).ToUpper();
                    String value = String.Empty;

                    if (kv.Length > 0)
                        value = Uri.UnescapeDataString(String.Join("=", kv.Skip(1).ToArray()));

                    this.list.Add(new QueryStringItem { Key = key, Value = value, Raw = str });
                }
            }
        }

        public String GetRaw(String key)
        {
            return this.list.Find(x => x.Key == key.ToUpper()).Raw;
        }

        public String this[String key]
        {
            get
            {
                QueryStringItem item = this.list.Find(x => x.Key == key.ToUpper());

                if (item != null)
                    return item.Value;

                return null;
            }
        }
    }

    class QueryStringItem
    {
        public String Key { get; set; }
        public String Value { get; set; }
        public String Raw { get; set; }
    }
}
