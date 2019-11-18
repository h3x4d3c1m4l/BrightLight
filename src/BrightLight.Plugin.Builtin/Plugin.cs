using BrightLight.PluginInterface.Result.Helpers;
using BrightLight.Plugin.Builtin.Providers;
using BrightLight.PluginInterface;
using System;
using System.Collections.Generic;
using BrightLight.Plugin.Builtin.Localization;

namespace BrightLight.Plugin.Builtin
{
    public class Plugin : IPlugin
    {
        public string Name => Resources.PluginName;
        public string VersionName => null;
        public string Author => "Sander \"h3x4d3c1m4l\" in 't Hout";
        public IImage Logo => null;
        public string Description => null;
        public string Website => null;
    }
}
