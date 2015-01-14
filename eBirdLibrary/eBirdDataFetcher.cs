using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;

// Things to do:
// 1. Sort the retured lists by common name.
// 2. Add unit testing project to test the parameter checking for each of these public methods.
// 3. Currently don't support the JSON format.
// 4. Instead of showing multiple entries for the same bird over whatever time period, Just show it once with a count of the number
//    of times it has been reported.

//http://www.findlatitudeandlongitude.com/?loc=Victoria%2C+BC%2C+Canada

//45.42, -75.43   Ottawa
//48.42,-123.36	Victoria
//29.69, -82.33	Gainsville


namespace eBirdLibrary
{
    /// <summary>
    /// Implements the IEBird interface (Fetches data from e-bird).
    /// </summary>
    public sealed class eBirdDataFetcher : IEBird
    {
        /// <summary>
        /// Make a web call. Note: Synchronous
        /// </summary>
        /// <param name="strURL">The URL of the web page</param>        
        private string makeWebCall(String strURL)
        {
            Trace.Assert(!String.IsNullOrEmpty(strURL));

            String strWebPage = "";
            if (!String.IsNullOrEmpty(strURL))
            {
                var wc = new WebClient();
                using (wc)
                {
                    strWebPage = wc.DownloadString(strURL);
                }                
            }

            return (strWebPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strXMLWebResult"></param>
        /// <returns></returns>
        private ObservableCollection<BirdSighting> xml_result_to_birdsighting_list(String strXMLWebResult)
        {
            ObservableCollection<BirdSighting> results = null;
            if (!String.IsNullOrEmpty(strXMLWebResult))
            {
                XDocument document = XDocument.Load(new StringReader(strXMLWebResult));
              
                var sightingsEnum = from sighting in document.Element("response").Element("result").Elements("sighting")
                                    select new BirdSighting
                                    {
                                        _common_name            = sighting.Element("com-name").Value,
                                        _scientific_name        = sighting.Element("sci-name").Value,
                                        _latitude               = sighting.Element("lat").Value,
                                        _longitude              = sighting.Element("lng").Value,
                                        _location_private       = sighting.Element("location-private").Value,
                                        _observation_date       = sighting.Element("obs-dt").Value,
                                        _location_name          = sighting.Element("loc-name") == null ? "" : sighting.Element("loc-name").Value,
                                        _number_observed        = sighting.Element("how-many") == null ? "1" : sighting.Element("how-many").Value,
                                        _Observation_Reviewed   = sighting.Element("obs-reviewed") == null ? "false" : sighting.Element("obs-reviewed").Value,
                                        _Observation_Valid      = sighting.Element("obs-valid") == null ? "false" : sighting.Element("obs-valid").Value,
                                        _location_id            = sighting.Element("loc-id") == null ? "" : sighting.Element("loc-id").Value
                                    };

                var lst = sightingsEnum.ToList<BirdSighting>();                
                results = new ObservableCollection<BirdSighting>(lst);              
            }

            return (results);
        }

        /// <summary>
        /// Only return the latest sighting of a specific bird at a given location instead of all of them 
        /// </summary>
        /// <param name="lstSeedList"></param>
        /// <returns></returns>
        List<BirdSighting> onlyReturnLatestSighting(List<BirdSighting> lstSeedList)
        {
            List<BirdSighting> lstFiltered = null;
            if (lstSeedList != null)
            {
                Dictionary<string, BirdSighting> dcLatestAtLocation = new Dictionary<string, BirdSighting>();
                foreach (BirdSighting bs in lstSeedList)
                {
                    var key = bs._scientific_name + bs._location_id;
                    if (dcLatestAtLocation.ContainsKey(key))
                    {
                        if (!String.IsNullOrEmpty(bs._observation_date) && 
                            !String.IsNullOrEmpty(dcLatestAtLocation[key]._observation_date))
                        {
                            DateTime dt1 = new DateTime();
                            DateTime.TryParse(bs._observation_date, out dt1);

                            DateTime dt2 = new DateTime();
                            DateTime.TryParse(dcLatestAtLocation[key]._observation_date, out dt2);

                            if (dt1 > dt2)
                            {
                                dcLatestAtLocation[key] = bs;
                            }
                        }
                    }
                    else
                    {
                        dcLatestAtLocation.Add(key, bs);
                    }
                }

                lstFiltered = dcLatestAtLocation.Values.ToList();
                lstFiltered.Sort();
            }
            
            return (lstFiltered);
        }

        /// <summary>
        /// Returns the most recent sighting date and specific location for each species of bird
        /// reported within the number of days specified by the "back" parameter and reported in the</summary>
        /// specific area.
        /// <param name="latitude">Decimal latitude, two decimal places required (-90.00 -> 90.00) Required.</param>
        /// <param name="longitude">Decimal longitude, two decimal places required (-180.00 -> 180.00) Required.</param>
        /// <returns>A collection of bird sightings.</returns>
        /// <exception cref="ArgumentException">Thrown if the latitude or longitude is out of range.</exception>
        public Task<ObservableCollection<BirdSighting>> fetch_observations_near_a_location_async(double latitude, 
                                                                                                 double longitude)
        {            
            if (((latitude < -90.00) || (latitude > 90.00)) ||
                (longitude < -180.00 ) || (longitude > 180.00 )
                )
            {
                throw new ArgumentException(String.Format("Invalid latitude {0} or longitude {1}", latitude, longitude));
            }

            return (fetch_observations_near_a_location_internal_async(latitude, longitude));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_observations_near_a_location_internal_async(double latitude, 
                                                                                                           double longitude)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var url = String.Format("http://ebird.org/ws1.1/data/obs/geo/recent?lng={0}&lat={1}", longitude, latitude);               
                var strWebResult = makeWebCall(url);
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return lstResults;
            });

            tsk.Start();
            return tsk;                       
        }
       
