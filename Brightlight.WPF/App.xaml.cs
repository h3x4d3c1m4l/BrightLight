using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Brightlight.WPF.Utils;
using BrightlightLib;
using BrightlightLib.ViewModels;
using CefSharp;

namespace Brightlight.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private HotKey _showHotKey; // TODO: config window to make this a custom set hotkey

        private App()
        {
            Global.MainViewModel = new MainViewModel();
            _showHotKey = new HotKey(Key.Space, KeyModifier.Alt, key =>
            {
                Global.MainViewModel.Query = string.Empty;
                Global.MainWindow.ShowMe();
            });

            Cef.Initialize();
            ShortcutUtils.GetShortcutTarget = WindowsShortcutUtils.GetTargetPath;
        }
    }
}
