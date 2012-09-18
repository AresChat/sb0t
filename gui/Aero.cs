using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace gui
{
    class Aero
    {
        [DllImport("dwmapi.dll")]
        private static extern void DwmIsCompositionEnabled(ref bool pfEnabled);
        [DllImport("dwmapi.dll")]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int left, right, top, bottom;
        }

        public static bool CanAero
        {
            get
            {
                if (Environment.OSVersion.Version.Major < 6)
                    return false;

                bool success = false;
                DwmIsCompositionEnabled(ref success);
                return success;
            }
        }

        public static void MakeWindowGlass(Window window)
        {
            if (CanAero)
            {
                IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                MARGINS m = new MARGINS();
                m.left = -1;
                m.right = -1;
                m.top = -1;
                m.bottom = -1;

                DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref m);
            }
        }

        private static bool hide_done = false;

        public static void HideTitleInfo(Window window, WTNCA attributes)
        {
            if (!hide_done)
            {
                hide_done = true;

                if (CanAero)
                {
                    WTA_OPTIONS options = new WTA_OPTIONS();
                    options.dwFlags = attributes;
                    options.dwMask = WTNCA.VALIDBITS;
                    SetWindowThemeAttribute(new WindowInteropHelper(window).Handle,
                        WINDOWTHEMEATTRIBUTETYPE.WTA_NONCLIENT, ref options, (uint)Marshal.SizeOf(typeof(WTA_OPTIONS)));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTA_OPTIONS
        {
            public WTNCA dwFlags;
            public WTNCA dwMask;
        }

        [Flags]
        public enum WTNCA : uint
        {
            NODRAWCAPTION = 1,
            NODRAWICON = 2,
            NOSYSMENU = 4,
            NOMIRRORHELP = 8,
            VALIDBITS = NODRAWCAPTION | NODRAWICON | NOSYSMENU | NOMIRRORHELP
        }

        enum WINDOWTHEMEATTRIBUTETYPE : uint
        {
            WTA_NONCLIENT = 1
        }

        [DllImport("uxtheme.dll")]
        private static extern int SetWindowThemeAttribute(IntPtr hWnd,
            WINDOWTHEMEATTRIBUTETYPE wtype, ref WTA_OPTIONS attributes, uint size);
    }
}
