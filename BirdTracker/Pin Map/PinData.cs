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
    /// A pin on a map can be so much more than just the Latitude and Longitude. Like a tool tip label or perhaps an image.    
    /// </summary>
    public class PinData : LatLongPair
    {
        /// <summary>
        /// The title of the tooltip.
        /// </summary>
        private string _tool_tip_title;
        public string TOOL_TIP_TITLE
        {
            get { return _tool_tip_title; }
            set { _tool_tip_title = value; }
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public PinData(double latitude, double longitude) 
                : base(latitude: latitude, longitude: longitude)
        {
        }

        /// <summary>
        /// CTOR with toot tip title
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="title"></param>
        public PinData(double latitude, double longitude, string title)
                : base(latitude: latitude, longitude: longitude)
        {
            TOOL_TIP_TITLE = title;
        }    
    }
}
