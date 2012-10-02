using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using iconnect;

namespace commands
{
    public partial class ServerEvents
    {
        public ServerEvents(IHostApp cb)
        {
            Server.SetCallback(cb);
        }

        public ICommandDefault[] DefaultCommandLevels
        {
            get
            {
                List<CommandItem> list = new List<CommandItem>();

                foreach (MethodInfo m in typeof(Eval).GetMethods())
                {
                    CommandLevel c = (CommandLevel)Attribute.GetCustomAttribute(m, typeof(CommandLevel), false);

                    if (c != null)
                        if (list.Find(x => x.Name == c.Name) == null)
                            list.Add(new CommandItem
                            {
                                Level = c.Level,
                                Name = c.Name
                            });
                }

                return list.ToArray();
            }
        }

        public byte[] Icon
        {
            get { return null; }
        }

        public UserControl GUI
        {
            get { return null; }
        }
    }
}
