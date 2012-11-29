using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core.Extensions
{
    class ExAccounts : IAccounts
    {
        public void Remove(IPassword password)
        {
            AccountManager.Remove(password);
        }

        public IPassword[] GetPasswords()
        {
            return AccountManager.Passwords;
        }
    }
}
