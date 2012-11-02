using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Spell Checker</summary>
    public interface ISpell
    {
        /// <summary>Spell Checker</summary>
        String Check(String text);
        /// <summary>Suggestions</summary>
        String[] Suggest(String word);
    }
}
