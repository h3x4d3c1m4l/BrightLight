using BrightLight.PluginInterface.Result.Helpers;
using BrightLight.WPF.IconUtils;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BrightLight.WPF.UI.Converters
{
    class BitmapToBitmapImageConverter : IValueConverter
    {
        // https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ExecutableIcon)
            {
                // extract icon van exe
                var executableIcon = value as ExecutableIcon;
                try
                {
                    var extractor = new IconExtractor(executableIcon.ExecutablePath);
                    var iconCollection = extractor.GetIcon(0);
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
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
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
