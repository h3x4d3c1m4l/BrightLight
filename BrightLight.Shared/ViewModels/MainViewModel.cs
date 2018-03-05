using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BrightLight.PluginInterface.Result;
using BrightLight.PluginInterface;
using System.Reflection;

namespace BrightLight.Shared.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IRunOnUiThreadHelper _runOnUiThreadHelper;

        private string query;

        public string Query
        {
            get => query;
            set
            {
                if (query == value) return;
                query = value;
                if (_searchCancellationTokenSource != null && !_searchCancellationTokenSource.IsCancellationRequested)
                    _searchCancellationTokenSource.Cancel();
                _runOnUiThreadHelper.RunOnUIThread(() =>
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
            if (SelectedSearchResult == null) return false;
            var result = SelectedSearchResult;
            if (!(result is UrlSearchResult) && !string.IsNullOrWhiteSpace(result.LaunchPath))
            {
                var processStartInfo = new ProcessStartInfo(result.LaunchPath, result.LaunchArguments) {UseShellExecute = true};
                Process.Start(processStartInfo);
                return true;
            }
            if (result is UrlSearchResult urlResult)
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
            var q = queryString ?? query;
            if (string.IsNullOrWhiteSpace(q)) return;
            _runOnUiThreadHelper.RunOnUIThread(() =>
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
                        result = await sp.SearchAsync(new SearchQuery {QueryString = q}, token.Token);
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
                            _runOnUiThreadHelper.RunOnUIThread(() => SearchResultCollections.Add(result));
                            if (SelectedSearchResult == null || (result.Results.Any() && SelectedSearchResult.ParentCollection.Relevance < result.Relevance))
                                SelectedSearchResult = result.Results.First(); // new result more important than old selected one so show the user the new result instead
                        }
                        Interlocked.Increment(ref n);
                        if (n == searchProviders.Count)
                            _runOnUiThreadHelper.RunOnUIThread(() => Searching = false);
                    }
                }, token.Token);
            }
        }

        public void PrepareApplicationExit()
        {
            // nothing to deinit yet
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
                return x;
            }
        }

        public MainViewModel(IRunOnUiThreadHelper runOnUiThreadHelper)
        {
            _runOnUiThreadHelper = runOnUiThreadHelper;
            Query = "";

            // throttle Query change
            Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                      .Where(x => x.EventArgs.PropertyName == "Query")
                      .Select(_ => Query)
                      .Throttle(TimeSpan.FromMilliseconds(500))
                      .Subscribe(StartQuerying);

            // get all search providers
            // http://codesnippets.fesslersoft.de/get-all-types-that-implement-a-specific-interface/
            var pluginAssemblies = new List<Assembly>
            {
                Assembly.Load("BrightLight.Plugin.Builtin")
                // TODO: load other plugins
            };

            var searchProviders = new List<ISearchProvider>();
            foreach (var a in pluginAssemblies)
            {
                var currentAssemblyPluginTypes = a.GetTypes().Where(y => typeof(IPlugin).IsAssignableFrom(y) && !y.IsInterface);

                foreach (var p in currentAssemblyPluginTypes)
                {
                    var plugin = (IPlugin)Activator.CreateInstance(p);
                    var pluginSearchProviders = plugin.Init(runOnUiThreadHelper);
                    foreach (var pSP in pluginSearchProviders)
                    {
                        searchProviders.Add(pSP);
                    }
                }
            }
            
            this.searchProviders = searchProviders;
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
