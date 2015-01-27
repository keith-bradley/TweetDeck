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
    /// An interface for creating a map with one or more Pins in it.
    /// </summary>
    interface IPinMap
    {
        /// <summary>
        /// Tell the map to display at the provided latitude & longitude.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <exception cref="ArgumentException">If the lat & long is an invalid coordinate.</exception>
        void set_map_location(double latitude, double longitude);

        /// <summary>
        /// Tell the map to display at the provided coordinates.
        /// </summary>
        /// <param name="pair"></param>
        /// <exception cref="ArgumentNullException">Thrown if pair is null.</exception>
        void set_map_location(LatLongPair pair);

        /// <summary>
        /// Tell the map to add a pin at the provided latitude & longitude.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns>True on success, false on failure.</returns>
        /// <exception cref="ArgumentException">Thrown if the lat & long is an invalid coordinate</exception>
        bool add_pin_to_map(double latitude, double longitude);

        /// <summary>
        /// Tell the map to add a pin at the provided location.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns>True on success, false on failure.</returns>              
        /// <exception cref="ArgumentNullException">Thrown if the pair is null.</exception>
        bool add_pin_to_map(LatLongPair pair);

        /// <summary>
        /// Tell the map to add a set of pins to the map.
        /// </summary>
        /// <param name="colCoordinates"></param>
        /// <returns></returns>
        bool add_pins_to_map(IEnumerable<LatLongPair> colCoordinates);

        /// <summary>
        /// Tell the map to remove a pin from the provided latitude & longitude.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if the lat or long is invalid.</exception>        
        bool remove_pin_from_map(double latitude, double longitude);

        /// <summary>
        /// Tell the map to remove a pin from the provided location.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the pair is null.</exception>
        bool remove_pin_from_map(LatLongPair pair);

        /// <summary>
        /// Remove a set of pins.
        /// </summary>
        /// <param name="colCoordinates"></param>
        /// <returns></returns>
        bool remove_pins_from_map(IEnumerable<LatLongPair> colCoordinates);

        /// <summary>
        /// Remove all of the pins from the map
        /// </summary>
        /// <returns></returns>
        bool clear_all_pins();

    }
}
