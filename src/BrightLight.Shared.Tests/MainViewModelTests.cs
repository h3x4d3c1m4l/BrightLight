using BrightLight.PluginInterface;
using BrightLight.Shared.ViewModels;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BrightLight.Shared.Tests
{
    public class MainViewModelTests
    {
        MainViewModel _mainViewModel;

        public MainViewModelTests()
        {
            // mock IRunOnUiThreadHelper
            var iRunOnUiThreadMock = new Mock<IRunOnUiThreadHelper>(MockBehavior.Strict);
            iRunOnUiThreadMock.Setup(x => x.RunOnUIThread(It.IsAny<Action>())).Callback<Action>(action => action());

            _mainViewModel = new MainViewModel(iRunOnUiThreadMock.Object);
        }

        [Fact]
        public async void SearchingShouldStartWhenQueryStringIsNotEmptyFor500Miliseconds()
        {
            // a Q&A engineer walks into a bar...

            // ... orders <null>
            _mainViewModel.Query = null;
            await Task.Delay(600);
            Assert.False(_mainViewModel.Searching);

            // ... orders some whitespace
            _mainViewModel.Query = "  \t \r\n";
            await Task.Delay(600);
            Assert.False(_mainViewModel.Searching);

            // ... orders a lizard
            _mainViewModel.Query = "a lizard";
            await Task.Delay(600);
            Assert.True(_mainViewModel.Searching);
        }
    }
}
