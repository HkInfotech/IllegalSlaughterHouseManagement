﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class TypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null)
            //    return IconFont.Tv;

            //Type valueAsType = (Type)value;
            //if (valueAsType == typeof(Color))
            //{
            //    return IconFont.PaintBrush;
            //}
            //else if (valueAsType == typeof(string))
            //{
            //    return IconFont.Font;
            //}
            //else if (valueAsType == typeof(Thickness))
            //{
            //    return IconFont.Gamepad;
            //}
            //else
            //{
            //    return IconFont.Calculator;
            //}
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}