using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace core
{
    class FilterImporter
    {
        public static void DoTasks()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\FILTER IMPORTER";

            if (Directory.Exists(path))
            {
                List<String> list = new List<String>();
                bool ff = false, jf = false, wf = false, pf = false;

                try
                {
                    String pathTo = Path.Combine(path, "joinfilters.xml");

                    if (File.Exists(pathTo))
                    {
                        list = new List<String>();

                        using (FileStream f = new FileStream(pathTo, FileMode.Open))
                        using (StreamReader reader = new StreamReader(f))
                        using (XmlReader xml = XmlReader.Create(reader))
                        {
                            xml.MoveToContent();
                            xml.ReadSubtree().ReadToFollowing("joinfilters");

                            while (xml.ReadToFollowing("joinfilter"))
                            {
                                xml.ReadSubtree().ReadToFollowing("trigger");
                                String trigger = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                                xml.ReadToFollowing("type");
                                String type = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                                xml.ReadToFollowing("args");
                                String args = xml.ReadElementContentAsString();

                                if (!String.IsNullOrEmpty(args))
                                {
                                    args = Encoding.UTF8.GetString(Convert.FromBase64String(args));
                                    list.Add(trigger + ", " + type + ", " + args);
                                }
                                else list.Add(trigger + ", " + type);

                                if (list.Count > 0)
                                    core.Events.ImportJoinFilters(list.ToArray());
                            }
                        }

                        File.Delete(pathTo);
                        jf = true;
                    }
                }
                catch { }

                list = new List<String>();

                try
                {
                    String pathTo = Path.Combine(path, "pmfilters.xml");

                    if (File.Exists(pathTo))
                    {
                        using (FileStream f = new FileStream(pathTo, FileMode.Open))
                        using (StreamReader reader = new StreamReader(f))
                        using (XmlReader xml = XmlReader.Create(reader))
                        {
                            xml.MoveToContent();
                            xml.ReadSubtree().ReadToFollowing("pmfilters");

                            while (xml.ReadToFollowing("pmfilter"))
                            {
                                xml.ReadSubtree().ReadToFollowing("trigger");
                                String trigger = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                                xml.ReadToFollowing("type");
                                String type = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                                xml.ReadToFollowing("args");
                                String args = xml.ReadElementContentAsString();

                                if (!String.IsNullOrEmpty(args))
                                {
                                    args = Encoding.UTF8.GetString(Convert.FromBase64String(args));
                                    args = args.Replace("\0", "\r\n");
                                    list.Add(trigger + ", " + type + ", " + args);
                                }
                                else list.Add(trigger + ", " + type);
                            }
                        }

                        File.Delete(pathTo);
                        pf = true;
                    }
                }
                catch { }

                try
                {
                    String pathTo = Path.Combine(path, "wordfilters.xml");

                    if (File.Exists(pathTo))
                    {
                        using (FileStream f = new FileStream(pathTo, FileMode.Open))
                        using (StreamReader reader = new StreamReader(f))
                        using (XmlReader xml = XmlReader.Create(reader))
                        {
                            xml.MoveToContent();
                            xml.ReadSubtree().ReadToFollowing("wordfilters");

                            while (xml.ReadToFollowing("wordfilter"))
                            {
                                xml.ReadSubtree().ReadToFollowing("trigger");
                                String trigger = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                                xml.ReadToFollowing("type");
                                String type = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                                xml.ReadToFollowing("args");
                                String args = xml.ReadElementContentAsString();

                                if (!String.IsNullOrEmpty(args))
                                {
                                    args = Encoding.UTF8.GetString(Convert.FromBase64String(args));
                                    args = args.Replace("\0", "\r\n");
                                    list.Add(trigger + ", " + type + ", " + args);
                                }
                                else list.Add(trigger + ", " + type);

                                if (list.Count > 0)
                                    core.Events.ImportWordFilters(list.ToArray());
                            }
                        }

                        File.Delete(pathTo);
                        wf = true;
                    }
                }
                catch { }

                try
                {
                    String pathTo = Path.Combine(path, "filefilters.xml");

                    if (File.Exists(pathTo))
                    {
                        list = new List<String>();

                        using (FileStream f = new FileStream(pathTo, FileMode.Open))
                        using (StreamReader reader = new StreamReader(f))
                        using (XmlReader xml = XmlReader.Create(reader))
                        {
                            xml.MoveToContent();
                            xml.ReadSubtree().ReadToFollowing("filefilters");

                            while (xml.ReadToFollowing("filefilter"))
                            {
                                xml.ReadSubtree().ReadToFollowing("trigger");
                                String trigger = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                                xml.ReadToFollowing("type");
                                String type = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                                xml.ReadToFollowing("args");
                                String args = xml.ReadElementContentAsString();

                                if (!String.IsNullOrEmpty(args))
                                {
                                    args = Encoding.UTF8.GetString(Convert.FromBase64String(args));
                                    list.Add(trigger + ", " + type + ", " + args);
                                }
                                else list.Add(trigger + ", " + type);

                                if (list.Count > 0)
                                    core.Events.ImportFileFilters(list.ToArray());
                            }
                        }

                        File.Delete(pathTo);
                        ff = true;
                    }
                }
                catch { }

                if (ff || jf || pf || wf)
                {
                    list = new List<String>();
                    String pathTo = Path.Combine(path, "LOG.TXT");

                    if (ff)
                        list.Add("Successfully imported file filters!");

                    if (jf)
                        list.Add("Successfully imported join filters!");

                    if (wf)
                        list.Add("Successfully imported word filters!");

                    if (pf)
                        list.Add("Successfully imported pm filters!");

                    try
                    {
                        File.WriteAllLines(pathTo, list.ToArray());
                    }
                    catch { }
                }
            }
        }
    }
}
