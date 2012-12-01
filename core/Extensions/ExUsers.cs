using System;
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
