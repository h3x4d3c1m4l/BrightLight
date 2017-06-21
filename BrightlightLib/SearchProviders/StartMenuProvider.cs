using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrightlightLib.IconExtractor;
using BrightlightLib.Models;
using BrightlightLib.ViewModels;

namespace BrightlightLib.SearchProviders
{
    class StartMenuProvider : ISearchProvider
    {
        private string allUsersStartMenuPath;

        private string currentUserStartMenuPath;

        public StartMenuProvider()
        {
            allUsersStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            currentUserStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
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

        private Bitmap GetBitmapFromApplicationExe(string exePath)
        {
            var ie = new IconExtractor.IconExtractor(exePath);
            if (ie.Count == 0)
            {
                return null;
            }

            var icon0 = ie.GetIcon(0);
            var icons = IconUtil.Split(icon0);
            var icon = icons.OrderByDescending(x => x.Height * x.Width).First();
            return icon.ToBitmap();
        }

        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            var files = new List<string>();
            addFiles(allUsersStartMenuPath, files);
            addFiles(currentUserStartMenuPath, files);

            var results = new SearchResultCollection
            {
                Title = "Start Menu",
                Relevance = SearchResultRelevance.ONTOP,
                FontAwesomeIcon = "bars"
            };
            results.Results = new ObservableCollection<SearchResult>();
            files.Add(@"C:\pannekoek.exe");
            foreach (var f in files)
            {
                var filenameWithoutExtension = Path.GetFileNameWithoutExtension(f);
                if (filenameWithoutExtension?.IndexOf(query.Query, StringComparison.CurrentCultureIgnoreCase) == -1) continue;

                results.Results.Add(new ExecutableResult
                {
                    Title = filenameWithoutExtension,
                    ParentCollection = results,
                    LaunchExePath = f,
                });
            }
            if (results.Results.Count == 0)
                return null; // nothing found

            var task = Task.Run(() =>
            {
                var toDelete = new List<SearchResult>();
                foreach (var r in results.Results)
                {
                    if (Directory.Exists(r.LaunchExePath) || !File.Exists(r.LaunchExePath) ||
                        !r.LaunchExePath.EndsWith(".lnk", StringComparison.CurrentCultureIgnoreCase))
                    {
                        toDelete.Add(r);
                        continue;
                    }

                    try
                    {
                        var target = ShortcutUtils.GetShortcutTarget(r.LaunchExePath);
                        var icon = GetBitmapFromApplicationExe(target);
                        r.Icon = icon;
                    }
                    catch (Exception)
                    { }
                }

                MainViewModel.UIThreadHelper.RunOnUIThread(() =>
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

