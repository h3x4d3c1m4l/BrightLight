using System;
using System.Windows.Threading;
using BrightLight.PluginInterface;

namespace BrightLight.DesktopApp.WPF
{
    class RunUsingWpfDispatcherHelper : IRunOnUiThreadHelper
    {
        private Dispatcher _dispatcher;

        public RunUsingWpfDispatcherHelper(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void RunOnUIThread(Action action)
        {
            _dispatcher.Invoke(action);
        }
    }
}
