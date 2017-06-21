using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using BrightlightLib.Annotations;

namespace BrightlightLib.Models
{
    public enum SearchResultTextFormat
    {
        UNKNOWN = 0,
        PLAINTEXT = 1,
        HTML = 2,
        URL = 3
    }

    public class SearchResult : INotifyPropertyChanged
    {
        private Bitmap _icon;
        public SearchResultCollection ParentCollection { get; set; }

        public string Title { get; set; }

        public string FontAwesomeIcon { get; set; } // e.g. wikipedia-w

        public Bitmap Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        } // e.g. extracted application executable icon

        public string LaunchExePath { get; set; }

        public string LaunchParameters { get; set; }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class HtmlResult : SearchResult
    {
        public string Html { get; set; }
    }

    public class TextResult : SearchResult
    {
        public string Text { get; set; }
    }

    public class MathResult : SearchResult
    {
        public string LaTeXExpression { get; set; }

        public string LaTeXEvaluated { get; set; }
    }

    public class UrlResult : SearchResult
    {
        public string Url { get; set; }
    }

    public class ExecutableResult : SearchResult
    {
    }
}
