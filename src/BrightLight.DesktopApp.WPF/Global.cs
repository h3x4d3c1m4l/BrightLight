using BrightLight.DesktopApp.WPF.UI.Windows;
using BrightLight.Shared.ViewModels;

namespace BrightLight.DesktopApp.WPF
{
    public static class Global
    {
        public static MainViewModel MainViewModel { get; set; }
        public static SettingsViewModel SettingsViewModel { get; set; }
        public static MainWindow MainWindow { get; set; }
        public static SettingsWindow SettingsWindow { get; set; }
    }
}
