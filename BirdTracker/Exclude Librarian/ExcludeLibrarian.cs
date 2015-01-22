/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BirdTracker.Support;

namespace BirdTracker.Exclude_Librarian
{
    /// <summary>
    /// The exclude librarian keeps track of the bird species that the user does not want to be told about.
    /// For example, I have no interest in knowing about the location of say a rock pidgeon so I would exclude it.
    /// </summary>
    public sealed class ExcludeLibrarian : IExcudeLibary
    {
        private static ExcludeLibrarian _librarian  = new ExcludeLibrarian();                   // The librarian
        private Dictionary<string, string> _library = new Dictionary<string, string>();         // The library.

        /// <summary>
        /// CTOR is private as this is a singleton
        /// </summary>
        private ExcludeLibrarian()
        {
        }

        /// <summary>
        /// Gets the singelton librarian.
        /// </summary>
        /// <returns></returns>
        public static ExcludeLibrarian get_instance()
        {
            return (_librarian);
        }

        /// <summary>
        /// Add an item to the library.
        /// </summary>
        /// <param name="scientific_name">The item in the library.</param>
        /// <returns>True if the item was added or is already in the library.</returns>
        /// <exception cref="ArgumentException">Thrown when scientific_name is null or empty.</exception>
        public bool add_item_to_library(string scientific_name)
        {
            if (String.IsNullOrEmpty(scientific_name))
            {
                throw new ArgumentException("Cannot add a blank item to the library.", "add_item_to_library");
            }

            bool bAdded = false;
            if (!_library.ContainsKey(scientific_name))
            {
                _library.Add(scientific_name, null);
                bAdded = true;
            }
            else
            {
                bAdded = true;
            }

            return (bAdded);
        }

        /// <summary>
        /// Remove an item from the library.
        /// </summary>
        /// <param name="scientific_name">The item in the library.</param>
        /// <returns>True if the item was removed from the library, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific_name is null or empty.</exception>
        public bool remove_item_from_library(string scientific_name)
        {
            if (String.IsNullOrEmpty(scientific_name))
            {
                throw new ArgumentException("Cannot remove a blank item from the library.", "remove_item_from_library");
            }

            bool bRemoved = false;
            if (_library.ContainsKey(scientific_name))
            {
                bRemoved = _library.Remove(scientific_name);            
            }

            return (bRemoved);
        }
        
        /// <summary>
        /// Tests if an item is in the library.
        /// </summary>
        /// <param name="scientific_name">The item that might be in the library.</param>
        /// <returns>True if the item is in the library, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific name is null or empty.</exception>
        public bool item_is_in_library(string scientific_name)
        {
            if (String.IsNullOrEmpty(scientific_name))
            {
                throw new ArgumentException("Cannot look up a blank item.", "item_is_in_library");
            }

            bool bIsInLibrary = _library.ContainsKey(scientific_name);
            return (bIsInLibrary);
        }

        /// <summary>
        /// Remove all items from the library.
        /// </summary>
        public void empty_library()
        {
            _library.Clear();
        }

        /// <summary>
        /// Save the library
        /// </summary>
        /// <returns>True on sucess, false on failure</returns>
        public bool save_library()
        {
            if (_library.Count > 0)
            {
                StringBuilder sb = new StringBuilder("<exclude_library><excludes>");
                foreach (var scientific_name in _library.Keys)
                {
                    sb.AppendFormat("<exclude>{0}</exclude>", scientific_name);
                }

                sb.Append("</excludes></exclude_library>");

                Properties.Settings.Default.EXCLUDE_LIST = sb.ToString();
                Properties.Settings.Default.Save();
            }
        
            return (true);
        }

        /// <summary>
        /// Load the saved library
        /// </summary>
        public void load_library()
        {
            var saved_library = Properties.Settings.Default.EXCLUDE_LIST;
            if (!String.IsNullOrEmpty(saved_library))
            {
                 var xdoc = Utilities.load_xml_from_string(saved_library);
                 if (xdoc != null)
                 {
                     var lstExcludedBirds = xdoc.Root.Element("excludes").Elements("exclude").Select(element => element.Value);
                     foreach (var exclude in lstExcludedBirds)
                     {
                         add_item_to_library(exclude);
                     }                                                                      
                 }
            }
        }

        /// <summary>
        /// Returns all of the excluded birds.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> get_all_excluded_birds()
        {           
            return _library.Keys;
        }
    }
}
