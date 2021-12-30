using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace dropf.Codes
{
    internal class FtpSite
    {
        public string Name;
        public string Host;
        public int Port;
        public string WebUrl;
        public int Mode;
        public string User;
        public string Password;
        public string RootFolder;
        public string UploadFolder;

        public string FullFtpFolder
        {
            get
            {
                return (!string.IsNullOrEmpty(RootFolder.Trim('/')) ? RootFolder.Trim('/') + "/" : "") + UploadFolder.Trim('/');
            }
        }
    }

    internal struct PrepareResult
    {
        public string FullUrl;
        public string FileName;
        public string Folder;
        public bool Cancelled;
    }

    internal class FtpHelper
    {
        private FtpSite _ftpSite;
        internal FtpSite FtpSite
        {
            get { return _ftpSite; }
            set { _ftpSite = value; }
        }

        internal PrepareResult PrepareFileForUpload(string fileName)
        {
            var retVal = new PrepareResult();

            // klasör varmı, yoksa oluşturalım
            Codes.FtpHelper fHelper = new Codes.FtpHelper(_ftpSite);
            if (!fHelper.DirectoryExists(_ftpSite.FullFtpFolder))
                fHelper.CreateDirectory(_ftpSite.FullFtpFolder);

            var tempFolder = Common.TempFolder;
            tempFolder += "\\" + fileName.Substring(tempFolder.Length).Replace("\\", "/").Trim('/').Split('/')[0];

            var jFileName = fileName.Substring(tempFolder.Length).Replace("\\", "/").Trim('/');
            var jFolder = "";

            if (!fHelper.DirectoryExists(_ftpSite.FullFtpFolder + "/" + Path.GetDirectoryName(jFileName)))
                fHelper.CreateDirectory(_ftpSite.FullFtpFolder + "/" + Path.GetDirectoryName(jFileName));

            var fullUrl = Common.GetProperUri("ftp", _ftpSite.Host, _ftpSite.Port, _ftpSite.FullFtpFolder);
            if (dropf.Properties.Settings.Default.CategorizeFolders)
            { // yıl/ay olarak klasör açıcaz
                var nFolder = DateTime.Today.Year + "/" + DateTime.Today.Month;
                if (!fHelper.DirectoryExists(_ftpSite.FullFtpFolder + "/" + nFolder))
                    fHelper.CreateDirectory(_ftpSite.FullFtpFolder + "/" + nFolder);
                fullUrl += "/" + nFolder;

                jFolder = nFolder;
            }
            else if (dropf.Properties.Settings.Default.OpenNewFolders)
            { // yeni klasör açalım dosya için
                var dateStr = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (!fHelper.DirectoryExists(_ftpSite.FullFtpFolder + "/" + dateStr))
                    fHelper.CreateDirectory(_ftpSite.FullFtpFolder + "/" + dateStr);

                jFolder = dateStr;
                fullUrl += "/" + dateStr;
            }

            if (dropf.Properties.Settings.Default.ReplaceFileNames) // isimleri düzenleyelim
                jFileName = Path.GetDirectoryName(jFileName) + "/" + arunes.Functions.ConvertToFileName(Path.GetFileName(jFileName), "_", 0);

            if (dropf.Properties.Settings.Default.PutTimestamp) // tarih koyalım başına
                jFileName = Path.GetDirectoryName(jFileName) + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(jFileName);

            if (dropf.Properties.Settings.Default.FileExistAction > 0)
            { // overwrite ise kontrole gerek yok
                if (fHelper.FileExists(fullUrl + "/" + jFileName, false))
                {
                    switch (dropf.Properties.Settings.Default.FileExistAction)
                    {
                        case 1: // rename
                            {
                                var newFileName = Path.GetFileNameWithoutExtension(jFileName) + "_{0}" + Path.GetExtension(jFileName);
                                bool isOk = false;
                                int newSuffix = 2;
                                do
                                {
                                    jFileName = string.Format(newFileName, newSuffix);
                                    if (!fHelper.FileExists(fullUrl + "/" + jFileName, false))
                                        isOk = true;
                                    else
                                        newSuffix++;

                                } while (!isOk);
                            }

                            break;

                        case 2: // ask
                            switch (System.Windows.MessageBox.Show(dropf.Lang.FileExistsPrompt, "dropf",
                                System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question))
                            {
                                case System.Windows.MessageBoxResult.Yes: // yaz 
                                    break;

                                case System.Windows.MessageBoxResult.No: // yeniden adlandır
                                    {
                                        var newFileName = Path.GetFileNameWithoutExtension(jFileName) + "_{0}" + Path.GetExtension(jFileName);
                                        bool isOk = false;
                                        int newSuffix = 2;
                                        do
                                        {
                                            jFileName = string.Format(newFileName, newSuffix);
                                            if (!fHelper.FileExists(fullUrl + "/" + jFileName, false))
                                                isOk = true;
                                            else
                                                newSuffix++;

                                        } while (!isOk);
                                    }
                                    break;

                                case System.Windows.MessageBoxResult.Cancel:
                                    retVal.Cancelled = true;
                                    break;
                            }
                            break;
                    }
                }
            }

            retVal.FullUrl = fullUrl;
            retVal.FileName = jFileName;
            retVal.Folder = jFolder;
            return retVal;
        }

        internal FtpHelper(FtpSite ftpSite)
        {
            _ftpSite = ftpSite;
        }

        internal bool CheckConnection()
        {
            return DoCommand("/", WebRequestMethods.Ftp.ListDirectory, true);
        }

        internal bool DirectoryExists(string folder)
        {
            return DoCommand(folder, WebRequestMethods.Ftp.Rename, true);
        }

        internal bool CreateDirectory(string folder)
        {
            var cFolder = "";
            foreach (var fld in folder.Trim('/').Split('/'))
            {
                cFolder += "/" + fld;
                if (!DirectoryExists(fld))
                {
                    if (!DoCommand(cFolder, WebRequestMethods.Ftp.MakeDirectory, true))
                        return false;
                }
            }

            return true;
        }

        internal bool FileExists(string file)
        {
            return DoCommand(file, WebRequestMethods.Ftp.GetDateTimestamp, true);
        }

        internal bool FileExists(string file, bool addHost)
        {
            return DoCommand(file, WebRequestMethods.Ftp.GetDateTimestamp, addHost);
        }

        internal bool DeleteFile(string file)
        {
            return DoCommand(file, WebRequestMethods.Ftp.DeleteFile, true);
        }

        internal bool DeleteFile(string file, bool addHost)
        {
            return DoCommand(file, WebRequestMethods.Ftp.DeleteFile, addHost);
        }

        internal bool DoCommand(string path, string command, bool addHost)
        {
            var req = FtpRequest(addHost ? Common.GetProperUri("ftp", _ftpSite.Host, _ftpSite.Port, path) : path);
            req.Method = command;
            if (command == "RENAME" && path.Trim().Length > 0) // for ftp servers like hostgator, they cannot understand list folders
                req.RenameTo = path;

            try
            {
                using (var response = (FtpWebResponse)req.GetResponse()) 
                {
                    StreamReader input = new StreamReader(response.GetResponseStream());
                    input.ReadToEnd();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal FtpWebRequest FtpRequest(string url)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(url);
            request.Credentials = new NetworkCredential(_ftpSite.User, _ftpSite.Password);
            request.UsePassive = _ftpSite.Mode == 2;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Timeout = 1000 * 10; //10sn
            request.ReadWriteTimeout = 1000 * 60 * 5; //5 dk
            request.Proxy = null; // proxy varsa kullanma
            
            return request;
        }
    }
}
