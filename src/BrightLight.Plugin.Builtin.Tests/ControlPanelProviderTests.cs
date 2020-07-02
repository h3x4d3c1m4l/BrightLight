using BrightLight.Plugin.Builtin.Providers;
using BrightLight.PluginInterface;
using BrightLight.PluginInterface.Result;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace BrightLight.Plugin.Builtin.Tests
{
    /// <summary>
    /// 
    /// Important note about these tests:
    ///
    /// Make sure to set Visual Studio test arch to x64! Otherwise the DisplayShouldGiveNonEmptyResultSet test will fail.
    ///
    /// </summary>
    public class ControlPanelProviderTests : IDisposable
    {
        private bool _disposedValue;

        private ISearchProvider _searchProvider;

        public ControlPanelProviderTests()
        {
            _searchProvider = new ControlPanelProvider();
        }

        [Fact]
        public async void SomeRandomInputStringShouldReturnEmptyResultSet()
        {
            var results = await _searchProvider.SearchAsync(new SearchQuery { QueryString = "Some Random String" }, CancellationToken.None);
            Assert.Empty(results.Results);
        }

        [Fact]
        public async void DisplayShouldGiveNonEmptyResultSet()
        {
            using var provider = new ControlPanelProvider();
            var results = await _searchProvider.SearchAsync(new SearchQuery { QueryString = "Display" }, CancellationToken.None);
            Assert.NotEmpty(results.Results);
        }

        [Fact]
        public async void EmptyAndNullQueryStringMayReturnNullOrEmptyResultSet()
        {
            SearchResultCollection results;

            using var provider = new ControlPanelProvider();
            results = await _searchProvider.SearchAsync(new SearchQuery { QueryString = string.Empty }, CancellationToken.None);
            Assert.Null(results?.Results?.FirstOrDefault());

            results = await _searchProvider.SearchAsync(new SearchQuery { QueryString = null }, CancellationToken.None);
            Assert.Null(results?.Results?.FirstOrDefault());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _searchProvider.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
