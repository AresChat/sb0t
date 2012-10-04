using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Private Message</summary>
    public interface IPrivateMsg
    {
        /// <summary>Check if PM contains specific text</summary>
        bool Contains(String text);
        /// <summary>Replace text in PM with new text</summary>
        void Replace(String oldText, String newText);
        /// <summary>Remove specific text from PM</summary>
        void Remove(String text);
        /// <summary>Prevent PM from being sent</summary>
        bool Cancel { get; set; }
    }
}
