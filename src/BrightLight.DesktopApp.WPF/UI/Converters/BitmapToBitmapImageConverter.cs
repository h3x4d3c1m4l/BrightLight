using BrightLight.PluginInterface.Result.Helpers;
using BrightLight.DesktopApp.WPF.IconUtils;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using IWshRuntimeLibrary;

namespace BrightLight.DesktopApp.WPF.UI.Converters
{
    public class BitmapToBitmapImageConverter : IValueConverter
    {
        // https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WindowsPEResourceIcon resIcon)
            {
                var exePath = resIcon.FilePath;

                // check if lnk, if so resolve the executable location first
                if (resIcon.FilePath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    var shell = new WshShell();
                    var shortcut = (IWshShortcut)shell.CreateShortcut(resIcon.FilePath);
                    exePath = shortcut.TargetPath;
                }

                // extract icon from exe
                try
                {
                    var extractor = new IconExtractor(exePath);
                    var iconCollection = extractor.GetIcon(resIcon.Index);
                    var biggestIcon = IconUtil.Split(iconCollection).OrderByDescending(x => x.Width * x.Height).FirstOrDefault();
                    if (biggestIcon != null)
                        return CreateBitmapImageFromBitmap(biggestIcon.ToBitmap());
                }
                catch (Exception)
                {
                    // no icon, file not found or whatever
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage CreateBitmapImageFromBitmap(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
