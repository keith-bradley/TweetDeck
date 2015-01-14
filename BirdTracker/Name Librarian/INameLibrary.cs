using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdTracker.Name_Librarian
{
    /// <summary>
    /// Because birds have both a common and a scientific name, it is usefull to have a library that allows for the
    /// lookup of one given the other.
    /// </summary>
    public interface INameLibrary
    {
        /// <summary>
        /// Adds a name pair to the library. 
        /// </summary>
        /// <param name="strCommon_Name">The common name of the bird.</param>
        /// <param name="strScientific_Name">The scientific name of the bird.</param>
        /// <returns>True on success, false on failure.</returns>
        /// <exception cref="ArgumentException">Thrown when either parameter is null or empty.</exception>
        bool add_name_pair(String strCommon_Name, String strScientific_Name);

        /// <summary>
        /// Given a common name, return the scientific name if we know it.
        /// </summary>
        /// <param name="strCommonName">The common name of the bird.</param>
        /// <returns>The common name of the bird or an empty string.</returns>
        /// <exception cref="ArgumentException">Thrown when the common name is null or empty.</exception>
        string lookup_scientific_name(String strCommon_Name);

        /// <summary>
        /// Given a scientific name, return the common name if we know it.
        /// </summary>
        /// <param name="strScientific_Name">The scientific name of the bird.</param>
        /// <returns>The common name of the bird or an empty string.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific name is null or blank.</exception>        
        string lookup_common_name(String strScientific_Name);

        /// <summary>
        /// Checks if the scientific name is listed in the library.
        /// </summary>
        /// <param name="strScientific_Name">The scientific name of the bird.</param>
        /// <returns>True if the bird is in the library, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific name is empty or blank.</exception>
        bool scientific_name_exists(String strScientific_Name);

        /// <summary>
        /// Checks if the common name is listed in the library.
        /// </summary>
        /// <param name="strCommon_Name">The common name of the bird.</param>
        /// <returns>True if the bird is in the library, false otherwise.</returns>
        bool common_name_exists(String strCommon_Name);

        /// <summary>
        /// Save the library. 
        /// </summary>
        /// <returns>True on Success, false otherwise.</returns>
        bool save_library();

        /// <summary>
        /// Load the library.
        /// </summary>        
        void load_library();


    }
}
