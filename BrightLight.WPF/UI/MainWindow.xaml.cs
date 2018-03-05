﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrightLight.Shared.ViewModels;
using BrightLight.PluginInterface.Result;
using CefSharp;
using CefSharp.Wpf;
using Image = System.Windows.Controls.Image;
using System.Windows.Forms;
using ListView = System.Windows.Controls.ListView;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace BrightLight.WPF.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowInteropHelper _windowInteropHelper;

        public int TopWhenNoResults { get; set; }

        public int TopWhenResults { get; set; }

        public MainWindow()
        {
            _windowInteropHelper = new WindowInteropHelper(this);
            InitializeComponent();
            Global.MainViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(MainViewModel.QueryNotEmpty)) return;
                Storyboard sb;
                if (Global.MainViewModel.QueryNotEmpty)
                    sb = FindResource("ShowResultsAnimation") as Storyboard;
                else
                    sb = FindResource("HideResultsAnimation") as Storyboard;
                sb.Begin();
            };
            Global.MainWindow = this;
        }

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new DWMBlurHack.AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = DWMBlurHack.AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new DWMBlurHack.WindowCompositionAttributeData();
            data.Attribute = DWMBlurHack.WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            DWMBlurHack.SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
            SearchTermTextBox.Focus();
        }

        private void HtmlRendererInitialized(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Global.MainViewModel.SelectedSearchResult is HtmlSearchResult htmlResult)
            {
                var html = htmlResult.Html;
                var browser = (ChromiumWebBrowser) sender;

                browser.LoadHtml(html);
            }
        }

        private void KeyboardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var somethingAsHappened = Global.MainViewModel.ExecuteActionFromSearchResult();
                if (somethingAsHappened)
                    HideMe();
                e.Handled = true;
            }
            if (e.Key == Key.Escape)
            {
                HideMe();
            }
        }
        
        public void ShowMe()
        {
            moveApplicationToCenterOfCurrentScreen();

            // detect on which
            Show();
            (FindResource("ShowMe") as Storyboard).Begin(Border);
            Activate();
            SearchTermTextBox.Focus();
        }

        protected override void OnDpiChanged(DpiScale oldDpiScale, DpiScale newDpiScale)
        {
            moveApplicationToCenterOfCurrentScreen();
        }

        private void moveApplicationToCenterOfCurrentScreen()
        {
            var cursorPos = System.Windows.Forms.Cursor.Position;
            var screen = Screen.FromPoint(new System.Drawing.Point(cursorPos.X, cursorPos.Y));
            var screenBounds = screen.Bounds;

            var dpiInfo = VisualTreeHelper.GetDpi(this);
            Left = (int)((screenBounds.Width / dpiInfo.DpiScaleX / 2) - (Width / 2) + screenBounds.X / dpiInfo.DpiScaleX);
            TopWhenNoResults = (int) ((screenBounds.Height / dpiInfo.DpiScaleY / 2) - (60 / 2) + screenBounds.Y);
            TopWhenResults = (int) ((screenBounds.Height / dpiInfo.DpiScaleY / 2) - (500 / 2) + screenBounds.Y);
            Top = TopWhenNoResults;
        }

        private void HideMe()
        {
            (FindResource("HideMe") as Storyboard).Begin(Border);
        }

        private void HideMeCompleted(object sender, EventArgs e)
        {
            Hide();
            Global.MainViewModel.Query = string.Empty;
        }

        private void ResultListViewOnLostFocus(object sender, RoutedEventArgs e)
        {
            var lv = (ListView) sender;
            lv.SelectedItems.Clear();
        }

        private void OpenSearchClick(object sender, RoutedEventArgs e)
        {
            ShowMe();
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Global.MainViewModel.PrepareApplicationExit();
            Close();
        }

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {
            Global.SettingsWindow.Show();
        }
    }
}
