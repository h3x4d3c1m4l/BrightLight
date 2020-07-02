using BrightLight.Plugin.Builtin.Providers;
using BrightLight.PluginInterface;
using BrightLight.PluginInterface.Result;
using BrightLight.PluginInterface.Result.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace BrightLight.Plugin.Builtin.Tests
{
    public class PathExecutablesProviderTests : IDisposable
    {
        private bool _disposedValue;

        private ISearchProvider _searchProvider;

        public PathExecutablesProviderTests()
        {
            _searchProvider = new PathExecutablesProvider();
        }

        [Fact]
        public async void CmdExeShouldBeFoundAndHaveCorrectIcon()
        {
            var results = await _searchProvider.SearchAsync(new SearchQuery { QueryString = "cmd.exe" }, CancellationToken.None);
            Assert.Collection(results.Results, result =>
            {
                Assert.Null(result.FontAwesomeIcon);
                Assert.Null(result.LaunchArguments);
                var cmdPath = Path.Combine(Environment.SystemDirectory, "cmd.exe");
                Assert.Equal(result.LaunchPath, cmdPath, true);
                Assert.Equal(result.Title, cmdPath, true);

                var icon = result.Icon as WindowsPEResourceIcon;
                Assert.Equal(icon.FilePath, cmdPath, true);
            });
        }

        [Fact]
        public async void RandomGuidShouldReturnEmptyResultSet()
        {
            var results = await _searchProvider.SearchAsync(new SearchQuery { QueryString = Guid.NewGuid().ToString() }, CancellationToken.None);
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
