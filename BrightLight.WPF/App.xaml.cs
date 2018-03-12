using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BrightLight.Shared.ViewModels;
using BrightLight.WPF.UI;
using CefSharp;

namespace BrightLight.WPF
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

            // https://github.com/cefsharp/CefSharp/issues/1714
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;

            Global.MainViewModel = new MainViewModel(new RunOnWpfUiThread());
            MainWindow = Global.MainWindow = new MainWindow();
            Global.SettingsWindow = new SettingsWindow();
            _showHotKey = new HotKey(Key.Space, KeyModifier.Alt, key =>
            {
                Global.MainViewModel.Query = string.Empty;
                Global.MainWindow.ShowMe();
            });

            InitializeCefSharp();
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings();

            // Set BrowserSubProcessPath based on app bitness at runtime
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe");

            // Make sure you set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }
    }
}

