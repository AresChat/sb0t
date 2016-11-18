/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
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
            TimerList.Reset();
            LiveScript.Reset();
            ErrorDispatcher.Reset();
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                    }
                }

                ScriptManager.DequeueCallbacks();
                TimerList.UpdateTimers();
                LiveScript.CheckTasks();
            }
        }

        public bool CanScribble(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    try
                    {
                        Objects.JSUser u = new Objects.JSUser(s.JS.Object.InstancePrototype, client, s.ScriptName);
                        bool result = s.JS.CallGlobalFunction<bool>("onScribbleCheck", u);

                        if (!result)
                            return false;
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }
                }
            }

            return true;
        }

        public bool Joining(IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    try
                    {
                        Objects.JSUser u = new Objects.JSUser(s.JS.Object.InstancePrototype, client, s.ScriptName);
                        bool result = s.JS.CallGlobalFunction<bool>("onJoinCheck", u);

                        if (!result)
                            return false;
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                ScriptManager.Scripts.ForEach(x =>
                {
                    if (!client.Link.IsLinked)
                    {
                        x.local_users.RemoveAll(z => z.Name == client.Name);
                        x.local_users.Add(new Objects.JSUser(x.JS.Object.InstancePrototype, client, x.ScriptName));
                    }
                    else
                        x.leaves.ForEach(z =>
                        {
                            if (z.Ident == client.Link.Ident)
                            {
                                z.users.RemoveAll(y => y.Name == client.Name);
                                z.users.Add(new Objects.JSUser(x.JS.Object.InstancePrototype, client, x.ScriptName));
                            }
                        });
                });

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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                    Objects.JSUser u = new Objects.JSUser(s.JS.Object.InstancePrototype, client, s.ScriptName);

                    if (u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onRejected", u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }

                ScriptManager.Scripts.ForEach(x =>
                {
                    if (!client.Link.IsLinked)
                        x.local_users.RemoveAll(z => z.Name == client.Name);
                    else
                        x.leaves.ForEach(z =>
                        {
                            if (z.Ident == client.Link.Ident)
                                z.users.RemoveAll(y => y.Name == client.Name);
                        });
                });
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                Objects.JSUser r = ScriptManager.Scripts[0].GetUser(client);

                if (Server.Scripting.ScriptInRoom)
                    if (Server.CanScript(client))
                    {
                        if (text.StartsWith("@"))
                        {
                            String js = "userobj = user(" + client.ID + "); null; " + text.Substring(1);

                            try
                            {
                                ScriptManager.Scripts[0].JS.Evaluate(js);
                            }
                            catch { }

                            return String.Empty;
                        }
                        else
                        {
                            String js = "userobj = user(" + client.ID + "); null; " + text;

                            try
                            {
                                object eval = ScriptManager.Scripts[0].JS.Evaluate(js);

                                if (eval is bool)
                                    js = eval.ToString().ToLower();
                                else
                                    js = eval.ToString();

                                if (js != "undefined")
                                    Server.Print(js);
                            }
                            catch (Jurassic.JavaScriptException e)
                            {
                                ErrorDispatcher.SendError(ScriptManager.Scripts[0].ScriptName, e.Message, e.LineNumber);
                            }
                            catch { }
                        }
                    }

                foreach (JSScript s in scripts)
                {
                    Objects.JSUser u = s.GetUser(client);

                    if (u != null)
                        try
                        {
                            result = s.JS.CallGlobalFunction<String>("onTextBefore", u, result);

                            if (String.IsNullOrEmpty(result))
                                return String.Empty;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                        try
                        {
                            result = s.JS.CallGlobalFunction<String>("onEmoteBefore", u, result);

                            if (String.IsNullOrEmpty(result))
                                return String.Empty;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                    Objects.JSUser t = s.GetUser(target);

                    if (u != null && t != null)
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
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
                    Objects.JSUser t = s.GetUser(target);

                    if (u != null && t != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onPM", u, t);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                if (Server.CanScript(client))
                {
                    client.Print("/loadscript <script>");
                    client.Print("/killscript <script>");
                    client.Print("/listscripts");
                    client.Print("/downloadscript <script>");
                    client.Print("/livescripts");
                    client.Print("/errors <on | off>");
                }

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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                    Objects.JSUser t = s.GetUser(target);

                    if (u != null && t != null)
                        try
                        {
                            bool result = s.JS.CallGlobalFunction<bool>("onIgnoring", u, t);

                            if (!result)
                                return false;
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                    Objects.JSUser t = s.GetUser(target);

                    if (u != null && t != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onIgnoredStateChanged", u, t, ignored);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void InvalidRegistration(IUser client) { }

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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void CaptchaSending(IUser client) { }

        public void CaptchaReply(IUser client, String reply) { }

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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void BansAutoCleared()
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    try
                    {
                        s.JS.CallGlobalFunction("onBansAutoCleared");
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }
                }
            }
        }

        public void Command(IUser client, String cmd, IUser target, String args)
        {
            if (this.CanScript)
            {
                if (Server.CanScript(client))
                {
                    if (cmd == "livescripts")
                    {
                        LiveScript.ListScripts(client);
                        return;
                    }

                    if (cmd.StartsWith("downloadscript "))
                    {
                        LiveScript.Download(cmd.Substring(15));
                        return;
                    }

                    if (cmd.StartsWith("loadscript "))
                    {
                        ScriptManager.Load(cmd.Substring(11), true);
                        return;
                    }

                    if (cmd.StartsWith("killscript "))
                    {
                        ScriptManager.KillScript(cmd.Substring(11));
                        return;
                    }

                    if (cmd == "listscripts")
                    {
                        ScriptManager.Scripts.ForEach(x =>
                        {
                            if (x.ScriptName != "room")
                                client.Print(x.ScriptName);
                        });

                        return;
                    }

                    if (cmd == "errors on")
                    {
                        ErrorDispatcher.AddErrors(client);
                        return;
                    }

                    if (cmd == "errors off")
                    {
                        ErrorDispatcher.RemoveErrors(client);
                        return;
                    }
                }

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
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
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
                        ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }
                }
            }
        }

        public void Linked()
        {
            if (this.CanScript)
            {
                ScriptManager.Scripts.ForEach(x =>
                {
                    x.leaves.ForEach(z => z.users.Clear());
                    x.leaves.Clear();
                });

                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    try
                    {
                        s.JS.CallGlobalFunction("onLinked");
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }
                }
            }
        }

        public void Unlinked()
        {
            if (this.CanScript)
            {
                ScriptManager.Scripts.ForEach(x =>
                {
                    x.leaves.ForEach(z => z.users.Clear());
                    x.leaves.Clear();
                });

                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    try
                    {
                        s.JS.CallGlobalFunction("onUnlinked");
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }
                }
            }
        }

        public void LeafJoined(ILeaf leaf)
        {
            if (this.CanScript)
            {
                ScriptManager.Scripts.ForEach(x =>
                {
                    x.leaves.FindAll(z => z.Ident == leaf.Ident).ForEach(z => z.users.Clear());
                    x.leaves.RemoveAll(z => z.Ident == leaf.Ident);
                    x.leaves.Add(new Objects.JSLeaf(x.JS.Object.InstancePrototype, leaf, x.ScriptName));
                });

                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSLeaf l = s.leaves.Find(x => x.Ident == leaf.Ident);

                    if (l != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onLeafJoin", l);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }

        public void LeafParted(ILeaf leaf)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSLeaf l = s.leaves.Find(x => x.Ident == leaf.Ident);

                    if (l != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onLeafPart", l);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }

                ScriptManager.Scripts.ForEach(x =>
                {
                    x.leaves.FindAll(z => z.Ident == leaf.Ident).ForEach(z => z.users.Clear());
                    x.leaves.RemoveAll(z => z.Ident == leaf.Ident);
                });
            }
        }

        public void LinkedAdminDisabled(ILeaf leaf, IUser client)
        {
            if (this.CanScript)
            {
                JSScript[] scripts = ScriptManager.Scripts.ToArray();

                foreach (JSScript s in scripts)
                {
                    Objects.JSLeaf l = s.leaves.Find(x => x.Ident == leaf.Ident);
                    Objects.JSUser u = s.GetUser(client);

                    if (l != null && u != null)
                        try
                        {
                            s.JS.CallGlobalFunction("onLinkedAdminDisabled", l, u);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(s.ScriptName, e.Message, e.LineNumber);
                        }
                        catch { }
                }
            }
        }
    }
}
