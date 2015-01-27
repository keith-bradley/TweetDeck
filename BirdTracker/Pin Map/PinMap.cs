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
    /// Contains the information about all of the pins that are on the map.
    /// </summary>
    public class PinMap : IPinMap
    {
        private LatLongPair _current_map_location;
        public LatLongPair CURRENT_MAP_LOCATION
        {
            get { return _current_map_location; }
            set { _current_map_location = value; }
        }

        /// <summary>
        /// The collection of pins that will be displayed on the map.
        /// </summary>
        private List<LatLongPair> _lst_of_pins = new List<LatLongPair>();
        public List<LatLongPair> LIST_OF_PINS
        {
            get { return _lst_of_pins; }            
        }
        
        /// <summary>
        /// CTOR
        /// </summary>
        public PinMap()
        {
        }

        /// <summary>
        /// Tell the map to display at the provided coordinates.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if pair is null.</exception>
        public void set_map_location(LatLongPair pair)
        {
            if (pair == null)
                { throw new ArgumentNullException("Pair cannot be null.", "set_map_location"); }

            CURRENT_MAP_LOCATION = pair;            
        }

        /// <summary>
        /// Tell the map to add a pin at the provided location.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns>True on success, false on failure.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the pair is null.</exception>
        public bool add_pin_to_map(LatLongPair pair)
        {
            if (pair == null)
                { throw new ArgumentNullException("Pair cannot be null.", "add_pin_to_map"); }

            _lst_of_pins.Add(pair);
            return (true);
        }

        /// <summary>
        /// Tell the map to add a set of pins to the map.
        /// </summary>
        /// <param name="colCoordinates"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the colCoordinates is null.</exception>
        public bool add_pins_to_map(IEnumerable<LatLongPair> colCoordinates)
        {
            if (colCoordinates == null)
                { throw new ArgumentNullException("colCoordinates cannot be null", "add_pins_to_map"); }

            if (colCoordinates != null) { _lst_of_pins.AddRange(colCoordinates); }
            return (true);
        }

        /// <summary>
        /// Tell the map to remove a pin from the provided location.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the pair is null.</exception>
        public bool remove_pin_from_map(LatLongPair pair)
        {
            if (pair == null)
                { throw new ArgumentNullException("pair cannot be null"); }

            return (true);
        }

        /// <summary>
        /// Remove a set of pins.
        /// </summary>
        /// <param name="colCoordinates"></param>
        /// <returns></returns>
        public bool remove_pins_from_map(IEnumerable<LatLongPair> colCoordinates)
        {
            if (colCoordinates == null)
                { throw new ArgumentNullException("colCoordinates cannot be null.", "remove_pins_from_map"); }
                        
            return (true);
        }

        /// <summary>
        /// Remove all of the pins from the map
        /// </summary>
        /// <returns></returns>
        public bool clear_all_pins()
        {
            _lst_of_pins.Clear();
            return (true);
        }
    }
}
