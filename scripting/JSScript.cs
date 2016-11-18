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
using System.Reflection;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting
{
    class JSScript
    {
        public String ScriptName { get; private set; }
        public ScriptEngine JS { get; private set; }
        public List<Objects.JSUser> local_users = new List<Objects.JSUser>();
        public List<Objects.JSLeaf> leaves = new List<Objects.JSLeaf>();
        public List<ulong> timer_idents = new List<ulong>();

        public JSScript(String name)
        {
            this.ScriptName = name;
            this.JS = new ScriptEngine();
            this.JS.ScriptName = name;

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            // set up global functions
            this.JS.EmbedGlobalClass(typeof(JSGlobal));
            
            //set up static classes
            var statics = types.Where(x => x.Namespace == "scripting.Statics" && x.IsSubclassOf(typeof(ObjectInstance)));
            this.JS.EmbedStatics(statics.ToArray());

            //set up instance classes
            var instances = types.Where(x => x.Namespace == "scripting.Instances" && x.IsSubclassOf(typeof(ClrFunction)));
            this.JS.EmbedInstances(instances.ToArray());

            //set up object prototypes
            var protos = types.Where(x => x.Namespace == "scripting.ObjectPrototypes" && x.IsSubclassOf(typeof(ClrFunction)));
            this.JS.EmbedObjectPrototypes(protos.ToArray());

            // set up default events
            StringBuilder events = new StringBuilder();
            events.AppendLine("function onScribbleCheck(userobj) { return true }");
            events.AppendLine("function onTextReceived(userobj, text) { }");
            events.AppendLine("function onTextBefore(userobj, text) { return text; }");
            events.AppendLine("function onTextAfter(userobj, text) { }");
            events.AppendLine("function onEmoteReceived(userobj, text) { }");
            events.AppendLine("function onEmoteBefore(userobj, text) { return text; }");
            events.AppendLine("function onEmoteAfter(userobj, text) { }");
            events.AppendLine("function onJoinCheck(userobj) { return true; }");
            events.AppendLine("function onJoin(userobj) { }");
            events.AppendLine("function onPartBefore(userobj) { }");
            events.AppendLine("function onPart(userobj) { }");
            events.AppendLine("function onTimer() { }");
            events.AppendLine("function onHelp(userobj) { }");
            events.AppendLine("function onCommand(userobj, command, target, args) { }");
            events.AppendLine("function onAvatar(userobj) { return true; }");
            events.AppendLine("function onPersonalMessage(userobj, msg) { return true; }");
            events.AppendLine("function onRejected(userobj) { }");
            events.AppendLine("function onLoad() { }");
            events.AppendLine("function onVroomJoinCheck(userobj, vroom) { return true; }");
            events.AppendLine("function onVroomJoin(userobj) { }");
            events.AppendLine("function onFileReceived(userobj, filename) { }");
            events.AppendLine("function onFloodBefore(userobj, msg) { return true; }");
            events.AppendLine("function onFlood(userobj) { }");
            events.AppendLine("function onBotPM(userobj, text) { return true; }");
            events.AppendLine("function onPMBefore(userobj, target, pm) { return true; }");
            events.AppendLine("function onPM(userobj, target) { }");
            events.AppendLine("function onNick(userobj, name) { return true; }");
            events.AppendLine("function onIgnoring(userobj, target) { return true; }");
            events.AppendLine("function onIgnoredStateChanged(userobj, target, ignored) { }");
            events.AppendLine("function onInvalidLoginAttempt(userobj) { }");
            events.AppendLine("function onLoginGranted(userobj) { }");
            events.AppendLine("function onAdminLevelChanged(userobj) { }");
            events.AppendLine("function onRegistering(userobj) { return true; }");
            events.AppendLine("function onRegistered(userobj) { }");
            events.AppendLine("function onUnregistered(userobj) { }");
            events.AppendLine("function onProxyDetected(userobj, reply) { return true; }");
            events.AppendLine("function onLogout(userobj) { }");
            events.AppendLine("function onIdled(userobj) { }");
            events.AppendLine("function onUnidled(userobj, seconds) { }");
            events.AppendLine("function onBansAutoCleared() { }");
            events.AppendLine("function onLinkError(msg) { }");
            events.AppendLine("function onLinked() { }");
            events.AppendLine("function onUnlinked() { }");
            events.AppendLine("function onLeafJoin(leaf) { }");
            events.AppendLine("function onLeafPart(leaf) { }");
            events.AppendLine("function onLinkedAdminDisabled(leaf, userobj) { }");
            this.JS.Evaluate(events.ToString());
        }

        public Objects.JSUser GetUser(IUser client)
        {
            if (client == null)
                return null;

            Objects.JSUser result = null;

            if (!client.Link.IsLinked)
                result = this.local_users.Find(x => x.Name == client.Name);
            else
                this.leaves.ForEach(x =>
                {
                    if (x.Ident == client.Link.Ident)
                    {
                        result = x.users.Find(z => z.Name == client.Name);

                        if (result != null)
                            return;
                    }
                });

            return result;
        }

        public Objects.JSUser GetIgnoredUser(String name)
        {
            Objects.JSUser result = this.local_users.Find(x => x.Name == name);

            if (result == null)
                this.leaves.ForEach(x =>
                {
                    result = x.users.Find(z => z.Name == name);

                    if (result != null)
                        return;
                });

            return result;
        }

        public bool LoadScript(String path)
        {
            Server.Users.Ares(x => this.local_users.Add(new Objects.JSUser(this.JS.Object.InstancePrototype, x, this.ScriptName)));
            Server.Users.Web(x => this.local_users.Add(new Objects.JSUser(this.JS.Object.InstancePrototype, x, this.ScriptName)));

            if (Server.Link.IsLinked)
            {
                Server.Link.ForEachLeaf(x =>
                {
                    this.leaves.Add(new Objects.JSLeaf(this.JS.Object.InstancePrototype, x, this.ScriptName));
                    
                    x.ForEachUser(z => this.leaves[this.leaves.Count - 1].users.Add(
                        new Objects.JSUser(this.JS.Object.InstancePrototype, z, this.ScriptName)));
                });
            }

            try
            {
                this.JS.ExecuteFile(path);
                return true;
            }
            catch (JavaScriptException e)
            {
                ErrorDispatcher.SendError(this.ScriptName, e.Message, e.LineNumber);
            }
            catch { }

            return false;
        }

        public void KillScript()
        {
            this.local_users.Clear();
            this.leaves.ForEach(x => x.users.Clear());
            this.leaves.Clear();
            this.JS = null;
            ScriptManager.RemoveCallbacks(this.ScriptName);
            this.timer_idents.Clear();
            TimerList.RemoveScriptTimers(this.ScriptName);
        }
    }
}
