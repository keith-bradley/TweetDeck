/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using Microsoft.Maps.MapControl.WPF;
using System.Linq;
using System.ComponentModel;

namespace BirdTracker.Pin_Map
{
    public class PinMapWindowVM : INotifyPropertyChanged
    {
        private PinMap _pinmap = new PinMap();
        public PinMap PIN_MAP
        {
            get { return _pinmap; }            
        }

        private Location _centre_of_map;
        public Location CENTRE_OF_MAP
        {
            get { return _centre_of_map; }
            set { 
                  _centre_of_map = value;
                  OnPropertyChanged("CENTRE_OF_MAP");
                }
        }

        /// <summary>
        /// Collection of Pushpins to be displayed on the map.
        /// </summary>
        private IEnumerable<Pushpin> _lstPushPins;
        public IEnumerable<Pushpin> LIST_OF_PUSHPINS
        {
            get { return _lstPushPins; }
            set { _lstPushPins = value; }
        }
        
        /// <summary>
        /// CTOR
        /// </summary>
        public PinMapWindowVM()
        {
            CENTRE_OF_MAP = new Location(latitude: 45.52, longitude: -75.43);        
        }
 
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propName)
        {
            try
            {
                if (!String.IsNullOrEmpty(propName) && (PropertyChanged != null))
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }  
    }
}
