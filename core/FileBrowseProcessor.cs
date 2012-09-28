using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core
{
    class FileBrowseProcessor
    {
        public static void AddShare(AresClient client, TCPPacketReader packet)
        {
            if (client.SharedFiles.Count > 12000)
                throw new Exception("addshare max limit exceeded");

            SharedFile file = new SharedFile();
            file.Mime = (MimeType)(byte)packet;
            file.Size = packet;
            ushort len = packet;
            packet.SkipBytes(len);
            file.Data = packet;
            PopulateMetaData(file);
            client.SharedFiles.Add(file);
            Events.FileReceived(client, file);
        }

        public static void RemShare(AresClient client, TCPPacketReader packet)
        {
            uint size = packet;
            client.SharedFiles.RemoveAll(x => x.Size == size);
        }

        public static void Browse(AresClient client, TCPPacketReader packet)
        {
            ushort ident = packet;
            byte mime = packet;
            String name = packet.ReadString(client);
            AresClient target = UserPool.AUsers.Find(x => x.Name == name);

            if (target == null)
                client.SendPacket(TCPOutbound.BrowseError(ident));
            else if (target.SharedFiles.Count == 0)
                client.SendPacket(TCPOutbound.BrowseError(ident));
            else
            {
                var linq = from x in target.SharedFiles
                           where (mime == 0 || mime == 255) || (byte)x.Mime == (mime == 8 ? 0 : mime)
                           select TCPOutbound.BrowseItem(ident, x);

                client.SendPacket(TCPOutbound.StartOfBrowse(ident, (ushort)linq.Count()));

                List<byte> buffer = new List<byte>();

                foreach (byte[] x in linq)
                {
                    buffer.AddRange(x);

                    if (buffer.Count > 1024)
                    {
                        client.SendPacket(TCPOutbound.ClientCompressed(buffer.ToArray()));
                        buffer.Clear();
                    }
                }

                if (buffer.Count > 0)
                    client.SendPacket(TCPOutbound.ClientCompressed(buffer.ToArray()));

                client.SendPacket(TCPOutbound.EndOfBrowse(ident));
            }
        }

        public static void Search(AresClient client, TCPPacketReader packet)
        {
            ushort ident = packet;
            packet.SkipByte();
            byte mime = packet;
            ushort len = packet;
            List<byte[]> items = new List<byte[]>();

            List<String> search_words = new List<String>(
                Encoding.UTF8.GetString(packet.ReadBytes(len)).ToUpper().Replace("\0",
                " ").Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries)
            );

            UserPool.AUsers.ForEachWhere(x =>
            {
                x.SharedFiles.ForEachWhere(y =>
                {
                    String[] words = y.FileName.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    words = words.Concat(y.Title.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                    
                    foreach (String str in words)
                        if (search_words.Contains(str.ToUpper()))
                        {
                            items.Add(TCPOutbound.SearchHit(client, ident, x, y));
                            break;
                        }
                },
                y => (mime == 0 || mime == 255) || (byte)y.Mime == (mime == 8 ? 0 : mime));
            },
            x => x.SocketConnected && x.Browsable);

            List<byte> buffer = new List<byte>();

            foreach (byte[] x in items)
            {
                buffer.AddRange(x);

                if (buffer.Count > 1024)
                {
                    client.SendPacket(TCPOutbound.ClientCompressed(buffer.ToArray()));
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
                client.SendPacket(TCPOutbound.ClientCompressed(buffer.ToArray()));

            client.SendPacket(TCPOutbound.EndOfSearch(ident));
        }

        private static void PopulateMetaData(SharedFile file)
        {
            TCPPacketReader packet = new TCPPacketReader(file.Data);
            packet.SkipBytes(16);

            if (file.Mime == MimeType.ARES_MIME_MP3)
                packet.SkipBytes(4);
            else if (file.Mime == MimeType.ARES_MIME_VIDEO)
                packet.SkipBytes(6);
            else if (file.Mime == MimeType.ARES_MIME_IMAGE)
                packet.SkipBytes(5);

            ushort len = packet;
            ushort counter = 0;
            file.FileName = String.Empty;
            file.Title = String.Empty;

            while (packet.Remaining >= 2)
            {
                byte length = packet;
                byte type = packet;

                if (length > packet.Remaining)
                    throw new Exception();

                switch (type)
                {
                    case 1:
                        file.Title = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;
                    case 15:
                        file.FileName = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    default:
                        packet.SkipBytes(length);
                        break;
                }

                counter += 2;
                counter += length;

                if (counter >= len)
                    break;
            }
        }
    }
}
