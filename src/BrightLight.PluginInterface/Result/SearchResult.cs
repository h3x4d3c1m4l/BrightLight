using BrightLight.PluginInterface.Result.Helpers;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace BrightLight.PluginInterface.Result
{
    public abstract class SearchResult : INotifyPropertyChanged
    {
        private IImage _icon;

        public SearchResultCollection ParentCollection { get; set; }

        public string Title { get; set; }

        public string FontAwesomeIcon { get; set; } // e.g. wikipedia-w

        public IImage Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        } // e.g. extracted application executable icon

        public string LaunchPath { get; set; }

        public string LaunchArguments { get; set; }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
