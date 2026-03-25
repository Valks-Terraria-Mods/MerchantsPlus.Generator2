using System.Globalization;
using Avalonia.Data.Converters;

namespace MerchantsPlus.Generator;

public sealed class MultiplyDoubleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var source = value is double numeric ? numeric : 0d;
        var factor = parameter is null
            ? 1d
            : double.TryParse(parameter.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : 1d;

        return source * factor;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
