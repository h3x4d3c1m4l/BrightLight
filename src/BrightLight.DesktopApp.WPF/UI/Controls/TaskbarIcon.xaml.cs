using BrightLight.DesktopApp.WPF.UI.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrightLight.DesktopApp.WPF.UI.Controls
{
    /// <summary>
    /// Interaction logic for TaskbarIcon.xaml
    /// </summary>
    public partial class TaskbarIcon : UserControl
    {
        SearchWindow _searchWindow;

        SettingsWindow _settingsWindow;

        public TaskbarIcon(SearchWindow searchWindow, SettingsWindow settingsWindow)
        {
            InitializeComponent();
            _searchWindow = searchWindow;
            _settingsWindow = settingsWindow;
        }

        private void OpenSearchClick(object sender, RoutedEventArgs e)
        {
            _searchWindow.Show();
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            _searchWindow.Close();
        }

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {
            _settingsWindow.Show();
        }
    }
}
