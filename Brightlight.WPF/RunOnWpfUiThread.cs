using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BrightlightLib;

namespace Brightlight.WPF
{
    class RunOnWpfUiThread : IRunOnUiThread
    {
        public void RunOnUIThread(Action action)
        {
            Global.MainWindow.Dispatcher.Invoke(action);
        }
    }
}
