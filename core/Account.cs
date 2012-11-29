using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core
{
    class Account : IPassword
    {
        public String Name { get; set; }
        public ILevel Level { get; set; }
        public Guid Guid { get; set; }
        public bool Owner { get; set; }
        public byte[] Password { get; set; }
    }
}
