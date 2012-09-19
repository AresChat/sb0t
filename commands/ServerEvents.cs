using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    public class ServerEvents : IExtension
    {
        public ServerEvents(IHostApp callback) { Server.SetCallback(callback); }

        public void ServerStarted() { }

        public void CycleTick() { }

        public void UnhandledProtocol(IUser client, bool custom, byte msg, byte[] packet) { }

        public bool Joining(IUser client) { return true; }

        public void Joined(IUser client)
        {
            
        }

        public void Rejected(IUser client, RejectedMsg msg) { }

        public void Parting(IUser client) { }

        public void Parted(IUser client) { }

        public bool AvatarReceived(IUser client) { return true; }

        public bool PersonalMessageReceived(IUser client, String text) { return true; }

        public void TextReceived(IUser client, String text) { }

        public String TextSending(IUser client, String text) { return text; }

        public void TextSent(IUser client, String text) { }

        public void EmoteReceived(IUser client, String text) { }

        public String EmoteSending(IUser client, String text) { return text; }

        public void EmoteSent(IUser client, String text) { }

        public void PrivateSending(IUser client, IUser target, IPrivateMsg msg) { }

        public void PrivateSent(IUser client, IUser target) { }

        public void BotPrivateSent(IUser client, String text) { }

        public void Command(IUser client, String command, IUser target, String args) { }

        public bool Nick(IUser client, String name) { return true; }

        public void Help(IUser client) { }

        public void FileReceived(IUser client, String filename, String title, MimeType type) { }

        public bool Ignoring(IUser client, IUser target) { return true; }

        public void IgnoredStateChanged(IUser client, IUser target, bool ignored) { }

        public void InvalidLoginAttempt(IUser client) { }

        public void LoginGranted(IUser client) { }

        public void AdminLevelChanged(IUser client) { }

        public bool Registering(IUser client) { return true; }

        public void Registered(IUser client) { }

        public void Unregistered(IUser client) { }

        public void CaptchaSending(IUser client) { }

        public void CaptchaReply(IUser client, String reply) { }

        public bool VroomChanging(IUser client, ushort vroom) { return true; }

        public void VroomChanged(IUser client) { }

        public bool Flooding(IUser client, byte msg) { return true; } // to do

        public void Flooded(IUser client) { } // to do

        public bool ProxyDetected(IUser client) { return true; } // to do
    }
}
