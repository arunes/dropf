using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows;
using System.IO;
using System.Xml.Linq;
using System.Windows.Resources;
using System.Net;

namespace dropf
{
    internal static class Common
    {
        internal static ImageBrush GetThemeImage(string image)
        {
            try
            {
                var imgPath = "themes/" + dropf.Properties.Settings.Default.Theme + "/" + image;
                return GetImage(imgPath);
            }
            catch
            {
                return new ImageBrush();
            }
        }

        internal static ImageBrush GetImage(string path)
        {
            if (path.StartsWith("themes"))
            { // theme img
                if (!path.StartsWith("themes/default"))
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(path, UriKind.Relative);
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    return new ImageBrush(bi);
                }
            }

            var aWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(); // x => x.IsActive
            return new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(aWindow), path)));
        }

        internal static BitmapImage GetThemeBitmap(string image)
        {
            try
            {
                var imgPath = "themes/" + dropf.Properties.Settings.Default.Theme + "/" + image;
                return GetBitmap(imgPath);
            }
            catch
            {
                return new BitmapImage();
            }
        }

        internal static BitmapImage GetBitmap(string path)
        {
            if (path.StartsWith("themes"))
            { // theme img
                if (!path.StartsWith("themes/default"))
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(path, UriKind.Relative);
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    return bi;
                }
            }

            var aWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(); // x => x.IsActive
            return new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(aWindow), path));
        }

        internal static string AppPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        internal static string TempFolder
        {
            get
            {
                var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var tPath = Path.Combine(appDataDir, "Temp");
                if (!Directory.Exists(tPath)) Directory.CreateDirectory(tPath);
                return tPath;
            }
        }

        internal static BitmapImage GetProperFileImage(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".png":
                case ".bmp":
                case ".tif":
                case ".tiff":
                case ".psd":
                    return GetThemeBitmap("filetypes/image.png");

                case ".doc":
                case ".docx":
                case ".ppt":
                case ".pptx":
                case ".xsl":
                case ".xslx":
                case ".xml":
                case ".txt":
                case ".rtf":
                case ".pdf":
                    return GetThemeBitmap("filetypes/document.png");

                case ".mov":
                case ".avi":
                case ".flv":
                case ".swf":
                case ".divx":
                case ".mpg":
                case ".mpeg":
                case ".mp4":
                case ".wmv":
                    return GetThemeBitmap("filetypes/video.png");

                case ".mp3":
                case ".vaw":
                case ".flac":
                case ".asf":
                    return GetThemeBitmap("filetypes/audio.png");

                case ".rar":
                case ".zip":
                case ".7z":
                    return GetThemeBitmap("filetypes/archive.png");

                default:
                    return GetThemeBitmap("filetypes/unknown.png");
            }

        }

        internal static void SetAutoStart(bool start)
        {
            using (var rkApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (start) rkApp.SetValue("dropf", "\"" + System.IO.Path.Combine(Common.AppPath, "startdropf.exe") + "\" wait");
                else if (rkApp.GetValue("dropf") != null) rkApp.DeleteValue("dropf", false);
                rkApp.Close();
            }
        }

        internal static bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return true;
            else
                return false;
        }

        internal static XDocument GetdropfXml()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            XDocument retVal = new XDocument();
            var xmlPath = Path.Combine(appDataDir, "dropf", "dropf.xml");
            if (!File.Exists(xmlPath)) retVal = CreatedropfXml();
            else retVal = XDocument.Load(xmlPath);

            return retVal;
        }

        internal static void SavedropfXml(XDocument xd)
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(Path.Combine(appDataDir, "dropf"))) Directory.CreateDirectory(Path.Combine(appDataDir, "dropf"));
            xd.Save(Path.Combine(appDataDir, "dropf", "dropf.xml"));
        }

        internal static string DoRequest(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            StreamReader input = new StreamReader(response.GetResponseStream());
            var retVal = input.ReadToEnd();
            input.Close();
            return retVal;
        }

        internal static Size GetScreenSize(Window mainWindow)
        {
            PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual(mainWindow);
            System.Windows.Media.Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
            int screenWidth = (int)Math.Floor(SystemParameters.PrimaryScreenWidth * m.M11);
            int screenHeight = (int)Math.Floor(SystemParameters.PrimaryScreenHeight * m.M22);
            return new Size(screenWidth, screenHeight);
        }

        internal static string GetProperUri(string protocol, string host, int port, params string[] parts)
        {
            string newUri = protocol + "://";
            if (host.StartsWith(newUri)) newUri += host.Remove(0, newUri.Length).Trim('/'); else newUri += host;
            if (protocol == "ftp") newUri += ":" + port;
            foreach (var part in parts) newUri += !string.IsNullOrEmpty(part.Trim('/')) ? "/" + part.Trim('/') : "";
            return newUri.Trim('/');
        }

        private static XDocument CreatedropfXml()
        {
            XDocument xd = new XDocument(
                new XElement("dropf",
                    new XElement("Sites"),
                    new XElement("History"))
                );

            SavedropfXml(xd);
            return xd;
        }
    }
}
