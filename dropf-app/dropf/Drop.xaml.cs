using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.ComponentModel;
using Hardcodet.Wpf.TaskbarNotification;

namespace dropf
{
    /// <summary>
    /// Interaction logic for Drop.xaml
    /// </summary>
    public partial class Drop
    {
        #region Globals
        private bool IsBusy = false;
        private BackgroundWorker bwUpload = new BackgroundWorker();
        private Codes.FtpSite ActiveFtp;
        private string LastUploadedFileUrl;
        private string LastError;
        private bool Cancelled = false;
        private bool CloseForm = false;
        private bool DragWithRight = false;
        #endregion

        public Drop()
        {
            if (!Lang.CheckAndSetLanguage()) this.Close();
            InitializeComponent();

            if (dropf.Properties.Settings.Default.FirstRun)
            {
                SetRightBottomPosition();
                dropf.Properties.Settings.Default.FirstRun = false;
                dropf.Properties.Settings.Default.Save();
            }

            PreviouseWindowState = WindowState;
            LayoutUpdated += Window_LayoutUpdated;
        }

        #region Single Instance Codes
        public WindowState PreviouseWindowState { get; private set; }
        private bool FirstArgsProcessed = false;

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            PreviouseWindowState = WindowState;
        }

        public bool Activate(bool restoreIfMinimized)
        {
            if (restoreIfMinimized && WindowState == WindowState.Minimized)
            {
                WindowState = PreviouseWindowState == WindowState.Normal ? WindowState.Normal : WindowState.Maximized;
            }
            return Activate();
        }

        System.Timers.Timer argTimer = new System.Timers.Timer();
        string[] Args = new string[0];
        bool SendToZip = false;
        public void ApendArgs(string[] args, bool firstArgs)
        {
            if (FirstArgsProcessed && firstArgs) return;
            if (args == null) return;
            SendToZip = Array.IndexOf(args, "zip") > -1;
            args = args.Where(file => file != "zip" && !file.StartsWith(Common.AppPath)).ToArray();

            if (args.Length > 0 && !IsBusy && myThumb.AllowDrop) // send to ile dosya gönderilmiş
            {
                Array.Resize(ref Args, args.Length);
                args.CopyTo(Args, 0);

                argTimer.Interval = 200;
                argTimer.Enabled = true;
                argTimer.Elapsed -= new System.Timers.ElapsedEventHandler(argTimer_Elapsed);
                argTimer.Elapsed += new System.Timers.ElapsedEventHandler(argTimer_Elapsed);
            }

            if (firstArgs && !FirstArgsProcessed) FirstArgsProcessed = true;
        }

