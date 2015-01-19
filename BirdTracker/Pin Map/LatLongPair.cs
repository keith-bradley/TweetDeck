using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdTracker.Pin_Map
{
    /// <summary>
    /// Container for latitude & longitude co-ordinates.
    /// </summary>
    public class LatLongPair
    {
        /// <summary>
        /// CTOR -Default
        /// </summary>
        public LatLongPair()
        {
        }

        /// <summary>
        /// CTOR - takes the coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <exception cref="ArgumentException">If the provided coordinates are out of bounds.</exception>
        public LatLongPair(double latitude, double longitude)
        {
            if (((latitude < -90.00) || (latitude > 90.00)) ||
               (longitude < -180.00) || (longitude > 180.00)
               )
            {
                throw new ArgumentException("Invalid Lat & Long", "LatLongPair");
            }

            Latitude = latitude;
            Longitude = longitude;
        }


        private double _lat;
        public double Latitude
        {
            get { return _lat; }
            set { _lat = value; }
        }

        private double _long;
        public double Longitude
        {
            get { return _long; }
            set { _long = value; }
        }            
    }
}
