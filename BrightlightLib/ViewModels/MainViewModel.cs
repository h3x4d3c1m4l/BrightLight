using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BrightlightLib.SearchProviders;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BrightlightLib.Models;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

namespace BrightlightLib.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static IRunOnUiThread UIThreadHelper { get; set; }

        private string query;

        public string Query
        {
            get => query;
            set
            {
                if (query == value) return;
                query = value;
                UIThreadHelper?.RunOnUIThread(() =>
                {
                    // clear the search results
                    SearchResultCollections.Clear();
                    SelectedSearchResult = null;
                    Searching = false;
                });
                OnPropertyChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(QueryNotEmpty));
            }
        }

        private readonly List<ISearchProvider> searchProviders;

        private SearchResult _selectedSearchResult;

        private CancellationTokenSource _searchCancellationTokenSource;

        private bool _searching;

        public bool ExecuteActionFromSearchResult()
        {
            var result = SelectedSearchResult;
            if (!(result is UrlResult) && !string.IsNullOrWhiteSpace(result.LaunchExePath))
            {
                var processStartInfo = new ProcessStartInfo(result.LaunchExePath, result.LaunchParameters) {UseShellExecute = true};
                Process.Start(processStartInfo);
                return true;
            }
            if (result is UrlResult urlResult)
            {
                var processStartInfo = new ProcessStartInfo(urlResult.Url) { UseShellExecute = true };
                Process.Start(processStartInfo);
                return true;
            }
            return false;
        }

        public bool Searching
        {
            get => _searching;
            set
            {
                if (_searching == value) return;
                _searching = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SearchResultCollection> SearchResultCollections { get; } = new ObservableCollection<SearchResultCollection>();

        private void StartQuerying(string queryString)
        {
            if (_searchCancellationTokenSource != null && !_searchCancellationTokenSource.IsCancellationRequested)
                _searchCancellationTokenSource.Cancel();

            var q = queryString ?? query;
            if (string.IsNullOrWhiteSpace(q)) return;
            UIThreadHelper.RunOnUIThread(() =>
            {
                Searching = true;
            });

            var token = _searchCancellationTokenSource = new CancellationTokenSource();
            var n = 0;
            foreach (var sp in searchProviders)
            {
                Task.Run(async () =>
                {
                    SearchResultCollection result;
                    try
                    {
                        result = await sp.SearchAsync(new SearchQuery {Query = q}, token.Token);
                    }
                    catch (Exception ex)
                    {
                        result = new SearchResultCollection
                        {
                            Failed = true,
                            FailedException = ex,
                            Title = "Error: " + sp.GetType().Name
                        };
                    }
                    if (!token.IsCancellationRequested)
                    {
                        if (result != null && (result.Failed || result.Results.Any()))
                        {
                            UIThreadHelper.RunOnUIThread(() => SearchResultCollections.Add(result));
                            if (SelectedSearchResult == null || (result.Results.Any() && SelectedSearchResult.ParentCollection.Relevance < result.Relevance))
                                SelectedSearchResult = result.Results.First(); // new result more important than old selected one so show the user the new result instead
                        }
                        Interlocked.Increment(ref n);
                        if (n == searchProviders.Count)
                            UIThreadHelper.RunOnUIThread(() => Searching = false);
                    }
                }, token.Token);
            }
        }

        public SearchResult SelectedSearchResult
        {
            get => _selectedSearchResult;
            set {
                _selectedSearchResult = value;
                OnPropertyChanged();
            }
        }

        public bool QueryNotEmpty
        {
            get
            {
                var x = !string.IsNullOrWhiteSpace(Query);
                Console.WriteLine("Sander: " + x);
                return x;
            }
        }

        public MainViewModel()
        {
            Query = "";

            // throttle Query change
            Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                      .Where(x => x.EventArgs.PropertyName == "Query")
                      .Select(_ => Query)
                      .Throttle(TimeSpan.FromMilliseconds(500))
                      .Subscribe(StartQuerying);

            // get all search providers
            searchProviders = new List<ISearchProvider>
            {
                new MathProvider(),
                new WikipediaProvider(),
                //new TestProvider(),
                new NumberProvider(),
                new PathExecutablesProvider(),
                new StartMenuProvider()
            };
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
