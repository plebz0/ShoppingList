using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;

namespace ShoppingList.Converters;

public class FirstOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                return item?.ToString() ?? string.Empty;
            }
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}