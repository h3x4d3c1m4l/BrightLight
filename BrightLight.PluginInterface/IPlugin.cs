using Brightlight.PluginInterface.Result.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrightLight.PluginInterface
{
    public interface IPlugin
    {
        string Name { get; }

        string VersionName { get; }

        string Author { get; }

        IImage Logo { get; }

        IReadOnlyCollection<ISearchProvider> Init(IRunOnUiThreadHelper runOnUiThreadHelper);
    }
}
