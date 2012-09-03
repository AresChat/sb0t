using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace captcha
{
    public class Captcha
    {
        private static List<String> words;
        private static Random rnd;

        public static void Initialize()
        {
            words = new List<String>(Resource1._4words.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            words.RemoveAll(x => x.Length != 4);
            rnd = new Random();
        }

        public static CaptchaItem Create()
        {
            CaptchaItem item = new CaptchaItem();
            item.Word = words[(int)Math.Floor(rnd.NextDouble() * words.Count)];
            item.Lines = new String[5];
            char[] letters = item.Word.ToCharArray();

            for (int i = 0; i < 5; i++)
            {
                item.Lines[i] = "\x000500   ";

                foreach (char l in letters)
                    item.Lines[i] += Letters.GetCaptcha(l, i) + "   ";
            }

            return item;
        }
    }

    public class CaptchaItem
    {
        public String Word { get; set; }
        public String[] Lines { get; set; }
    }
}
