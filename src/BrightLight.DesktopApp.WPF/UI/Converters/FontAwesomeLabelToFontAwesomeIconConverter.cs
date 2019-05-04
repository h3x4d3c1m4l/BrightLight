using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using FontAwesome5;
using FontAwesome5.WPF;

namespace BrightLight.WPF.UI.Converters
{
    class FontAwesomeLabelToFontAwesomeIconConverter : IValueConverter
    {
        public static Dictionary<string, EFontAwesomeIcon> faDict;

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
            var type = typeof(EFontAwesomeIcon);
            var enumValues = Enum.GetValues(type);

            faDict = new Dictionary<string, EFontAwesomeIcon>();
            foreach (var e in enumValues)
            {
                if ((EFontAwesomeIcon)e == EFontAwesomeIcon.None) continue;
                var memInfo = type.GetMember(e.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(FontAwesomeInformationAttribute), false);
                if (!attributes.Any()) continue;
                var id = ((FontAwesomeInformationAttribute)attributes[0]).Label;
                if (!faDict.Keys.Contains(id))
                    faDict.Add(id, (EFontAwesomeIcon) e);
            }
        }
    }
}
