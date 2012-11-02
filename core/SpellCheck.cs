using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHunspell;

namespace core
{
    class SpellCheck
    {
        private static Hunspell Engine { get; set; }

        private static void Init()
        {
            Engine = new Hunspell();
            Engine.Load("dictionary.aff", "dictionary.dic");
            Engine.Add("lol");
            Engine.Add("rofl");
            Engine.Add("lmao");
            Engine.Add("lmfao");
            Engine.Add("pmsl");
            Engine.Add("wtf");
            Engine.Add("nvm");
            Engine.Add("irl");
            Engine.Add("ily");
            Engine.Add("bff");
            Engine.Add("tyt");
            Engine.Add("ttyl");
            Engine.Add("brb");
            Engine.Add("bbs");
            Engine.Add("bbl");
            Engine.Add("hb");
            Engine.Add("ty");
            Engine.Add("yw");
            Engine.Add("afaik");
            Engine.Add("afk");
            Engine.Add("idk");
            Engine.Add("bf");
            Engine.Add("gf");
            Engine.Add("btw");
            Engine.Add("np");
            Engine.Add("omg");
            Engine.Add("oic");
            Engine.Add("tmi");
            Engine.Add("asl");
            Engine.Add("ik");
            Engine.Add("ikr");
            Engine.Add("k");
            Engine.Add("plz");
        }

        public static String Correct(String text)
        {
            List<String> list = new List<String>(text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            if (Engine == null)
                Init();

            for (int i = 0; i < list.Count; i++)
            {
                if (!Engine.Spell(list[i]))
                {
                    List<String> suggestions = Engine.Suggest(list[i]);

                    if (suggestions != null)
                        if (suggestions.Count > 0)
                            list[i] = suggestions[0];
                }
            }

            return String.Join(" ", list.ToArray());
        }

        public static String[] Suggest(String word)
        {
            String str = word.Trim();

            if (!str.Contains(" "))
            {
                if (Engine == null)
                    Init();

                List<String> suggestions = Engine.Suggest(str);

                if (suggestions != null)
                    return suggestions.ToArray();
            }

            return new String[] { };
        }
    }
}
