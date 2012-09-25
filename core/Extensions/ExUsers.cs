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
            UserPool.AUsers.ForEachWhere(action, x => x.LoggedIn && !x.Quarantined);
            UserPool.WUsers.ForEachWhere(action, x => x.LoggedIn && !x.Quarantined);
        }

        public void Ares(Action<IUser> action)
        {
            UserPool.AUsers.ForEachWhere(action, x => x.LoggedIn && !x.Quarantined);
        }

        public void Web(Action<IUser> action)
        {
            UserPool.WUsers.ForEachWhere(action, x => x.LoggedIn && !x.Quarantined);
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

            return result;
        }
    }
}
