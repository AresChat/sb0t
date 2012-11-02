using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core.Extensions
{
    class ExSpelling : ISpell
    {
        public String Check(String text)
        {
            return SpellCheck.Correct(text);
        }

        public String[] Suggest(String text)
        {
            return SpellCheck.Suggest(text);
        }

        public bool Confirm(String text)
        {
            return SpellCheck.Confirm(text);
        }
    }
}
