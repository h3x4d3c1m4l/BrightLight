using Microsoft.Toolkit.Wpf.UI.Controls;
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

namespace BrightLight.WPF.UI
{
    /// <summary>
    /// Interaction logic for WebViewWrapper.xaml
    /// </summary>
    public partial class WebViewWrapper : UserControl
    {
        private static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(Uri),
            typeof(WebViewWrapper),
            new FrameworkPropertyMetadata(new Uri("about:blank"), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((WebViewWrapper) dependencyObject).WebView.Navigate(dependencyPropertyChangedEventArgs.NewValue as string);
        }

        public Uri Source
        {
            get
            {
                VerifyAccess();
                return (Uri)GetValue(SourceProperty);
            }

            set
            {
                VerifyAccess();
                SetValue(SourceProperty, value);

                //if (_webView.InitializationState == InitializationState.IsInitialized)
                {
                    WebView.Navigate(value);
                }
            }
        }

        public WebViewWrapper()
        {
            InitializeComponent();
        }
    }
}
