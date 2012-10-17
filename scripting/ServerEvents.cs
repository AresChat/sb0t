using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace scripting
{
    public partial class ServerEvents : IExtension
    {
        private bool CanScript { get; set; }

        public void ServerStarted()
        {
            this._second_timer = 0;
            this.CanScript = Server.Scripting.ScriptEnabled;
            ScriptManager.AutoRun();
        }

        private uint _second_timer = 0;
        public void CycleTick()
        {
            if (this.CanScript)
            {
                if (Server.Time > this._second_timer)
                {
                    this._second_timer = Server.Time;
                    JSScript[] scripts = ScriptManager.Scripts.ToArray();

                    foreach (JSScript s in scripts)
                    {
                        try
                        {
                            s.JS.CallGlobalFunction("onTimer");
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                    }
                }

                // other timers and checks here


            }
        }

        public bool Joining(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onJoinCheck", u);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public void Joined(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onJoin", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void Rejected(IUser client, RejectedMsg msg)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onRejected", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void Parting(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onPartBefore", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void Parted(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onPart", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public bool AvatarReceived(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onAvatar", u);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public bool PersonalMessageReceived(IUser client, String text)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onPersonalMessage", u, text);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public void TextReceived(IUser client, String text)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onTextReceived", u, text);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public String TextSending(IUser client, String text)
        {
            String result = text;

            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                    {
                        try
                        {
                            result = s.JS.CallGlobalFunction<String>("onTextBefore", u, result);

                            if (String.IsNullOrEmpty(result))
                                return String.Empty;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                    }
                }
            }

            return result;
        }

        public void TextSent(IUser client, String text)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onTextAfter", u, text);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void EmoteReceived(IUser client, String text)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onEmoteReceived", u, text);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public String EmoteSending(IUser client, String text)
        {
            String result = text;

            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                    {
                        try
                        {
                            result = s.JS.CallGlobalFunction<String>("onEmoteBefore", u, result);

                            if (String.IsNullOrEmpty(result))
                                return String.Empty;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                    }
                }
            }

            return result;
        }

        public void EmoteSent(IUser client, String text)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onEmoteAfter", u, text);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void PrivateSending(IUser client, IUser target, IPrivateMsg msg)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);
                    Objects.JSUser t = s.GetUser(client);

                    if (u != null && t != null)
                    {
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onPMBefore", u, t,
                                new Objects.JSPM(s.JS.Object.InstancePrototype, msg));

                            if (!result)
                            {
                                msg.Cancel = true;
                                return;
                            }
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                    }
                }
            }
        }

        public void PrivateSent(IUser client, IUser target)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);
                    Objects.JSUser t = s.GetUser(client);

                    if (u != null && t != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onPM", u, t);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void BotPrivateSent(IUser client, String text)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onBotPM", u, text);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public bool Nick(IUser client, String name)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onNick", u, name);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public void Help(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onHelp", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void FileReceived(IUser client, String filename, String title, MimeType type)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onFileReceived", u, filename);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public bool Ignoring(IUser client, IUser target)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);
                    Objects.JSUser t = s.GetUser(client);

                    if (u != null && t != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onIgnoring", u, t);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public void IgnoredStateChanged(IUser client, IUser target, bool ignored)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);
                    Objects.JSUser t = s.GetUser(client);

                    if (u != null && t != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onIgnoredStateChanged", u, t, ignored);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void InvalidLoginAttempt(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onInvalidLoginAttempt", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void LoginGranted(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onLoginGranted", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void AdminLevelChanged(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onAdminLevelChanged", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public bool Registering(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onRegistering", u);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public void Registered(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onRegistered", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void Unregistered(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onUnregistered", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void CaptchaSending(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onCaptchaSending", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void CaptchaReply(IUser client, String reply)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onCaptchaReply", u, reply);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public bool VroomChanging(IUser client, ushort vroom)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onVroomJoinCheck", u, (int)vroom);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public void VroomChanged(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onVroomJoin", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public bool Flooding(IUser client, byte msg)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onFloodBefore", u, (int)msg);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public void Flooded(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onFlood", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public bool ProxyDetected(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onProxyDetected", u);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }

            return true;
        }

        public void Logout(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onLogout", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void Idled(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onIdled", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void Unidled(IUser client, uint seconds_away)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onUnidled", u, (int)seconds_away);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void BansAutoCleared()
        {
            if (this.CanScript)
            {
                if (Server.Time > this._second_timer)
                {
                    this._second_timer = Server.Time;
                    JSScript[] scripts = ScriptManager.Scripts.ToArray();

                    foreach (JSScript s in scripts)
                    {
                        try
                        {
                            s.JS.CallGlobalFunction("onBansAutoCleared");
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                    }
                }
            }
        }

        public void Command(IUser client, String cmd, IUser target, String args)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);
                    Objects.JSUser t = s.GetUser(target);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onCommand", u, cmd, t, args);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void LinkError(ILinkError error)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    try
                    {
                        s.JS.CallGlobalFunction("onLinkError", (int)error);
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }
                }
            }
        }

        public void Linked()
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    try
                    {
                        s.JS.CallGlobalFunction("onLinked");
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }
                }
            }
        }

        public void Unlinked()
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    try
                    {
                        s.JS.CallGlobalFunction("onUnlinked");
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        ScriptManager.OnError(s.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }
                }
            }
        }

        public void LeafJoined(ILeaf leaf) { }

        public void LeafParted(ILeaf leaf) { }

        public void LinkedAdminDisabled(ILeaf leaf, IUser client) { }
    }
}
