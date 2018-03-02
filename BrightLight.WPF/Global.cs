using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightLight.WPF.UI;
using BrightLight.Shared.ViewModels;

namespace BrightLight.WPF
{
    public static class Global
    {
        public static MainViewModel MainViewModel { get; set; }
        public static MainWindow MainWindow { get; set; }
    }
}
