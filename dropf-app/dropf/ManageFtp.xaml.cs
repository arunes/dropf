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
using System.Xml.Linq;

namespace dropf
{
    /// <summary>
    /// Interaction logic for ManageFtp.xaml
    /// </summary>
    public partial class ManageFtp : Window
    {
        private bool DontWatchValues = false;
        private List<Codes.FtpSite> ftpSites = new List<Codes.FtpSite>();
        public ManageFtp()
        {
            InitializeComponent();
        }

        private delegate void EmptyDelegate();
        public static void DoEvents()
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Background, new EmptyDelegate(delegate { }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisableForm(true);
            LoadFtpSites();
            ((TreeViewItem)tvSites.Items[0]).IsExpanded = true;

            btnBrowseRoot.Visibility = btnBrowseUpload.Visibility = System.Windows.Visibility.Hidden;

            // set events
            txtName.LostFocus += new RoutedEventHandler(ValueChanged);
            txtName.LostFocus += new RoutedEventHandler(NameChanged);
            txtHost.LostFocus += new RoutedEventHandler(ValueChanged);
            txtPort.LostFocus += new RoutedEventHandler(ValueChanged);
            txtHttpUrl.LostFocus += new RoutedEventHandler(ValueChanged);
            rbModeActive.Checked += new RoutedEventHandler(ValueChanged);
            rbModeActive.Unchecked += new RoutedEventHandler(ValueChanged);
            rbModePassive.Checked += new RoutedEventHandler(ValueChanged);
            rbModePassive.Unchecked += new RoutedEventHandler(ValueChanged);
            txtUser.LostFocus += new RoutedEventHandler(ValueChanged);
            txtPassword.LostFocus += new RoutedEventHandler(ValueChanged);
            txtRootFolder.LostFocus += new RoutedEventHandler(ValueChanged);
            txtUploadFolder.LostFocus += new RoutedEventHandler(ValueChanged);

            txtName.GotFocus += new RoutedEventHandler(TextGotFocus);
            txtName.GotFocus += new RoutedEventHandler(TextGotFocus);
            txtHost.GotFocus += new RoutedEventHandler(TextGotFocus);
            txtPort.GotFocus += new RoutedEventHandler(TextGotFocus);
            txtHttpUrl.GotFocus += new RoutedEventHandler(TextGotFocus);
            txtUser.GotFocus += new RoutedEventHandler(TextGotFocus);
            txtPassword.GotFocus += new RoutedEventHandler(TextGotFocus);
            txtRootFolder.GotFocus += new RoutedEventHandler(TextGotFocus);
            txtUploadFolder.GotFocus += new RoutedEventHandler(TextGotFocus);

            if (ftpSites.Count > 0)
            {
                ((TreeViewItem)((TreeViewItem)tvSites.Items[0]).Items[0]).IsSelected = true;
                txtName.Focus();
            }
        }

        void NameChanged(object sender, RoutedEventArgs e)
        {
            ((TreeViewItem)tvSites.SelectedItem).Header = txtName.Text;
        }

        void ValueChanged(object sender, RoutedEventArgs e)
        {
            var curTag = ((TreeViewItem)tvSites.SelectedItem).Tag;
            if (DontWatchValues) return;
            if (arunes.Functions.IsValidNumber(curTag))
            {
                int index = Convert.ToInt32(curTag);
                var site = ftpSites[index];
                site.Name = txtName.Text.Trim();
                site.Host = txtHost.Text.Trim();
                site.Port = arunes.Functions.IsValidNumber(txtPort.Text) ? Convert.ToInt32(txtPort.Text) : 21;
                site.WebUrl = txtHttpUrl.Text.Trim();
                site.Mode = ((rbModeActive.IsChecked.HasValue ? rbModeActive.IsChecked.Value : false) ? 1 : 2);
                site.User = txtUser.Text.Trim();
                site.Password = txtPassword.Password.Trim();
                site.RootFolder = txtRootFolder.Text.Trim();
                site.UploadFolder = txtUploadFolder.Text.Trim();

                ftpSites[index] = site;
            }
        }

        void TextGotFocus(object sender, RoutedEventArgs e)
        {
            Type senderType = sender.GetType();
            if (senderType.Name == "TextBox")
            {
                var tb = sender as TextBox;
                tb.SelectAll();
            }
            else if (senderType.Name == "PasswordBox")
            {
                PasswordBox pb = sender as PasswordBox;
                pb.SelectAll();
            }
        }

