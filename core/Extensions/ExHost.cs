using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using iconnect;

namespace core.Extensions
{
    class ExHost : IHostApp
    {
        public ExHost(String datapath)
        {
            this.Users = new ExUsers();
            this.Room = new ExRoom();
            this.Stats = new ExStats();
            this.Compression = new ExCompression();
            this.Hashlinks = new ExHashlink();
            this.Scripting = new ExScripting();
            this.Spelling = new ExSpelling();
            this.Channels = new ExChannels();

            this.DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName;

            if (!String.IsNullOrEmpty(datapath))
                this.DataPath += "\\Extensions\\" + datapath;

            if (!Directory.Exists(this.DataPath))
                Directory.CreateDirectory(this.DataPath);

            this.DataPath += "\\";
        }

        public IPool Users { get; private set; }
        public IRoom Room { get; private set; }
        public IStats Stats { get; private set; }
        public ICompression Compression { get; private set; }
        public IHashlink Hashlinks { get; private set; }
        public String DataPath { get; private set; }
        public IHub Hub { get { return ServerCore.Linker; } }
        public IScripting Scripting { get; private set; }
        public ISpell Spelling { get; private set; }
        public IChannels Channels { get; private set; }

        public void ClearBans()
        {
            BanSystem.ClearBans();
        }

        public void WriteLog(String text)
        {
            ServerCore.Log(text);
        }

        public void WriteChatLog(String text)
        {
            ChatLog.WriteLine(text);
        }

        public uint Timestamp
        {
            get { return Helpers.UnixTime; }
        }

        public ulong Ticks
        {
            get { return Time.Now; }
        }

        public ILevel GetLevel(String command)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\commands");
            object value = key.GetValue(command);
            key.Close();
            return (ILevel)(byte)(int)value;
        }

        public void PublicToTarget(IUser client, String sender, String text)
        {
            if (client is AresClient)
            {
                AresClient obj = (AresClient)client;
                obj.SendPacket(TCPOutbound.Public(obj, sender, text));
            }
            else if (client is ib0t.ib0tClient)
            {
                ib0t.ib0tClient obj = (ib0t.ib0tClient)client;
                obj.QueuePacket(ib0t.WebOutbound.PublicTo(obj, sender, text));
            }
            else if (client is LinkLeaf.LinkUser)
            {
                LinkLeaf.LinkUser obj = (LinkLeaf.LinkUser)client;

                if (ServerCore.Linker.IsLinked)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPublicTextToUser(ServerCore.Linker,
                        obj.LinkCredentials.Ident, obj.Name, sender, text));
            }
        }

        public void EmoteToTarget(IUser client, String sender, String text)
        {
            if (client is AresClient)
            {
                AresClient obj = (AresClient)client;
                obj.SendPacket(TCPOutbound.Emote(obj, sender, text));
            }
            else if (client is ib0t.ib0tClient)
            {
                ib0t.ib0tClient obj = (ib0t.ib0tClient)client;
                obj.QueuePacket(ib0t.WebOutbound.EmoteTo(obj, sender, text));
            }
            else if (client is LinkLeaf.LinkUser)
            {
                LinkLeaf.LinkUser obj = (LinkLeaf.LinkUser)client;

                if (ServerCore.Linker.IsLinked)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafEmoteTextToUser(ServerCore.Linker,
                        obj.LinkCredentials.Ident, obj.Name, sender, text));
            }
        }
    }
}
