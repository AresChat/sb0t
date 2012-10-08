using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.LinkHub
{
    class LeafPool
    {
        public static List<Leaf> Leaves { get; set; }

        private static uint NextID { get; set; }

        public static uint NextIdent
        {
            get { return NextID++; }
        }

        public static void Build()
        {
            NextID = 0;
            Leaves = new List<Leaf>();
        }

        public static void Destroy()
        {

        }
    }
}
