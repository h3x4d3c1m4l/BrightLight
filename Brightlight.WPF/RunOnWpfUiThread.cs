using System;
using BrightLight.PluginInterface;

namespace BrightLight.WPF
{
    class RunOnWpfUiThread : IRunOnUiThreadHelper
    {
        public void RunOnUIThread(Action action)
        {
            Global.MainWindow?.Dispatcher.Invoke(action);
        }
    }
}
