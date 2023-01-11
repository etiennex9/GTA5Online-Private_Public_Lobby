using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace GTA5_Private_Public_Lobby
{
    public class CollectionContainsItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length > 1)
            {
                for (var i = 1; i < values.Length; i++)
                {
                    if (values[i] is IEnumerable<object> list && list.Contains(values[0]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}