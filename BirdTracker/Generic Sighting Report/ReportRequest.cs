using System;

namespace BirdTracker.Generic_Sighting_Report
{
    public enum REPORT_TYPE
    {
        eLOCAL_SIGHTINGS,           // Sightings arround a lat,Long position
        eNOTABLE_SIGHTINGS          // Notable sightings around a lat,long position
    }

    /// <summary>
    /// Contains the information required for fetching a bird report.
    /// </summary>
    public class ReportRequest
    {
        public ReportRequest()
        {
        }
       
        private REPORT_TYPE _report_type;
        public REPORT_TYPE REPORT_TYPE
        {
            get { return _report_type; }
            set {_report_type = value; }
        }
                              
        private string _report_title;
        public string REPORT_TITLE
        {
            get { return _report_title; }
            set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Report Title cannot be blank", "REPORT_TITLE");

                    _report_title = value;                        
                }
        }
                               
        private double _lattitude;
        public double Lattitude
        {
            get { return _lattitude; }
            set {
                    if ((value < -90) || (value > 90))
                    {
                        throw new ArgumentOutOfRangeException("Lattitude must be between -90 and 90", "Lattitude");
                    }

                    _lattitude = value; 
                }
        }
                       
        private double _longitude;
        public double Longitude
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
    }
}
