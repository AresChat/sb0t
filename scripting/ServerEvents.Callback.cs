using System.Windows.Controls;
using System.Windows.Media.Imaging;
using iconnect;

namespace scripting
{
    public partial class ServerEvents
    {
        public ServerEvents(IHostApp cb)
        {
            Server.SetCallback(cb);
        }

        public BitmapSource Icon { get { return null; } }
        public UserControl GUI { get { return null; } }
        public void Dispose() { }
        public void Load() { }
        public void UnhandledProtocol(IUser client, bool custom, byte msg, byte[] packet) { }
    }
}
