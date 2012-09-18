using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    public interface IPrivateMsg
    {
        bool Contains(String text);
        void Replace(String oldText, String newText);
        void Remove(String text);
    }
}
