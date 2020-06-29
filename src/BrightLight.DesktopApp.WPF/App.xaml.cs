using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BrightLight.Shared.ViewModels;
using BrightLight.DesktopApp.WPF.UI.Windows;
using BrightLight.PluginInterface;
using BrightLight.DesktopApp.WPF.UI.Controls;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Themes.Default;
using BrightLight.DesktopApp.WPF.UI.Tools;
using System.Diagnostics;
using Application = System.Windows.Application;
using Container = SimpleInjector.Container;
using BrightLight.Shared;

namespace BrightLight.DesktopApp.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private HotKey _showHotKey; // TODO: config window to make this a custom set hotkey

        private Mutex _mutex;

        private static Container Container;

        private App()
        {
            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _mutex = new Mutex(false, $"h3x4d3c1m4l BrightLight - {Environment.UserName}", out bool createdNew);
            if (!createdNew)
            {
                // app already running
                MessageBox.Show("BrightLight is already running!", "BrightLight - Already running", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);
            }

            InitializeComponent();
            AppBuilder.Configure<AvaloniaApp>().UseWin32().UseDirect2D1().SetupWithoutStarting();

            var test = new SearchWindow(new MainViewModel(new RunUsingWpfDispatcherHelper(Dispatcher)));
            Container = new Container();
            Container.RegisterInstance(Dispatcher);
            Container.RegisterSingleton<IRunOnUiThreadHelper, RunUsingWpfDispatcherHelper>();
            Container.RegisterSingleton<MainViewModel>();
            Container.RegisterSingleton<SettingsViewModel>();
            Container.RegisterSingleton<SearchWindow>();
            Container.RegisterSingleton<SettingsWindow>();
            Container.RegisterSingleton<TaskbarIcon>();
            Container.RegisterSingleton(() => Settings.LoadOrDefault());
            Container.Verify();

            _showHotKey = new HotKey(Key.Space, KeyModifier.Ctrl | KeyModifier.Shift, key =>
            {
                Container.GetInstance<MainViewModel>().Reset();
                Container.GetInstance<SearchWindow>().Show();
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

        private class AvaloniaApp : Avalonia.Application
        {
            public override void Initialize()
            {
                Styles.Add(new DefaultTheme());
                var loader = new AvaloniaXamlLoader();
                var baseLight = (IStyle)loader.Load(
                    new Uri("resm:Avalonia.Themes.Default.Accents.BaseLight.xaml?assembly=Avalonia.Themes.Default"));
                Styles.Add(baseLight);
            }
        }
    }
}

