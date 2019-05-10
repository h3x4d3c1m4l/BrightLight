using BrightLight.PluginInterface.Result.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrightLight.PluginInterface
{
    /// <summary>
    /// Interface for internal and external plugins.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Name of the plugin. For example "Movie, music album websites"
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the plugin. For example "Search websites like IMDB, AllMusic.com, etc"
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Version number based on Semantic Versioning principles.
        /// </summary>
        string VersionName { get; }

        /// <summary>
        /// Your name.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Website where the plugin can be found.
        /// </summary>
        string Website { get; }

        /// <summary>
        /// TODO: unused at the moment
        /// </summary>
        IImage Logo { get; }
    }

    public interface IPlugin<T> : IPlugin
    {
        T Settings { get; set; }
    }
}
