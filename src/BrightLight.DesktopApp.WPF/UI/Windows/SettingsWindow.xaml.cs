using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BrightLight.DesktopApp.WPF.UI.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            //var view = new MainView();
            //view.AttachedToVisualTree += delegate
            //{
            //    ((TopLevel)view.GetVisualRoot()).AttachDevTools();
            //};
            //Host.Content = view;
            //var btn = (Avalonia.Controls.Button)RightBtn.Content;
            //btn.Click += delegate
            //{
            //    btn.Content += "!";
            //};
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
