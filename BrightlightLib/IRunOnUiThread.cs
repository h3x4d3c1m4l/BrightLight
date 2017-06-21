using System;

namespace BrightlightLib
{
    public interface IRunOnUiThread
    {
        void RunOnUIThread(Action action);
    }
}