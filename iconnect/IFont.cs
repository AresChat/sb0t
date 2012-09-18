using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    public interface IFont
    {
        String Family { get; set; }
        byte Size { get; set; }
        byte NameColor { get; set; }
        byte TextColor { get; set; }
        byte[] NameColorNew { get; set; }
        byte[] TextColorNew { get; set; }
        bool HasFont { get; set; }
    }
}
