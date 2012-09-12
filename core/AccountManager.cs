using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;

namespace core
{
    class AccountManager
    {
        private static Random rnd;
        private static List<Account> list;

        public static void LoadPasswords()
        {
            list = new List<Account>();
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
                            client.Captcha = a.Captcha;
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
                    client.Captcha = a.Captcha;
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
            if (Events.Registering(client))
            {
                if (list.RemoveAll(x => x.Guid.Equals(client.Guid)) > 0)
                    client.Level = Level.Regular;

                using (SHA1 sha1 = SHA1.Create())
                    list.Add(new Account
                    {
                        Captcha = client.Captcha,
                        Guid = client.Guid,
                        Level = Level.Regular,
                        Name = client.Name,
                        Owner = false,
                        Password = sha1.ComputeHash(Encoding.UTF8.GetBytes(password))
                    });

                SaveAccounts();
                Events.Registered(client);
                client.Registered = true;
                Events.LoginGranted(client);
                ServerCore.Log(client.Name + " has registered");
            }
        }

        public static void UpdateAccount(IClient client)
        {
            Account a = list.Find(x => x.Guid.Equals(client.Guid));

            if (a != null)
            {
                a.Captcha = client.Captcha;
                a.Level = client.Level;
                SaveAccounts();
            }
        }

        private static void SaveAccounts()
        {

        }

    }
}
