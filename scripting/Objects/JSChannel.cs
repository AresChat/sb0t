using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Objects
{
    class JSChannel : ObjectInstance
    {
        public IChannelItem Item { get; set; }

        public JSChannel(ObjectInstance prototype, IChannelItem item)
            : base(prototype)
        {
            this.Item = item;
            this.PopulateFunctions();
        }

        [JSProperty(Name = "hashlink")]
        public String Hashlink
        {
            get
            {
                Hashlink obj = new Hashlink
                {
                    IP = this.Item.IP,
                    Name = this.Item.Name,
                    Port = this.Item.Port
                };

                return "arlnk://" + Server.Hashlinks.Encrypt(obj);
            }
            set { }
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.Item.Name; }
            set { }
        }

        [JSProperty(Name = "topic")]
        public String Topic
        {
            get { return this.Item.Topic; }
            set { }
        }

        [JSProperty(Name = "version")]
        public String Version
        {
            get { return this.Item.Version; }
            set { }
        }

        [JSProperty(Name = "userCount")]
        public int Users
        {
            get { return this.Item.Users; }
            set { }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.Item.Port; }
            set { }
        }

        [JSProperty(Name = "externalIp")]
        public String IP
        {
            get { return this.Item.IP.ToString(); }
            set { }
        }

        [JSProperty(Name = "language")]
        public int Language
        {
            get { return this.Item.Language; }
            set { }
        }

        public override string ToString()
        {
            return "[object Channel]";
        }
    }
}
