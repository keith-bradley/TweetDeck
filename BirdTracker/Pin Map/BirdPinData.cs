/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdTracker.Pin_Map
{
    /// <summary>
    /// Specific pin data related to the BirdTracker application.
    /// </summary>
    public class BirdPinData : PinData
    {
        private string _scientific_name;
        public string SCIENTIFIC_NAME
        {
            get { return _scientific_name; }
            set { _scientific_name = value; }
        }
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="lattitude"></param>
        /// <param name="longitude"></param>
        /// <param name="common_name"></param>
        /// <param name="scientific_name"></param>
        public BirdPinData(double latitude, double longitude, string common_name, string scientific_name)
                : base(latitude: latitude, longitude: longitude, title: common_name)
        {
            SCIENTIFIC_NAME = scientific_name;
        }
    
       
    }
}
