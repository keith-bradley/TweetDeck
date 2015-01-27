/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

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
        /// <summary>
        /// CTOR
        /// </summary>
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
                        { throw new ArgumentException("Report Title cannot be blank", "REPORT_TITLE"); }

                    _report_title = value;                        
                }
        }                               
    }
}
