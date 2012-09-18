using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    public interface IPool
    {
        IUser User(String name);
        IUser User(ushort id);
        void ForEach(Action action);
        ITypePool Ares { get; }
        ITypePool ib0t { get; }
    }
}
