/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BirdTracker.Support;

namespace BirdTracker.Name_Librarian
{
    /// <summary>
    /// The name librarian manages the name library which is a collection of scientific and their equivalent common names.
    /// </summary>
    public class NameLibrarian : INameLibrary
    {
        /// <summary>
        /// We have two dictionaries to allow for a fast bi-directional lookup.
        /// </summary>
        private Dictionary<string, string> common_to_scientific_dictionary = new Dictionary<string, string>();
        private Dictionary<string, string> scientific_to_common_dictionary = new Dictionary<string, string>();

        private readonly string _xml_name_library = "name_library";
        private readonly string _xml_items = "items";
        private readonly string _xml_item = "item";
        private readonly string _xml_common = "common";
        private readonly string _xml_scientific = "scientific";


        private static NameLibrarian _librarian = new NameLibrarian();       
        public static NameLibrarian NAME_LIBRARIAN
        {
            get { return _librarian; }            
        }
        
        /// <summary>
        /// CTOR is private at this is a singleton
        /// </summary>
        private NameLibrarian()
        {
        }

        /// <summary>
        /// Adds a name pair to the library. 
        /// </summary>
        /// <param name="strCommon_Name">The common name of the bird.</param>
        /// <param name="strScientific_Name">The scientific name of the bird.</param>
        /// <returns>True on success, false on failure.</returns>
        /// <exception cref="ArgumentException">Thrown when either parameter is null or empty.</exception>
        public bool add_name_pair(string strCommon_Name, string strScientific_Name)
        {
            if (String.IsNullOrEmpty(strCommon_Name))
                { throw new ArgumentException("The common name cannot be blank", "add_name_pair"); }
            else if (String.IsNullOrEmpty(strScientific_Name))
                { throw new ArgumentException("The scientific name cannot be blank", "add_name_pair"); }

            bool bAdded = false;

            if (!common_to_scientific_dictionary.ContainsKey(strCommon_Name) &&
                !scientific_to_common_dictionary.ContainsKey(strScientific_Name))
            {
                common_to_scientific_dictionary.Add(strCommon_Name, strScientific_Name);
                scientific_to_common_dictionary.Add(strScientific_Name, strCommon_Name);
                bAdded = true;
            }

            return (bAdded);
        }

        /// <summary>
        /// Given a common name, return the scientific name if we know it.
        /// </summary>
        /// <param name="strCommonName">The common name of the bird.</param>
        /// <returns>The common name of the bird or an empty string.</returns>
        /// <exception cref="ArgumentException">Thrown when the common name is null or empty.</exception>
        public string lookup_scientific_name(string strCommon_Name)
        {
            if (String.IsNullOrEmpty(strCommon_Name))
                { throw new ArgumentException("The common name cannot be blank", "lookup_scientific_name"); }

            string strScientific_name = "";
            if (common_to_scientific_dictionary.ContainsKey(strCommon_Name))
            {
                strScientific_name = common_to_scientific_dictionary[strCommon_Name];
            }

            return (strScientific_name);
        }

        /// <summary>
        /// Given a scientific name, return the common name if we know it.
        /// </summary>
        /// <param name="strScientific_Name">The scientific name of the bird.</param>
        /// <returns>The common name of the bird or an empty string.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific name is null or blank.</exception>
        public string lookup_common_name(string strScientific_Name)
        {
            if (String.IsNullOrEmpty(strScientific_Name))
                { throw new ArgumentException("The scientific name cannot be blank", "lookup_common_name"); }

            string strCommon_Name = "";
            if (scientific_to_common_dictionary.ContainsKey(strScientific_Name))
            {
                strCommon_Name = scientific_to_common_dictionary[strScientific_Name];
            }

            return (strCommon_Name);
        }

        /// <summary>
        /// Checks if the scientific name is listed in the library.
        /// </summary>
        /// <param name="strScientific_Name">The scientific name of the bird.</param>
        /// <returns>True if the bird is in the library, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific name is empty or blank.</exception>
        public bool scientific_name_exists(string strScientific_Name)
        {
            if (String.IsNullOrEmpty(strScientific_Name))
                { throw new ArgumentException("The scientific name cannot be blank", "scientific_name_exists"); }

            var bExists = scientific_to_common_dictionary.ContainsKey(strScientific_Name);
            return (bExists);
        }
        
        /// <summary>
        /// Checks if the common name is listed in the library.
        /// </summary>
        /// <param name="strCommon_Name">The common name of the bird.</param>
        /// <returns>True if the bird is in the library, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when the common name is null or empty.</exception>
        public bool common_name_exists(string strCommon_Name)
        {
            if (String.IsNullOrEmpty(strCommon_Name))
                { throw new ArgumentException("The common name cannot be blank", "common_name_exists"); }

            var bExists = common_to_scientific_dictionary.ContainsKey(strCommon_Name);
            return (bExists);
        }

        /// <summary>
        /// Save the library. 
        /// </summary>
        /// <returns>True on Success, false otherwise.</returns>
        public bool save_library()
        {
            bool bSaved = false;

            if ((common_to_scientific_dictionary.Count > 0) && 
                (scientific_to_common_dictionary.Count > 0))
            {
                StringBuilder sb = new StringBuilder(String.Format("<{0}><{1}>", _xml_name_library, _xml_items));
                foreach (var item in common_to_scientific_dictionary.Keys)
                {
                    sb.AppendFormat("<{2}><{3}>{0}</{3}><{4}>{1}</{4}></{2}>",
                                    item, common_to_scientific_dictionary[item],
                                    _xml_item, _xml_common, _xml_scientific);
                }

                sb.Append(String.Format("</{0}></{1}>", _xml_items, _xml_name_library));

                Properties.Settings.Default.NAME_LIBRARY = sb.ToString();
                Properties.Settings.Default.Save();
                bSaved = true;      
            }

            return (bSaved);
        }

        /// <summary>
        /// Load the library.
        /// </summary>        
        public void load_library()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.NAME_LIBRARY))
            {
                var xDoc = Utilities.load_xml_from_string(Properties.Settings.Default.NAME_LIBRARY);
                if (xDoc != null)
                {
                    var list = (from item in xDoc.Root.Element(_xml_items).Elements(_xml_item)
                                        select new string_pair 
                                {
                                    KEY = (string)item.Element(_xml_common),
                                    VALUE = (string)item.Element(_xml_scientific)
                                }).ToList();

                    if ((list != null) && (list.Count > 0))
                    {
                        foreach (var pair in list)
                        {
                            common_to_scientific_dictionary.Add(pair.KEY, pair.VALUE);
                            scientific_to_common_dictionary.Add(pair.VALUE, pair.KEY);
                        }
                    }
                }
            }
        }
    }

    struct string_pair
    {
        private string _key;
        public string KEY
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _value;
        public string VALUE
        {
            get { return _value; }
            set { _value = value; }
        }        
    }
}
