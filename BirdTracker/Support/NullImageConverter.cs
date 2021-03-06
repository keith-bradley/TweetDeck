﻿/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace BirdTracker.Support
{
    /// <summary>
    /// 
    /// </summary>
    public class NullImageConverter : IValueConverter
    {
        public object Convert(object value, 
                              Type targetType, 
                              object parameter, 
                              CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            return value;
        }

        public object ConvertBack(object value, 
                                 Type targetType, 
                                 object parameter, 
                                 CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
