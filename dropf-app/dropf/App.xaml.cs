using System;
using System.Windows;
using SingleInstanceApplication;
using System.Reflection;

namespace dropf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // register single instance app. and check for existence of other process
            if (!ApplicationInstanceManager.CreateSingleInstance(
                    Assembly.GetExecutingAssembly().GetName().Name,
                    SingleInstanceCallback)) return; // exit, if same app. is running

            base.OnStartup(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Activated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var win = MainWindow as Drop;
            if (win == null) return;

            // add 1st args
            win.ApendArgs(Environment.GetCommandLineArgs(), true);
        }

        /// <summary>
        /// Single instance callback handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SingleInstanceApplication.InstanceCallbackEventArgs"/> instance containing the event data.</param>
        private void SingleInstanceCallback(object sender, InstanceCallbackEventArgs args)
        {
            if (args == null || Dispatcher == null) return;
            Action<bool> d = (bool x) =>
            {
                var win = MainWindow as Drop;
                if (win == null) return;

                win.ApendArgs(args.CommandLineArgs, false);
                win.Activate(x);
            };
            Dispatcher.Invoke(d, true);
        }
    }
}
