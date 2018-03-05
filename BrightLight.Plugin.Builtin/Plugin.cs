using BrightLight.PluginInterface.Result.Helpers;
using BrightLight.Plugin.Builtin.Providers;
using BrightLight.PluginInterface;
using System;
using System.Collections.Generic;

namespace BrightLight.Plugin.Builtin
{
    public class Plugin : IPlugin
    {
        public static IRunOnUiThreadHelper RunOnUiThreadHelper;

        public string Name => "Builtin";
        public string VersionName => "";
        public string Author => "Sander \"h3x4d3c1m4l\" in 't Hout";
        public IImage Logo => null;

        public IReadOnlyCollection<ISearchProvider> Init(IRunOnUiThreadHelper runOnUiThreadHelper)
        {
            RunOnUiThreadHelper = runOnUiThreadHelper;
            return new List<ISearchProvider>
            {
                new MathProvider(),
                new PathExecutablesProvider(),
                new StartMenuProvider(),
                new WikipediaProvider()
            };
        }
    }
}
