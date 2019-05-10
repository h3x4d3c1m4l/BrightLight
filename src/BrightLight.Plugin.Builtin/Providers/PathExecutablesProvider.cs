using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;
using BrightLight.PluginInterface.Result;
using BrightLight.PluginInterface;
using BrightLight.PluginInterface.Result.Helpers;
using BrightLight.Plugin.Builtin.Localization;

namespace BrightLight.Plugin.Builtin.Providers
{
    public class PathExecutablesProvider : ISearchProvider
    {
        public string Name => Resources.ProviderPathExecutablesName;

        public string Description => Resources.ProviderPathExecutablesDescription;

        // https://stackoverflow.com/questions/3855956/check-if-an-executable-exists-in-the-windows-path
        // TODO: https://weblogs.asp.net/whaggard/111232
        public static string GetFullPathUsingEnvironmentVariablePath(string exeName)
        {
            try
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        FileName = "where",
                        Arguments = exeName,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                p.Start();
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                if (p.ExitCode != 0)
                    return null;

                // just return first match
                return output.Substring(0, output.IndexOf(Environment.NewLine));
            }
            catch (Win32Exception)
            {
                throw new Exception("'where' command is not on path");
            }
        }

        public static string GetFullPathFromRegistryAppPaths(string exeName) // eg. iexplore or iexplore.exe or firefox or other stuff from HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\
        {
            if (!exeName.EndsWith(".exe"))
                exeName += ".exe";

            var fullPath = Registry.LocalMachine
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("App Paths")?
                .OpenSubKey(exeName)?
                .GetValue(null)?
                .ToString();
            return File.Exists(fullPath) ? fullPath : null;
        }

        public void Dispose()
        {
        }

        public (string filename, string arguments) SplitFilenameAndArguments(string filenameAndArguments)
        {
            string arguments = null;
            string filename;
            if (filenameAndArguments.StartsWith("\"")) // e.g. "C:\Program Files\Some Vendor\Some App\App.exe" arg1 %1
            {
                var filenameSplit = filenameAndArguments.Split(new[] {'"'}, 3);
                filename = filenameSplit[1];
                if (filenameSplit.Length == 3)
                    arguments = filenameSplit[2].Remove(0, 1);
            }
            else
            {
                var filenameSplit = filenameAndArguments.Split(new[] { ' ' }, 2);  // e.g. C:\SomeVendor\SomeApp\App.exe arg1 %1
                filename = filenameSplit[0];
                arguments = filenameSplit[1];
            }
            return (filename, arguments);
        }

        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            string title, launchExePath, launchParameters;

            try
            {
                var uri = new Uri(query.QueryString); // e.g. spotify:23978by4239874by2374239832y, https://www.google.com
                var subkey = Registry.ClassesRoot.OpenSubKey(uri.Scheme);
                var value = subkey?.OpenSubKey("shell")?
                    .OpenSubKey("open")?
                    .OpenSubKey("command")?
                    .GetValue(null)?
                    .ToString();

                if (value == null)
                    return null; // it seems to be a uri, but no executable found that can handle it

                var valueSplit = SplitFilenameAndArguments(value);
                if (!File.Exists(valueSplit.filename)) return null;

                title = uri.Scheme;
                launchExePath = valueSplit.filename;
                launchParameters = valueSplit.arguments.Replace("%1", query.QueryString);
            }
            catch (Exception)
            {
                // check if the user references to a file
                var exeSplit = query.QueryString.Split(new[] { ' ' }, 2);
                var exeFilename = exeSplit[0];
                var exeArguments = exeSplit.Length > 1 ? exeSplit[1] : null;

                var fullpath = GetFullPathFromRegistryAppPaths(exeFilename) ?? GetFullPathUsingEnvironmentVariablePath(exeFilename);
                if (fullpath == null || !File.Exists(fullpath)) // nothing found
                    return null;

                title = fullpath;
                launchExePath = fullpath;
                launchParameters = exeArguments;
            }

            var results = new SearchResultCollection
            {
                Title = "Run",
                Relevance = SearchResultRelevance.ONTOP,
                FontAwesomeIcon = "Window Maximize"
            };
            results.Results = new ObservableCollection<SearchResult>
            {
                new ExecutableSearchResult
                {
                    Title = title,
                    LaunchPath = launchExePath,
                    LaunchArguments = launchParameters,
                    ParentCollection = results,
                    Icon = new WindowsPEResourceIcon { FilePath = launchExePath }
                }
            };

            return results;
        }
    }
}
