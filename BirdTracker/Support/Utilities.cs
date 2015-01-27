/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.IO;
using System.Xml.Linq;

namespace BirdTracker.Support
{
    /// <summary>
    /// A collection of common static methods.
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// Loads an xml string into a XDocument object so that you can use LINQ to XML.
        /// </summary>
        /// <param name="strXML">The xml string</param>
        /// <returns>null or the XDocument object</returns>
        /// <exception cref="ArgumentException">Thrown when strXML is null or blank</exception>
        public static XDocument load_xml_from_string(String strXML)
        {
            if (String.IsNullOrEmpty(strXML))
            {
                throw new ArgumentException("XML string cannot be null or blank", "load_xml_from_string");
            }

            XDocument xdoc1 = XDocument.Load(new StringReader(strXML));
            return (xdoc1);
        }

        /// <summary>
        /// Convert a string to a double. Will not throw.
        /// </summary>
        /// <param name="str_number">The number as a string.</param>
        /// <returns>The number or zero.</returns>
        public static double string_to_double(string str_number)
        {
            double result;
            double.TryParse(str_number, out result);
            return (result);
        }



    }
}
