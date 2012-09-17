using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gui
{
    partial class MainWindow
    {
        private List<Command> commands = new List<Command>();

        private void AdminCommandSetup()
        {
            listView1.ItemsSource = commands;

            for (int i = 0; i < 100; i++)
                commands.Add(new Command { Level = "Moderator", Name = "testing" });
        }
    }
}
