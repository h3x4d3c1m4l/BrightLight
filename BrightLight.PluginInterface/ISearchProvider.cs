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
        Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct);
    }
}
