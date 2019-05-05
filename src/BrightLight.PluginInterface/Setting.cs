using System;
using System.Collections.Generic;
using System.Text;

namespace BrightLight.PluginInterface
{
    public class SettingAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
