using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace gui
{
    public class Command : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
          DependencyProperty.Register("Name", typeof(String),
         typeof(Command), new UIPropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty LevelProperty =
           DependencyProperty.Register("Level", typeof(String),
          typeof(Command), new UIPropertyMetadata(null));

        public string Level
        {
            get { return (string)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public String[] Options
        {
            get
            {
                return new String[]
                {
                    "Regular",
                    "Moderator",
                    "Administrator",
                    "Host",
                    "Disabled"
                };
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!MainWindow.SETTING_UP)
                CommandManager.UpdateCommand(this.Name, this.Level);

            base.OnPropertyChanged(e);
        }
    }
}
