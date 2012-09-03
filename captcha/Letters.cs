using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace captcha
{
    class Letters
    {
        public static String GetCaptcha(char letter, int index)
        {
            switch (letter)
            {
                case 'a': return a[index];
                case 'b': return b[index];
                case 'c': return c[index];
                case 'd': return d[index];
                case 'e': return e[index];
                case 'f': return f[index];
                case 'g': return g[index];
                case 'h': return h[index];
                case 'i': return i[index];
                case 'j': return j[index];
                case 'k': return k[index];
                case 'l': return l[index];
                case 'm': return m[index];
                case 'n': return n[index];
                case 'o': return o[index];
                case 'p': return p[index];
                case 'q': return q[index];
                case 'r': return r[index];
                case 's': return s[index];
                case 't': return t[index];
                case 'u': return u[index];
                case 'v': return v[index];
                case 'w': return w[index];
                case 'x': return x[index];
                case 'y': return y[index];
                case 'z': return z[index];
            }

            return null;
        }

        private static String[] a = new String[]
        {
            " \x000514        \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514          \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500"
        };

        private static String[] b = new String[]
        {
            "\x000514         \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514         \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514         \x000500 "
        };

        private static String[] c = new String[]
        {
            " \x000514         \x000500",
            "\x000514  \x000500        ",
            "\x000514  \x000500        ",
            "\x000514  \x000500        ",
            " \x000514         \x000500"
        };

        private static String[] d = new String[]
        {
            "\x000514         \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514         \x000500 "
        };

        private static String[] e = new String[]
        {
            "\x000514          \x000500",
            "\x000514  \x000500        ",
            "\x000514          \x000500",
            "\x000514  \x000500        ",
            "\x000514          \x000500"
        };

        private static String[] f = new String[]
        {
            "\x000514          \x000500",
            "\x000514  \x000500        ",
            "\x000514          \x000500",
            "\x000514  \x000500        ",
            "\x000514  \x000500        "
        };

        private static String[] g = new String[]
        {
            " \x000514         \x000500",
            "\x000514  \x000500        ",
            "\x000514  \x000500    \x000514    \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            " \x000514         \x000500"
        };

        private static String[] h = new String[]
        {
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514          \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500"
        };

        private static String[] i = new String[]
        {
            "\x000514  \x000500",
            "\x000514  \x000500",
            "\x000514  \x000500",
            "\x000514  \x000500",
            "\x000514  \x000500"
        };

        private static String[] j = new String[]
        {
            "\x000514          \x000500",
            "    \x000514  \x000500    ",
            "    \x000514  \x000500    ",
            "\x000514  \x000500  \x000514  \x000500    ",
            " \x000514    \x000500     "
        };

        private static String[] k = new String[]
        {
            "\x000514  \x000500    \x000514  \x000500",
            "\x000514  \x000500  \x000514  \x000500  ",
            "\x000514    \x000500    ",
            "\x000514  \x000500  \x000514  \x000500  ",
            "\x000514  \x000500    \x000514  \x000500"
        };

        private static String[] l = new String[]
        {
            "\x000514  \x000500        ",
            "\x000514  \x000500        ",
            "\x000514  \x000500        ",
            "\x000514  \x000500        ",
            "\x000514          \x000500"
        };

        private static String[] m = new String[]
        {
            " \x000514        \x000500 ",
            "\x000514  \x000500  \x000514  \x000500  \x000514  \x000500",
            "\x000514  \x000500  \x000514  \x000500  \x000514  \x000500",
            "\x000514  \x000500  \x000514  \x000500  \x000514  \x000500",
            "\x000514  \x000500  \x000514  \x000500  \x000514  \x000500"
        };

        private static String[] n = new String[]
        {
            " \x000514        \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500"
        };

        private static String[] o = new String[]
        {
            " \x000514        \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            " \x000514        \x000500 "
        };

        private static String[] p = new String[]
        {
            "\x000514         \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514         \x000500 ",
            "\x000514  \x000500        ",
            "\x000514  \x000500        "
        };

        private static String[] q = new String[]
        {
            " \x000514        \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500   \x000514  \x000500 \x000514  \x000500",
            "\x000514  \x000500    \x000514    \x000500",
            " \x000514         \x000500"
        };

        private static String[] r = new String[]
        {
            "\x000514         \x000500 ",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514         \x000500 ",
            "\x000514  \x000500    \x000514  \x000500  ",
            "\x000514  \x000500      \x000514  \x000500"
        };

        private static String[] s = new String[]
        {
            " \x000514         \x000500",
            "\x000514  \x000500        ",
            " \x000514        \x000500 ",
            "        \x000514  \x000500",
            "\x000514         \x000500 "
        };

        private static String[] t = new String[]
        {
            "\x000514          \x000500",
            "    \x000514  \x000500    ",
            "    \x000514  \x000500    ",
            "    \x000514  \x000500    ",
            "    \x000514  \x000500    "
        };

        private static String[] u = new String[]
        {
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            "\x000514  \x000500      \x000514  \x000500",
            " \x000514        \x000500 "
        };

        private static String[] v = new String[]
        {
            "\x000514  \x000500      \x000514  \x000500",
            " \x000514  \x000500    \x000514  \x000500 ",
            "  \x000514  \x000500  \x000514  \x000500  ",
            "   \x000514    \x000500   ",
            "    \x000514  \x000500    "
        };

        private static String[] w = new String[]
        {
            "\x000514  \x000500  \x000514  \x000500  \x000514  \x000500",
            "\x000514  \x000500  \x000514  \x000500  \x000514  \x000500",
            "\x000514  \x000500  \x000514  \x000500  \x000514  \x000500",
            "\x000514  \x000500  \x000514  \x000500  \x000514  \x000500",
            " \x000514        \x000500 "
        };

        private static String[] x = new String[]
        {
            "\x000514  \x000500      \x000514  \x000500",
            "  \x000514  \x000500  \x000514  \x000500  ",
            "    \x000514  \x000500    ",
            "  \x000514  \x000500  \x000514  \x000500  ",
            "\x000514  \x000500      \x000514  \x000500"
        };

        private static String[] y = new String[]
        {
            "\x000514  \x000500      \x000514  \x000500",
            "  \x000514  \x000500  \x000514  \x000500  ",
            "    \x000514  \x000500    ",
            "    \x000514  \x000500    ",
            "    \x000514  \x000500    "
        };

        private static String[] z = new String[]
        {
            "\x000514          \x000500",
            "      \x000514  \x000500  ",
            "    \x000514  \x000500    ",
            "  \x000514  \x000500      ",
            "\x000514          \x000500"
        };
    }
}
