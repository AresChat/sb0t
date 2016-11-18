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
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Objects
{
    class JSUser : ObjectInstance
    {
        private String ScriptName { get; set; }
        private JSUserFont _font { get; set; }

        internal JSUser(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUser(ObjectInstance prototype, IUser user, String script)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["User"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = user;
            this.ScriptName = script;
            this._font = new JSUserFont(this.Prototype, user, script);
        }

        protected override string InternalClassName
        {
            get { return "User"; }
        }

        internal IUser parent;

        [JSProperty(Name = "visible")]
        public bool Visible
        {
            get
            {
                if (this.parent.Link.IsLinked)
                    return this.parent.Link.Visible;

                return !this.parent.Cloaked;
            }
            set { }
        }

        [JSProperty(Name = "localEP")]
        public String LocalEP
        {
            get
            {
                if (this.parent.LocalEP == null)
                    return "0.0.0.0:0";

                else return this.parent.LocalEP.ToString();
            }
            set { }
        }

        [JSProperty(Name = "font")]
        public JSUserFont Font
        {
            get { return this._font; }
            set
            {
                if (value != null)
                    if (value is JSUserFont)
                    {
                        this._font.Enabled = value.Enabled;
                        this._font.NameColor = value.NameColor;
                        this._font.TextColor = value.TextColor;
                        this._font.Name = value.Name;
                    }
            }
        }

        [JSProperty(Name = "age")]
        public int Age
        {
            get { return this.parent.Age; }
            set { }
        }

        [JSProperty(Name = "leaf")]
        public JSLeaf Leaf
        {
            get
            {
                if (!this.parent.Link.IsLinked)
                    return null;

                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.ScriptName);

                if (script != null)
                    return script.leaves.Find(x => x.Ident == this.parent.Link.Ident);

                return null;
            }
            set { }
        }

        [JSProperty(Name = "avatar")]
        public object Avatar
        {
            get { return new JSAvatarImage(this.Engine.Object.InstancePrototype) { Data = this.parent.Avatar }; }
            set
            {
                if (value is JSAvatarImage)
                {
                    JSAvatarImage av = (JSAvatarImage)value;

                    if (av.DoesExist)
                    {
                        this.parent.Avatar = av.Data;
                        return;
                    }
                }

                this.parent.Avatar = null;
            }
        }

        [JSProperty(Name = "browsable")]
        public bool Browsable
        {
            get { return this.parent.Browsable; }
            set { }
        }

        [JSProperty(Name = "cloaked")]
        public bool Cloaked
        {
            get { return this.parent.Cloaked; }
            set { this.parent.Cloaked = value; }
        }

        [JSProperty(Name = "country")]
        public String Country
        {
            get { return Helpers.CountryCodeToString(this.parent.Country); }
            set { }
        }

        [JSProperty(Name = "customName")]
        public object CustomName
        {
            get { return this.parent.CustomName; }
            set
            {
                if (value is Null)
                {
                    this.parent.CustomName = null;
                    return;
                }

                this.parent.CustomName = value.ToString();
            }
        }

        [JSProperty(Name = "externalIp")]
        public String ExternalIp
        {
            get { return this.parent.ExternalIP.ToString(); }
            set { }
        }

        [JSProperty(Name = "dns")]
        public String DNS
        {
            get { return this.parent.DNS; }
            set { }
        }

        [JSProperty(Name = "fastPing")]
        public bool FastPing
        {
            get { return this.parent.FastPing; }
            set { }
        }

        [JSProperty(Name = "fileCount")]
        public int FileCount
        {
            get { return this.parent.FileCount; }
            set { }
        }

        [JSProperty(Name = "gender")]
        public String Gender
        {
            get
            {
                switch (this.parent.Sex)
                {
                    case 1:
                        return "Male";

                    case 2:
                        return "Female";

                    default:
                        return "Unknown";
                }
            }
            set { }
        }

        [JSProperty(Name = "guid")]
        public String Guid
        {
            get { return this.parent.Guid.ToString(); }
            set { }
        }

        [JSProperty(Name = "id")]
        public int Id
        {
            get
            {
                if (this.parent.Link.IsLinked)
                    return -1;

                return this.parent.ID;
            }
            set { }
        }

        [JSProperty(Name = "ignores")]
        public JSIgnoreCollection Ignores
        {
            get { return new JSIgnoreCollection(this.Engine.Object.InstancePrototype, this.parent.IgnoreList.ToArray(), this.ScriptName); }
            set { }
        }

        [JSProperty(Name = "level")]
        public int Level
        {
            get { return (int)((byte)this.parent.Level); }
            set
            {
                if (!this.parent.Owner)
                    if (value >= 0 && value <= 3)
                        if (ScriptCanLevel.Enabled)
                            this.parent.SetLevel((ILevel)(byte)value);
            }
        }

        [JSProperty(Name = "linked")]
        public bool Linked
        {
            get { return this.parent.Link.IsLinked; }
            set { }
        }

        [JSProperty(Name = "localIp")]
        public String LocalIP
        {
            get { return this.parent.LocalIP.ToString(); }
            set { }
        }

        [JSProperty(Name = "muzzled")]
        public bool Muzzled
        {
            get { return this.parent.Muzzled; }
            set { this.parent.Muzzled = value; }
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.parent.Name; }
            set { this.parent.Name = value; }
        }

        [JSProperty(Name = "orgName")]
        public String OrgName
        {
            get { return this.parent.OrgName; }
            set { }
        }

        [JSProperty(Name = "personalMessage")]
        public String PersonalMessage
        {
            get { return this.parent.PersonalMessage; }
            set { this.parent.PersonalMessage = value; }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.parent.DataPort; }
            set { }
        }

        [JSProperty(Name = "region")]
        public String Region
        {
            get { return this.parent.Region; }
            set { }
        }

        [JSProperty(Name = "version")]
        public String Version
        {
            get { return this.parent.Version; }
            set { }
        }

        [JSProperty(Name = "captcha")]
        public bool Captcha
        {
            get { return this.parent.Captcha; }
            set { }
        }

        [JSProperty(Name = "idle")]
        public bool Idle
        {
            get { return this.parent.Idle; }
            set { }
        }

        [JSProperty(Name = "registered")]
        public bool Registered
        {
            get { return this.parent.Registered; }
            set { }
        }

        [JSProperty(Name = "owner")]
        public bool Owner
        {
            get { return this.parent.Owner; }
            set { }
        }

        [JSProperty(Name = "webClient")]
        public bool WebClient
        {
            get { return this.parent.WebClient; }
            set { }
        }

        [JSProperty(Name = "customClient")]
        public bool CustomClient
        {
            get { return this.parent.CustomClient; }
            set { }
        }

        [JSProperty(Name = "ghost")]
        public bool Ghost
        {
            get { return this.parent.Ghosting; }
            set { }
        }

        [JSProperty(Name = "canHTML")]
        public bool CanHTML
        {
            get { return this.parent.SupportsHTML; }
            set { }
        }

        [JSProperty(Name = "joinTime")]
        public double JoinTime
        {
            get { return this.parent.JoinTime; }
            set { }
        }

        [JSProperty(Name = "vroom")]
        public int Vroom
        {
            get { return this.parent.Vroom; }
            set
            {
                if (value < 0 || value > 65535)
                    return;

                this.parent.Vroom = (ushort)value;
            }
        }

        [JSFunction(Name = "ban", IsWritable = false, IsEnumerable = true)]
        public void Ban()
        {
            if (!this.parent.Owner)
                this.parent.Ban();
        }

        [JSFunction(Name = "disconnect", IsWritable = false, IsEnumerable = true)]
        public void Disconnect()
        {
            if (!this.parent.Owner)
                this.parent.Disconnect();
        }

        [JSFunction(Name = "nudge", IsWritable = false, IsEnumerable = true)]
        public void Nudge(object a)
        {
            if (this.parent.CustomClient)
            {
                if (!(a is Undefined))
                    if (a != null)
                    {
                        this.parent.Nudge(a.ToString());
                        return;
                    }

                this.parent.Nudge(Server.Chatroom.BotName);
            }
        }

        [JSFunction(Name = "redirect", IsWritable = false, IsEnumerable = true)]
        public void Redirect(object a)
        {
            if (a is String || a is ConcatenatedString)
                if (!this.parent.Owner)
                    this.parent.Redirect(a.ToString());
        }

        [JSFunction(Name = "restoreAvatar", IsWritable = false, IsEnumerable = true)]
        public void RestoreAvatar()
        {
            this.parent.RestoreAvatar();
        }

        [JSFunction(Name = "sendEmote", IsWritable = false, IsEnumerable = true)]
        public void SendEmote(object a)
        {
            if (!(a is Undefined) && a != null)
                this.parent.SendEmote(a.ToString());
        }

        [JSFunction(Name = "sendText", IsWritable = false, IsEnumerable = true)]
        public void SendText(object a)
        {
            if (!(a is Undefined) && a != null)
                this.parent.SendText(a.ToString());
        }

        [JSFunction(Name = "sendHTML", IsWritable = false, IsEnumerable = true)]
        public void SendHTML(object a)
        {
            if (!(a is Undefined) && a != null)
                this.parent.SendHTML(a.ToString());
        }

        [JSFunction(Name = "setTopic", IsWritable = false, IsEnumerable = true)]
        public void SetTopic(object a)
        {
            if (!(a is Undefined))
                this.parent.Topic(a.ToString());
            else if (a != null)
                this.parent.Topic(Server.Chatroom.Topic);
        }

        [JSFunction(Name = "setUrl", IsWritable = false, IsEnumerable = true)]
        public void SetUrl(object a, object b)
        {
            if (a is Undefined && b is Undefined)
                this.parent.URL(String.Empty, String.Empty);
            else if (!(a is Undefined) && !(b is Undefined))
                if (a != null && b != null)
                    this.parent.URL(a.ToString(), b.ToString());
        }

        [JSFunction(Name = "scribble", IsWritable = false, IsEnumerable = true)]
        public void Scribble(object a, object b)
        {
            if (!this.parent.CustomClient)
                return;

            if (a is JSScribbleImage)
            {
                JSScribbleImage scr = (JSScribbleImage)a;
                scr.SendScribble(Server.Chatroom.BotName, this.parent);
            }
            else if (!(a is Undefined) && b is JSScribbleImage)
            {
                JSScribbleImage scr = (JSScribbleImage)b;
                scr.SendScribble(a.ToString(), this.parent);
            }
        }
    }
}
