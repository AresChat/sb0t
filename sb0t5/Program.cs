using System;
using core;

namespace sb0t5
{
    class Program
    {
        static ServerCore server;
   
        static void Main(string[] args)
        {
            Console.Title = "sb0t v.5 console";
            Console.ForegroundColor = ConsoleColor.Green;

            server = new ServerCore();

            ServerCore.LogUpdate += LogUpdate;

            if (!server.Open(new ServerCredentials
            {
                Name = "sb0t5 test room",
                Topic = "welcome to my room",
                Port = 54321,
                Bot = "sb0t"
            }))
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        static void LogUpdate(object sender, ServerLogEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Message))
                Console.WriteLine(DateTime.Now + " log: " + e.Message);

            if (e.Error != null)
                Console.WriteLine(DateTime.Now + " error: " + e.Error.Message + "\n" + e.Error.StackTrace);
        }
    }
}
