using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace core
{
    class CaptchaManager
    {
        private static List<CaptchaRecord> list { get; set; }
        private static String DataPath { get; set; }

        public static bool HasCaptcha(IClient client)
        {
            return list.Find(x => x.Guid.Equals(client.Guid)) != null;
        }

        public static void AddCaptcha(IClient client)
        {
            CaptchaRecord record = new CaptchaRecord
            {
                Guid = client.Guid,
                Time = (int)(uint)(Helpers.UnixTime - 1000000000)
            };

            list.Add(record);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"insert into captchas (guid, time) values (@guid, @time)";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add(new SQLiteParameter("@guid", record.Guid.ToString()));
                    command.Parameters.Add(new SQLiteParameter("@time", record.Time));
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void Load()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Dat";

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\captcha.dat";

            if (!File.Exists(DataPath))
                CreateDatabase();

            list = new List<CaptchaRecord>();

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("delete from captchas where time<@time", connection))
                {
                    int time = (int)(uint)(Helpers.UnixTime - 1000000000);
                    time -= 1209600;
                    command.Parameters.Add(new SQLiteParameter("@time", time));
                    command.ExecuteNonQuery();
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("select * from captchas", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        list.Add(new CaptchaRecord
                        {
                            Guid = new Guid((String)reader["guid"]),
                            Time = (int)reader["time"]
                        });
            }
        }

        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DataPath);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"create table captchas
                                 (
                                     guid text not null,
                                     time int not null
                                 )";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    command.ExecuteNonQuery();
            }
        }
    }

    class CaptchaRecord
    {
        public Guid Guid { get; set; }
        public int Time { get; set; }
    }
}
