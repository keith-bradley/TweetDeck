using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace BirdTracker.Exclude_Librarian
{
    public class ExcludeListItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public String _scientific_name { get; set; }
        public String _common_name     { get; set; }

        private BitmapSource _thumbnl;
        public BitmapSource ThumbNail
        {
            get { return _thumbnl; }
            set
            {
                _thumbnl = value;
                OnPropertyChanged("ThumbNail");
            }
        }

        /// <summary>
        /// CTOR
        /// </summary>
        public ExcludeListItem()
        {
        }
        
        protected virtual void OnPropertyChanged(string propName)
        {
            if (!String.IsNullOrEmpty(propName) && (PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }          
    }
}
