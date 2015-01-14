using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdTracker.Exclude_Librarian
{
    /// <summary>
    /// An exclude library keeps track of items that the end user wants to exclude.
    /// </summary>
    public interface IExcudeLibary
    {
        /// <summary>
        /// Add an item to the library.
        /// </summary>
        /// <param name="item">The item in the library.</param>
        /// <returns>True if the item was added or is already in the library</returns>
        /// <exception cref="ArgumentException">Thrown when scientific_name is null or empty.</exception>
        bool add_item_to_library(string item);

        /// <summary>
        /// Remove an item from the library.
        /// </summary>
        /// <param name="item">The item in the library.</param>
        /// <returns>True if the item was removed from the library, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific_name is null or empty.</exception>
        bool remove_item_from_library(string item);

        /// <summary>
        /// Tests if an item is in the library.
        /// </summary>
        /// <param name="item">The item that might be in the library.</param>
        /// <returns>True if the item is in the library, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific name is null or empty.</exception>
        bool item_is_in_library(string item);

        /// <summary>
        /// Remove all items from the library.
        /// </summary>
        void empty_library();

        /// <summary>
        /// Save the library
        /// </summary>
        /// <returns>True on sucess, false on failure</returns>
        bool save_library();

        /// <summary>
        /// Load the library
        /// </summary>
        void load_library();

        /// <summary>
        /// Returns all of the birds that have been excluded.
        /// </summary>        
        IEnumerable<string> get_all_excluded_birds();
    }
}
