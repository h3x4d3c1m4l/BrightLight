using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BrightLight.Shared
{
    public class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