        public Task<ObservableCollection<BirdSighting>> fetch_observations_near_a_location_async(double latitude, 
                                                                                                 double longitude, 
                                                                                                 int distance,
                                                                                                 int back, 
                                                                                                 int maxResults, 
                                                                                                 string locale,
                                                                                                 string format, 
                                                                                                 bool provisional, 
                                                                                                 bool hotspot)             
        {
            if (((latitude < -90.00) || (latitude > 90.00)) ||
                ((longitude < -180.00) || (longitude > 180.00)) ||
                ((distance < 0 ) || (distance > 50)) ||
                ((back < 1) || (back > 30)) ||
                ((maxResults < 1 ) || (maxResults > 10000)) 
                )
            {
                throw new ArgumentException(String.Format("Latitude: {0} Longitude: {1}  Distance {2} Back {3} MaxResults {4}",
                                                          latitude, longitude, distance, back, maxResults));
            }

            return (fetch_observations_near_a_location_internal_async(latitude, longitude, distance, back, maxResults, locale, format, provisional, hotspot)); 
       }

        private Task<ObservableCollection<BirdSighting>> fetch_observations_near_a_location_internal_async(double latitude, 
                                                                                                           double longitude, 
                                                                                                           int distance,
                                                                                                           int back, 
                                                                                                           int maxResults, 
                                                                                                           string locale,
                                                                                                           string format, 
                                                                                                           bool provisional, 
                                                                                                           bool hotspot)
       {
           var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
           {               
               var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/geo/recent?");
               sbUrl.AppendFormat("lat={0}", latitude);
               sbUrl.AppendFormat("&lng={0}", longitude);                              
               sbUrl.AppendFormat("&dist={0}", distance);
               sbUrl.AppendFormat("&locale={0}", locale);
               sbUrl.AppendFormat("&fmt={0}", format);
               sbUrl.AppendFormat("&includeProvisional={0}", provisional);
               sbUrl.AppendFormat("&hotspot={0}", hotspot);

               var strWebResult = makeWebCall(sbUrl.ToString());
               var lstResults = xml_result_to_birdsighting_list(strWebResult);
               return lstResults;
           });

