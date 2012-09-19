using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core.Extensions
{
    class ExHashlink : IHashlink
    {
        public IHashlinkRoom Decrypt(String hashlink)
        {
            return Hashlink.DecodeHashlink(hashlink);
        }

        public String Encrypt(IHashlinkRoom room)
        {
            return Hashlink.EncodeHashlink(room);
        }
    }
}
