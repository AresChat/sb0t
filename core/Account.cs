using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class Account
    {
        public String Name { get; set; }
        public Level Level { get; set; }
        public Guid Guid { get; set; }
        public bool Captcha { get; set; }
        public bool Owner { get; set; }
        public byte[] Password { get; set; }
    }
}
