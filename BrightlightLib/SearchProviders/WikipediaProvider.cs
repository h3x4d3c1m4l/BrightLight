using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using BrightlightLib.Models;

namespace BrightlightLib.SearchProviders
{
    public class WikipediaProvider : ISearchProvider
    {
        private HttpClient _client;

        private const string url = "https://www.wikipedia.org/w/api.php";

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                disposedValue = true;
            }
        }

        public WikipediaProvider()
        {
            _client = new HttpClient();
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            var parametersToAdd = new Dictionary<string, string> { { "action", "query" }, { "list", "search" }, { "srsearch", query.Query }, { "format", "json" } };
            var uri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(url, parametersToAdd);

            var getResult = await _client.GetAsync(uri, ct);
            var resultStr = await getResult.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<WikipediaSearchResult>(resultStr);

            var resultObj = new SearchResultCollection
            {
                Title = "Wikipedia",
                FontAwesomeIcon = "wikipedia-w"
            };
            resultObj.Results = new ObservableCollection<SearchResult>(result.Query.Search.Select(x => new UrlResult
            {
                Title = x.Title,
                Url = "https://en.m.wikipedia.org/wiki/" + x.Title,
                ParentCollection = resultObj
            }));
            return resultObj;
        }
    }

    public class WikipediaSearchResult
    {
        [JsonProperty("query")]
        public WikipediaSearchResultQuery Query { get; set; }
    }

    public class WikipediaSearchResultQuery
    {
        [JsonProperty("search")]
        public List<WikipediaSearchResultItem> Search { get; set; }
    }

    public class WikipediaSearchResultItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }
    }
}
