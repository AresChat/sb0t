using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Net;
using System.Security.Cryptography;

namespace core
{
    class ObSalt
    {
        private static String DataPath { get; set; }
        private static List<Item> list { get; set; }

        private class Item
        {
            public IPAddress Address { get; set; }
            public byte[] Salt { get; set; }
            public uint LastUsed { get; set; }
        }

        public static void Init()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Dat";

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\salt.dat";

            if (!File.Exists(DataPath))
                CreateDatabase();

            list = new List<Item>();
            uint time = Helpers.UnixTime;

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("delete from salt where s < @s", connection))
                {
                    int offset = (int)time;
                    offset -= 1209600;
                    command.Parameters.Add(new SQLiteParameter("@s", offset));
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand("select * from salt", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        String ip = (String)reader["ip"];
                        String s = (String)reader["s"];
                        Item item = new Item();
                        item.Address = IPAddress.Parse(ip);
                        item.Salt = new Guid(s).ToByteArray();
                        item.LastUsed = (uint)(int)reader["lu"];
                        list.Add(item);
                    }
            }
        }

        public static void GetSalt(IClient client)
        {
            uint time = Helpers.UnixTime;
            Item item = list.Find(x => x.Address.Equals(client.ExternalIP));

            if (item != null)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand("update salt set lu=@lu where ip=@ip", connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@lu", (int)time));
                        command.Parameters.Add(new SQLiteParameter("@ip", client.ExternalIP.ToString()));
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                item = new Item();
                item.Address = client.ExternalIP;
                item.Salt = Guid.NewGuid().ToByteArray();
                item.LastUsed = time;
                list.Add(item);

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand("insert into salt (ip, s, lu) values (@ip, @s, @lu)", connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@ip", item.Address.ToString()));
                        command.Parameters.Add(new SQLiteParameter("@s", new Guid(item.Salt).ToString()));
                        command.Parameters.Add(new SQLiteParameter("@lu", (int)item.LastUsed));
                        command.ExecuteNonQuery();
                    }
                }
            }

            byte[] addr = client.ExternalIP.GetAddressBytes();
            byte[] salt = addr.Concat(item.Salt).ToArray();

            int excess = client.Encryption.Mode == EncryptionMode.Encrypted ? 4 : 2;
            int h = 19, l = 0;

            using (SHA1 sha1 = SHA1.Create())
                salt = sha1.ComputeHash(salt);

            for (int i = 0; i < excess; i++)
                addr[addr.Length - (i + 1)] = salt[i % 2 == 0 ? h-- : l++];

            client.ExternalIP = new IPAddress(addr);
        }

        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DataPath);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"create table salt
                                 (
                                     ip text not null,
                                     s text not null,
                                     lu int not null
                                 )";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    command.ExecuteNonQuery();
            }
        }
    }
}
