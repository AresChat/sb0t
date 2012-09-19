using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iconnect;

namespace core.Extensions
{
    class ExHost : IHostApp
    {
        public ExHost(String name)
        {
            this.Users = new ExUsers();
            this.Room = new ExRoom();
            this.Stats = new ExStats();
            this.Compression = new ExCompression();
            this.Hashlinks = new ExHashlink();

            this.DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName;

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
    }
}
