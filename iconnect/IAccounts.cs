using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Password Accounts</summary>
    public interface IAccounts
    {
        /// <summary>Get collection of user passwords</summary>
        IPassword[] GetPasswords();
        /// <summary>Remove a user password</summary>
        void Remove(IPassword password);
    }
}
