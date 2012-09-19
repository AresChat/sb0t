using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Hashlink decoder / encoder</summary>
    public interface IHashlink
    {
        /// <summary>Decrypt a hashlink</summary>
        IHashlinkRoom Decrypt(String hashlink);
        /// <summary>Encrypt a hashlink</summary>
        String Encrypt(IHashlinkRoom room);
    }
}
