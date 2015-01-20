﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdTracker.Pin_Map
{
    public class PinMapWindowVM
    {
        private PinMap _pinmap = new PinMap();
        public PinMap PIN_MAP
        {
            get { return _pinmap; }
            
        }
        


        /// <summary>
        /// CTOR
        /// </summary>
        public PinMapWindowVM()
        {
        }

        public void initialize(IEnumerable<LatLongPair> collection_of_locations_to_be_pinned)
        {
            if (collection_of_locations_to_be_pinned == null)
            {
                throw new ArgumentNullException("collection of locations cannot be null.", "initialize");
            }

            _pinmap.add_pins_to_map(collection_of_locations_to_be_pinned);

            
            foreach (var pin_location in collection_of_locations_to_be_pinned)
            {

            }


        }

                       
        public void set_map_location(double latitude, double longitude)
        {
            _pinmap.set_map_location(latitude: latitude, longitude: longitude);
        }
            
        


    
    }
}