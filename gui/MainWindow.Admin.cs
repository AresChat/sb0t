using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace gui
{
    partial class MainWindow
    {
        private List<Command> commands = new List<Command>();

        private void AdminCommandSetup()
        {
            listView1.ItemsSource = commands;
            ICommandDefault[] list = this.server.DefaultCommandLevels;

            foreach (ICommandDefault c in this.server.DefaultCommandLevels)
            {
                ILevel l = CommandManager.GetLevel(c.Name);

                if (l == (ILevel)255)
                {
                    l = c.Level;
                    CommandManager.SetLevel(c.Name, c.Level);
                }

                commands.Add(new Command { Level = CommandManager.LevelToString(l), Name = c.Name });
            }
        }        
    }
}
