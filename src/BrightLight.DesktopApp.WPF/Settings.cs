using Microsoft.Win32;
using Nett;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BrightLight.DesktopApp.WPF
{
    public class Settings : INotifyPropertyChanged
    {
        #region File/Registry paths
        private static string BRIGHTLIGHT_EXE_PATH = Assembly.GetEntryAssembly().Location;

        private static string BRIGHTLIGHT_EXE_DIR_PATH = Path.GetDirectoryName(BRIGHTLIGHT_EXE_PATH);

        private static string PORTABLE_DATA_PATH = Path.Combine(BRIGHTLIGHT_EXE_DIR_PATH, "Data", "PORTABLE").ToString();

        private static string PORTABLE_DATA_PORTABLEMODE_PATH = Path.Combine(BRIGHTLIGHT_EXE_DIR_PATH, "Data", "PORTABLE").ToString();

        private static string SETTINGS_PATH = File.Exists(PORTABLE_DATA_PORTABLEMODE_PATH) ?
            Path.Combine(BRIGHTLIGHT_EXE_DIR_PATH, "Data", "Settings.toml").ToString() :
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BrightLight", "Settings.toml").ToString();

        private static RegistryKey AUTOSTART_KEY = Registry.CurrentUser.OpenSubKey(REGKEY_RUN, true);

        private const string REGKEY_RUN = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        private const string REGKEY_RUN_VALUE_NAME = "BrightLight";
        #endregion

        [TomlIgnore]
        public bool AutoStart
        {
            get
            {
                var curVal = AUTOSTART_KEY.GetValue(REGKEY_RUN_VALUE_NAME);
                return 
                    curVal != null &&
                    curVal is string &&
                    string.Equals((string)curVal, BRIGHTLIGHT_EXE_PATH, StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                if (!value)
                    AUTOSTART_KEY.DeleteValue(REGKEY_RUN_VALUE_NAME);
                else
                    AUTOSTART_KEY.SetValue(REGKEY_RUN_VALUE_NAME, BRIGHTLIGHT_EXE_PATH);
                OnPropertyChanged();
            }
        }

        [TomlIgnore]
        public bool Portable
        {
            get
            {
                return File.Exists(PORTABLE_DATA_PORTABLEMODE_PATH);
            }
            set
            {
                if (!value)
                {
                    File.Delete(PORTABLE_DATA_PORTABLEMODE_PATH);
                }
                else
                {
                    Directory.CreateDirectory(PORTABLE_DATA_PATH);
                    File.Create(PORTABLE_DATA_PORTABLEMODE_PATH);
                }
                OnPropertyChanged();
            }
        }

        #region Load/Save
        private Settings()
        {
        }

        public static Settings LoadOrDefault()
        {
            if (!File.Exists(SETTINGS_PATH))
                return new Settings();
            
            return Toml.ReadFile<Settings>(SETTINGS_PATH);
        }

        public void Save()
        {
            Toml.WriteFile(this, SETTINGS_PATH);
        }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
