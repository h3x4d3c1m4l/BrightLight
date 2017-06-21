using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrightlightLib.Models;

namespace BrightlightLib.SearchProviders
{
    class TestProvider : ISearchProvider
    {
        public void Dispose()
        {
        }

        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            var results = new SearchResultCollection
            {
                Title = "Test results!!",
                Relevance = SearchResultRelevance.DEFAULT,
                //FontAwesomeIcon = "exclamation-circle"
            };

            results.Results = new ObservableCollection<SearchResult>
            {
                new TextResult
                {
                    ParentCollection = results,
                    Title = "Sample text",
                    Text =
                        "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\" \"Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?\""
                },
                new UrlResult
                {
                    ParentCollection = results,
                    Title = "Sample url",
                    Url = "https://nl.wikipedia.org/wiki/Lorem_ipsum"
                },
                new ExecutableResult
                {
                    ParentCollection = results,
                    Title = "Sample exe",
                    LaunchExePath = "C:\\loremipsum.exe",
                    LaunchParameters = "paragraph1 paragraph2"
                },
                new HtmlResult
                {
                    ParentCollection = results,
                    Title = "Sample HTML",
                    Html = "<p>Hoi hoi test</p>"
                }
            };

            return results;
        }
    }
}
