using System;
using System.Globalization;
using System.Windows.Data;

namespace ETAG_ERP.Converters
{
    /// <summary>
    /// Converter that converts null values to boolean values
    /// </summary>
    public class NullToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Convert null to boolean
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (value is string stringValue)
            {
                return !string.IsNullOrEmpty(stringValue);
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            if (value is int intValue)
            {
                return intValue != 0;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue != 0;
            }

            if (value is double doubleValue)
            {
                return doubleValue != 0;
            }

            if (value is float floatValue)
            {
                return floatValue != 0;
            }

            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue != DateTime.MinValue;
            }

            // For other types, check if they are not null
            return value != null;
        }

        /// <summary>
        /// Convert boolean back to null
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new object() : null;
            }

            return null;
        }
    }

    /// <summary>
    /// Converter that converts null values to boolean values with invert option
    /// </summary>
    public class NullToBooleanInvertConverter : IValueConverter
    {
        /// <summary>
        /// Convert null to boolean (inverted)
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }

            if (value is string stringValue)
            {
                return string.IsNullOrEmpty(stringValue);
            }

            if (value is bool boolValue)
            {
                return !boolValue;
            }

            if (value is int intValue)
            {
                return intValue == 0;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue == 0;
            }

            if (value is double doubleValue)
            {
                return doubleValue == 0;
            }

            if (value is float floatValue)
            {
                return floatValue == 0;
            }

            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue == DateTime.MinValue;
            }

            // For other types, check if they are null
            return value == null;
        }

        /// <summary>
        /// Convert boolean back to null (inverted)
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? null : new object();
            }

            return null;
        }
    }

    /// <summary>
    /// Converter that converts null values to boolean values with custom null value
    /// </summary>
    public class NullToBooleanCustomConverter : IValueConverter
    {
        /// <summary>
        /// Convert null to boolean
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if parameter specifies custom null value
            if (parameter is string param)
            {
                var parts = param.Split('|');
                if (parts.Length == 2)
                {
                    var nullValue = parts[0].Trim();
                    var nonNullValue = parts[1].Trim();

                    if (value == null)
                    {
                        return ParseBoolean(nullValue);
                    }
                    else
                    {
                        return ParseBoolean(nonNullValue);
                    }
                }
            }

            // Default behavior
            if (value == null)
            {
                return false;
            }

            if (value is string stringValue)
            {
                return !string.IsNullOrEmpty(stringValue);
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            if (value is int intValue)
            {
                return intValue != 0;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue != 0;
            }

            if (value is double doubleValue)
            {
                return doubleValue != 0;
            }

            if (value is float floatValue)
            {
                return floatValue != 0;
            }

            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue != DateTime.MinValue;
            }

            // For other types, check if they are not null
            return value != null;
        }

        /// <summary>
        /// Convert boolean back to null
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new object() : null;
            }

            return null;
        }

        /// <summary>
        /// Parse boolean string to boolean value
        /// </summary>
        private bool ParseBoolean(string booleanString)
        {
            switch (booleanString.ToLowerInvariant())
            {
                case "true":
                case "1":
                case "yes":
                case "on":
                    return true;
                case "false":
                case "0":
                case "no":
                case "off":
                    return false;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Converter that converts null values to boolean values with string handling
    /// </summary>
    public class NullToBooleanStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert null to boolean
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (value is string stringValue)
            {
                return !string.IsNullOrEmpty(stringValue) && !string.IsNullOrWhiteSpace(stringValue);
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            if (value is int intValue)
            {
                return intValue != 0;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue != 0;
            }

            if (value is double doubleValue)
            {
                return doubleValue != 0;
            }

            if (value is float floatValue)
            {
                return floatValue != 0;
            }

            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue != DateTime.MinValue;
            }

            // For other types, check if they are not null
            return value != null;
        }

        /// <summary>
        /// Convert boolean back to null
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new object() : null;
            }

            return null;
        }
    }

    /// <summary>
    /// Converter that converts null values to boolean values with empty string handling
    /// </summary>
    public class NullToBooleanEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Convert null to boolean
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (value is string stringValue)
            {
                return !string.IsNullOrEmpty(stringValue);
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            if (value is int intValue)
            {
                return intValue != 0;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue != 0;
            }

            if (value is double doubleValue)
            {
                return doubleValue != 0;
            }

            if (value is float floatValue)
            {
                return floatValue != 0;
            }

            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue != DateTime.MinValue;
            }

            // For other types, check if they are not null
            return value != null;
        }

        /// <summary>
        /// Convert boolean back to null
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new object() : null;
            }

            return null;
        }
    }
}