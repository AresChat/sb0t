using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core
{
    class UserLinkCredentials : ILink
    {
        public bool IsLinked { get; set; }
        public uint Ident { get; set; }
        public bool Visible { get; set; }
    }
}
