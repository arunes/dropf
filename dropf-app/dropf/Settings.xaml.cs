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
using System.Globalization;

namespace dropf
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetSettings();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void cbCategorize_Checked(object sender, RoutedEventArgs e)
        {
            cbOpenNewFolders.IsEnabled = !cbCategorize.IsChecked.Value;
        }

        private void cbUseUrlShortener_Checked(object sender, RoutedEventArgs e)
        {
            slUrlShortener.IsEnabled = cbUseUrlShortener.IsChecked.Value;
        }

        private void GetSettings()
        {
            SetLanguages();
            SetFileExistsActions();
            SetCompressionLevels();
            SetUrlShorteners();

            var uLCID = dropf.Properties.Settings.Default.Language;
            slLanguage.SelectedItem = slLanguage.Items.Cast<arunes.ListItem>().FirstOrDefault(x => (int)x.Value == uLCID);

            cbAutoCopyUrl.IsChecked = dropf.Properties.Settings.Default.AutoCopy;
            cbStartWithWindows.IsChecked = dropf.Properties.Settings.Default.StartWithWindows;
            cbAddToWindowsSendTo.IsChecked = IsInSendToMenu();
            cbReplaceFileNames.IsChecked = dropf.Properties.Settings.Default.ReplaceFileNames;
            cbCategorize.IsChecked = dropf.Properties.Settings.Default.CategorizeFolders;
            if (!cbCategorize.IsChecked.Value) cbOpenNewFolders.IsChecked = dropf.Properties.Settings.Default.OpenNewFolders;
            cbPutTimeStamp.IsChecked = dropf.Properties.Settings.Default.PutTimestamp;
            cbAlwaysSendTextAsTxt.IsChecked = dropf.Properties.Settings.Default.AlwaysSendTextAsTxt;
            cbZipMultipleFiles.IsChecked = dropf.Properties.Settings.Default.ZipMultipleFiles;
            cbZipFolders.IsChecked = dropf.Properties.Settings.Default.ZipFolders;
            cbEncryptZipFile.IsEnabled = cbZipMultipleFiles.IsChecked.Value || cbZipMultipleFiles.IsChecked.Value;
            cbEncryptZipFile.IsChecked = dropf.Properties.Settings.Default.EncryptZipFile;
            pwdZipPassword.Password = dropf.Properties.Settings.Default.ZipPassword;
            pwdZipPassword.IsEnabled = cbEncryptZipFile.IsChecked.Value;

            slFileExistsAction.SelectedItem = slFileExistsAction.Items.Cast<arunes.ListItem>().FirstOrDefault(x => (int)x.Value == dropf.Properties.Settings.Default.FileExistAction);
            slZipLevel.SelectedItem = slZipLevel.Items.Cast<arunes.ListItem>().FirstOrDefault(x => (int)x.Value == dropf.Properties.Settings.Default.ZipLevel);

            cbUseUrlShortener.IsChecked = dropf.Properties.Settings.Default.UseUrlShortener;
            if (cbUseUrlShortener.IsChecked.Value) slUrlShortener.SelectedItem = slUrlShortener.Items.Cast<arunes.ListItem>().FirstOrDefault(x => x.Value.ToString() == dropf.Properties.Settings.Default.UrlShortenerService);
        }

        private void SetUrlShorteners()
        {
            slUrlShortener.Items.Add(new arunes.ListItem("2d1.in", "2d1"));
            slUrlShortener.Items.Add(new arunes.ListItem("goo.gl", "googl"));
            slUrlShortener.Items.Add(new arunes.ListItem("is.gd", "isgd"));
            slUrlShortener.SelectedIndex = 0;
        }

        private void SetCompressionLevels()
        {
            slZipLevel.Items.Add(new arunes.ListItem(dropf.Lang.Store, 0));
            slZipLevel.Items.Add(new arunes.ListItem(dropf.Lang.Fastest, 1));
            slZipLevel.Items.Add(new arunes.ListItem(dropf.Lang.Fast, 3));
            slZipLevel.Items.Add(new arunes.ListItem(dropf.Lang.Normal, 5));
            slZipLevel.Items.Add(new arunes.ListItem(dropf.Lang.Good, 7));
            slZipLevel.Items.Add(new arunes.ListItem(dropf.Lang.Best, 9));
            slZipLevel.SelectedIndex = 0;
        }

        private void SetFileExistsActions()
        {
            slFileExistsAction.Items.Add(new arunes.ListItem(dropf.Lang.Overwrite, 0));
            slFileExistsAction.Items.Add(new arunes.ListItem(dropf.Lang.Rename, 1));
            slFileExistsAction.Items.Add(new arunes.ListItem(dropf.Lang.Ask, 2));
            slLanguage.SelectedIndex = 0;
        }

        private void SetLanguages()
        {
            foreach (var lang in Lang.AvailableLanguages())
                slLanguage.Items.Add(new arunes.ListItem(lang.Value, lang.Key));

            slLanguage.SelectedIndex = 0;
        }

        private void SaveSettings()
        {
            if (pwdZipPassword.IsEnabled && pwdZipPassword.Password.Trim().Length == 0)
            {
                pwdZipPassword.Focus();
                pwdZipPassword.Background = Brushes.Red;
                pwdZipPassword.Foreground = Brushes.White;
                return;
            }

            var uLCID = (int)((arunes.ListItem)slLanguage.SelectedItem).Value;
            if (dropf.Properties.Settings.Default.Language != uLCID)
            {
                if (MessageBox.Show(Lang.LanguageChanged, "dropf", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    this.DialogResult = true;
            }

            dropf.Properties.Settings.Default.Language = uLCID;
            dropf.Properties.Settings.Default.AutoCopy = cbAutoCopyUrl.IsChecked.Value;
            dropf.Properties.Settings.Default.StartWithWindows = cbStartWithWindows.IsChecked.Value;

            if (!IsInSendToMenu() && cbAddToWindowsSendTo.IsChecked.Value)
                AddToWindowsSendToMenu();
            else if (IsInSendToMenu() && !cbAddToWindowsSendTo.IsChecked.Value)
                RemoveFromWindowsSendToMenu();

            dropf.Properties.Settings.Default.ReplaceFileNames = cbReplaceFileNames.IsChecked.Value;
            dropf.Properties.Settings.Default.CategorizeFolders = cbCategorize.IsChecked.Value;
            dropf.Properties.Settings.Default.OpenNewFolders = !cbCategorize.IsChecked.Value ? cbOpenNewFolders.IsChecked.Value : false;
            dropf.Properties.Settings.Default.PutTimestamp = cbPutTimeStamp.IsChecked.Value;
            dropf.Properties.Settings.Default.AlwaysSendTextAsTxt = cbAlwaysSendTextAsTxt.IsChecked.Value;
            dropf.Properties.Settings.Default.FileExistAction = (int)((arunes.ListItem)slFileExistsAction.SelectedItem).Value;
            dropf.Properties.Settings.Default.ZipLevel = (int)((arunes.ListItem)slZipLevel.SelectedItem).Value;
            dropf.Properties.Settings.Default.ZipMultipleFiles = cbZipMultipleFiles.IsChecked.Value;
            dropf.Properties.Settings.Default.ZipFolders = cbZipFolders.IsChecked.Value;
            dropf.Properties.Settings.Default.EncryptZipFile = cbEncryptZipFile.IsChecked.Value;
            dropf.Properties.Settings.Default.ZipPassword = pwdZipPassword.Password;
            dropf.Properties.Settings.Default.UseUrlShortener = cbUseUrlShortener.IsChecked.Value;
            dropf.Properties.Settings.Default.UrlShortenerService = cbUseUrlShortener.IsChecked.Value ? ((arunes.ListItem)slUrlShortener.SelectedItem).Value.ToString() : "";
            Common.SetAutoStart(cbStartWithWindows.IsChecked.Value);

            dropf.Properties.Settings.Default.Save();
            this.Close();
        }

        private void RemoveFromWindowsSendToMenu()
        {
            try
            {
                var sendToDir = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
                System.IO.File.Delete(System.IO.Path.Combine(sendToDir, "dropf.lnk"));
                System.IO.File.Delete(System.IO.Path.Combine(sendToDir, "dropf (zip).lnk"));
            }
            catch { }
        }

        private bool IsInSendToMenu()
        { 
            var sendToDir = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
            return System.IO.File.Exists(System.IO.Path.Combine(sendToDir, "dropf.lnk")) || 
                System.IO.File.Exists(System.IO.Path.Combine(sendToDir, "dropf (zip).lnk"));
        }

        private void AddToWindowsSendToMenu()
        {
            var sendToDir = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
            using (Shell.ShellLink shortcut = new Shell.ShellLink())
            {
                shortcut.Target = System.Reflection.Assembly.GetExecutingAssembly().Location;
                shortcut.WorkingDirectory = Common.AppPath;
                shortcut.Description = "dropf";
                shortcut.IconPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                shortcut.IconIndex = 0;
                shortcut.DisplayMode = Shell.ShellLink.LinkDisplayMode.edmNormal;
                shortcut.Save(System.IO.Path.Combine(sendToDir, "dropf.lnk"));
            }

            using (Shell.ShellLink shortcut = new Shell.ShellLink())
            {
                shortcut.Target = System.Reflection.Assembly.GetExecutingAssembly().Location;
                shortcut.Arguments = "zip";
                shortcut.WorkingDirectory = Common.AppPath;
                shortcut.Description = "dropf (zip)";
                shortcut.IconPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                shortcut.IconIndex = 0;
                shortcut.DisplayMode = Shell.ShellLink.LinkDisplayMode.edmNormal;
                shortcut.Save(System.IO.Path.Combine(sendToDir, "dropf (zip).lnk"));
            }
        }

        private void cbZipMultipleFiles_Click(object sender, RoutedEventArgs e)
        {
            cbEncryptZipFile.IsEnabled = cbZipMultipleFiles.IsChecked.Value || cbZipFolders.IsChecked.Value;
        }

        private void cbZipFolders_Click(object sender, RoutedEventArgs e)
        {
            cbEncryptZipFile.IsEnabled = cbZipMultipleFiles.IsChecked.Value || cbZipFolders.IsChecked.Value; 
        }

        private void cbEncryptZipFile_Click(object sender, RoutedEventArgs e)
        {
            pwdZipPassword.IsEnabled = cbEncryptZipFile.IsChecked.Value;
        }

    }
}
