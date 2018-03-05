using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BrightLight.PluginInterface.Result.Helpers
{
    public class ExecutableIcon : IImage
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ExecutablePath { get; set; }

        public void Dispose()
        {
        }
    }
}
