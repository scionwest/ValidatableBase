using Scionwest.Validatable.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Scionwest.Validatable.Converters
{
    public class IValidationMessageCollectionToStringCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is IEnumerable<IValidationMessage>))
            {
                return new List<string>();
            }

            List<string> lst = (value as IEnumerable<IValidationMessage>).Select(m => m.Message).ToList();
            return lst;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
