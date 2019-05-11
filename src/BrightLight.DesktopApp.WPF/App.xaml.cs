using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BrightLight.Shared.ViewModels;
using BrightLight.DesktopApp.WPF.UI.Windows;
using Autofac;
using BrightLight.PluginInterface;
using Autofac.Core;
using BrightLight.DesktopApp.WPF.UI.Controls;
using Avalonia;
using Application = Avalonia.Application;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Themes.Default;
using BrightLight.DesktopApp.WPF.UI.Tools;

namespace BrightLight.DesktopApp.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private HotKey _showHotKey; // TODO: config window to make this a custom set hotkey

        private Mutex _mutex;

        private static IContainer Container;

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
            AppBuilder.Configure<AvaloniaApp>().UseWin32().UseDirect2D1().SetupWithoutStarting();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(Dispatcher);
            builder.RegisterType<RunUsingWpfDispatcherHelper>().As<IRunOnUiThreadHelper>().SingleInstance();
            builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<SettingsViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<MainWindow>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<SettingsWindow>().AsSelf().SingleInstance();
            builder.RegisterType<TaskbarIcon>().AsSelf().AutoActivate().SingleInstance();
            builder.Register((c) => Settings.LoadOrDefault()).AsSelf().SingleInstance();
            Container = builder.Build();

            Container.Resolve<SettingsWindow>().Show();

            _showHotKey = new HotKey(Key.Space, KeyModifier.Alt, key =>
            {
                Container.Resolve<MainViewModel>().Reset();
                Container.Resolve<MainWindow>().Show();
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

    public class AvaloniaApp : Application
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

