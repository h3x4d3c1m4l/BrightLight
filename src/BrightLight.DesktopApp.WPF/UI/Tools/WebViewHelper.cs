using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace BrightLight.DesktopApp.WPF.UI.Tools
{
    /// <summary>
    /// Helper class for Microsoft.Toolkit.Wpf.UI.Controls.WebViewCompatible.
    /// This is needed because navigation using a binding on the Source property does not work.
    /// </summary>
    public class WebViewHelper
    {
        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.RegisterAttached("Body", typeof(string), typeof(WebViewHelper), new PropertyMetadata(OnBodyChanged));

        public static string GetBody(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(BodyProperty);
        }

        public static void SetBody(DependencyObject dependencyObject, string body)
        {
            dependencyObject.SetValue(BodyProperty, body);
        }

        private static void OnBodyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = (WebView)d;
            webBrowser.NavigateToString((string)e.NewValue);
        }

        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.RegisterAttached("Url", typeof(string), typeof(WebViewHelper), new PropertyMetadata(OnUrlChanged));

        public static string GetUrl(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(UrlProperty);
        }

        public static void SetUrl(DependencyObject dependencyObject, string url)
        {
            dependencyObject.SetValue(BodyProperty, url);
        }

        private static void OnUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webView = (WebViewCompatible)d;
            webView.Dispatcher.BeginInvoke(new Action(() =>
            {
                webView.Navigate((string)e.NewValue);
            }));
        }
    }

}
