using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Custom Font</summary>
    public interface IFont
    {
        /// <summary>Get or set font name</summary>
        String Family { get; set; }
        /// <summary>Get or set font size</summary>
        byte Size { get; set; }
        /// <summary>Get or set name color</summary>
        byte NameColor { get; set; }
        /// <summary>Get or set text color</summary>
        byte TextColor { get; set; }
        /// <summary>Get or set name color (new protocol)</summary>
        byte[] NameColorNew { get; set; }
        /// <summary>Get or set text color (new protocol)</summary>
        byte[] TextColorNew { get; set; }
        /// <summary>Get or set font active status</summary>
        bool HasFont { get; set; }
    }
}
