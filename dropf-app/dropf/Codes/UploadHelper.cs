using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using System.Windows;
using System.Windows.Media.Imaging;

namespace dropf.Codes
{
    internal static class UploadHelper
    {
        internal static List<string> GetWillUploadedFiles(string[] fileList, bool forceZip)
        {
            // önce tüm dosyaları temp klasörüne alalım
            var cTempFolder = Path.Combine(Common.TempFolder, DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(cTempFolder); // temp klasör açtık
            foreach (var file in fileList)
            {
                var fileFolderName = Path.GetFileName(file);
                if (Common.IsDirectory(file))
                    arunes.Functions.CopyDirectory(file, Path.Combine(cTempFolder, fileFolderName));
                else
                    File.Copy(file, Path.Combine(cTempFolder, fileFolderName), true);
            }

            var sendFiles = new List<string>();
            var isMultiOrFolder = fileList.Length > 1 || (fileList.Length == 1 && Common.IsDirectory(fileList[0]));
            if (forceZip || (isMultiOrFolder && (dropf.Properties.Settings.Default.ZipMultipleFiles || dropf.Properties.Settings.Default.ZipFolders)))
            { // ziplicez
                var zipName = "archive_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                var zipFolder = cTempFolder;

                if (fileList.Length == 1)
                { // folderda, kendi ismiyle zipleyelim
                    zipName = Path.GetFileName(fileList[0]) + ".zip";
                    zipFolder += "\\" + Path.GetFileName(fileList[0]);   
                }

                var sendFile = Path.Combine(Common.TempFolder, zipName);
                ZipFiles(zipFolder, sendFile);
                
                File.Move(sendFile, Path.Combine(cTempFolder, Path.GetFileName(sendFile)));
                sendFiles.Add(Path.Combine(cTempFolder, Path.GetFileName(sendFile)));
            }
            else if (isMultiOrFolder)
            { // ziplemicez çoklu dosya yada klasör
                foreach (var file in fileList)
                {
                    var fileFolderName = Path.GetFileName(file);
                    if (Common.IsDirectory(file))
                        sendFiles.AddRange(arunes.Functions.GetAllFilePaths(Path.Combine(cTempFolder, Path.GetFileName(fileFolderName))));
                    else
                        sendFiles.Add(Path.Combine(cTempFolder, Path.GetFileName(fileFolderName)));                    
                }
            }
            else
            { // tek dosya
                sendFiles.Add(Path.Combine(cTempFolder, Path.GetFileName(fileList[0])));
            }

            return sendFiles;
        }

        internal static void ZipFiles(string inputFolderPath, string outputPathAndFile)
        {
            ArrayList ar = GenerateFileList(inputFolderPath); // generate file list
            int TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
            // find number of chars to remove     // from orginal file path
            TrimLength += 1; //remove '\'
            FileStream ostream;
            byte[] obuffer;
            ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outputPathAndFile)); // create zip stream
            oZipStream.SetLevel(dropf.Properties.Settings.Default.ZipLevel); // maximum compression

            if (dropf.Properties.Settings.Default.EncryptZipFile && dropf.Properties.Settings.Default.ZipPassword.Length > 0)
                oZipStream.Password = dropf.Properties.Settings.Default.ZipPassword;

            ZipEntry oZipEntry;
            foreach (string Fil in ar) // for each file, generate a zipentry
            {
                oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                oZipStream.PutNextEntry(oZipEntry);

                if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
                {
                    ostream = File.OpenRead(Fil);
                    obuffer = new byte[ostream.Length];
                    ostream.Read(obuffer, 0, obuffer.Length);
                    oZipStream.Write(obuffer, 0, obuffer.Length);
                }
            }
            oZipStream.Finish();
            oZipStream.Close();
        }

        internal static string[] GetClipboardData()
        {
            var fileList = new string[0];
            if (Clipboard.ContainsFileDropList())
            { // dosya listesi varmış
                var files = Clipboard.GetFileDropList();
                Array.Resize(ref fileList, files.Count);
                files.CopyTo(fileList, 0);
            }
            else if (Clipboard.ContainsText())
            { // text var
                Array.Resize(ref fileList, 1);
                var extension = "";
                var textData = Clipboard.GetText();
                if (dropf.Properties.Settings.Default.AlwaysSendTextAsTxt)
                    extension = "txt"; // hep txt
                else if (Clipboard.ContainsText(TextDataFormat.CommaSeparatedValue))
                    extension = "csv";
                else if (Clipboard.ContainsText(TextDataFormat.Html))
                    extension = "htm";
                else if (Clipboard.ContainsText(TextDataFormat.Rtf))
                    extension = "rtf";
                else if (Clipboard.ContainsText(TextDataFormat.Text) || Clipboard.ContainsText(TextDataFormat.UnicodeText))
                    extension = "txt";
                else if (Clipboard.ContainsText(TextDataFormat.Xaml))
                    extension = "xaml";

                var newFileName = System.IO.Path.Combine(Common.TempFolder, "clipboard_" + 
                    DateTime.Now.ToString("yyyyMMddHHmm") + "." + extension);

                System.IO.File.WriteAllText(newFileName, textData, Encoding.UTF8);
                fileList[0] = newFileName;
            }
            else if (Clipboard.ContainsImage())
            {
                Array.Resize(ref fileList, 1);
                var copyImage = Clipboard.GetImage();
                var newFileName = System.IO.Path.Combine(Common.TempFolder, "clipboard_" +
                    DateTime.Now.ToString("yyyyMMddHHmm") + ".png");

                using (var fileStream = new FileStream(newFileName, FileMode.Create))
                {
                    //BitmapEncoder encoder = new PngBitmapEncoder();
                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(copyImage));
                    encoder.Save(fileStream);
                }
                fileList[0] = newFileName;
            }

            Clipboard.Clear();
            return fileList;
        }

        internal static string[] TakeSSFile(Window mainWindow)
        {
            var size = Common.GetScreenSize(mainWindow);
            var screenWidth = (int)size.Width;
            var screenHeight = (int)size.Height;

            System.Drawing.Bitmap bmpScreenShot = new System.Drawing.Bitmap(screenWidth, screenHeight);
            System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage((System.Drawing.Image)bmpScreenShot);
            gfx.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenWidth, screenHeight));

            var newFileName = System.IO.Path.Combine(Common.TempFolder, "screenshot_" +
                DateTime.Now.ToString("yyyyMMddHHmm") + ".jpg");

            bmpScreenShot.Save(newFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            mainWindow.Visibility = Visibility.Visible;

            return new string[] { newFileName };
        }

        private static ArrayList GenerateFileList(string Dir)
        {
            ArrayList fils = new ArrayList();
            if (Common.IsDirectory(Dir))
            {
                bool Empty = true;
                foreach (string file in Directory.GetFiles(Dir)) // add each file in directory
                {
                    fils.Add(file);
                    Empty = false;
                }

                if (Empty)
                {
                    if (Directory.GetDirectories(Dir).Length == 0)
                    // if directory is completely empty, add it
                    {
                        fils.Add(Dir + @"/");
                    }
                }

                foreach (string dirs in Directory.GetDirectories(Dir)) // recursive
                {
                    foreach (object obj in GenerateFileList(dirs))
                    {
                        fils.Add(obj);
                    }
                }
            }
            else
                fils.Add(Dir);
           
            return fils; // return file list
        }
    }
}
