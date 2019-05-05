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
        public TaskbarIcon()
        {
            InitializeComponent();
        }
        
        private void OpenSearchClick(object sender, RoutedEventArgs e)
        {
            Global.MainWindow.ShowMe();
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Global.MainViewModel.PrepareApplicationExit();
            Global.MainWindow.Close();
        }

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {
            Global.SettingsWindow.Show();
        }
    }
}
