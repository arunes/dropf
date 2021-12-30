using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace dropf
{
    public static class Lang
    {
        public static string About { get { return GetVal("About"); } }
        public static string AboutTitle { get { return GetVal("AboutTitle"); } }
        public static string Active { get { return GetVal("Active"); } }
        public static string Add { get { return GetVal("Add"); } }
        public static string AlwaysOnTop { get { return GetVal("AlwaysOnTop"); } }
        public static string Ask { get { return GetVal("Ask"); } }
        public static string AutoCopyUrl { get { return GetVal("AutoCopyUrl"); } }
        public static string Best { get { return GetVal("Best"); } }
        public static string BetaTesters { get { return GetVal("BetaTesters"); } }
        public static string Cancel { get { return GetVal("Cancel"); } }
        public static string CancelActiveUpload { get { return GetVal("CancelActiveUpload"); } }
        public static string CancelActiveUploadExit { get { return GetVal("CancelActiveUploadExit"); } }
        public static string CategorizeFolders { get { return GetVal("CategorizeFolders"); } }
        public static string Clear { get { return GetVal("Clear"); } }
        public static string ClickAndCopyUrl { get { return GetVal("ClickAndCopyUrl"); } }
        public static string Close { get { return GetVal("Close"); } }
        public static string Closed { get { return GetVal("Closed"); } }
        public static string Delete { get { return GetVal("Delete"); } }
        public static string Developer { get { return GetVal("Developer"); } }
        public static string Display { get { return GetVal("Display"); } }
        public static string Exit { get { return GetVal("Exit"); } }
        public static string Fast { get { return GetVal("Fast"); } }
        public static string Fastest { get { return GetVal("Fastest"); } }
        public static string FileExistsPrompt { get { return GetVal("FileExistsPrompt"); } }
        public static string FilenameUploadSettings { get { return GetVal("FilenameUploadSettings"); } }
        public static string FtpAccounts { get { return GetVal("FtpAccounts"); } }
        public static string RootFolder { get { return GetVal("RootFolder"); } }
        public static string UploadFolder { get { return GetVal("UploadFolder"); } }
        public static string FtpUrlError { get { return GetVal("FtpUrlError"); } }
        public static string GeneralSettings { get { return GetVal("GeneralSettings"); } }
        public static string Good { get { return GetVal("Good"); } }
        public static string Hide { get { return GetVal("Hide"); } }
        public static string Host { get { return GetVal("Host"); } }
        public static string HttpUrl { get { return GetVal("HttpUrl"); } }
        public static string IfFileExists { get { return GetVal("IfFileExists"); } }
        public static string Language { get { return GetVal("Language"); } }
        public static string LockPosition { get { return GetVal("LockPosition"); } }
        public static string Manage { get { return GetVal("Manage"); } }
        public static string ManageFtpAccountsTitle { get { return GetVal("ManageFtpAccountsTitle"); } }
        public static string Mode { get { return GetVal("Mode"); } }
        public static string Name { get { return GetVal("Name"); } }
        public static string NewSite { get { return GetVal("NewSite"); } }
        public static string NoFtpAccountFound { get { return GetVal("NoFtpAccountFound"); } }
        public static string Normal { get { return GetVal("Normal"); } }
        public static string NoUrlFound { get { return GetVal("NoUrlFound"); } }
        public static string OK { get { return GetVal("OK"); } }
        public static string Opacity { get { return GetVal("Opacity"); } }
        public static string OpenNewFolders { get { return GetVal("OpenNewFolders"); } }
        public static string Overwrite { get { return GetVal("Overwrite"); } }
        public static string Passive { get { return GetVal("Passive"); } }
        public static string Password { get { return GetVal("Password"); } }
        public static string Port { get { return GetVal("Port"); } }
        public static string Preparing { get { return GetVal("Preparing"); } }
        public static string PutTimestamps { get { return GetVal("PutTimestamps"); } }
        public static string QuickFtpJump { get { return GetVal("QuickFtpJump"); } }
        public static string Rename { get { return GetVal("Rename"); } }
        public static string ReplaceFileNames { get { return GetVal("ReplaceFileNames"); } }
        public static string Service { get { return GetVal("Service"); } }
        public static string Settings { get { return GetVal("Settings"); } }
        public static string SettingsTitle { get { return GetVal("SettingsTitle"); } }
        public static string Show { get { return GetVal("Show"); } }
        public static string Sites { get { return GetVal("Sites"); } }
        public static string Size { get { return GetVal("Size"); } }
        public static string StartWithWindows { get { return GetVal("StartWithWindows"); } }
        public static string Store { get { return GetVal("Store"); } }
        public static string Theme { get { return GetVal("Theme"); } }
        public static string UploadCompleted { get { return GetVal("UploadCompleted"); } }
        public static string UploadException { get { return GetVal("UploadException"); } }
        public static string UploadHistory { get { return GetVal("UploadHistory"); } }
        public static string Uploading { get { return GetVal("Uploading"); } }
        public static string UploadWebException { get { return GetVal("UploadWebException"); } }
        public static string UrlCopied { get { return GetVal("UrlCopied"); } }
        public static string UrlShortenerSettings { get { return GetVal("UrlShortenerSettings"); } }
        public static string UserName { get { return GetVal("UserName"); } }
        public static string UseUrlShortener { get { return GetVal("UseUrlShortener"); } }
        public static string ZipLevel { get { return GetVal("ZipLevel"); } }
        public static string LanguageChanged { get { return GetVal("LanguageChanged"); } }
        public static string ProgramAlreadyOpen { get { return GetVal("ProgramAlreadyOpen"); } }
        public static string GoTodropfWebsite { get { return GetVal("GoTodropfWebsite"); } }
        public static string SendFromClipboard { get { return GetVal("SendFromClipboard"); } }
        public static string TakeSSAndSend { get { return GetVal("TakeSSAndSend"); } }
        public static string TakingSS { get { return GetVal("TakingSS"); } }
        public static string CheckForUpdates { get { return GetVal("CheckForUpdates"); } }
        public static string CheckForUpdatesTitle { get { return GetVal("CheckForUpdatesTitle"); } }
        public static string CheckingUpdates { get { return GetVal("CheckingUpdates"); } }
        public static string NewVersionFound { get { return GetVal("NewVersionFound"); } }
        public static string YouAreUsingLastVersion { get { return GetVal("YouAreUsingLastVersion"); } }
        public static string Download { get { return GetVal("Download"); } }
        public static string CheckForUpdatesFail { get { return GetVal("CheckForUpdatesFail"); } }
        public static string NoInternetConnection { get { return GetVal("NoInternetConnection"); } }
        public static string AlwaysSendTextAsTxt { get { return GetVal("AlwaysSendTextAsTxt"); } }
        public static string Translations { get { return GetVal("Translations"); } }
        public static string AddToWindowsSendTo { get { return GetVal("AddToWindowsSendTo"); } }
        public static string Check { get { return GetVal("Check"); } }
        public static string CheckingFtpConnection { get { return GetVal("CheckingFtpConnection"); } }
        public static string FtpConnectionSuccessfully { get { return GetVal("FtpConnectionSuccessfully"); } }
        public static string FtpConnectionError { get { return GetVal("FtpConnectionError"); } }
        public static string UrlCannotCopied { get { return GetVal("UrlCannotCopied"); } }
        public static string GoTodropfForum { get { return GetVal("GoTodropfForum"); } }
        public static string Help { get { return GetVal("Help"); } }
        public static string ZipMultipleFiles { get { return GetVal("ZipMultipleFiles"); } }
        public static string ZipFolders { get { return GetVal("ZipFolders"); } }
        public static string HelpDropfTranslations { get { return GetVal("HelpDropfTranslations"); } }

        public static string ResetPosition { get { return GetVal("ResetPosition"); } }
        public static string EncryptZipFile { get { return GetVal("EncryptZipFile"); } }
        public static string ZipPassword { get { return GetVal("ZipPassword"); } }

        public static string LanguageName
        {
            get
            {
                var nativeName = System.Globalization.CultureInfo.GetCultureInfo(dropf.Properties.Settings.Default.Language).NativeName;
                var languageName = nativeName.Substring(0, nativeName.IndexOf('(')).Trim();
                return languageName;
            }
        }

        public static Dictionary<int, string> AvailableLanguages()
        {
            var availableLangs = new Dictionary<int, string>();
            var langDir = System.IO.Path.Combine(Common.AppPath, "languages");
            if (Directory.Exists(langDir))
            {
                var lngFiles = System.IO.Directory.GetFiles(langDir, "*.lng");
                foreach (var lngFile in lngFiles)
                {
                    try
                    {
                        var cultureName = System.IO.Path.GetFileNameWithoutExtension(lngFile);
                        var culture = CultureInfo.GetCultureInfo(cultureName);
                        availableLangs.Add(culture.LCID, culture.NativeName);
                    }
                    catch { }
                }
            }

            return availableLangs;
        }

        public static bool CheckAndSetLanguage()
        {
            var aLanguages = AvailableLanguages();
            if (aLanguages.Count > 0)
            {
                var defLang = 1033;
                if (!aLanguages.ContainsKey(defLang)) defLang = aLanguages.FirstOrDefault().Key;

                if (dropf.Properties.Settings.Default.FirstRun)
                {
                    if (aLanguages.ContainsKey(CultureInfo.CurrentUICulture.LCID))
                        dropf.Properties.Settings.Default.Language = CultureInfo.CurrentUICulture.LCID;
                    else if (aLanguages.ContainsKey(CultureInfo.CurrentCulture.LCID))
                        dropf.Properties.Settings.Default.Language = CultureInfo.CurrentCulture.LCID;
                    else
                        dropf.Properties.Settings.Default.Language = defLang;
                }
                else
                {
                    if (!aLanguages.ContainsKey(dropf.Properties.Settings.Default.Language))
                        dropf.Properties.Settings.Default.Language = defLang;
                }

                dropf.Properties.Settings.Default.Save();
            }
            else
            {
                System.Windows.MessageBox.Show("Dil dosyası bulunamadı!\nLanguage file not found!",
                    "dropf", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        private static string GetVal(string key)
        {
            try
            {
                if (Ini.ContainsKey(key))
                    return Ini[key].Replace("#n", "\n");
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        private static Dictionary<string, string> Ini
        {
            get
            {
                var curLang = CultureInfo.GetCultureInfo(dropf.Properties.Settings.Default.Language);
                var iniFile = Path.Combine(Common.AppPath, string.Format(@"languages\{0}.lng", curLang.Name));

                if (!File.Exists(iniFile))
                { // böyle bi dosya yokmuş

                }

                Codes.IniFile ini = new Codes.IniFile(iniFile);
                return ini.GetSection("dropf");
            }
        }
    }
}
