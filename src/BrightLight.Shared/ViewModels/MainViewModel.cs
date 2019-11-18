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
using System.IO;
using Container = SimpleInjector.Container;

namespace BrightLight.Shared.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IRunOnUiThreadHelper _runOnUiThreadHelper;

        private string _query;

        public string Query
        {
            get => _query;
            set
            {
                if (_query == value) return;
                _query = value;
                _runOnUiThreadHelper.RunOnUIThread(() =>
                {
                    ResetAllButQuery();
                });
                OnPropertyChanged();
                OnPropertyChanged(nameof(MayQuery));
            }
        }

        public void Reset()
        {
            Query = string.Empty;
            ResetAllButQuery();
        }

        private void ResetAllButQuery()
        {
            if (_searchCancellationTokenSource != null && !_searchCancellationTokenSource.IsCancellationRequested)
                _searchCancellationTokenSource.Cancel();
            SearchResultCollections.Clear();
            SelectedSearchResult = null;
            Searching = false;
        }

        private readonly IList<ISearchProvider> _searchProviders;

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
            if (result is ActionSearchResult actionResult)
            {
                actionResult.Action?.Invoke();
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
            var q = queryString ?? _query;
            if (string.IsNullOrWhiteSpace(q)) return;
            _runOnUiThreadHelper.RunOnUIThread(() =>
            {
                Searching = true;
            });

            var token = _searchCancellationTokenSource = new CancellationTokenSource();
            var n = 0;
            foreach (var sp in _searchProviders)
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
                        if (n == _searchProviders.Count)
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

        public bool MayQuery => !string.IsNullOrWhiteSpace(Query) && Query.Length > 1;

        private Container _pluginContainer;

        public MainViewModel(IRunOnUiThreadHelper runOnUiThreadHelper)
        {
            _runOnUiThreadHelper = runOnUiThreadHelper;
            Query = string.Empty;

            // plugin initialization
            // https://simpleinjector.readthedocs.io/en/latest/advanced.html#plugins
            var pluginDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

            var pluginAssemblies =
                from file in new DirectoryInfo(pluginDirectory).GetFiles("BrightLight.Plugin.*.dll")
                where !file.Name.EndsWith(".resources.dll")
                select Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));

            _searchProviders = new List<ISearchProvider>();
            _pluginContainer = new Container();
            _pluginContainer.RegisterInstance(runOnUiThreadHelper);
            _pluginContainer.Verify();
            _searchProviders = _pluginContainer.Collection.Create<ISearchProvider>(pluginAssemblies);

            // throttle Query change
            Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                      .Where(x => x.EventArgs.PropertyName == nameof(Query))
                      .Select(_ => Query)
                      .Throttle(TimeSpan.FromMilliseconds(500))
                      .Subscribe(StartQuerying);
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