        void argTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            argTimer.Enabled = false;
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() => { FilesDropped(Args, true); }));
        }
        #endregion

        #region Window Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            imgMain.Source = Common.GetThemeImage("main.png").ImageSource;
            GetThemes();
            GetHistory();
            GetFtpSites();
            SetVisual();
            SetBackgroundWorker();

            if (myThumb.AllowDrop) CheckFtpSite(dropf.Properties.Settings.Default.SelectedFtpSite);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            nIcon.Dispose();
            base.OnClosing(e);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // 34-43, 74, 83
            byte keyCode = Convert.ToByte(e.Key);
            if ((keyCode >= 35 && keyCode <= 43) || (keyCode >= 75 && keyCode <= 83))
            {
                var aIndex = keyCode > 43 ? keyCode - 74 : keyCode - 34;
                CheckFtpSite(aIndex - 1);
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (DroppedFiles.Length > 0)
            {
                imgMain.Source = Common.GetThemeImage("main.png").ImageSource;
                Array.Resize(ref DroppedFiles, 0);
            }
        }
        #endregion

        #region Background Worker
        private void SetBackgroundWorker()
        {
            bwUpload.WorkerReportsProgress = true;
            bwUpload.WorkerSupportsCancellation = true;
            bwUpload.DoWork += new DoWorkEventHandler(bwUpload_DoWork);
            bwUpload.ProgressChanged += new ProgressChangedEventHandler(bwUpload_ProgressChanged);
            bwUpload.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwUpload_RunWorkerCompleted);
        }

        void bwUpload_DoWork(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;
            List<string> fileNames = e.Argument as List<string>;
            var results = new List<string>();

            for (int i = 0; i < fileNames.Count; i++)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() => {
                    lblUpload.Content = dropf.Lang.Preparing;
                    if (fileNames.Count > 1)
                        lblUpload.Content += (i + 1) + "/" + fileNames.Count;
                }));
                
                var fileName = fileNames[i];

                try
                {
                    Codes.FtpHelper fHelper = new Codes.FtpHelper(ActiveFtp);
                    var prep = fHelper.PrepareFileForUpload(fileName);
                    string fullUrl = prep.FullUrl;
                    string jFileName = prep.FileName;
                    string jFolder = prep.Folder;
                    if (prep.Cancelled)
                    {
                        LastError = "cancelled";
                        e.Cancel = true;
                    }

                    if (!e.Cancel)
                    {
                        System.Net.FtpWebRequest request = fHelper.FtpRequest(fullUrl + "/" + jFileName.Trim('/'));
                        request.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                        //request.UseBinary = false;

                        using (var inputStream = System.IO.File.OpenRead(fileName))
                        {
                            using (var outputStream = request.GetRequestStream())
                            {
                                var buffer = new byte[1024 * 10]; // 10 kb de bir bildirsin
                                int totalReadBytesCount = 0;
                                int readBytesCount;
                                while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    outputStream.Write(buffer, 0, readBytesCount);
                                    totalReadBytesCount += readBytesCount;
                                    var progress = totalReadBytesCount * 100.0 / inputStream.Length;
                                    bwUpload.ReportProgress((int)progress);

                                    if (bwUpload.CancellationPending)
                                    { // cancel
                                        outputStream.Close();
                                        fHelper.DeleteFile(fullUrl, false);
                                        CloseForm = !Cancelled;
                                        e.Cancel = true;
                                        break;
                                    }
                                }
                            }

                            var uDir = System.IO.Path.GetDirectoryName(jFileName);
                            results.Add(jFolder + "/" +
                                (uDir.Length > 0 && uDir != "\\" ? uDir + "/" : "") +
                                System.IO.Path.GetFileName(jFileName));
                        }
                    }
                }
                catch (System.Net.WebException ex)
                {
                    LastError = dropf.Lang.UploadWebException + "\n" + ex.Message;
                    e.Cancel = true;
                }
                catch (Exception ex)
                {
                    LastError = dropf.Lang.UploadException + "\n" + ex.Message;
                    e.Cancel = true;
                }
            }

            e.Result = results;
        }

        void bwUpload_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 0 && !cmiCancel.IsEnabled) cmiCancel.IsEnabled = true;
            lblUpload.Content = string.Format(dropf.Lang.Uploading, e.ProgressPercentage);
            pbUpload.Value = e.ProgressPercentage;
            DoEvents();
        }

        void bwUpload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // temp klasörünü silelim
            try { System.IO.Directory.Delete(Common.TempFolder, true); }
            catch { }

            if (e.Cancelled)
            { // iptal edilmiş
                if (CloseForm)
                    this.Close(); // kapatalım programı
                else if (LastError != "cancelled" && !Cancelled)
                    MessageBox.Show(LastError, "dropf", MessageBoxButton.OK, MessageBoxImage.Error);

                Cancelled = false;
            }
            else
            { // başarıyla tamamlanmış               
                var balloonMsg = "";
                var balloonIcon = BalloonIcon.Info;

                var webUrls = new List<string>();
                var results = e.Result as List<string>;
                foreach (var result in results)
                {
                    var webUrl = Common.GetProperUri("http", ActiveFtp.WebUrl, ActiveFtp.Port, ActiveFtp.UploadFolder, result);
                    if (Uri.IsWellFormedUriString(webUrl, UriKind.Absolute))
                    { // düzgün format
                        Uri uploadedFile = new Uri(webUrl);
                        if (dropf.Properties.Settings.Default.UseUrlShortener)
                        {
                            Codes.UrlShortener us = new Codes.UrlShortener(webUrl, dropf.Properties.Settings.Default.UrlShortenerService);
                            var shortened = us.GetShortUrl();
                            if (shortened != null) webUrl = shortened;
                        }
                    }

                    webUrls.Add(webUrl);
                    AddToHistory(System.IO.Path.GetFileName(result), webUrl);
                    //LastUploadedFileUrl = webUrl;
                }

                if (dropf.Properties.Settings.Default.AutoCopy)
                {
                    var clipboardData = string.Join("\n", webUrls.ToArray());
                    if (Codes.Clipboard.SetText(clipboardData))
                        balloonMsg = dropf.Lang.UrlCopied;
                    else
                    {
                        balloonMsg = dropf.Lang.UrlCannotCopied;
                        balloonIcon = BalloonIcon.Warning;
                    }
                }
                else
                    balloonMsg = dropf.Lang.ClickAndCopyUrl;

                nIcon.ShowBalloonTip(dropf.Lang.UploadCompleted, balloonMsg, balloonIcon);
                GetHistory();  
            }

            if (SiteBeforeRight > -1) { CheckFtpSite(SiteBeforeRight); SiteBeforeRight = -1; }
            lblUpload.Visibility = pbUpload.Visibility = System.Windows.Visibility.Hidden;
            if (dropf.Properties.Settings.Default.FtpQuickJump) tcQuickFtp.Visibility = System.Windows.Visibility.Visible;
            cmiCancel.IsEnabled = false;
            Cursor = Cursors.Arrow;
            IsBusy = false;
            myThumb.AllowDrop = true;
        }
        #endregion

        #region MyThumb Events
        private void myThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (dropf.Properties.Settings.Default.LockPosition) return;
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }

        private void myThumb_DragEnter(object sender, DragEventArgs e)
        {
            DragWithRight = e.KeyStates == DragDropKeyStates.RightMouseButton;

            if (e.Data.GetDataPresent(DataFormats.FileDrop) && !IsBusy)
            {
                e.Effects = DragDropEffects.Copy;
                imgMain.Source = Common.GetThemeImage("main-hover.png").ImageSource;
            }
            else
                e.Effects = DragDropEffects.None;
        }

        private void myThumb_DragLeave(object sender, DragEventArgs e)
        {
            imgMain.Source = Common.GetThemeImage("main.png").ImageSource;
            DragWithRight = false;
        }

        private string[] DroppedFiles = new string[0];
        private void myThumb_Drop(object sender, DragEventArgs e)
        {
            if (IsBusy || !myThumb.AllowDrop) return;
            DroppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            var sendFiles = true;
            if (DragWithRight)
            {
                sendFiles = false;
                List<string> sites = new List<string>();
                foreach (MenuItem item in cmiFtpAccounts.Items)
                {
                    if (item.Name == "cmiNoFtpAccount") break;
                    sites.Add(item.Header.ToString());
                }

                //if (sites.Count > 1)
                {
                    ContextMenu cm = new ContextMenu();
                    for (int i = 0; i < sites.Count; i++)
                    {
                        var nMenu = new MenuItem { Header = sites[i], Tag = i };
                        nMenu.Click += (o, a) => { CheckFtpSite((int)((MenuItem)o).Tag, true); FilesDropped(DroppedFiles); };
                        cm.Items.Add(nMenu);
                    }

                    cm.Items.Add(new Separator());
                    var cmiRightCancel = new MenuItem { Header = Lang.Cancel };
                    cmiRightCancel.Click += delegate { imgMain.Source = Common.GetThemeImage("main.png").ImageSource; };
                    cm.Items.Add(cmiRightCancel);
                    cm.PlacementTarget = sender as UIElement;

                    IntPtr handle = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
                    System.ComponentModel.BackgroundWorker bg = new System.ComponentModel.BackgroundWorker();
                    bg.DoWork += new System.ComponentModel.DoWorkEventHandler(delegate
                    {
                        System.Threading.Thread.Sleep(10);
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            this.Activate();
                            cm.IsOpen = true;
                        }));
                    });
                    bg.RunWorkerAsync();
                }
                //else
                //    sendFiles = true;
            }

            if (sendFiles)
            {
                imgMain.Source = Common.GetThemeImage("main.png").ImageSource;
                FilesDropped(DroppedFiles);
            }
            DragWithRight = false;
        }

        private void myThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dropf.Properties.Settings.Default.Position = new Point(this.Left, this.Top);
            dropf.Properties.Settings.Default.Save();
        }
        #endregion

        #region Window Methods
        private delegate void EmptyDelegate();
        public static void DoEvents()
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Background, new EmptyDelegate(delegate { }));
        }

        private void FilesDropped(string[] fileList)
        {
            FilesDropped(fileList, false);
        }

        private void FilesDropped(string[] fileList, bool fromSendTo)
        {
            if (!arunes.Functions.IsInternetConnected(null))
            {
                MessageBox.Show(Lang.NoInternetConnection, "dropf", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (fileList.Length > 0)
            {
                if (!Uri.IsWellFormedUriString(Common.GetProperUri("ftp", ActiveFtp.Host, ActiveFtp.Port, ActiveFtp.FullFtpFolder), UriKind.Absolute))
                    MessageBox.Show(dropf.Lang.FtpUrlError, "dropf", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                {
                    myThumb.AllowDrop = false;
                    IsBusy = true;

                    pbUpload.Value = 0;
                    if (this.Width > 48) lblUpload.Visibility = System.Windows.Visibility.Visible;
                    pbUpload.Visibility = System.Windows.Visibility.Visible;

                    if (dropf.Properties.Settings.Default.FtpQuickJump) tcQuickFtp.Visibility = System.Windows.Visibility.Hidden;
                    Cursor = Cursors.Wait;
                    imgMain.Source = Common.GetThemeImage("main.png").ImageSource;

                    var sendFiles = Codes.UploadHelper.GetWillUploadedFiles(fileList, (fromSendTo && SendToZip));
                    bwUpload.RunWorkerAsync(sendFiles);
                }
            }
        }

        private bool ShowDialog(Window window)
        {
            window.Owner = this;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            bool? result = window.ShowDialog();
            return result.HasValue ? result.Value : false;
        }

        private void SetVisual()
        {
            SetTheme();
            ChangeSize(dropf.Properties.Settings.Default.Size.Width,
                dropf.Properties.Settings.Default.Size.Height);
            this.Opacity = dropf.Properties.Settings.Default.Opacity;

            cmiQuickFtpJump.IsChecked = dropf.Properties.Settings.Default.FtpQuickJump;
            if (!dropf.Properties.Settings.Default.FtpQuickJump) tcQuickFtp.Visibility = System.Windows.Visibility.Hidden;

            if (dropf.Properties.Settings.Default.AlwaysOnTop)
            {
                cmiAlwaysOnTop.IsChecked = this.Topmost = true;
                this.Activate();
            }

            cmiLockPosition.IsChecked = dropf.Properties.Settings.Default.LockPosition;
            CheckOpacity();
            CheckSize();
            CheckTheme();
        }

        private void SetPosition()
        {
            var sPoint = dropf.Properties.Settings.Default.Position;
            if (sPoint != new Point(0, 0))
            {
                if ((sPoint.X + this.Width) > SystemParameters.FullPrimaryScreenWidth)
                    sPoint.X = SystemParameters.FullPrimaryScreenWidth - this.Width;

                if ((sPoint.Y + this.Height) > SystemParameters.FullPrimaryScreenHeight)
                    sPoint.Y = SystemParameters.FullPrimaryScreenHeight - this.Height;

                this.Left = sPoint.X < -1 ? 0 : sPoint.X;
                this.Top = sPoint.Y < -1 ? 0 : sPoint.Y; ;
            }
        }

        private void SetRightBottomPosition()
        {
            var left = SystemParameters.FullPrimaryScreenWidth - this.Width;
            var top = SystemParameters.FullPrimaryScreenHeight - this.Height;

            dropf.Properties.Settings.Default.Position = new Point(left, top);
            dropf.Properties.Settings.Default.Save();
        }

        private void ChangeSize(int width, int height)
        {
            this.Height = myThumb.Height = imgMain.Height = height;
            this.Width = myThumb.Width = imgMain.Width = pbUpload.Width = width;
            pbUpload.Margin = new Thickness(0, height - pbUpload.Height, 0, 0);

            if (width < 64)
            {
                if (dropf.Properties.Settings.Default.FtpQuickJump)
                    tcQuickFtp.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                tcQuickFtp.Margin = new Thickness(0, height - (tcQuickFtp.Height + 2), 0, 0);
                if (dropf.Properties.Settings.Default.FtpQuickJump)
                    tcQuickFtp.Visibility = System.Windows.Visibility.Visible;
            }

            dropf.Properties.Settings.Default.Size = new System.Drawing.Size(width, height);
            dropf.Properties.Settings.Default.Save();

            SetPosition();
        }

        private void HideShowDropf()
        {
            this.Visibility = this.IsVisible ? Visibility.Hidden : Visibility.Visible;
            if (this.IsVisible)
            {
                niShowHide.Header = cmiShowHide.Header =
                    dropf.Lang.Hide;
            }
            else
            {
                niShowHide.Header = cmiShowHide.Header =
                    dropf.Lang.Show;

                this.Activate();
            }
        }

        private void ExitDropf()
        {
            if (IsBusy)
            {
                switch (MessageBox.Show(dropf.Lang.CancelActiveUploadExit, "dropf", MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        bwUpload.CancelAsync();
                        break;

                    case MessageBoxResult.No:
                        break;
                }
            }
            else
                this.Close();
        }
        #endregion

        #region History Methods
        private void AddToHistory(string fileName, string url)
        {
            var dropfXml = Common.GetdropfXml();
            dropfXml.Root.Element("History").AddFirst(
                new System.Xml.Linq.XElement("Url",
                    new System.Xml.Linq.XAttribute("File", fileName),
                    new System.Xml.Linq.XAttribute("Url", url)));
            Common.SavedropfXml(dropfXml);
        }

        private void GetHistory()
        {
            ClearHistoryOnMenu();

            var historyXml = Common.GetdropfXml();
            var history = historyXml.Root.Element("History").Elements("Url").ToList();
            if (history.Count > 0)
            {
                for (int i = (history.Count > 10 ? 10 : history.Count) - 1; i >= 0; i--)
                {
                    var hist = history[i];

                    var newItem = new MenuItem();
                    newItem.Header = hist.Attribute("File").Value;
                    newItem.Icon = new Image { Width = 16, Height = 16, Source = Common.GetProperFileImage(System.IO.Path.GetExtension(hist.Attribute("File").Value)) };
                    newItem.Click += new RoutedEventHandler(HistoryItemClicked);
                    newItem.Tag = hist.Attribute("Url").Value;
                    newItem.ToolTip = dropf.Lang.ClickAndCopyUrl + " - " + newItem.Tag;
                    cmiUploadHistory.Items.Insert(0, newItem);
                }

                cmiNoUrlFound.Visibility = Visibility.Collapsed;
            }
            else
                cmiNoUrlFound.Visibility = Visibility.Visible;
        }

        void HistoryItemClicked(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            if (Codes.Clipboard.SetText(item.Tag.ToString()))
                nIcon.ShowBalloonTip("dropf", dropf.Lang.UrlCopied, BalloonIcon.Info);
            else
                nIcon.ShowBalloonTip("dropf", dropf.Lang.UrlCannotCopied, BalloonIcon.Warning);
        }

        private void ClearHistory()
        {
            var dropfXml = Common.GetdropfXml();
            dropfXml.Root.Element("History").RemoveAll();
            Common.SavedropfXml(dropfXml);
        }

        private void ClearHistoryOnMenu()
        {
            var iCount = 0;
            foreach (MenuItem mi in cmiUploadHistory.Items)
            {
                if (mi.Name == "cmiNoUrlFound") break;
                iCount++;
            }

            for (int i = 0; i < iCount; i++)
                cmiUploadHistory.Items.RemoveAt(0);
        }
        #endregion

        #region Ftp Sites Methods
        private void GetFtpSites()
        {
            foreach (TabItem ti in tcQuickFtp.Items) // tabları saklayalım
                ti.Visibility = System.Windows.Visibility.Hidden;

            var sitesXml = Common.GetdropfXml();
            var sites = sitesXml.Root.Element("Sites").Elements("Site").ToList();
            if (sites.Count > 0)
            {
                for (int i = sites.Count - 1; i >= 0; i--)
                {
                    var site = sites[i];

                    var newItem = new MenuItem();
                    newItem.Header = site.Element("Name").Value;
                    newItem.Click += new RoutedEventHandler(FtpAccountClicked);
                    cmiFtpAccounts.Items.Insert(0, newItem);

                    ((TabItem)tcQuickFtp.Items[i]).Visibility = System.Windows.Visibility.Visible;
                }

                if (dropf.Properties.Settings.Default.FtpQuickJump) tcQuickFtp.Visibility = Visibility.Visible;
                cmiNoFtpAccount.Visibility = Visibility.Collapsed;
                CheckFtpSite(dropf.Properties.Settings.Default.SelectedFtpSite);
                imgMain.Source = Common.GetThemeImage("main.png").ImageSource;
                myThumb.AllowDrop = true;
            }
            else
            {
                tcQuickFtp.Visibility = System.Windows.Visibility.Hidden;
                cmiNoFtpAccount.Visibility = Visibility.Visible;
                imgMain.Source = Common.GetThemeImage("main-inactive.png").ImageSource;
                myThumb.AllowDrop = false;
            }
        }

        void FtpAccountClicked(object sender, RoutedEventArgs e)
        {
            var index = cmiFtpAccounts.Items.IndexOf(((MenuItem)sender));
            CheckFtpSite(index);
        }

        private void ClearFtpSites()
        {
            var iCount = 0;
            foreach (MenuItem mi in cmiFtpAccounts.Items)
            {
                if (mi.Name == "cmiNoFtpAccount") break;
                iCount++;
            }

            for (int i = 0; i < iCount; i++)
                cmiFtpAccounts.Items.RemoveAt(0);
        }

        private void CheckFtpSite(int siteIndex)
        {
            CheckFtpSite(siteIndex, false);
        }

        int SiteBeforeRight = -1;
        private void CheckFtpSite(int siteIndex, bool temp)
        {
            var sitesXml = Common.GetdropfXml();
            if (siteIndex > sitesXml.Root.Element("Sites").Elements("Site").Count() - 1) return;
            var iCount = 0;
            foreach (MenuItem mi in cmiFtpAccounts.Items)
            {
                if (mi.Name == "cmiNoFtpAccount") break;
                if (mi.IsChecked && temp) SiteBeforeRight = cmiFtpAccounts.Items.IndexOf(mi);
                mi.IsChecked = false;
                iCount++;
            }

            if (siteIndex >= iCount) siteIndex = 0;

            ((MenuItem)cmiFtpAccounts.Items[siteIndex]).IsChecked = true;
            dropf.Properties.Settings.Default.SelectedFtpSite = siteIndex;
            dropf.Properties.Settings.Default.Save();
            if (siteIndex < 4)
            {
                tcQuickFtp.SelectedIndex = siteIndex;
                imgMain.Focus();
            }

            var site = sitesXml.Root.Element("Sites").Elements("Site").ElementAt(siteIndex);
            ActiveFtp = new Codes.FtpSite
            {
                Name = site.Element("Name").Value,
                Host = site.Element("Host").Value,
                Port = Convert.ToInt32(site.Element("Port").Value),
                WebUrl = site.Element("HttpUrl").Value,
                Mode = Convert.ToInt32(site.Element("Mode").Value), // 0: default, 1:active, 2:passive
                User = site.Element("User").Value,
                Password = site.Element("Password").Value,
                RootFolder = site.Element("RootFolder").Value,
                UploadFolder = site.Element("UploadFolder").Value
            };
        }
        #endregion

        #region Theme Methods
        private void GetThemes()
        {
            var defTheme = new MenuItem();
            defTheme.Header = "Default";
            defTheme.Tag = "default";
            defTheme.Click += new RoutedEventHandler(ThemeClicked);
            cmiTheme.Items.Add(defTheme);

            foreach (var tDir in System.IO.Directory.GetDirectories(System.IO.Path.Combine(Common.AppPath, "themes")))
            {
                var nTheme = new MenuItem();
                nTheme.Header = System.IO.Path.GetFileName(tDir);
                nTheme.Tag = nTheme.Header;
                nTheme.Click += new RoutedEventHandler(ThemeClicked);
                cmiTheme.Items.Add(nTheme);
            }
        }

        private void ThemeClicked(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            dropf.Properties.Settings.Default.Theme = mi.Tag.ToString();
            dropf.Properties.Settings.Default.Save();
            CheckTheme();
            SetTheme();
        }

        private void CheckTheme()
        {
            foreach (MenuItem cmi in cmiTheme.Items) cmi.IsChecked = false;
            foreach (MenuItem cmi in cmiTheme.Items)
            {
                if (cmi.Tag.ToString() == dropf.Properties.Settings.Default.Theme)
                {
                    cmi.IsChecked = true;
                    break;
                }
            }
        }

        private void SetTheme()
        {
            if (cmiNoFtpAccount.Visibility == System.Windows.Visibility.Visible)
                imgMain.Source = Common.GetThemeImage("main-inactive.png").ImageSource;
            else
                imgMain.Source = Common.GetThemeImage("main.png").ImageSource;

            cmiShowHide.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/show-hide.png") };
            cmiSendFromClipboard.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/clipboard-paste.png") };
            cmiUploadHistory.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/history.png") };
            cmiClearHistory.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/clear.png") };
            cmiManageFtp.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/manage-ftp.png") };
            cmiSettings.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/settings.png") };
            cmiDisplay.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/display.png") };
            cmiTheme.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/theme.png") };
            cmiSize.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/size.png") };
            cmiOpacity.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/opacity.png") };
            cmiAbout.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/about.png") };
            cmiHelp.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/help.png") };
            cmiCheckForUpdates.Icon = new Image { Width = 16, Height = 16, Source = Common.GetThemeBitmap("icons/update.png") };

            GetHistory();
        }
        #endregion

        #region ContextMenu Events
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var validData = Codes.Clipboard.ContainsText() ||
                Codes.Clipboard.ContainsImage() ||
                Codes.Clipboard.ContainsFileDropList();

            cmiSendFromClipboard.IsEnabled = validData && !IsBusy && myThumb.AllowDrop;
            cmiTakeSSAndUpload.IsEnabled = !IsBusy && myThumb.AllowDrop;
        }

        private void cmiCancel_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show(dropf.Lang.CancelActiveUpload, "dropf", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    Cancelled = true;
                    bwUpload.CancelAsync();
                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void cmiShowHide_Click(object sender, RoutedEventArgs e)
        {
            HideShowDropf();
        }

        private void cmiSendFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            var files = Codes.UploadHelper.GetClipboardData();
            if (files.Length > 0) FilesDropped(files);
        }

        System.Timers.Timer ssTimer = new System.Timers.Timer();
        private void cmiTakeSSAndUpload_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(Lang.TakingSS, "dropf", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Visibility = System.Windows.Visibility.Collapsed;
            ssTimer.Interval = 500;
            ssTimer.Enabled = true;
            ssTimer.Elapsed -= new System.Timers.ElapsedEventHandler(ssTimer_Elapsed);
            ssTimer.Elapsed += new System.Timers.ElapsedEventHandler(ssTimer_Elapsed);
        }

        void ssTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ssTimer.Enabled = false;
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    (Action)(() =>
                    {
                        var files = Codes.UploadHelper.TakeSSFile(this);
                        if (files.Length > 0) FilesDropped(files);
                    }));
        }

        private void cmiManageFtp_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDialog(new ManageFtp()))
            {
                ClearFtpSites();
                GetFtpSites();
            }
        }

        private void cmiClearHistory_Click(object sender, RoutedEventArgs e)
        {
            ClearHistory();
        }

        private void cmiSettings_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDialog(new Settings()))
            {
                System.Diagnostics.ProcessStartInfo startdropfProcess =
                    new System.Diagnostics.ProcessStartInfo(System.IO.Path.Combine(Common.AppPath, "startdropf.exe"));
                startdropfProcess.WorkingDirectory = Common.AppPath;
                startdropfProcess.UseShellExecute = false;
                startdropfProcess.CreateNoWindow = true;
                System.Diagnostics.Process.Start(startdropfProcess);
                this.Close();
            }
        }

        private void cmiQuickFtpJump_Click(object sender, RoutedEventArgs e)
        {
            tcQuickFtp.Visibility = cmiQuickFtpJump.IsChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            dropf.Properties.Settings.Default.FtpQuickJump = cmiQuickFtpJump.IsChecked;
            dropf.Properties.Settings.Default.Save();
        }

        private void cmiLockPosition_Click(object sender, RoutedEventArgs e)
        {
            dropf.Properties.Settings.Default.LockPosition = cmiLockPosition.IsChecked;
            dropf.Properties.Settings.Default.Save();
        }

        private void cmiAlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = cmiAlwaysOnTop.IsChecked;
            dropf.Properties.Settings.Default.AlwaysOnTop = cmiAlwaysOnTop.IsChecked;
            dropf.Properties.Settings.Default.Save();
        }

        private void OpacityClicked(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            this.Opacity = Convert.ToDouble(mi.Tag);
            dropf.Properties.Settings.Default.Opacity = this.Opacity;
            dropf.Properties.Settings.Default.Save();
            CheckOpacity();
        }

        private void CheckOpacity()
        {
            cmiOpacity1.IsChecked = cmiOpacity2.IsChecked =
                cmiOpacity3.IsChecked = cmiOpacity4.IsChecked =
                cmiOpacityClosed.IsChecked = false;

            var opacity = dropf.Properties.Settings.Default.Opacity;
            if (opacity == 1) cmiOpacityClosed.IsChecked = true;
            else if (opacity == .2) cmiOpacity1.IsChecked = true;
            else if (opacity == .4) cmiOpacity2.IsChecked = true;
            else if (opacity == .6) cmiOpacity3.IsChecked = true;
            else if (opacity == .8) cmiOpacity4.IsChecked = true;
        }

        private void SizeClicked(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            foreach (MenuItem cmi in cmiSize.Items) cmi.IsChecked = false;
            mi.IsChecked = true;

            int width = Convert.ToInt32(mi.Tag.ToString().Split('x')[0]);
            int height = Convert.ToInt32(mi.Tag.ToString().Split('x')[1]);
            ChangeSize(width, height);
        }

        private void CheckSize()
        {
            foreach (MenuItem cmi in cmiSize.Items) cmi.IsChecked = false;
            foreach (MenuItem cmi in cmiSize.Items)
            {
                if (dropf.Properties.Settings.Default.Size.Width == Convert.ToInt32(cmi.Tag.ToString().Split('x')[0]) &&
                    dropf.Properties.Settings.Default.Size.Height == Convert.ToInt32(cmi.Tag.ToString().Split('x')[1]))
                {
                    cmi.IsChecked = true;
                    break;
                }
            }
        }

        private void cmiAbout_Click(object sender, RoutedEventArgs e)
        {
            ShowDialog(new About());
        }

        private void cmiHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://forum.dropf.com");
        }

        private void cmiCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            ShowDialog(new CheckUpdates());
        }

        private void cmiExit_Click(object sender, RoutedEventArgs e)
        {
            ExitDropf();
        }
        #endregion

        #region Notify Icon Events
        private void NIconDoubleClick_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            HideShowDropf();
        }

        private void nIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            if (!Codes.Clipboard.SetText(LastUploadedFileUrl))
                MessageBox.Show(Lang.UrlCannotCopied, "dropf", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void niResetPosition_Click(object sender, RoutedEventArgs e)
        {
            SetRightBottomPosition();
            SetPosition();
            this.Visibility = System.Windows.Visibility.Visible;
            this.Activate();
        }

        private void niGoToWebsite_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://dropf.com");
        }

        private void niGoToDropfForum_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://forum.dropf.com");
        }

        private void niShowHide_Click(object sender, RoutedEventArgs e)
        {
            HideShowDropf();
        }
        #endregion

        #region QuickFtp Events
        private void tcQuickFtp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && this.IsLoaded) CheckFtpSite(tcQuickFtp.SelectedIndex);
        }
        #endregion

    }
}
