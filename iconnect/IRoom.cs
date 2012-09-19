using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Chatroom Properties</summary>
    public interface IRoom
    {
        /// <summary>Get or set the global custom names setting</summary>
        bool CustomNamesEnabled { get; set; }
        /// <summary>Get the chatroom name</summary>
        String Name { get; }
        /// <summary>Get or set the chatroom main topic</summary>
        String Topic { get; set; }
        /// <summary>Get the chatroom external IP address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Get the chatroom local IP address</summary>
        IPAddress LocalIP { get; }
        /// <summary>Get the chatroom data port</summary>
        ushort Port { get; }
        /// <summary>Get the chatroom hashlink</summary>
        String Hashlink { get; }
        /// <summary>Get the chatroom running status</summary>
        bool IsRunning { get; }
        /// <summary>Get the chatroom preferred langauge code</summary>
        byte Language { get; }
        /// <summary>Update the main URL tag</summary>
        void UpdateURL(String address, String text);
        /// <summary>Clear the main URL tag</summary>
        void ClearURL();
        /// <summary>Get the chatroom bot name</summary>
        String BotName { get; }
    }
}
