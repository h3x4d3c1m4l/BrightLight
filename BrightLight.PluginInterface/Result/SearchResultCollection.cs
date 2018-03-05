using BrightLight.PluginInterface.Result.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace BrightLight.PluginInterface.Result
{
    public enum SearchResultRelevance
    {
        ONTOP = 1,
        DEFAULT = 0,
        HIDE = -1
    }

    public class SearchResultCollection : INotifyPropertyChanged
    {
        public SearchResultRelevance Relevance { get; set; }

        public string FontAwesomeIcon { get; set; } // e.g. wikipedia-w

        public IImage Icon { get; set; } // e.g. extracted application executable icon

        public string Title { get; set; }

        public ObservableCollection<SearchResult> Results { get; set; }

        public bool Busy { get; set; }

        public bool Failed { get; set; }

        public Exception FailedException { get; set; }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
