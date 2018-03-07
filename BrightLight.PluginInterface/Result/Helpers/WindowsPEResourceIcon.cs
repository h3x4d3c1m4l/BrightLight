using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BrightLight.PluginInterface.Result.Helpers
{
    public class WindowsPEResourceIcon : IImage
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string FilePath { get; set; }

        public int Index { get; set; }

        public void Dispose()
        {
        }
    }
}