           tsk.Start();
           return tsk;            
       }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_observations_of_a_species_async(double latitude,
                                                                                                            double longitude, 
                                                                                                            string strScientificName)
        {
            if (((latitude < -90.00) || (latitude > 90.00)) ||
                ((longitude < -180.00) || (longitude > 180.00)) ||
                (String.IsNullOrEmpty(strScientificName))
                )
            {
                throw new ArgumentException(String.Format("Lattitude: {0} Longitude {1} Name {2}", latitude, longitude, strScientificName));
            }

            return (fetch_recent_nearby_observations_of_a_species_internal_async(latitude, longitude, strScientificName));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_observations_of_a_species_internal_async(double latitude, 
                                                                                                                      double longitude, 
                                                                                                                      string strScientificName)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var url = String.Format("http://ebird.org/ws1.1/data/obs/geo_spp/recent?lng={0}&lat={1}&sci={2}",longitude, latitude, strScientificName);
                var strWebResult = makeWebCall(url);
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return tsk;
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_observations_of_a_species_async(double latitude, 
                                                                                                            double longitude,
                                                                                                            string strScientificName,
                                                                                                            int distance, 
                                                                                                            int back, 
                                                                                                            int maxresults, 
                                                                                                            string locale,
                                                                                                            string format, 
                                                                                                            bool provisional, 
                                                                                                            bool hotspot)
        {
            if (((latitude < -90.00) || (latitude > 90.00)) ||
                ((longitude < -180.00) || (longitude > 180.00)) ||
                ((distance < 0) || (distance > 50)) ||
                ((back < 1) || (back > 30)) ||
                ((maxresults < 1) || (maxresults > 10000)) ||
                String.IsNullOrEmpty(strScientificName)
                )
            {
                throw new ArgumentException(String.Format("Latitude {0} Longitude {1} Distance {2} Back {3} MaxResults {4} Name {5}",
                                                           latitude, longitude, distance, back, maxresults, strScientificName));
            }

            return (fetch_recent_nearby_observations_of_a_species_internal_async(latitude,longitude,strScientificName,distance,back,maxresults,locale,
                                                                                 format, provisional, hotspot));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_observations_of_a_species_internal_async(double latitude, 
                                                                                                                      double longitude, 
                                                                                                                      string scientificName,
                                                                                                                      int distance, 
                                                                                                                      int back, 
                                                                                                                      int maxresults, 
                                                                                                                      string locale,
                                                                                                                      string format, 
                                                                                                                      bool provisional, 
                                                                                                                      bool hotspot)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/geo_spp/recent?");
                sbUrl.AppendFormat("lat={0}", latitude);
                sbUrl.AppendFormat("&lng={0}", longitude);
                sbUrl.AppendFormat("&sci={0}", scientificName);
                sbUrl.AppendFormat("&dist={0}", distance);
                sbUrl.AppendFormat("&back={0}", back);
                sbUrl.AppendFormat("&maxResults={0}", maxresults);
                sbUrl.AppendFormat("&locale={0}", locale);
                sbUrl.AppendFormat("&fmt={0}", format);
                sbUrl.AppendFormat("&includeProvisional={0}", provisional);
                sbUrl.AppendFormat("&hotspot={0}", hotspot);

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();                      
            return (tsk);
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_hotspots_async(List<string> hotspots)
        {
            if ((hotspots == null) || (hotspots.Count > 10))
            {
                throw new ArgumentException(String.Format("Hotspots is either null or exceeds 10: {0}"), hotspots == null ? "null" : hotspots.Count.ToString());
            }
                        
            return fetch_recent_observations_at_hotspots_internal_async(hotspots);
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_hotspots_internal_async(List<string> hotspots)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/hotspot/recent?");

                int iCount = 1;
                foreach (String strLocId in hotspots)
                {                    
                    sbUrl.AppendFormat("r={0}", strLocId);
                    if (iCount != hotspots.Count)
                    {
                        sbUrl.Append("&");                        
                    }

                    iCount = iCount + 1;
                }

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return (tsk);        
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_hotspots_async(List<string> hotspots, 
                                                                                                    int back, 
                                                                                                    int maxresults, 
                                                                                                    string detail,
                                                                                                    string locale, 
                                                                                                    string format, 
                                                                                                    bool includeProvisional)
        {
            if ((hotspots == null) || (hotspots.Count > 10))
            {
                throw new ArgumentException("invalid");         //To do: Add more error checking....
            }

            return (fetch_recent_observations_at_hotspots_internal_async(hotspots, back, maxresults, detail,
                                                                         locale, format, includeProvisional));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_hotspots_internal_async(List<string> hotspots, 
                                                                                                              int back, 
                                                                                                              int maxresults, 
                                                                                                              string detail,
                                                                                                              string locale, 
                                                                                                              string format, 
                                                                                                              bool includeProvisional)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/hotspot/recent?");

                int iCount = 1;
                foreach (String strLocId in hotspots)
                {
                    sbUrl.AppendFormat("r={0}", strLocId);
                    if (iCount != hotspots.Count)
                    {
                        sbUrl.Append("&");
                    }

                    iCount = iCount + 1;
                }

                sbUrl.AppendFormat("&back={0}", back);
                sbUrl.AppendFormat("&maxResults={0}", maxresults);
                sbUrl.AppendFormat("&detail={0}", detail);
                sbUrl.AppendFormat("&locale={0}", locale);
                sbUrl.AppendFormat("&fmt={0}", format);
                sbUrl.AppendFormat("&includeProvisional={0}", includeProvisional);

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return (tsk);
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_hotspots_async(List<string> hotspots, 
                                                                                                                 string scientificName)
        {
            if ( ((hotspots == null) || 
                  (hotspots.Count > 10)) ||
                 (String.IsNullOrEmpty(scientificName))                
               )
            {
                throw new ArgumentException("invalid");
            }
           
            return (fetch_recent_observations_of_a_species_at_hotspots_internal_async(hotspots, scientificName));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_hotspots_internal_async(List<string> hotspots, 
                                                                                                                           string scientificName)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/hotspot_spp/recent?");

                int iCount = 1;
                foreach (String strLocId in hotspots)
                {
                    sbUrl.AppendFormat("r={0}", strLocId);
                    if (iCount != hotspots.Count)
                    {
                        sbUrl.Append("&");
                    }

                    iCount = iCount + 1;
                }

                sbUrl.AppendFormat("&sci={0}", scientificName.Replace(" ","%20"));
                sbUrl.Append("&back=30");

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return (tsk);
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_hotspots_async(List<string> hotspots, 
                                                                                                                 string scientificName, 
                                                                                                                 int back,
                                                                                                                 int maxResults, 
                                                                                                                 string detail, 
                                                                                                                 string locale, 
                                                                                                                 string format,
                                                                                                                 bool includeProvisional)
        {            
            // To do: Parameter checking
                        
            return (fetch_recent_observations_of_a_species_at_hotspots_internal_async(hotspots, scientificName, back,
                                                                                      maxResults, detail, locale, format, includeProvisional));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_hotspots_internal_async(List<string> hotspots,
                                                                                                                           string scientificName, 
                                                                                                                           int back,
                                                                                                                           int maxResults, 
                                                                                                                           string detail, 
                                                                                                                           string locale, 
                                                                                                                           string format,
                                                                                                                           bool includeProvisional)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/hotspot_spp/recent?");

                int iCount = 1;
                foreach (String strLocId in hotspots)
                {
                    sbUrl.AppendFormat("r={0}", strLocId);
                    if (iCount != hotspots.Count)
                    {
                        sbUrl.Append("&");
                    }

                    iCount = iCount + 1;
                }

                sbUrl.AppendFormat("&sci={0}", scientificName);
                sbUrl.AppendFormat("&back={0}", back);
                sbUrl.AppendFormat("&maxResults={0}", maxResults);
                sbUrl.AppendFormat("&detail={0}", detail);
                sbUrl.AppendFormat("&fmt={0}", format);
                sbUrl.AppendFormat("&includeProvisional={0}", includeProvisional);

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return (tsk);
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_locations_async(List<string> locations)
        {
            if ((locations == null) || (locations.Count > 10))
            {
                throw new ArgumentException("invalid");
            }

            return (fetch_recent_observations_at_locations_internal_async(locations));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_locations_internal_async(List<string> locations)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/loc/recent?");

                int iCount = 1;
                foreach (String strLocId in locations)
                {
                    sbUrl.AppendFormat("r={0}", strLocId);
                    if (iCount != locations.Count)
                    {
                        sbUrl.Append("&");
                    }

                    iCount = iCount + 1;
                }

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return (tsk);            
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_locations_async(List<string> locations, 
                                                                                                     int back,
                                                                                                     int maxResults, 
                                                                                                     string detail, 
                                                                                                     string locale, 
                                                                                                     string format,
                                                                                                     bool includeProvisional)
        {
            if ((locations == null) || (locations.Count > 10))
            {
                throw new ArgumentException("invalid");         // To do: Add more sanity checking here.
            }

            return (fetch_recent_observations_at_locations_internal_async(locations, back, maxResults, detail, locale, format, includeProvisional));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_observations_at_locations_internal_async(List<string> locations, 
                                                                                                               int back, 
                                                                                                               int maxResults, 
                                                                                                               string detail, 
                                                                                                               string locale, 
                                                                                                               string format, 
                                                                                                               bool includeProvisional)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/loc/recent?");                

                int iCount = 1;
                foreach (String strLocId in locations)
                {
                    sbUrl.AppendFormat("r={0}", strLocId);
                    if (iCount != locations.Count)
                    {
                        sbUrl.Append("&");
                    }

                    iCount = iCount + 1;
                }

                sbUrl.AppendFormat("&back={0}",back);
                sbUrl.AppendFormat("&maxResults={0}", maxResults);
                sbUrl.AppendFormat("&detail={0}", detail);
                sbUrl.AppendFormat("&locale={0}", locale);
                sbUrl.AppendFormat("&fmt={0}", format);
                sbUrl.AppendFormat("&includeProvisional={0}", includeProvisional);

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return (tsk);            
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_locations_async(List<string> locations, 
                                                                                                                  string scientificName)
        {
            if ((locations == null) || 
                (locations.Count > 10) || 
                String.IsNullOrEmpty(scientificName))
            {
                throw new ArgumentException("invalid");
            }
                        
            return (fetch_recent_observations_of_a_species_at_locations_internal_async(locations, scientificName));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_locations_internal_async(List<string> locations,
                                                                                                                            string scientificName)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/loc_spp/recent?");

                int iCount = 1;
                foreach (String strLocId in locations)
                {
                    sbUrl.AppendFormat("r={0}", strLocId);
                    if (iCount != locations.Count)
                    {
                        sbUrl.Append("&");
                    }

                    iCount = iCount + 1;
                }

                sbUrl.AppendFormat("&sci={0}", scientificName);

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return (tsk);                        
        }


        public Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_locations_async(List<string> locations,
                                                                                                                  string scientificName,
                                                                                                                  int back,
                                                                                                                  int maxResults, 
                                                                                                                  string detail, 
                                                                                                                  string locale, 
                                                                                                                  string frmt, 
                                                                                                                  bool includeProvisional)
        {
            if ((locations == null) || (locations.Count > 10) || String.IsNullOrEmpty(scientificName))
            {
                throw new ArgumentException("invalid");         // To do: Add more parameter checking.
            }

            return (fetch_recent_observations_of_a_species_at_locations_internal_async(locations, scientificName, back, maxResults, detail, locale, frmt, includeProvisional));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_observations_of_a_species_at_locations_internal_async(List<string> locations,
                                                                                                                            string scientificName,
                                                                                                                            int back, 
                                                                                                                            int maxResults,
                                                                                                                            string detail, 
                                                                                                                            string locale, 
                                                                                                                            string frmt, 
                                                                                                                            bool includeProvisional)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var sbUrl = new StringBuilder("http://ebird.org/ws1.1/data/obs/loc_spp/recent?");

                int iCount = 1;
                foreach (String strLocId in locations)
                {
                    sbUrl.AppendFormat("r={0}", strLocId);
                    if (iCount != locations.Count)
                    {
                        sbUrl.Append("&");
                    }

                    iCount = iCount + 1;
                }

                sbUrl.AppendFormat("&sci={0}", scientificName);
                sbUrl.AppendFormat("&back={0}", back);
                sbUrl.AppendFormat("&maxResults={0}", maxResults);
                sbUrl.AppendFormat("&detail={0}", detail);
                sbUrl.AppendFormat("&locale={0}", locale);
                sbUrl.AppendFormat("&fmt={0}", frmt);
                sbUrl.AppendFormat("&includeProvisional={0}", includeProvisional);

                var strWebResult = makeWebCall(sbUrl.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return (lstResults);
            });

            tsk.Start();
            return (tsk);            
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_notable_observations_async(double latitude, 
                                                                                                       double longitude)
        {
            if (((latitude < -90.00) || (latitude > 90.00)) ||
                (longitude < -180.00 ) || (longitude > 180.00 )
            )
            {
                throw new ArgumentException(String.Format("Invalid latitude {0} or longitude {1}", latitude, longitude));
            }

            return (fetch_recent_nearby_notable_observations_intenal_async(latitude, longitude));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_notable_observations_intenal_async(double latitude, 
                                                                                                                double longitude)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var url = String.Format("http://ebird.org/ws1.1/data/notable/geo/recent?lng={0}&lat={1}", longitude, latitude);
                var strWebResult = makeWebCall(url);
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return lstResults;
            });

            tsk.Start();
            return tsk;            
        }

        public Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_notable_observations_async(double latitude, 
                                                                                                       double longitude, 
                                                                                                       int dist, 
                                                                                                       int back,
                                                                                                       int maxResults, 
                                                                                                       string detail, 
                                                                                                       string locale, 
                                                                                                       string format,
                                                                                                       bool hotspot)
        {
            if (((latitude < -90.00) || (latitude > 90.00)) ||
                 (longitude < -180.00) || (longitude > 180.00)
                )
            {
                throw new ArgumentException(String.Format("Invalid latitude {0} or longitude {1}", latitude, longitude));    // To do: Add more parameter checking...
            }

            return (fetch_recent_nearby_notable_observations_internal_async(latitude, longitude, dist, back, maxResults, detail,
                                                                            locale, format, hotspot));
        }

        private Task<ObservableCollection<BirdSighting>> fetch_recent_nearby_notable_observations_internal_async(double latitude, 
                                                                                                                 double longitude, 
                                                                                                                 int dist, 
                                                                                                                 int back,
                                                                                                                 int maxResults, 
                                                                                                                 string detail, 
                                                                                                                 string locale, 
                                                                                                                 string format,
                                                                                                                 bool hotspot)
        {
            var tsk = new Task<ObservableCollection<BirdSighting>>(() =>
            {                
                var url = new StringBuilder(String.Format("http://ebird.org/ws1.1/data/notable/geo/recent?lng={0}&lat={1}", longitude, latitude));
                url.AppendFormat("&dist={0}", dist);
                url.AppendFormat("&back={0}", back);
                url.AppendFormat("&maxResults={0}", maxResults);
                url.AppendFormat("&detail={0}", detail);
                url.AppendFormat("&locale={0}", locale);
                url.AppendFormat("&fmt={0}", format);
                url.AppendFormat("&hotspot={0}", hotspot);
                                
                var strWebResult = makeWebCall(url.ToString());
                var lstResults = xml_result_to_birdsighting_list(strWebResult);
                return lstResults;
            });

            tsk.Start();
            return tsk;            
        }
    }
}