        private void ClearAllErrors()
        {
            txtName.Background =
                txtHost.Background =
                txtPort.Background =
                txtHttpUrl.Background =
                txtUser.Background =
                txtPassword.Background = Brushes.White;
        }

        private void GiveError(Control control)
        {
            control.Background = Brushes.LightYellow;
            control.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var sitesXml = Common.GetdropfXml();
            var sitesRoot = sitesXml.Root.Element("Sites");
            sitesRoot.RemoveAll();

            var errorOccurred = false;
            for (int i = 0; i < ftpSites.Count; i++)
            {
                var site = ftpSites[i];
                errorOccurred = false;

                ClearAllErrors();
                if (string.IsNullOrEmpty(site.Name)) { GiveError(txtName); errorOccurred = true; }
                else if (ftpSites.Count(x => x.Name == site.Name) > 1) { GiveError(txtName); errorOccurred = true; }
                else if (string.IsNullOrEmpty(site.Host)) { GiveError(txtHost); errorOccurred = true; }
                else if (site.Port == 0) { GiveError(txtPort); errorOccurred = true; }
                else if (string.IsNullOrEmpty(site.WebUrl)) { GiveError(txtHttpUrl); errorOccurred = true; }
                else if (string.IsNullOrEmpty(site.User)) { GiveError(txtUser); errorOccurred = true; }
                else if (string.IsNullOrEmpty(site.Password)) { GiveError(txtPassword); errorOccurred = true; }

                if (errorOccurred)
                {
                    ((TreeViewItem)((TreeViewItem)tvSites.Items[0]).Items[i]).IsSelected = true; ;
                    break;
                }
                else
                {
                    sitesRoot.Add(new XElement("Site",
                        new XElement("Name", site.Name),
                        new XElement("Host", site.Host),
                        new XElement("Port", site.Port),
                        new XElement("HttpUrl", site.WebUrl),
                        new XElement("Mode", site.Mode),
                        new XElement("User", site.User),
                        new XElement("Password", site.Password),
                        new XElement("RootFolder", site.RootFolder),
                        new XElement("UploadFolder", site.UploadFolder)
                        ));
                }
            }

            if (!errorOccurred)
            {
                Common.SavedropfXml(sitesXml);
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void LoadFtpSites()
        {
            var sitesXml = Common.GetdropfXml();
            var sites = sitesXml.Root.Element("Sites").Elements("Site").ToList();
            for (int i = 0; i < sites.Count; i++)
            {
                var site = sites[i];
                ftpSites.Add(new Codes.FtpSite
                {
                    Name = site.Element("Name").Value,
                    Host = site.Element("Host").Value,
                    Port = Convert.ToInt32(site.Element("Port").Value),
                    WebUrl = site.Element("HttpUrl").Value,
                    Mode = Convert.ToInt32(site.Element("Mode").Value), // 1:active, 2:passive
                    User = site.Element("User").Value,
                    Password = site.Element("Password").Value,
                    RootFolder = site.Element("RootFolder").Value,
                    UploadFolder = site.Element("UploadFolder").Value
                });

                var tvItem = new TreeViewItem();
                tvItem.Tag = i;
                tvItem.Header = site.Element("Name").Value;
                ((TreeViewItem)tvSites.Items[0]).Items.Add(tvItem);
            }
        }

        private void DisableForm(bool disable)
        {
            if (txtName == null) return;

            txtName.IsEnabled = txtHost.IsEnabled = txtPort.IsEnabled =
                txtHttpUrl.IsEnabled = rbModeActive.IsEnabled =
                rbModePassive.IsEnabled = txtUser.IsEnabled = txtPassword.IsEnabled =
                txtRootFolder.IsEnabled = txtUploadFolder.IsEnabled = btnCheck.IsEnabled = !disable;

            if (disable)
            {
                txtName.Text = txtHost.Text = txtHttpUrl.Text = txtUser.Text = txtPassword.Password = txtRootFolder.Text = txtUploadFolder.Text = null;
                rbModePassive.IsChecked = true;
                txtPort.Text = "21";
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newNodeText = GetNewNodeText(dropf.Lang.NewSite);
            TreeViewItem newNode = new TreeViewItem();
            newNode.Tag = ftpSites.Count.ToString();
            newNode.Header = newNodeText;
            ((TreeViewItem)tvSites.Items[0]).Items.Add(newNode);

            ftpSites.Add(new Codes.FtpSite
            {
                Name = newNodeText,
                Host = null,
                Port = 21,
                WebUrl = null,
                Mode = 2, // 1:active, 2:passive
                User = null,
                Password = null,
                RootFolder = null,
                UploadFolder = null
            });

            newNode.IsSelected = true;
            txtName.SelectAll();
            txtName.Focus();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSite();
        }

        private void DeleteSite()
        {
            if (tvSites.SelectedItem != null)
            {
                if (!tvSites.SelectedItem.Equals(tvSites.Items[0]))
                {
                    var curTag = ((TreeViewItem)tvSites.Items[0]).Items.IndexOf(tvSites.SelectedItem);
                    ftpSites.RemoveAt(Convert.ToInt32(curTag));
                    ((TreeViewItem)tvSites.Items[0]).Items.RemoveAt(curTag);
                    tvSites.Focus();
                }
            }

            if (ftpSites.Count > 0) ((TreeViewItem)((TreeViewItem)tvSites.Items[0]).Items[0]).IsSelected = true;
        }

        private void tvSites_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete) DeleteSite();
        }

        private string GetNewNodeText(string newName)
        {
            var retVal = newName;
            bool isOk = false;
            int newSuffix = 1;
            do
            {
                bool match = false;
                foreach (TreeViewItem node in ((TreeViewItem)tvSites.Items[0]).Items)
                {
                    if (node.Header.ToString() == retVal && !match)
                    {
                        match = true;
                        newSuffix++;
                        break;
                    }
                }

                if (!match) isOk = true;
                else retVal = newName + " #" + newSuffix;
            } while (!isOk);

            return retVal;
        }

        private void tvSites_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var curTag = ((TreeViewItem)tvSites.Items[0]).Items.IndexOf(tvSites.SelectedItem);
            if (curTag > -1)
            {
                DontWatchValues = true;
                var site = ftpSites[curTag];
                txtName.Text = site.Name;
                txtHost.Text = site.Host;
                txtPort.Text = site.Port.ToString();
                txtHttpUrl.Text = site.WebUrl;
                rbModeActive.IsChecked = site.Mode == 1;
                rbModePassive.IsChecked = site.Mode == 2;
                txtUser.Text = site.User;
                txtPassword.Password = site.Password;
                txtRootFolder.Text = site.RootFolder;
                txtUploadFolder.Text = site.UploadFolder;
                //txtName.Focus();
                DontWatchValues = false;
            }

            DisableForm(curTag == -1);
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearAllErrors();
            lblCheckConnection.Foreground = Brushes.Black;
            if (string.IsNullOrEmpty(txtHost.Text)) { GiveError(txtHost); }
            else if (!arunes.Functions.IsValidNumber(txtPort.Text)) { GiveError(txtPort); }
            else if (string.IsNullOrEmpty(txtUser.Text)) { GiveError(txtUser); }
            else if (string.IsNullOrEmpty(txtPassword.Password)) { GiveError(txtPassword); }
            else
            {
                var curTag = ((TreeViewItem)tvSites.Items[0]).Items.IndexOf(tvSites.SelectedItem);
                if (curTag > -1)
                {
                    lblCheckConnection.Content = Lang.CheckingFtpConnection;
                    lblCheckConnection.Visibility = System.Windows.Visibility.Visible;
                    btnCancel.IsEnabled = btnCheck.IsEnabled = btnOk.IsEnabled = false;
                    DoEvents();
                    Codes.FtpHelper fh = new Codes.FtpHelper(ftpSites[curTag]);
                    if (fh.CheckConnection())
                    {
                        lblCheckConnection.Content = Lang.FtpConnectionSuccessfully;
                        lblCheckConnection.Foreground = Brushes.Green;
                    }
                    else
                    {
                        lblCheckConnection.Content = Lang.FtpConnectionError;
                        lblCheckConnection.Foreground = Brushes.Red;
                    }

                    btnCancel.IsEnabled = btnCheck.IsEnabled = btnOk.IsEnabled = true;
                }
            }
        }


    }
}
