using System;
using System.Collections.Generic;
using BirdTracker.Interfaces;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace BirdTracker.Location_Manager
{
    /// <summary>
    /// A singleton class for keeping track of E-Bird Locations to Real World Location.
    /// A E-Bird location is some numeric code and does not have any real meaning to a human user.
    /// The real world location is human readable - i.e. Mud Lake, Britania Bay etc.
    /// </summary>
    public class LocationManager : ILocationManager
    {
        private static LocationManager            _instance   = new LocationManager();
        private static Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        /// <summary>
        /// Returns a reference to the librarian.
        /// </summary>        
        public static LocationManager getInstance()
        {
            return (_instance);
        }

        /// <summary>
        /// CTOR - Private as this is a singleton.
        /// </summary>
        private LocationManager()
        {
        }

        /// <summary>
        /// Add a E-Bird Location/Real World location map.
        /// </summary>
        /// <param name="strEBirdLocation">E-Bird Location</param>
        /// <param name="strRealWorldLocation">Real World Location</param>
        /// <returns>True on success, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when either parameter is null or empty.</exception>
        public bool add_mapping(string strEBirdLocation,
                                string strRealWorldLocation)
        {
            if (String.IsNullOrEmpty(strEBirdLocation))
            {
                throw new ArgumentException("Ebird location cannot be empty.", "add_mapping");
            }
            else if (String.IsNullOrEmpty(strRealWorldLocation))
            {
                throw new ArgumentException("Real world location cannot be empty.", "add_mapping");
            }
            
            bool bAdded = false;
            if (!_dictionary.ContainsKey(strEBirdLocation))
            {
                _dictionary.Add(strEBirdLocation, strRealWorldLocation);
                bAdded = true;
            }

            return (bAdded);
        }

        /// <summary>
        /// Remove a mapping.
        /// </summary>
        /// <param name="strEBirdLocation">E-Bird Location</param>
        /// <param name="strRealWorldLocation">Real World Location</param>
        /// <returns>True if the pair have been removed, false otheriwse.</returns>
        /// <exception cref="ArgumentException">Thrown when either parameter is null or empty.</exception>
        public bool remove_mapping(string strEBirdLocation, 
                                   string strRealWorldLocation)
        {
            if (String.IsNullOrEmpty(strEBirdLocation))
            {
                throw new ArgumentException("Ebird location cannot be empty.", "remove_mapping");
            }
            else if (String.IsNullOrEmpty(strRealWorldLocation))
            {
                throw new ArgumentException("Real world location cannot be empty.", "remove_mapping");
            }

            bool bRemoved = false;
            if (_dictionary.ContainsKey(strEBirdLocation))
                    {  bRemoved = _dictionary.Remove(strEBirdLocation); }            

            return (bRemoved);
        }

        /// <summary>
        /// Retrieve a real world location given a E-Bird Location.
        /// </summary>
        /// <param name="strEBirdLocation">The e-bird location L12345</param>
        /// <returns>The real world location, i.e. Mud Lake</returns>
        /// <exception cref="ArgumentException">Thrown when the EBirdLocation is null or empty.</exception>
        public string get_real_world_location(string strEBirdLocation)
        {
            if (String.IsNullOrEmpty(strEBirdLocation))
            {
                throw new ArgumentException("EBird Location cannot be null", "get_real_world_location");
            }

            string location = "";
            if ( _dictionary.ContainsKey(strEBirdLocation))
            {
                location = _dictionary[strEBirdLocation];
            }
            
            return (location);
        }
    }
}
