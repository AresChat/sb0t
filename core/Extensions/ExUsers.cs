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

namespace core.Extensions
{
    class ExUsers : IPool
    {
        public void All(Action<IUser> action)
        {
            Ares(action);
            Web(action);
            Linked(action);
        }

        public void Quarantined(Action<IQuarantined> action)
        {
            UserPool.AUsers.ForEachWhere(action, x => x.LoggedIn && x.Quarantined);
            UserPool.WUsers.ForEachWhere(action, x => x.LoggedIn && x.Quarantined);
        }

        public void Ares(Action<IUser> action)
        {
            UserPool.AUsers.ForEachWhere(action, x => x.LoggedIn && !x.Quarantined);
        }

        public void Web(Action<IUser> action)
        {
            UserPool.WUsers.ForEachWhere(action, x => x.LoggedIn && !x.Quarantined);
        }

        public void Linked(Action<IUser> action)
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                ServerCore.Linker.Leaves.ForEach(x => x.Users.ForEach(action));
        }

        public void Records(Action<IRecord> action)
        {
            UserHistory.list.ForEach(action);
        }

        public void Banned(Action<IBan> action)
        {
            BanSystem.Eval(action);
        }

        public IUser Find(Predicate<IUser> predicate)
        {
            IUser result = UserPool.AUsers.Find(predicate);

            if (result == null)
                result = UserPool.WUsers.Find(predicate);

            if (result == null)
                if (ServerCore.Linker.Busy)
                    ServerCore.Linker.Leaves.ForEach(x =>
                    {
                        result = x.Users.Find(predicate);

                        if (result != null)
                            return;
                    });

            return result;
        }
    }
}
