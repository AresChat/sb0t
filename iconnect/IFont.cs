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
        bool Enabled { get; }
        /// <summary>Custom Font Style</summary>
        String NameColor { get; }
        /// <summary>Custom Font Style</summary>
        String TextColor { get; }
        /// <summary>Custom Font Style</summary>
        String FontName { get; }
    }
}
