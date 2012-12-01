using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>User Pool</summary>
    public interface IPool
    {
        /// <summary>Find a specific user</summary>
        IUser Find(Predicate<IUser> predicate);
        /// <summary>Action all Ares clients</summary>
        void Ares(Action<IUser> action);
        /// <summary>Action all Web Browser clients</summary>
        void Web(Action<IUser> action);
        /// <summary>Action all Linked clients</summary>
        void Linked(Action<IUser> action);
        /// <summary>Action all clients</summary>
        void All(Action<IUser> action);
        /// <summary>Action all banned users</summary>
        void Banned(Action<IBan> action);
        /// <summary>Action all user records from this session</summary>
        void Records(Action<IRecord> action);
        /// <summary>Action all quarantined users</summary>
        void Quarantined(Action<IQuarantined> action);
    }
}
