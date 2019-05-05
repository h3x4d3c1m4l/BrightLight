using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BrightLight.Shared.ViewModels;
using BrightLight.DesktopApp.WPF.UI.Windows;

namespace BrightLight.DesktopApp.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private HotKey _showHotKey; // TODO: config window to make this a custom set hotkey

        private Mutex _mutex;

        private App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _mutex = new Mutex(false, $"h3x4d3c1m4l BrightLight - {Environment.UserName}", out bool createdNew);
            if (!createdNew)
            {
                // app already running
                MessageBox.Show("BrightLight is already running!", "BrightLight - Already running", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);
            }

            InitializeComponent();

            Global.MainViewModel = new MainViewModel(new RunOnWpfUiThread());
            MainWindow = Global.MainWindow = new MainWindow();
            Global.SettingsWindow = new SettingsWindow();
            _showHotKey = new HotKey(Key.Space, KeyModifier.Alt, key =>
            {
                Global.MainViewModel.Query = string.Empty;
                Global.MainWindow.ShowMe();
            });
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GC.KeepAlive(_mutex); // don't GC our mutex
            var ex = (Exception)e.ExceptionObject;
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var logDir = Path.Combine(programData, "BrightLight", "log");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
            var logfilePath = Path.Combine(logDir, $"UE-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.log");
            File.WriteAllText(logfilePath, ex.ToString());

            MessageBox.Show($"A fatal error occured in BrightLight and the application will be closed. A log file has been written to disk to help the developers fix the issue.\r\n\r\n{logfilePath}", "BrightLight - Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

