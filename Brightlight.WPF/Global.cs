using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brightlight.WPF.UI;
using BrightlightLib.ViewModels;

namespace Brightlight.WPF
{
    public static class Global
    {
        public static MainViewModel MainViewModel { get; set; }
        public static MainWindow MainWindow { get; set; }
    }
}
