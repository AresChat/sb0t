using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class JoinFilter
    {
        public static bool IsPreFiltered(IUser client)
        {
            return false;
        }
    }
}
