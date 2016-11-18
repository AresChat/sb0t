/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
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
        private IPAddress SocketAddr { get; set; }
        public String UserName { get; set; }

        public Socket Sock { get; set; }
        public bool Dead { get; set; }

        public WebWorker(AresClient client)
        {
            this.Sock = client.Sock;
            this.SocketAddr = ((IPEndPoint)this.Sock.RemoteEndPoint).Address;
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
                byte[] rec_buf = buf.Take(size).ToArray();
                this.socket_health = 0;
                this.data_in.AddRange(rec_buf);
                //UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.NoSuch(x, Encoding.UTF8.GetString(rec_buf))), x => x.LoggedIn);
               /* if (this.data_in.Count > 65535) // malicious
                {
                    this.Disconnect(); // kick for now, maybe black list?
                    this.Dead = true;
                    return;
                }*/
            }

            if (this.data_in.Count > 0)
            {
                if (this.current_item.GotHeader)
                {
                    if (this.data_in.Count >= this.current_item.ContentLength)
                    {
                        this.current_item.PostData = Encoding.UTF8.GetString(this.data_in.GetRange(0, this.current_item.ContentLength).ToArray());
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

                        if (line.ToUpper().StartsWith("GET /") || line.ToUpper().StartsWith("HEAD /"))
                        {
                            if (line.ToUpper().StartsWith("GET /"))
                            {
                                this.current_item.FileName = line.Substring(5);
                                this.current_item.RequestType = HttpRequestType.Get;
                            }
                            else if (line.ToUpper().StartsWith("POST /"))
                            {
                                this.current_item.FileName = line.Substring(6);
                                this.current_item.RequestType = HttpRequestType.Post;
                            }
                            else
                            {
                                this.current_item.FileName = line.Substring(6);
                                this.current_item.RequestType = HttpRequestType.Head;
                            }

                            if (this.current_item.FileName.IndexOf(" ") > -1)
                                this.current_item.FileName = this.current_item.FileName.Substring(0, this.current_item.FileName.IndexOf(" "));

                            if (this.current_item.FileName.IndexOf("?") > -1)
                            {
                                this.current_item.QueryString = new QueryStringCollection(this.current_item.FileName.Substring(this.current_item.FileName.IndexOf("?") + 1));                                
                                this.current_item.FileName = this.current_item.FileName.Substring(0, this.current_item.FileName.IndexOf("?"));

                                if (this.current_item.QueryString["user"] != null)
                                    this.UserName = this.current_item.QueryString["user"];
                            }

                            this.current_item.FileName = Uri.UnescapeDataString(this.current_item.FileName);
                        }
                        else if (line.ToUpper().StartsWith("CONTENT-LENGTH:"))
                        {
                            line = line.Substring(15).Trim();
                            int.TryParse(line, out this.current_item.ContentLength);
                        }
                        else if (line.ToUpper().StartsWith("COOKIE:"))
                        {
                            line = line.Substring(7).Trim();
                            this.current_item.Cookie = line;
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
                if (filename == "favicon.ico")
                {
                    byte[] content = Resource1.fi;
                    String mime = this.GetMIME(filename.ToLower());
                    byte[] header = this.BuildHeader(mime, content.Length, false);
                    this.data_out.Add(header);

                    if (this.current_item.RequestType != HttpRequestType.Head)
                        this.data_out.Add(content);
                }
                else if (filename == "sb0t.js")
                {
                    byte[] content = Resource1.sb0t;
                    content = Zip.GCompress(content);
                    String mime = this.GetMIME(filename.ToLower());
                    byte[] header = this.BuildHeader(mime, content.Length, true);
                    this.data_out.Add(header);

                    if (this.current_item.RequestType != HttpRequestType.Head)
                        this.data_out.Add(content);
                }
                else if (filename == "font.htm") // ajax callback from default sbot template
                {
                    this.ParseFont();
                    byte[] content = new byte[] { 79, 75 };
                    content = Zip.GCompress(content);
                    String mime = this.GetMIME(filename.ToLower());
                    byte[] header = this.BuildHeader(mime, content.Length, true);
                    this.data_out.Add(header);

                    if (this.current_item.RequestType != HttpRequestType.Head)
                        this.data_out.Add(content);
                }
                else if (File.Exists(Settings.WebPath + filename) && !this.IsBadName(filename))
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

                        bool compress = this.ShouldCompress(filename);

                        if (compress)
                            content = Zip.GCompress(content);

                        String mime = this.GetMIME(filename.ToLower());
                        byte[] header = this.BuildHeader(mime, content.Length, compress);
                        this.data_out.Add(header);

                        if (this.current_item.RequestType != HttpRequestType.Head)
                            this.data_out.Add(content);
                    }
                    catch
                    {
                        this.data_out.Add(this.Build404());
                    }
                }
                else this.data_out.Add(this.Build404());
            }
        }

        private bool ShouldCompress(String filename)
        {
            String f = filename.ToUpper();

            return f.EndsWith(".HTM") ||
                   f.EndsWith(".HTML") ||
                   f.EndsWith(".CSS") ||
                   f.EndsWith(".JS") ||
                   f.EndsWith(".TXT") ||
                   f.EndsWith(".BMP");
        }

        private void ParseFont()
        {
            try
            {
                String _font = this.current_item.QueryString["f"];
                String _name = this.current_item.QueryString["n"];

                if (!String.IsNullOrEmpty(_font) && !String.IsNullOrEmpty(_name))
                {
                    String[] split = _font.Split(new String[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length >= 2)
                    {
                        String font_family = split[0];
                        String font_color = split[1];
                        String name = Encoding.UTF8.GetString(Convert.FromBase64String(_name));
                        name = Uri.UnescapeDataString(name);
                        AresClient target = UserPool.AUsers.Find(x => x.LoggedIn && x.Name.Equals(name) && x.SocketAddr.Equals(this.SocketAddr));

                        if (target != null)
                        {
                            AresFont f = new AresFont();
                            f.size = 10;
                            f.Enabled = true;
                            f.FontName = font_family;
                            f.TextColor = font_color;

                            if (split.Length >= 3)
                                f.NameColor = split[2];
                            else
                                f.NameColor = "#000000";

                            if (f.FontName.ToUpper().Contains("VERDANA") && f.TextColor.Contains("000000"))
                                f.Enabled = false;

                            if (f.Enabled)
                            {
                                f.oldN = Helpers.HTMLColorToAresColor(f.NameColor);
                                f.oldT = Helpers.HTMLColorToAresColor(f.TextColor);
                                target.Font = f;

                                if (!target.Quarantined)
                                {
                                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomFont(x, target)),
                                        x => x.IsCbot && !x.Quarantined && x.LoggedIn && x.Vroom == target.Vroom && !x.Quarantined);
                                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.FontTo(x, target.Name, f.oldN, f.oldT)),
                                        x => x.LoggedIn && x.Vroom == target.Vroom && !x.Quarantined);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private String[] bad_chars_script = new String[]
        {
            "..",
            "\\"
        };

        private byte[] BuildHeader(String mime, int len, bool compress)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK");
            sb.AppendLine("Date: " + this.BuildTimestamp(DateTime.Now.ToUniversalTime()));
            sb.AppendLine("Server: sb0t style server");
            sb.AppendLine("Connection: keep-alive");
            sb.AppendLine("Accept-Ranges: bytes");
            sb.AppendLine("Content-Length: " + len);

            if (compress)
                sb.AppendLine("Content-Encoding: gzip");

            sb.AppendLine("Content-Type: " + mime);

            if (this.current_item != null)
                if (this.current_item.Cookie != null)
                    sb.AppendLine("Set-Cookie: " + this.current_item.Cookie);

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
            sb.Append(months[d.Month - 1] + " ");
            sb.Append(d.Year + " ");
            sb.Append((d.Hour > 10 ? d.Hour.ToString() : ("0" + d.Hour)) + ":");
            sb.Append((d.Minute > 10 ? d.Minute.ToString() : ("0" + d.Minute)) + ":");
            sb.Append((d.Second > 10 ? d.Second.ToString() : ("0" + d.Second)) + " GMT");

            return sb.ToString();
        }

        private String GetMIME(String path)
        {
            if (path.EndsWith(".gif")) return "image/gif";
            if (path.EndsWith(".jpg")) return "image/jpg";
            if (path.EndsWith(".bmp")) return "image/bmp";
            if (path.EndsWith(".png")) return "image/png";
            if (path.EndsWith(".ico")) return "image/ico";
            if (path.EndsWith(".html")) return "text/html; charset=utf-8";
            if (path.EndsWith(".htm")) return "text/html; charset=utf-8";
            if (path.EndsWith(".txt")) return "text/plain; charset=utf-8";
            if (path.EndsWith(".css")) return "text/css";
            if (path.EndsWith(".js")) return "text/javascript; charset=utf-8";
            if (path.EndsWith(".wav")) return "audio/vnd.wave";
            if (path.EndsWith(".mp3")) return "audio/mpeg";
            if (path.EndsWith(".ogg")) return "video/ogg";
            if (path.EndsWith(".oga")) return "audio/ogg";
            if (path.EndsWith(".mpg") || path.EndsWith(".mpeg")) return "video/mpeg";
            if (path.EndsWith(".mp4") || path.EndsWith(".m4v")) return "video/mp4";
            if (path.EndsWith(".m4a")) return "audio/mp4";
            if (path.EndsWith(".swf")) return "application/x-shockwave-flash";

            return "application/octet-stream";
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
        public HttpRequestType RequestType { get; set; }
        public String PostData { get; set; }
        public String Cookie { get; set; }

        public WWItem()
        {
            this.ContentLength = 0;
            this.GotHeader = false;
            this.FileName = null;
            this.QueryString = new QueryStringCollection();
            this.RequestType = HttpRequestType.Get;
            this.PostData = null;
            this.Cookie = null;
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

        public QueryStringCollection()
        {
            this.list = new List<QueryStringItem>();
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

    enum HttpRequestType
    {
        Head,
        Post,
        Get
    }
}
