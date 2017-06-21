using System;
using System.Threading.Tasks;
using System.Threading;
using BrightlightLib.Models;

namespace BrightlightLib
{
    public interface ISearchProvider : IDisposable
    {
        Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct);
    }
}
