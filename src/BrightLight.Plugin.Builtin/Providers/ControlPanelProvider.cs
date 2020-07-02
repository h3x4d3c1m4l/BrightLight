using BrightLight.Plugin.Builtin.Localization;
using BrightLight.PluginInterface;
using BrightLight.PluginInterface.Result;
using BrightLight.PluginInterface.Result.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanara.PInvoke;

namespace BrightLight.Plugin.Builtin.Providers
{
    public class ControlPanelProvider : ISearchProvider
    {
        public string Name => Resources.ProviderControlPanelName;

        public string Description => Resources.ProviderControlPanelDescription;

        public void Dispose()
        {
        }

        // enumerate "God mode"
        // inspired by the source code of ClassicShell
        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query.QueryString))
                return null;

            var results = new ObservableCollection<SearchResult>();
            var resultCollection = new SearchResultCollection
            {
                Title = "Control Panel",
                Icon = new WindowsPEResourceIcon
                {
                    FilePath = @"C:\windows\system32\control.exe"
                },
                Results = results
            };

            HRESULT hresult;
            hresult = Shell32.SHGetDesktopFolder(out Shell32.IShellFolder desktopShellFolder);
            if (hresult != HRESULT.S_OK)
                throw new Exception($"Shell32.SHGetDesktopFolder returned '{hresult}'");

            hresult = Shell32.SHParseDisplayName("shell:::{ED7BA470-8E54-465E-825C-99712043E01C}", IntPtr.Zero, out Shell32.PIDL godmodePidl, 0, out Shell32.SFGAO psfgaoOut);
            if (hresult != HRESULT.S_OK)
                throw new Exception($"Shell32.SHParseDisplayName returned '{hresult}'");

            var godmodeShellFolder = (Shell32.IShellFolder)desktopShellFolder.BindToObject(godmodePidl, null, new Guid("000214E6-0000-0000-C000-000000000046"));
            if (godmodeShellFolder == null)
                throw new Exception($"BindToObject returned null");

            var itemEnum = godmodeShellFolder.EnumObjects(IntPtr.Zero, Shell32.SHCONTF.SHCONTF_CHECKING_FOR_CHILDREN | Shell32.SHCONTF.SHCONTF_FLATLIST | Shell32.SHCONTF.SHCONTF_NONFOLDERS | Shell32.SHCONTF.SHCONTF_FOLDERS | Shell32.SHCONTF.SHCONTF_INCLUDEHIDDEN | Shell32.SHCONTF.SHCONTF_INCLUDESUPERHIDDEN | Shell32.SHCONTF.SHCONTF_STORAGE);
            if (godmodeShellFolder == null)
                throw new Exception($"EnumObjects returned null");

            var itemPidlArr = new IntPtr[1];
            while (itemEnum.Next(1, itemPidlArr, out uint fetched) == HRESULT.S_OK && fetched > 0)
            {
                var combinedPidl = Shell32.PIDL.Combine(godmodePidl, itemPidlArr[0]);

                Shell32.SHCreateItemFromIDList(combinedPidl, typeof(Shell32.IShellItem2).GUID, out object shellItemObj);
                Shell32.IShellItem2 shellItem = (Shell32.IShellItem2)shellItemObj;

                try
                {
                    var extractIcon = (Shell32.IExtractIcon)godmodeShellFolder.GetUIObjectOf(IntPtr.Zero, 1, new IntPtr[] { itemPidlArr[0] }, typeof(Shell32.IExtractIcon).GUID, IntPtr.Zero);
                    var iconPathSb = new StringBuilder(255);
                    extractIcon.GetIconLocation(Shell32.GetIconLocationFlags.GIL_FORSHELL, iconPathSb, iconPathSb.Capacity, out int iconIndex, out Shell32.GetIconLocationResultFlags flags);

                    var name = shellItem.GetString(Ole32.PROPERTYKEY.System.ItemNameDisplay).ToString();
                    var parsingPath = shellItem.GetString(Ole32.PROPERTYKEY.System.ParsingPath).ToString();
                    var keywords = shellItem.GetString(Ole32.PROPERTYKEY.System.Keywords).ToString().Split(';').Select(x => x.ToLower().Trim());
                    var queryStringLc = query.QueryString.ToLower();
                    if (name.ToLower().Contains(queryStringLc) || keywords.Any(x => x.Contains(queryStringLc))) {
                        results.Add(new ActionSearchResult
                        {
                            Title = name,
                            ParentCollection = resultCollection,
                            Icon = new WindowsPEResourceIcon
                            {
                                FilePath = iconPathSb.ToString(),
                                //Index = iconIndex == -1 ? 1 : iconIndex
                                //TODO: support for iconIndex < 0 (which means: find icon by resource id instead of icon index)
                            },
                            Action = () =>
                            {
                                dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
                                var folder = shell.NameSpace(@"shell:::{ED7BA470-8E54-465E-825C-99712043E01C}");
                                foreach (var item in folder.Items)
                                {
                                    var itemFolder = ((string)item.path.ToString());
                                    if (itemFolder == parsingPath)
                                        item.InvokeVerb("open");
                                }
                            }
                        });
                    }
                }
                catch (Exception)
                {
                }
            }

            return resultCollection;
        }
    }
}
