using System;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace BirdTracker.Interfaces
{
    /// <summary>
    /// The ILocationManager interface keeps track of E-Bird Locations to Human Readable Locations.
    /// </summary>
    public interface ILocationManager
    {
        /// <summary>
        /// Add a mapping to the manager.
        /// </summary>
        /// <param name="strEBirdLocation">The E-Bird location.</param>
        /// <param name="strRealWorldLocation">The real world location.</param>
        /// <returns>True on being entered in the system, false on failure.</returns>
        /// <exception cref="ArgumentException">Thrown when either parameter is null or empty.</exception>
        bool add_mapping(String strEBirdLocation, 
                         String strRealWorldLocation);

        /// <summary>
        /// Remove a mapping from the manager.
        /// </summary>
        /// <param name="strEBirdLocation">The E-Bird location.</param>
        /// <param name="strRealWorldLocation">The real world location.</param>
        /// <returns>True if the pair have been removed, false otheriwse.</returns>
        /// <exception cref="ArgumentException">Thrown when either parameter is null or empty.</exception>
        bool remove_mapping(String strEBirdLocation, 
                            String strRealWorldLocation);

        /// <summary>
        /// Retrieve the real world location.
        /// </summary>
        /// <param name="strEBirdLocation">The e-bird location L12345</param>
        /// <returns>The real world location, i.e. Mud Lake</returns>
        /// <exception cref="ArgumentException">Thrown when the EBirdLocation is null or empty.</exception>
        string get_real_world_location(String strEBirdLocation);
    
    }
}
