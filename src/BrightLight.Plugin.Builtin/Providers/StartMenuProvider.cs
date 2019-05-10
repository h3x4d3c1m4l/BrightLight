using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BrightLight.Plugin.Builtin;
using BrightLight.Plugin.Builtin.Localization;
using BrightLight.PluginInterface;
using BrightLight.PluginInterface.Result;
using BrightLight.PluginInterface.Result.Helpers;

namespace BrightLight.Plugin.Builtin.Providers
{
    public class StartMenuProvider : ISearchProvider
    {
        private string _allUsersStartMenuPath;

        private string _currentUserStartMenuPath;

        private dynamic _wscriptShell;

        public string Name => Resources.ProviderStartMenuName;

        public string Description => Resources.ProviderStartMenuDescription;

        public StartMenuProvider()
        {
            var wscriptShellType = Type.GetTypeFromProgID("WScript.Shell");
            _wscriptShell = Activator.CreateInstance(wscriptShellType);
            _allUsersStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            _currentUserStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        }

        public void Dispose()
        {
        }

        // https://stackoverflow.com/questions/1393178/unauthorizedaccessexception-cannot-resolve-directory-getfiles-failure/9831340#9831340
        private static void addFiles(string path, ICollection<string> files)
        {
            try
            {
                Directory.GetFiles(path)
                    .ToList()
                    .ForEach(files.Add);

                Directory.GetDirectories(path)
                    .ToList()
                    .ForEach(s => addFiles(s, files));
            }
            catch (UnauthorizedAccessException ex)
            {
                // ok, so we are not allowed to dig into that directory. Move on.
            }
        }

        //private Bitmap GetBitmapFromApplicationExe(string exePath)
        //{
        //    var ie = new IconExtractor.IconExtractor(exePath);
        //    if (ie.Count == 0)
        //    {
        //        return null;
        //    }

        //    var icon0 = ie.GetIcon(0);
        //    var icons = IconUtil.Split(icon0);
        //    var icon = icons.OrderByDescending(x => x.Height * x.Width).First();
        //    return icon.ToBitmap();
        //}

        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            var files = new List<string>();
            addFiles(_allUsersStartMenuPath, files);
            addFiles(_currentUserStartMenuPath, files);

            var results = new SearchResultCollection
            {
                Title = "Start Menu",
                Relevance = SearchResultRelevance.ONTOP,
                FontAwesomeIcon = "Bars"
            };
            results.Results = new ObservableCollection<SearchResult>();
            foreach (var f in files)
            {
                var filenameWithoutExtension = Path.GetFileNameWithoutExtension(f);
                if (filenameWithoutExtension?.IndexOf(query.QueryString, StringComparison.CurrentCultureIgnoreCase) == -1) continue;

                results.Results.Add(new ExecutableSearchResult
                {
                    Title = filenameWithoutExtension,
                    ParentCollection = results,
                    LaunchPath = f,
                });
            }
            if (results.Results.Count == 0)
                return null; // nothing found

            var task = Task.Run(() =>
            {
                var toDelete = new List<SearchResult>();
                foreach (var r in results.Results)
                {
                    if (Directory.Exists(r.LaunchPath) || !File.Exists(r.LaunchPath) ||
                        !r.LaunchPath.EndsWith(".lnk", StringComparison.CurrentCultureIgnoreCase))
                    {
                        toDelete.Add(r);
                        continue;
                    }

                    try
                    {
                        var shortcut = _wscriptShell.CreateShortcut(r.LaunchPath);
                        var shortcutTarget = shortcut.TargetPath.ToString();
                        var shortcutArguments = shortcut.Arguments.ToString();
                        if (!string.IsNullOrWhiteSpace(shortcutTarget) && shortcutTarget.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(shortcutTarget))
                        {
                            r.Icon = new WindowsPEResourceIcon { FilePath = shortcutTarget };
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                Plugin.RunOnUiThreadHelper.RunOnUIThread(() =>
                {
                    foreach (var d in toDelete)
                    {
                        results.Results.Remove(d);
                        if (results.Results.Count == 0)
                            results.Relevance = SearchResultRelevance.HIDE;
                    }
                });
            }, ct);

            return results;
        }
    }
}

