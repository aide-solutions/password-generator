
namespace Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Core;

    [ValueConversion(typeof(PasswordStrengthEnum), typeof(String))]
    public class PasswordStrengthEnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetName(typeof (PasswordStrengthEnum), value);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || String.IsNullOrEmpty((string)value)) return PasswordStrengthEnum.NotDefined;

            PasswordStrengthEnum strength;

            Enum.TryParse((string) value, true, out strength);
            
            return strength;
        }
    }
}