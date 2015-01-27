/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using System.Text;

namespace BirdTracker
{
    /// <summary>
    /// The types of reports that are supported by the application
    /// </summary>
    public enum ReportType
    {
        eSIGHTINGSNEARALOCATION     = 0,          // Sightings around a lattitude / longitude
        eNOTABLE_SIGHTINGS          = 1,          // Notable sightings around a lattitude / longitude
        eSPECIES_SIGHTING           = 2,          // A specific sighting of a specific bird at a lattitude / longitude
        eHOTSPOT_SIGHTING           = 3,          // Sightings at what ebird considers a hotspot (not sure what the criteria is.
        eSPECIES_NEAR_HOTSPOT       = 4,          // Sightings of a specific bird at a hotspot.
        eSIGHTINGS_NEAR_EBIRD_LOC   = 5,          // Sightings of around an ebird defined location.
    }
        
    /// <summary>
    /// Defines the type of report to request.
    /// </summary>
    public class ReportRequest
    {
        public ReportRequest(ReportType type)
        {
            REPORT_TYPE = type;
        }

        //For serialization
        public ReportRequest()
        {
        }

        public ReportType   REPORT_TYPE   { get; set; }
        
        /// <summary>
        /// The title of the report.
        /// </summary>
        private String _report_title;
        public String REPORT_TITLE
        {
            get { return _report_title; }
            set {
                    if (String.IsNullOrEmpty(value))
                        { throw new ArgumentException("Report Title should not be blank", "REPORT_TITLE"); }

                    _report_title = value; 
                }
        }
                                       
        private double _lattitude;
        public double LATTITUDE
        {
            get { return _lattitude; }
            set {
                    if ((value < -90) || (value > 90))
                        { throw new ArgumentOutOfRangeException("Lattitude must be between -90 and 90", "Lattitude"); }

                    _lattitude = value; 
                }
        }
                                                   
        private double _longitude;
        public double LONGITUDE
        {
            get { return _longitude; }
            set {
                    if ((value < -180) || (value > 180))
                    {
                        throw new ArgumentOutOfRangeException("Longitude must be between -180 and 180", "Longitude");
                    }

                    _longitude = value; 
                }
        }
                                     
        public String       SPECIES       { get; set; }
        public List<string> HOT_SPOTS     { get; set; }

        /// <summary>
        /// Converts the object to XML.
        /// </summary>
        /// <returns>The object as XML.</returns>
        public string to_xml()
        {
            StringBuilder sb = new StringBuilder("<Report_Request>");
            sb.AppendFormat("<report_title>{0}</report_title>"  , REPORT_TITLE);
            sb.AppendFormat("<report_type>{0}</report_type>"    , REPORT_TYPE.ToString());
            sb.AppendFormat("<lattitude>{0}</lattitude>"        , LATTITUDE);
            sb.AppendFormat("<longitude>{0}</longitude>"        , LONGITUDE);
            sb.AppendFormat("<species>{0}</species>"            , SPECIES);

            if ((HOT_SPOTS != null) && (HOT_SPOTS.Count > 0))
            {
                sb.Append("<hot_spots>");
                foreach (var spot in HOT_SPOTS)
                {
                    sb.AppendFormat("<spot>{0}</spot>", spot);
                }
                sb.Append("</hot_spots>");
            }

            sb.Append("</Report_Request>");
            return (sb.ToString());
        }
    }
}
