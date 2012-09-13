using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace core
{
    class AccountManager
    {
        private static Random rnd;
        private static List<Account> list;
        private static String DataPath { get; set; }

        public static void LoadPasswords()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Accounts";

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\accounts.dat";

            if (!File.Exists(DataPath))
                CreateDatabase();

            list = new List<Account>();

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("select * from accounts", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        list.Add(new Account
                        {
                            Name = (String)reader["name"],
                            Level = (Level)(byte)(int)reader["level"],
                            Guid = new Guid((String)reader["guid"]),
                            Password = (byte[])reader["password"]
                        });
            }
        }

        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DataPath);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"create table accounts
                                 (
                                     name text not null,
                                     level int not null,
                                     guid text not null,
                                     password blob not null
                                 )";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    command.ExecuteNonQuery();
            }
        }

        public static uint NextCookie
        {
            get
            {
                if (rnd == null)
                    rnd = new Random();

                return (uint)Math.Floor(rnd.NextDouble() * (uint.MaxValue - 1));
            }
        }

        public static void SecureLogin(IClient client, byte[] password)
        {
            // option to not match guids
            List<IPAddress> addresses = new List<IPAddress>();
            byte[] ext_ip = Settings.Get<byte[]>("ip");
            addresses.Add(IPAddress.Loopback);
            addresses.Add(Settings.LocalIP);

            if (ext_ip != null)
                addresses.Add(new IPAddress(ext_ip));

            using (SHA1 sha1 = SHA1.Create())
            {
                String owner = Settings.Get<String>("owner");

                if (!String.IsNullOrEmpty(owner))
                    foreach (IPAddress ip in addresses)
                    {
                        byte[] pwd = sha1.ComputeHash(Encoding.UTF8.GetBytes(owner));
                        pwd = sha1.ComputeHash(SecurePassword(pwd, client.Cookie, ip));

                        if (pwd.SequenceEqual(password))
                        {
                            client.Registered = true;
                            client.Captcha = true;
                            client.Owner = true;
                            Events.LoginGranted(client);
                            client.Level = Level.Host;
                            ServerCore.Log(client.Name + " logged in with the room owner password");
                            return;
                        }
                    }

                var linq = from x in list
                           where x.Guid.Equals(client.Guid)
                           select x;

                foreach (Account a in linq)
                    foreach (IPAddress ip in addresses)
                    {
                        byte[] pwd = sha1.ComputeHash(SecurePassword(a.Password, client.Cookie, ip));

                        if (pwd.SequenceEqual(password))
                        {
                            client.Registered = true;
                            client.Captcha = true;
                            Events.LoginGranted(client);
                            client.Level = a.Level;
                            ServerCore.Log(client.Name + " logged in with " + a.Name + "'s password");
                            return;
                        }
                    }
            }
        }

        private static byte[] SecurePassword(byte[] password, uint cookie, IPAddress ip)
        {
            List<byte> buffer = new List<byte>();
            buffer.AddRange(BitConverter.GetBytes(cookie));
            buffer.AddRange(ip.GetAddressBytes());
            buffer.AddRange(password);
            return buffer.ToArray();
        }

        public static void Login(IClient client, String password)
        {
            // option to not match guids
            String owner = Settings.Get<String>("owner");

            if (!String.IsNullOrEmpty(owner))
                if (password == owner)
                {
                    client.Registered = true;
                    client.Captcha = true;
                    client.Owner = true;
                    Events.LoginGranted(client);
                    client.Level = Level.Host;
                    ServerCore.Log(client.Name + " logged in with the room owner password");
                    return;
                }

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] pwd = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                Account a = list.Find(x => x.Password.SequenceEqual(pwd) && x.Guid.Equals(client.Guid));

                if (a == null)
                    Events.InvalidLoginAttempt(client);
                else
                {
                    client.Registered = true;
                    client.Captcha = true;
                    Events.LoginGranted(client);
                    client.Level = a.Level;
                    ServerCore.Log(client.Name + " logged in with " + a.Name + "'s password");
                    return;
                }
            }

            Events.InvalidLoginAttempt(client);
        }

        public static void Register(IClient client, String password)
        {
            if (password.Length < 3)
                return;

            if (Events.Registering(client))
            {
                list.RemoveAll(x => x.Guid.Equals(client.Guid));

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand("delete from accounts where guid=@guid", connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@guid", client.Guid.ToString()));
                        command.ExecuteNonQuery();
                    }
                }

                if (client.Level != Level.Regular)
                    client.Level = Level.Regular;

                byte[] pwd;

                using (SHA1 sha1 = SHA1.Create())
                {
                    pwd = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));

                    list.Add(new Account
                    {
                        Guid = client.Guid,
                        Level = Level.Regular,
                        Name = client.Name,
                        Owner = false,
                        Password = pwd
                    });
                }

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
                {
                    connection.Open();

                    String query = @"insert into accounts (name, level, guid, password) 
                                     values (@name, @level, @guid, @password)";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@name", client.Name));
                        command.Parameters.Add(new SQLiteParameter("@level", (int)(byte)client.Level));
                        command.Parameters.Add(new SQLiteParameter("@guid", client.Guid.ToString()));
                        command.Parameters.Add(new SQLiteParameter("@password", pwd));
                        command.ExecuteNonQuery();
                    }
                }

                Events.Registered(client);
                client.Registered = true;
                Events.LoginGranted(client);
                ServerCore.Log(client.Name + " has registered");
            }
        }

        public static void Unregister(IClient client)
        {
            if (!client.Registered || client.Owner)
                return;

            list.RemoveAll(x => x.Guid.Equals(client.Guid));
            client.Level = Level.Regular;
            client.Registered = false;

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"delete from accounts
                                 where guid=@guid";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add(new SQLiteParameter("@level", (int)(byte)client.Level));
                    command.Parameters.Add(new SQLiteParameter("@guid", client.Guid.ToString()));
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateAccount(IClient client)
        {
            if (!client.Registered)
                return;

            Account a = list.Find(x => x.Guid.Equals(client.Guid));

            if (a != null)
            {
                a.Level = client.Level;

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
                {
                    connection.Open();

                    String query = @"update accounts set level=@level
                                     where guid=@guid";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@level", (int)(byte)client.Level));
                        command.Parameters.Add(new SQLiteParameter("@guid", client.Guid.ToString()));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}
