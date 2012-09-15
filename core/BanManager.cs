using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Net;

namespace core
{
    class BanManager
    {
        private static List<Ban> list;
        private static String DataPath { get; set; }

        public static bool IsBanned(IClient client)
        {
            return list.Find(x => x.ExternalIP.Equals(client.ExternalIP) ||
                x.Guid.Equals(client.Guid)) == null;
        }

        public static void AddBan(IClient client)
        {
            Ban ban = new Ban
            {
                Name = client.Name,
                Version = client.Version,
                Guid = client.Guid,
                ExternalIP = client.ExternalIP,
                LocalIP = client.LocalIP,
                Port = client.DataPort
            };

            list.Add(ban);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"insert into bans (name, version, guid, externalip, localip, port) 
                                     values (@name, @version, @guid, @externalip, @localip, @port)";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add(new SQLiteParameter("@name", client.Name));
                    command.Parameters.Add(new SQLiteParameter("@version", client.Version));
                    command.Parameters.Add(new SQLiteParameter("@guid", client.Guid.ToString()));
                    command.Parameters.Add(new SQLiteParameter("@externalip", client.ExternalIP.ToString()));
                    command.Parameters.Add(new SQLiteParameter("@localip", client.LocalIP.ToString()));
                    command.Parameters.Add(new SQLiteParameter("@port", (int)client.DataPort));
                    command.ExecuteNonQuery();
                }
            }
        }

        public static String RemoveBan(String name)
        {
            int index = list.FindIndex(x => x.Name.StartsWith(name));
            String result = null;

            if (index > -1)
            {
                result = list[index].Name;
                list.RemoveAt(index);

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
                {
                    connection.Open();

                    String query = @"delete from bans
                                     where name=@name
                                     limit 1";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@name", result));
                        command.ExecuteNonQuery();
                    }
                }
            }

            return result;
        }

        public static void LoadBans()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Dat";

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\bans.dat";

            if (!File.Exists(DataPath))
                CreateDatabase();

            list = new List<Ban>();

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("select * from bans", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        list.Add(new Ban
                        {
                            Name = (String)reader["name"],
                            Version = (String)reader["version"],
                            Guid = new Guid((String)reader["guid"]),
                            ExternalIP = IPAddress.Parse((String)reader["externalip"]),
                            LocalIP = IPAddress.Parse((String)reader["localip"]),
                            Port = (ushort)(int)reader["port"]
                        });
            }
        }

        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DataPath);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"create table bans
                                 (
                                     name text not null,
                                     version text not null,
                                     guid text not null,
                                     externalip text not null,
                                     localip text not null,
                                     port int not null
                                 )";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    command.ExecuteNonQuery();
            }
        }
    }
}
