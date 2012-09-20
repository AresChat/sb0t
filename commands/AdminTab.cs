using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    /// <summary>
    /// Allow this command to feature on the Admin GUI Tab
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class CommandLevel : Attribute
    {
        public ILevel Level { get; private set; }
        public String Name { get; private set; }

        /// <summary>
        /// Allow this command to feature on the Admin GUI Tab
        /// </summary>
        /// <param name="name">Command Name</param>
        /// <param name="level">Default Level for this command if it hasn't been user assigned</param>
        public CommandLevel(String name, ILevel level)
        {
            this.Name = name;
            this.Level = level;
        }
    }

    class CommandItem : ICommandDefault
    {
        public String Name { get; set; }
        public ILevel Level { get; set; }
    }
}
