using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace testext
{
    class Server
    {
        private static IHostApp Callback { get; set; }

        /// <summary>
        /// Path to data folder
        /// </summary>
        public static String DataPath
        {
            get { return Callback.DataPath; }
        }

        public static void SetCallback(IHostApp cb)
        {
            Callback = cb;
        }

        /// <summary>
        /// Access to the link hub
        /// </summary>
        public static IHub Link
        {
            get { return Callback.Hub; }
        }

        /// <summary>
        /// Access to the user pools
        /// </summary>
        public static IPool Users
        {
            get { return Callback.Users; }
        }

        /// <summary>
        /// Hashlink encode/decoder
        /// </summary>
        public static IHashlink Hashlinks
        {
            get { return Callback.Hashlinks; }
        }

        /// <summary>
        /// Compression utility
        /// </summary>
        public static ICompression Compression
        {
            get { return Callback.Compression; }
        }

        /// <summary>
        /// Server statistics
        /// </summary>
        public static IStats Stats
        {
            get { return Callback.Stats; }
        }

        /// <summary>
        /// Chatroom properties
        /// </summary>
        public static IRoom Chatroom
        {
            get { return Callback.Room; }
        }

        /// <summary>
        /// Send an announcement to all users in all user pools
        /// </summary>
        /// <param name="text"></param>
        public static void Print(String text)
        {
            Callback.Users.All(x => x.Print(text));
        }

        /// <summary>
        /// Send an announcement to specific users
        /// </summary>
        /// <param name="text"></param>
        /// <param name="predicate"></param>
        public static void Print(String text, Predicate<IUser> predicate)
        {
            Callback.Users.All(x =>
            {
                if (predicate(x))
                    x.Print(text);
            });
        }

        /// <summary>
        /// Write to the debug log
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLog(String text)
        {
            Callback.WriteLog(text);
        }

        /// <summary>
        /// Get current timestamp
        /// </summary>
        public static uint Time
        {
            get { return Callback.Timestamp; }
        }

        /// <summary>
        /// Get a user defined minimum admin level for a default command
        /// </summary>
        /// <param name="command">command name</param>
        /// <returns></returns>
        public static ILevel GetLevel(String command)
        {
            return Callback.GetLevel(command);
        }

        /// <summary>Clear the ban list</summary>
        public static void ClearBans()
        {
            Callback.ClearBans();
        }
    }
}
