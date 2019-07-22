using System;
using System.Threading.Tasks;
using System.Threading;
using BrightLight.PluginInterface.Result;

namespace BrightLight.PluginInterface
{
    /// <summary>
    /// Interface for a search provider. e.g. WikipediaSearchProvider that searched Wikipedia for articles that match the search query.
    /// </summary>
    public interface ISearchProvider : IDisposable
    {
        string Name { get; }

        string Description { get; }

        Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct);
    }

    public interface ISearchProvider<T> : ISearchProvider
    {
        T Settings { get; set; }
    }
}
