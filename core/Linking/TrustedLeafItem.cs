using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Linking
{
    public class TrustedLeafItem
    {
        public String Name { get; set; }
        public Guid Guid { get; set; }

        public override string ToString()
        {
            if (this.Name != null)
                return this.Name;

            return base.ToString();
        }
    }
}
