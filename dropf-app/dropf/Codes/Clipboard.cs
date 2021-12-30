using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dropf.Codes
{
    internal static class Clipboard
    {
        internal static bool SetText(string text)
        {
            var copied = false;

            for (int i = 0; i < 5; i++)
            { // 5 sefer deneyelim
                try
                {
                    System.Windows.Clipboard.SetText(text);
                    copied = true;
                    break;
                }
                catch { System.Threading.Thread.Sleep(10); }
            }

            return copied;
        }

        internal static void Clear() { System.Windows.Clipboard.Clear(); }
        internal static bool ContainsFileDropList() { return System.Windows.Clipboard.ContainsFileDropList(); }
        internal static bool ContainsText() { return System.Windows.Clipboard.ContainsText(); }
        internal static bool ContainsText(System.Windows.TextDataFormat format) { return System.Windows.Clipboard.ContainsText(format); }
        internal static bool ContainsImage() { return System.Windows.Clipboard.ContainsImage(); }
        internal static System.Collections.Specialized.StringCollection GetFileDropList() { return System.Windows.Clipboard.GetFileDropList(); }
        internal static string GetText() { return System.Windows.Clipboard.GetText(); }
        internal static System.Windows.Media.Imaging.BitmapSource GetImage() { return System.Windows.Clipboard.GetImage(); }

    }
}
