using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    public interface IExtension
    {
        void ServerStarted();
        void UnhandledProtocol(IUser client, byte msg, byte[] packet);
        bool Joining(IUser client);
        void Joined(IUser client);
        void Rejected(IUser client, RejectedMsg msg);
        void Parting(IUser client);
        void Parted(IUser client);
        bool AvatarReceived(IUser client);
        bool PersonalMessageReceived(IUser client, String text);
        void TextReceived(IUser client, String text);
        String TextSending(IUser client, String text);
        void TextSent(IUser client, String text);
        void EmoteReceived(IUser client, String text);
        String EmoteSending(IUser client, String text);
        void EmoteSent(IUser client, String text);
        void PrivateSending(IUser client, IUser target, IPrivateMsg msg);
        void PrivateSent(IUser client, IUser target);
        void BotPrivateSent(IUser client, String text);
        void Command(IUser client, String command, IUser target, String args);
        bool Nick(IUser client, String name);
        void Help(IUser client);
        void FileReceived(IUser client, String name, MimeType type);
        bool Ignoring(IUser client, IUser target);
        void IgnoredStateChanged(IUser client, IUser target, bool ignored);
        void InvalidLoginAttempt(IUser client);
        void LoginGranted(IUser client);
        void AdminLevelChanged(IUser client);
        bool Registering(IUser client);
        void Registered(IUser client);
        void Unregistered(IUser client);
        void CaptchaSending(IUser client);
        void CaptchaReply(IUser client, String reply);
        bool VroomChanging(IUser client, ushort vroom);
        void VroomChanged(IUser client);
        bool Flooding(IUser client, byte msg);
        void Flooded(IUser client);
    }
}
