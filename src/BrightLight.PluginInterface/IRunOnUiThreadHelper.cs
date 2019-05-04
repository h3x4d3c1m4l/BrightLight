using System;

namespace BrightLight.PluginInterface
{
    public interface IRunOnUiThreadHelper
    {
        void RunOnUIThread(Action action);
    }
}