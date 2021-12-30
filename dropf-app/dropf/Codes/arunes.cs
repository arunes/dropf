using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace arunes
{
    internal static class Functions
    {
        internal static void CopyDirectory(string source, string destination)
        {
            // klasör yoksa yeni oluşturuyoruz
            if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);

            // klasördeki dosya ve alt klasörler arasında döngü kurduk
            foreach (var entry in Directory.GetFileSystemEntries(source))
            {
                if (Directory.Exists(entry))
                { // alt klasör kopyalayalım
                    CopyDirectory(entry, Path.Combine(destination, Path.GetFileName(entry)));
                }
                else
                { // dosya kopyalayalım
                    File.Copy(entry, Path.Combine(destination, Path.GetFileName(entry)), true);
                }
            }
        }

        internal static List<string> GetAllFilePaths(string dir)
        {
            var files = new List<string>();

            // klasördeki dosya ve alt klasörler arasında döngü kurduk
            foreach (var entry in Directory.GetFileSystemEntries(dir))
            {
                if (Directory.Exists(entry))
                { // buda bi klasörmüş alt klasör kopyalayalım
                    files.AddRange(GetAllFilePaths(entry));
                }
                else
                { // dosya kopyalayalım
                    files.Add(entry);
                }
            }

            return files;
        }

        internal static string ConvertToFileName(string input, string spaceChar, int limit)
        {
            string[] fileNameParts = input.Split('.');
            string fileExt = fileNameParts.Length > 1 ? fileNameParts.Last() : string.Empty;
            if (fileExt != string.Empty) Array.Resize(ref fileNameParts, fileNameParts.Length - 1);
            string fileName = string.Join(".", fileNameParts);

            fileName = fileName.Replace(" ", spaceChar);
            fileName = ReplaceTurkishChars(fileName);
            fileName = fileName.ToLower(new CultureInfo(1033));
            fileName = Regex.Replace(fileName, @"[^A-Z^a-z^0-9^.^-^_]", "");
            fileName += fileExt != string.Empty ? "." + fileExt : "";
            return limit == 0 ? fileName : GetFromLeft(fileName, limit, "");
        }

        internal static string ReplaceTurkishChars(string input)
        {
            return input
                .Replace("ç", "c")
                .Replace("Ç", "C")
                .Replace("ğ", "g")
                .Replace("Ğ", "G")
                .Replace("ı", "i")
                .Replace("İ", "I")
                .Replace("ö", "o")
                .Replace("Ö", "O")
                .Replace("ş", "s")
                .Replace("Ş", "S")
                .Replace("ü", "u")
                .Replace("Ü", "U");
        }

        internal static string GetFromLeft(string text, int length, string ifBigger)
        {
            if (string.IsNullOrEmpty(text)) return null;
            if (text.Length > length)
                return text.Substring(0, length) + ifBigger;
            else
                return text;
        }

        internal static bool IsValidNumber(object number)
        {
            if (number == null) return false;

            Int64 tmpNumber;
            if (Int64.TryParse(number.ToString(), out tmpNumber))
                return true;
            else
                return false;
        }

        internal static bool IsInternetConnected(string webSite)
        {
            try
            {
                webSite = webSite == null ? "www.google.com" : webSite;
                System.Net.IPHostEntry ip = System.Net.Dns.GetHostEntry(webSite);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    internal class ListItem
    {
        private string _display;
        private object _value;
        public ListItem(string display, object value)
        {
            _display = display;
            _value = value;
        }

        public string Display
        {
            get { return _display; }
        }

        public object Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return _display;
        }
    }
}
