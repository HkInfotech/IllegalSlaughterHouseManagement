﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class DoubleToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var newStep = Math.Round((double)value / 1.0);

            return (int)newStep;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}