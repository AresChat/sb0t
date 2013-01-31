using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Custom Font Style</summary>
    public interface IFont
    {
        /// <summary>Custom Font Style</summary>
        bool Enabled { get; set; }
        /// <summary>Custom Font Style</summary>
        String NameColor { get; set; }
        /// <summary>Custom Font Style</summary>
        String TextColor { get; set; }
        /// <summary>Custom Font Style</summary>
        String FontName { get; set; }
        /// <summary>Custom Font Style</summary>
        int Size { get; set; }
    }
}
