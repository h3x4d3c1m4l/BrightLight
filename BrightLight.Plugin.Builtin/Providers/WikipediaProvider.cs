using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading;
using BrightLight.PluginInterface.Result;
using BrightLight.PluginInterface;
using Microsoft.AspNetCore.WebUtilities;

namespace BrightLight.Plugin.Builtin.Providers
{
    public class WikipediaProvider : ISearchProvider
    {
        private HttpClient _client;

        private const string url = "https://www.wikipedia.org/w/api.php";

        public WikipediaProvider()
        {
            _client = new HttpClient();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            var parametersToAdd = new Dictionary<string, string> { { "action", "query" }, { "list", "search" }, { "srsearch", query.QueryString }, { "format", "json" } };
            var uri = QueryHelpers.AddQueryString(url, parametersToAdd);

            var getResult = await _client.GetAsync(uri, ct);
            var resultStr = await getResult.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<WikipediaSearchResult>(resultStr);

            var resultObj = new SearchResultCollection
            {
                Title = "Wikipedia",
                FontAwesomeIcon = "wikipedia-w"
            };
            resultObj.Results = new ObservableCollection<SearchResult>(result.Query.Search.Select(x => new UrlSearchResult
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
