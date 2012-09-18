using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    public interface ITypePool
    {
        IUser User(String name);
        IUser User(ushort id);
        void ForEach(Action action);
    }
}
