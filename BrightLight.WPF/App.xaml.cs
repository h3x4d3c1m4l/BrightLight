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

        private App()
        {
            Global.MainViewModel = new MainViewModel(new RunOnWpfUiThread());
            MainWindow = Global.MainWindow = new MainWindow();
            Global.SettingsWindow = new SettingsWindow();
            _showHotKey = new HotKey(Key.Space, KeyModifier.Alt, key =>
            {
                Global.MainViewModel.Query = string.Empty;
                Global.MainWindow.ShowMe();
            });

            Cef.Initialize();
        }
    }
}
