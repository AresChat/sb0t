using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class KewlText
    {
        private static List<Guid> list { get; set; }

        public static void Clear()
        {
            list = new List<Guid>();
        }

        public static void Add(IUser client)
        {
            if (list.FindIndex(x => x.Equals(client.Guid)) == -1)
                list.Add(client.Guid);
        }

        public static void Remove(IUser client)
        {
            list.RemoveAll(x => x.Equals(client.Guid));
        }

        public static bool IsKewlText(IUser client)
        {
            return list.FindIndex(x => x.Equals(client.Guid)) > -1;
        }

        public static String UnicodeText(String text)
        {
            String outText = text;

            char[] oldU = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] oldL = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] newU = "ΛβÇĐΞ₣ĢĦÏĴКĿMИΘ₱QЯŠŦЏ√ШЖ¥Ź".ToCharArray();
            char[] newL = "αвс∂εfgнιјκłмησρqяѕтυνωxчz".ToCharArray();

            for (int x = 0; x < 26; x++)
            {
                outText = outText.Replace(oldL[x].ToString(), newL[x].ToString());
                outText = outText.Replace(oldU[x].ToString(), newU[x].ToString());
            }

            return outText;
        }
    }
}
