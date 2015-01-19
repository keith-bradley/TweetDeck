using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace eBirdLibrary
{
    // An interface for communicating with the Cornel University e-bird database.
    // Currently supports version 1.1 of the API.
    //
    // http://ebird.org/ws1.1/data/obs/geo/recent?lng=-75.43&lat=45.42  (Ottawa's Lat and Long)
    //
    //
    //  Note: This interface specifies 2 versions of the same call. The one with the fewest 
    //        number of parameters has no optional paramaters. The other one will have optional parameters.
    //    
    
    /// <summary>
    /// A single sighting of a bird
    /// </summary>
    public class BirdSighting : INotifyPropertyChanged, IComparable
    {
        public event PropertyChangedEventHandler PropertyChanged;
                
        public String _common_name          { get; set; }
        public String _scientific_name      { get; set; }
        public String _observation_date     { get; set; }
        public String _number_observed      { get; set; }
        public String _location_id          { get; set; }
        public String _location_private     { get; set; }
        public String _location_name        { get; set; }
        public String _latitude             { get; set; }
        public String _longitude            { get; set; }
        public String _Observation_Reviewed { get; set; }
        public String _Observation_Valid    { get; set; }
        
        private BitmapSource _thumbnl;
        public BitmapSource ThumbNail
        {
            get { return _thumbnl; }
            set { _thumbnl = value;
                  OnPropertyChanged("ThumbNail");
                }
        }
        
        protected virtual void OnPropertyChanged(string propName)
        {
            if (!String.IsNullOrEmpty(propName) && (PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        /// <summary>
        /// Default compare method. Here we compare the birds common_name
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            BirdSighting otherBird = obj as BirdSighting;
            if (otherBird != null)
            {
                return this._common_name.CompareTo(otherBird._common_name);
            }
            else
                throw new ArgumentException("Object is not a BirdSighting");
        }
    }

    interface IEBird
    {        
        /// <summary>
        /// Returns the most recent sighting date and specific location for each species of bird
        /// reported within the number of days specified by the "back" parameter and reported in the
        /// specific area.
        /// 
        /// Example: http://ebird.org/ws1.1/data/obs/geo/recent?lng=-75.43&lat=45.42
        /// 
        /// </summary>
        /// <param name="latitude">Decimal latitude, two decimal places required (-90.00 -> 90.00) Required.</param>
        /// <param name="longitude">Decimal longitude, two decimal places required (-180.00 -> 180.00) Required.</param>
        /// <returns>A collection of bird sightings:
        /// JSON      XML  
        /// ConName | com-name  : Species common name. Not included in the 'simple' detail if a scientific name was specified as an input paramter.
        /// sciName | sci-name  : Species scientific name. 
        /// obsDt   | obs-dt    : Observation date formated YYYY-MM-DD or YYYY-MM-DD hh:mm
        /// howMany | how-many  : The number observed
        /// locId   | loc-id    : Unique identifier for the location
        /// locationPrivate |location-private : true if location is not a birding hotspot. False otherwise
        /// locName | loc-name : location name.
        /// lat | lat : Latitude of the location.        
        /// lng | lng : Longitude of the location.
        /// obsReviewed | obs-reviewed : 'true' if obs has been reviewed. 'false' otherwise.
        /// obsValid | obs-valid  : true if obs has been deemed valid by either the automatic filters or a regional reviewer. 'false' otherwise.
        /// </returns>        
        Task<ObservableCollection<BirdSighting>> fetch_observations_near_a_location_async(double latitude, 
                                                                                          double longitude);

        /// <summary>
        /// Returns the most recent sighting date and specific location for each species of bird
        /// reported within the number of days specified by the "back" parameter and reported in the
        /// specific area.
        /// </summary>
        /// <param name="latitude">Decimal latitude, two decimal places requuired (-90.00 -> 90.00) Required.</param>
        /// <param name="longitude">Decimal longitude, two decimal places required (-180.00 -> 180.00) Required.</param>
        /// <param name="distance">Distance defining radius of interest from given lat/long in klms (0-50) 25 is the default.</param>
        /// <param name="back">The number of days back to look for observations. (1-30 days) 7 is the default.</param>
        /// <param name="maxResults">The number of results to return in this request. (1-10,000). Null = All results.</param>
        /// <param name="locale">Java standard locale codes (en_US, fr_ca) etc.</param>
        /// <param name="format">Format of the response (json, xml).</param>
        /// <param name="provisional">Set to true if flagged records that have not yet been reviewed to be included in the results.</param>
        /// <param name="hotspot">Set to true if resullts should be limited to sightings at birding hotspots.</param>
        /// <returns>See return information above</returns>
        Task<ObservableCollection<BirdSighting>> fetch_observations_near_a_location_async(double latitude, 
                                                                                          double longitude, 
                                                                                          int distance,
                                                                                          int back, 
                                                                                          int maxResults, 
                                                                                          string locale,
                                                                                          string format, 
                                                                                          bool provisional, 
                                                                                          bool hotspot);
        
        /// <summary>
        /// Returns the most recent sighting date and location for the requested species of bird.
        /// </summary>
        /// <param name="latitude">Decimal latitude, two decimal places required (-90.00 -> 90.00) Required.</param>
        /// <param name="longitude">Decimal longitude, two decimal places required</param>
        /// <param name="strScientificName">The scientific name of the bird.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_observations_of_a_species_async(double latitude, 
                                                                                                     double longitude, 
                                                                                                     string strScientificName);

        /// <summary>
        /// Returns the most recent sighting date and location for the requested species of bird.
        /// </summary>
        /// <param name="latitude">Decimal latitude, two decimal places required (-90.00 -> 90.00) Required.</param>
        /// <param name="longitude">Decimal longitude, two decimal places required.</param>
        /// <param name="strScientificName">The scientific name of the bird.</param>
        /// <param name="distance">Distance defining radius of interest from given lat/long in klms (0-50) 25 is the default.</param>
        /// <param name="back">The number of days back to look for observations. (1-30 days) 7 is the default.</param>
        /// <param name="maxresults">The number of results to return in this request. (1-10,000). Null = All results.</param>
        /// <param name="locale">Java standard locale codes (en_US, fr_ca) etc.</param>
        /// <param name="format">Format of the response (json, xml).</param>
        /// <param name="provisional">Set to true if flagged records that have not yet been reviewed to be included in the results.</param>
        /// <param name="hotspot">Set to true if resullts should be limited to sightings at birding hotspots.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_observations_of_a_species_async(double latitude, 
                                                                                                     double longitude, 
                                                                                                     string strScientificName,
                                                                                                     int distance, 
                                                                                                     int back, 
                                                                                                     int maxresults, 
                                                                                                     string locale,
                                                                                                     string format, 
                                                                                                     bool provisional, 
                                                                                                     bool hotspot);                

        /// <summary>
        /// Returns the most recent sighting date and specific location for each species of bird reported
        /// within the number of days specified by the "back" parameter and reported at birding hotspots
        ///
        /// Locations: Ottawa--Central Experimental Farm Arboretum : L351702
        ///            Ottawa--Shirley's Bay : L351705                 
        /// </summary>
        /// <param name="hotspots">Codes for region of interest, here, regions are the locIDs of hotspots. Maximum of 10.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_hotspots_async(List<string> hotspots);

        /// <summary>
        /// Returns the most recent sighting date and specific location for each species of bird reported
        /// within the number of days specified by the "back" parameter and reported at birding hotspots
        ///
        /// Locations: Ottawa--Central Experimental Farm Arboretum : L351702
        ///            Ottawa--Shirley's Bay : L351705                 
        /// </summary>
        /// <param name="hostspots">Codes for region of interest, here, regions are the locIDs of hotspots. Maximum of 10.</param>
        /// <param name="back">The number of days back to look for observations. (1-30 days) 7 is the default.</param>
        /// <param name="maxresults">The number of results to return in this request. (1-10,000). Null = All results.</param>
        /// <param name="detail">Either simple or full (Simple is the default)</param>
        /// <param name="locale">Java standard locale codes (en_US, fr_ca) etc.</param>
        /// <param name="format">Format of the response (json, xml).</param>
        /// <param name="includeProvisional">Set to true if flagged records that have not yet been reviewed to be included in the results.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_hotspots_async(List<string> hotspots, 
                                                                                             int back, 
                                                                                             int maxresults, 
                                                                                             string detail,
                                                                                             string locale, 
                                                                                             string format, 
                                                                                             bool includeProvisional);

        /// <summary>
        /// Returns the most recent sighting date and specific location for the requested species of bird reported within the
        /// number of days specified by the "back" parameter and reported in a given list of hotspots.
        /// </summary>
        /// <param name="hotspots">Codes for region of interest, here, regions are the locIDs of hotspots. Maximum of 10.</param>
        /// <param name="scientificName">The scientific name of the bird.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_hotspots_async(List<string> hotspots, 
                                                                                                          string scientificName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotspots"></param>
        /// <param name="scientificName"></param>
        /// <param name="back"></param>
        /// <param name="maxResults"></param>
        /// <param name="detail"></param>
        /// <param name="locale"></param>
        /// <param name="format"></param>
        /// <param name="includeProvisional"></param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_hotspots_async(List<string> hotspots, 
                                                                                                          string scientificName, 
                                                                                                          int back,
                                                                                                          int maxResults, 
                                                                                                          string detail, 
                                                                                                          string locale, 
                                                                                                          string format,
                                                                                                          bool includeProvisional);
        /// <summary>
        /// Returns a list of recent observations at birding locations.
        /// </summary>
        /// <param name="locations">Code(s) for region of interest, here, regions are any locId. Maximum of 10 ids.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_locations_async(List<string> locations);

        /// <summary>
        /// Returns a list of recent observations at birding locations.
        /// </summary>
        /// <param name="locations">Code(s) for region of interest, here, regions are any locId. Maximum of 10 ids.</param>
        /// <param name="back">The number of days back to look for observations. (1-30 days) 7 is the default.</param>
        /// <param name="maxResults">The number of results to return in this request. (1-10,000). Null = All results.</param>
        /// <param name="detail">Either simple or full (Simple is the default)</param>
        /// <param name="locale">Java standard locale codes (en_US, fr_ca) etc.</param>
        /// <param name="format">Format of the response (json, xml).</param>
        /// <param name="includeProvisional">Set to true if flagged records that have not yet been reviewed to be included in the results.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_locations_async(List<string> locations, 
                                                                                              int back, 
                                                                                              int maxResults, 
                                                                                              string detail, 
                                                                                              string locale, 
                                                                                              string format,
                                                                                              bool includeProvisional);
        /// <summary>
        /// Returns a list of all bird species observed in a given list of locations along with their most recent sighting and location.
        /// </summary>
        /// <param name="locations">Code(s) for region of interest, here, regions are any locId. Maximum of 10 ids.</param>
        /// <param name="scientificName">The scientific name of the bird.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_locations_async(List<string> locations, 
                                                                                                           string scientificName);

        /// <summary>
        /// Returns a list of all bird species observed in a given list of locations along with their most recent sighting and location.
        /// </summary>
        /// <param name="locations">Code(s) for region of interest, here, regions are any locId. Maximum of 10 ids.</param>
        /// <param name="scientificName">The scientific name of the bird.</param>
        /// <param name="back">The number of days back to look for observations. (1-30 days) 7 is the default</param>
        /// <param name="maxResults">The number of results to return in this request. (1-10,000). Null = All results.</param>
        /// <param name="detail">Either simple or full (Simple is the default)</param>
        /// <param name="locale">Java standard locale codes (en_US, fr_ca) etc</param>
        /// <param name="frmt">Format of the response (json, xml).</param>
        /// <param name="includeProvisional">Set to true if flagged records that have not yet been reviewed to be included in the results.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_locations_async(List<string> locations, 
                                                                                                           string scientificName, 
                                                                                                           int back, 
                                                                                                           int maxResults, 
                                                                                                           string detail, 
                                                                                                           string locale, 
                                                                                                           string frmt, 
                                                                                                           bool includeProvisional);
        /// <summary>
        /// Returns all recent observations of notable bird species near a given area.
        /// </summary>
        /// <param name="latitude">Decimal latitude, two decimal places required (-90.00 -> 90.00) Required.</param>
        /// <param name="longitude">Decimal longitude, two decimal places required.</param>        
        Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_notable_observations_async(double latitude, 
                                                                                                double longitude);

        /// <summary>
        /// Returns all recent observations of notable bird species near a given area.
        /// </summary>
        /// <param name="latitude">Decimal latitude, two decimal places required (-90.00 -> 90.00) Required.</param>
        /// <param name="longitude">Decimal longitude, two decimal places required.</param>
        /// <param name="dist">Distance defining radius of interest from given lat/long in klms (0-50) 25 is the default.</param>
        /// <param name="back">The number of days back to look for observations. (1-30 days) 7 is the default</param>
        /// <param name="maxResults">The number of results to return in this request. (1-10,000). Null = All results.</param>
        /// <param name="detail">Either simple or full (Simple is the default)</param>
        /// <param name="locale">Java standard locale codes (en_US, fr_ca) etc</param>
        /// <param name="format">Format of the response (json, xml).</param>
        /// <param name="hotspot">Codes for region of interest, here, regions are the locIDs of hotspots. Maximum of 10.</param>
        Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_notable_observations_async(double latitude, 
                                                                                                double longitude, 
                                                                                                int dist, 
                                                                                                int back, 
                                                                                                int maxResults, 
                                                                                                string detail, 
                                                                                                string locale, 
                                                                                                string format, 
                                                                                                bool hotspot);
    }
}
