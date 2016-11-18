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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using iconnect;

namespace scripting
{
    class LiveScript
    {
        private static ConcurrentQueue<LiveScriptItem> items = new ConcurrentQueue<LiveScriptItem>();

        public static void Reset()
        {
            items = new ConcurrentQueue<LiveScriptItem>();
        }

        public static void CheckTasks()
        {
            while (items.Count > 0)
            {
                LiveScriptItem i;

                if (items.TryDequeue(out i))
                {
                    switch (i.Type)
                    {
                        case ListScriptReceiveType.List:
                            ListScripts(i.Target, i.Result);
                            break;

                        case ListScriptReceiveType.Failed:
                            Server.Print("unable to download from live script: " + i.Args);
                            break;

                        case ListScriptReceiveType.ReadyLoad:
                            Server.Print("successfully downloaded from live script: " + i.Args);
                            ScriptManager.Load(i.Args, true);
                            break;
                    }
                }
                else break;
            }
        }

        private static void ListScripts(IUser target, String result)
        {
            if (target != null)
            {
                String[] lines = result.Split(new String[] { "\r\n" }, StringSplitOptions.None);

                foreach (String str in lines)
                    target.Print(str);
            }
        }

        public static void ListScripts(IUser target)
        {
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    WebRequest request = WebRequest.Create("http://chatrooms.marsproject.net/livescript/callisto.aspx?action=list");

                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        List<byte> receiver = new List<byte>();
                        byte[] buf = new byte[1024];
                        int size = 0;

                        while ((size = stream.Read(buf, 0, 1024)) > 0)
                            receiver.AddRange(buf.Take(size));

                        buf = receiver.ToArray();
                        items.Enqueue(new LiveScriptItem
                        {
                            Target = target,
                            Type = ListScriptReceiveType.List,
                            Result = Encoding.Default.GetString(buf)
                        });
                    }
                }
                catch { }
            })).Start();
        }

        public static void Download(String filename)
        {
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    WebRequest request = WebRequest.Create("http://chatrooms.marsproject.net/livescript/callisto.aspx?action=files&name=" + filename);

                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        List<byte> receiver = new List<byte>();
                        byte[] buf = new byte[1024];
                        int size = 0;

                        while ((size = stream.Read(buf, 0, 1024)) > 0)
                            receiver.AddRange(buf.Take(size));

                        buf = receiver.ToArray();
                        String received = Encoding.Default.GetString(buf);

                        if (received.IndexOf(":") == -1)
                            items.Enqueue(new LiveScriptItem
                            {
                                Type = ListScriptReceiveType.Failed,
                                Args = filename
                            });
                        else
                        {
                            String[] files = received.Split(new String[] { "\r\n" }, StringSplitOptions.None);
                            String path = Path.Combine(Server.DataPath, filename);

                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            foreach (String f in files)
                            {
                                String[] args = f.Split(new String[] { ":" }, StringSplitOptions.None);
                                String content = GetFile(filename, args[1]);

                                if (args[0] == "script")
                                    File.WriteAllText(Path.Combine(path, args[1]), content);
                                else
                                {
                                    if (!Directory.Exists(Path.Combine(path, "data")))
                                        Directory.CreateDirectory(Path.Combine(path, "data"));

                                    File.WriteAllText(Path.Combine(path, "data", args[1]), content);
                                }
                            }

                            items.Enqueue(new LiveScriptItem
                            {
                                Type = ListScriptReceiveType.ReadyLoad,
                                Args = filename
                            });
                        }
                    }
                }
                catch
                {
                    items.Enqueue(new LiveScriptItem
                    {
                        Type = ListScriptReceiveType.Failed,
                        Args = filename
                    });
                }
            })).Start();
        }

        private static String GetFile(String script, String filename)
        {
            String result = null;

            try
            {
                WebRequest request = WebRequest.Create("http://chatrooms.marsproject.net/livescript/callisto.aspx?action=download&script=" + script + "&name=" + filename);

                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    List<byte> receiver = new List<byte>();
                    byte[] buf = new byte[1024];
                    int size = 0;

                    while ((size = stream.Read(buf, 0, 1024)) > 0)
                        receiver.AddRange(buf.Take(size));

                    buf = receiver.ToArray();
                    result = Encoding.UTF8.GetString(buf);
                }
            }
            catch { }

            return result;
        }
    }

    class LiveScriptItem
    {
        public IUser Target { get; set; }
        public ListScriptReceiveType Type { get; set; }
        public String Result { get; set; }
        public String Args { get; set; }
    }

    enum ListScriptReceiveType
    {
        ReadyLoad,
        Failed,
        List
    }
}
