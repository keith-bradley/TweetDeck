using System.IO;
using System.Xml.Serialization;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace BirdTracker.Support
{
    public static class Helper
    {
        /// <summary>
        /// Serialize any object to an xml string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToString(object obj)
        {
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());

                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    return writer.ToString();
                }
            }

            return (null);
        }    
    }
}
