using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BigRedButton
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MuteButtonService service;
        TaskbarIcon notifyIcon;
        MainWindow window;

        protected override void OnExit(ExitEventArgs e)
        {
            using (service) { }
            using (notifyIcon) { }; //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

        public void ShowMainWindow()
        {
            if (window == null)
            {
                window = new BigRedButton.MainWindow();
                window.Closed += OnWindowClosed;
            }

            window.Show();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            window = null;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            service = new MuteButtonService();
            service.Start();
            Install();

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            if (e.Args.Length > 0 && e.Args[0] == "-background")
            {
                return;
            }

            ShowMainWindow();
        }

        public MuteButtonService MuteButtonService => service;

        private void Install()
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                var path = this.GetType().Assembly.Location;
                key.SetValue("BigRedButton", path + " -background");
            }
        }
    }
}
