using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Statics.JSGlobal gbl;

        public JSScript(String name)
        {
            this.ScriptName = name;
            this.JS = new ScriptEngine();
            this.JS.ScriptName = name;

            // set up global functions
            this.gbl = new Statics.JSGlobal(this);
            this.JS.EmbedIntoGlobal("print", new Action<object, object>((a, b) => this.gbl.Print(a, b)));
            this.JS.EmbedIntoGlobal("user", new Func<object, Objects.JSUser>(a => this.gbl.User(a)));
            this.JS.EmbedIntoGlobal("sendPM", new Action<object, object, object>((a, b, c) => this.gbl.SendPM(a, b, c)));
            this.JS.EmbedIntoGlobal("clrName", new Func<object, String>(a => this.gbl.ClrName(a)));
            this.JS.EmbedIntoGlobal("byteLength", new Func<object, int>(a => this.gbl.ByteLength(a)));
            

            // set up default events
            StringBuilder events = new StringBuilder();
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
            events.AppendLine("function onUnload() { }");
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
            events.AppendLine("function onCaptchaSending(userobj) { }");
            events.AppendLine("function onCaptchaReply(userobj, reply) { }");
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
                ScriptManager.OnError(this.ScriptName, e.Message, e.LineNumber);
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
        }
    }
}
