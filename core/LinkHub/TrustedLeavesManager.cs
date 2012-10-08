using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace core.LinkHub
{
    public class TrustedLeavesManager
    {
        private static List<TrustedLeafItem> items { get; set; }
        private static String DataPath { get; set; }

        public static void Init()
        {
            items = new List<TrustedLeafItem>();

            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Dat";

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\trusted.dat";

            if (!File.Exists(DataPath))
                CreateDatabase();

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("select * from trusted", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        items.Add(new TrustedLeafItem
                        {
                            Name = (String)reader["name"],
                            Guid = new Guid((String)reader["guid"])
                        });
            }
        }

        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DataPath);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"create table trusted
                                 (
                                     name text not null,
                                     guid text not null
                                 )";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    command.ExecuteNonQuery();
            }
        }

        public static bool AddItem(TrustedLeafItem item)
        {
            if (IsTrusted(item))
                return false;

            items.Add(item);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"insert into trusted (name, guid) values (@name, @guid)";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add(new SQLiteParameter("@name", item.Name));
                    command.Parameters.Add(new SQLiteParameter("@guid", item.Guid.ToString()));
                    command.ExecuteNonQuery();
                }
            }

            return true;
        }

        public static void RemoveItem(TrustedLeafItem item)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"delete from trusted where name=@name and guid=@guid";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add(new SQLiteParameter("@name", item.Name));
                    command.Parameters.Add(new SQLiteParameter("@guid", item.Guid.ToString()));
                    command.ExecuteNonQuery();
                }
            }

            items.RemoveAll(x => x.Guid.Equals(item.Guid) && x.Name == item.Name);
        }

        public static TrustedLeafItem[] Items
        {
            get { return items.ToArray(); }
        }

        public static bool IsTrusted(TrustedLeafItem item)
        {
            return items.Find(x => x.Guid.Equals(item.Guid) && x.Name == item.Name) != null;
        }
    }
}
