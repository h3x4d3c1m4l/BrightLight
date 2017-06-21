using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using FontAwesome.WPF;

namespace Brightlight.WPF.UI.Converters
{
    class FontAwesomeIdToFontAwesomeIconConverter : IValueConverter
    {
        public static Dictionary<string, FontAwesomeIcon> faDict;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (faDict == null)
                    buildFontAwesomeDictionary();

                return faDict[stringValue];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static void buildFontAwesomeDictionary()
        {
            var type = typeof(FontAwesomeIcon);
            var enumValues = Enum.GetValues(type);

            faDict = new Dictionary<string, FontAwesomeIcon>();
            foreach (var e in enumValues)
            {
                if ((FontAwesomeIcon)e == FontAwesomeIcon.None) continue;
                var memInfo = type.GetMember(e.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(IconIdAttribute), false);
                if (!attributes.Any()) continue;
                var id = ((IconIdAttribute)attributes[0]).Id;
                if (!faDict.Keys.Contains(id))
                    faDict.Add(id, (FontAwesomeIcon) e);
            }
        }
    }
}
