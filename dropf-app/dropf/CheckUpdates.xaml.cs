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

namespace dropf
{
    /// <summary>
    /// Interaction logic for CheckUpdates.xaml
    /// </summary>
    public partial class CheckUpdates : Window
    {
        public CheckUpdates()
        {
            InitializeComponent();
        }

        System.Timers.Timer chkTimer = new System.Timers.Timer();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chkTimer.Interval = 100;
            chkTimer.Enabled = true;
            chkTimer.Elapsed += new System.Timers.ElapsedEventHandler(chkTimer_Elapsed);
        }

        void chkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            chkTimer.Enabled = false;

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    (Action)(() =>
                    {
                        tbDescription.Text = Lang.YouAreUsingLastVersion;
                        return; // TODO: check updates

                        try
                        {
                            Codes.CheckUpdate cu = new Codes.CheckUpdate();
                            if (cu.NewVersionFound)
                            {
                                tbDescription.Text = string.Format(Lang.NewVersionFound, cu.LastVersion);
                                txtWhatsNew.Text = cu.WhatsNew;
                                btnDownload.IsEnabled = true;
                                btnDownload.Click += delegate { System.Diagnostics.Process.Start(cu.DownloadLink); this.Close(); };
                            }
                            else
                                tbDescription.Text = Lang.YouAreUsingLastVersion;
                        }
                        catch
                        {
                            tbDescription.Text = Lang.CheckForUpdatesFail;
                        }
                    }));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
