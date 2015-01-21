using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BirdTracker.Exclude_Librarian;
using BirdTracker.Image_Librarian;
using BirdTracker.Interfaces;
using BirdTracker.Location_Manager;
using BirdTracker.Name_Librarian;
using BirdTracker.Support;
using eBirdLibrary;
using BirdTracker.Pin_Map;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace BirdTracker
{
    /// <summary>
    /// A ViewModel for the BirdSighting Report.
    /// </summary>
    public class GenericSightingReportVM : INotifyPropertyChanged
    {
        private ILocationManager location_manager = LocationManager.getInstance();
        private INameLibrary     named_library    = NameLibrarian.get_instance();
        private IExcudeLibary    exclude_library  = ExcludeLibrarian.get_instance();

        private ReportRequest ReportRequest;

        private String _ReportTitle;
        public String Report_Title
        {
            get { return _ReportTitle; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Report Title cannot be blank.", "Report_Title");

                _ReportTitle = value;
                OnPropertyChanged("Report_Title");
            }
        }

        private ObservableCollection<eBirdLibrary.BirdSighting> _lstSightings;
        public ObservableCollection<eBirdLibrary.BirdSighting> ListSightings
        {
            get { return _lstSightings; }
            set
            {
                _lstSightings = value;
                OnPropertyChanged("ListSightings");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propName)
        {
            try
            {
                if (!String.IsNullOrEmpty(propName) && (PropertyChanged != null))
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private ICommand _CloseReportCMD;
        public ICommand CLOSE_REPORT_CMD
        {
            get { return _CloseReportCMD; }
            set { _CloseReportCMD = value; }
        }

        private ICommand _GetLocationReport;
        public ICommand GET_LOCATIONREPORT_CMD
        {
            get { return _GetLocationReport; }
            set { _GetLocationReport = value; }
        }

        private ICommand _getSpeciesReport;
        public ICommand GET_SPECIES_REPORT_CMD
        {
            get { return _getSpeciesReport; }
            set { _getSpeciesReport = value; }
        }

        /// <summary>
        /// Add the selected species to the excluded species list.
        /// </summary>
        private ICommand _excludeSpeciesCMD;
        public ICommand EXCLUDE_SPECIES_CMD
        {
            get { return _excludeSpeciesCMD; }
            set { _excludeSpeciesCMD = value; }
        }

        /// <summary>
        /// Show the location where this species was spoted on a map.
        /// </summary>
        private ICommand _pinLocationCMD;
        public ICommand PIN_LOCATION_CMD
        {
            get { return _pinLocationCMD; }
            set { _pinLocationCMD = value; }
        }

        /// <summary>
        /// Take all of the sightings in the column and pin them on a map.
        /// </summary>
        private ICommand _pin_all_locations_CMD;
        public ICommand PIN_ALL_LOCATIONS_CMD
        {
            get { return _pin_all_locations_CMD; }
            set { _pin_all_locations_CMD = value; }
        }
        
        /// <summary>
        /// Reference to the windows manager (MainWindow.xaml).
        /// </summary>
        private IWindowManager _WindowManager;
        public IWindowManager WindowManager
        {
            get { return _WindowManager; }
            set {
                    if (value == null)
                    {
                        throw new ArgumentNullException("Window Manager cannot be null.", "WindowManager");
                    }

                    _WindowManager = value; 
                }
        }
                                   
        /// <summary>
        /// The view
        /// </summary>
        private GenericBirdSightingsReport _view;
        public GenericBirdSightingsReport _View
        {
            get { return _view; }
            set {
                if (value == null)
                {
                    throw new ArgumentNullException("The view cannot be null", "_View");
                }

                    _view = value; 
                }
        }
        
        /// <summary>
        /// CTOR - Create a generic report.
        /// Note you must call setReportRequestAndFetchData after calling the constructor.
        /// </summary>
        public GenericSightingReportVM()
        {
            _lstSightings          = new ObservableCollection<BirdSighting>();
            CLOSE_REPORT_CMD       = new RelayCommand(new Action<object>(closeReport));
            GET_LOCATIONREPORT_CMD = new RelayCommand(new Action<object>(mapLocation));
            GET_SPECIES_REPORT_CMD = new RelayCommand(new Action<object>(speciesLocation));
            EXCLUDE_SPECIES_CMD    = new RelayCommand(new Action<object>(exclude_species));
            PIN_LOCATION_CMD       = new RelayCommand(new Action<object>(pin_location_on_map));
            PIN_ALL_LOCATIONS_CMD  = new RelayCommand(new Action<object>(pin_all_locations_on_map));
        }

        /// <summary>
        /// Close the report.
        /// </summary>        
        private void closeReport(object o)
        {
            if (WindowManager != null)
                { WindowManager.closeReport(_View); }
        }

        /// <summary>
        /// Pin a single location on a map
        /// </summary>
        /// <param name="o">The listbox control.</param>
        private void pin_location_on_map(object o)
        {
            var lb = o as ListBox;
            var si = lb.SelectedItems;
            if (si != null)
            {
                var item = si[0] as BirdSighting;

                PinMapWindow window = new PinMapWindow();
                double lat;
                double.TryParse(item._latitude, out lat);

                double lng;
                double.TryParse(item._longitude, out lng);

                var col = new List<BirdPinData>();
                var data = new BirdPinData(latitude: lat, longitude: lng, common_name: item._common_name, scientific_name: item._scientific_name);
                data.DATE_REPORTED = item._observation_date;
                col.Add(data);
                window.initialize(col);
                window.ShowDialog();
            }        
        }

        /// <summary>
        /// Pin all of the bird sightings on a map.
        /// </summary>
        /// <param name="o">The listbox control.</param>
        private void pin_all_locations_on_map(object o)
        {
            var lb = o as ListBox;
            var items = lb.Items;
            if ((items != null) & (items.Count > 0))
            {
                var collection = new List<BirdPinData>();
                foreach (var item in items)
                {
                    var bird_sighting = item as BirdSighting;
                    double lat;
                    double.TryParse(bird_sighting._latitude, out lat);

                    double lng;
                    double.TryParse(bird_sighting._longitude, out lng);

                    var data = new BirdPinData(latitude: lat, longitude: lng, common_name: bird_sighting._common_name,
                                                    scientific_name: bird_sighting._scientific_name);

                    data.DATE_REPORTED = bird_sighting._observation_date;
                    collection.Add(data);
                }

                PinMapWindow window = new PinMapWindow();
                window.initialize(collection);
                window.ShowDialog();            
            }
        }

        /// <summary>
        /// Exclude the specified species
        /// </summary>
        /// <param name="o"></param>
        private void exclude_species(object o)
        {
            // When dealing with a context menu (cm) we have to get the control that the cm is bound to.
            // Then we can use the members of the control to get the data we are really looking for.
            var lb = o as ListBox;
            var si = lb.SelectedItems;
            if (si != null)
            {
                var item = si[0] as BirdSighting;
                if (WindowManager != null)
                {
                    if (WindowManager.add_species_to_exclude_list(item._scientific_name))
                    {
                        WindowManager.refresh_excluded_species_from_all_columns();
                    }
                }                
            }                     
        }

        /// <summary>
        /// 
        /// </summary>
        public void stop_showing_excluced_species()
        {
            ObservableCollection<BirdSighting> cleaned_collection = new ObservableCollection<BirdSighting>();            
            foreach (var sighting in ListSightings)
            {
                if (!exclude_library.item_is_in_library(sighting._scientific_name))
                {
                    cleaned_collection.Add(sighting);
                }
            }

            ListSightings = cleaned_collection;
        }

        /// <summary>
        /// User has clicked on a location they want to see a report about.
        /// </summary>        
        private void mapLocation(object o)
        {
            if (WindowManager != null)
            {
                var locationLabel = o.ToString();                       //Location ID: L631202
                if (!String.IsNullOrEmpty(locationLabel))
                {
                    var arr = locationLabel.Split(':');
                    var loc = arr[1].Trim();

                    if (!String.IsNullOrEmpty(loc))
                    {
                        var realworldlocation = location_manager.get_real_world_location(loc);

                        ReportRequest rep = new ReportRequest(ReportType.eSIGHTINGS_NEAR_EBIRD_LOC);
                        rep.REPORT_TITLE = String.Format("Birds at Location: {0}", realworldlocation);
                        rep.HOT_SPOTS = new List<string>() { loc };
                        WindowManager.createReportWindow(rep);
                    }
                }
            }
        }

        /// <summary>
        /// User has clicked on the scientific name of a bird which generates a report
        /// of the species at a given location.
        /// </summary>        
        private void speciesLocation(object p)
        {
            if (p != null)
            {
                find_species_at_lat_long_params parms = p as find_species_at_lat_long_params;

                ReportRequest rep = new ReportRequest(ReportType.eSPECIES_SIGHTING);
                rep.REPORT_TITLE = String.Format("{0} around {1} : {2}", 
                                                 parms.species_common_name, ReportRequest.LATTITUDE, ReportRequest.LONGITUDE);
                
                rep.SPECIES   = parms.species_name;
                rep.LATTITUDE = ReportRequest.LATTITUDE;
                rep.LONGITUDE = ReportRequest.LONGITUDE;

                WindowManager.createReportWindow(rep);
            }
        }
        
        /// <summary>
        /// Runs through the provided list of bird sightings and removes those birds that are on the exclude list.
        /// </summary>
        /// <param name="lstBirds">List of bird sightings.</param>
        /// <returns>A clean collection of bird sightings.</returns>
        private ObservableCollection<BirdSighting> remove_excluded_birds(ObservableCollection<BirdSighting> lstBirds)
        {
            ObservableCollection<BirdSighting> cleaned_list = new ObservableCollection<BirdSighting>();
            if ((lstBirds != null) && (lstBirds.Count > 0))
            {                
                foreach (var sighting in lstBirds)
                {
                    if (!exclude_library.item_is_in_library(sighting._scientific_name))
                    {
                        cleaned_list.Add(sighting);
                    }
                }
            }

            return (cleaned_list);
        }

        /// <summary>
        /// Tells the VM what type of report to get. 
        /// </summary>
        /// <param name="rr">The report request.</param>
        public void setReportRequestAndFetchData(ReportRequest rr)
        {
            if (rr != null)
            {
                ReportRequest = rr;
                Report_Title = rr.REPORT_TITLE;
                fetch_and_populate_column();
            }
        }

        /// <summary>
        /// Fetches the data from E-Bird and populates the column.
        /// </summary>
        private void fetch_and_populate_column()
        {
            var fetchTask = fetch_data_asyc();
            if (fetchTask != null)
            {
                List<BirdSighting> lstMissingImages = null;
                fetchTask.ContinueWith((result) =>
                {
                    foreach (var sgt in result.Result)
                    {
                        named_library.add_name_pair(sgt._common_name, sgt._scientific_name);
                    }

                    ListSightings = remove_excluded_birds(result.Result);
                    foreach (var sighting in ListSightings)
                    {
                        location_manager.add_mapping(sighting._location_id, sighting._location_name);
                    }

                    lstMissingImages = fetchandassignimages(result.Result);

                    if ((lstMissingImages != null) && (lstMissingImages.Count > 0))
                    {
                        System.Diagnostics.Trace.WriteLine(String.Format("Missing {0} images", lstMissingImages.Count()));

                        ImageLibrarian librarian = ImageLibrarian.getInstance();
                        foreach (BirdSighting bs in lstMissingImages)
                        {
                            var localBS = bs;
                            var img_fetch_tsk = librarian.getImageURL_async(localBS._scientific_name);
                            if (img_fetch_tsk != null)
                            {
                                img_fetch_tsk.ContinueWith((imgRes) =>
                                {
                                    if (localBS != null)
                                    {
                                        localBS.ThumbNail = imgRes.Result;
                                    }
                                });
                            }
                        }
                    }

                    fetchTask.Dispose();

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// Fetches the data and re-populates the column.
        /// </summary>
        public void refresh()
        {
            fetch_and_populate_column();
        }

        /// <summary>
        /// Fetches the data from the web.
        /// </summary>
        private Task<ObservableCollection<BirdSighting>> fetch_data_asyc() 
        {
            if (ListSightings != null)
                { ListSightings.Clear(); }

            Task<ObservableCollection<BirdSighting>> tsk = null;
            var fetcher = new eBirdDataFetcher();

            try
            {
                switch (ReportRequest.REPORT_TYPE)
                {
                    case ReportType.eSIGHTINGSNEARALOCATION:
                        { tsk = fetcher.fetch_observations_near_a_location_async(ReportRequest.LATTITUDE, ReportRequest.LONGITUDE); } break;
                    case ReportType.eNOTABLE_SIGHTINGS:
                        { tsk = fetcher.fetch_recent_nearby_notable_observations_async(ReportRequest.LATTITUDE, ReportRequest.LONGITUDE); } break;
                    case ReportType.eSPECIES_SIGHTING:
                        { tsk = fetcher.fetch_recent_nearby_observations_of_a_species_async(ReportRequest.LATTITUDE, ReportRequest.LONGITUDE, ReportRequest.SPECIES); } break;
                    case ReportType.eHOTSPOT_SIGHTING:
                        { tsk = fetcher.fetch_recent_observations_at_hotspots_async(ReportRequest.HOT_SPOTS); } break;
                    case ReportType.eSPECIES_NEAR_HOTSPOT:
                        { tsk = fetcher.fetch_recent_observations_of_a_species_at_hotspots_async(ReportRequest.HOT_SPOTS, ReportRequest.SPECIES); } break;
                    case ReportType.eSIGHTINGS_NEAR_EBIRD_LOC:
                        { tsk = fetcher.fetch_recent_observations_at_locations_async(ReportRequest.HOT_SPOTS); } break;
                }
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("Caught an argument exception: " + ae.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return (tsk);
        }

        /// <summary>
        /// Takes the results from the web service and updates the GUI.
        /// </summary>
        /// <param name="tsk">A task with the results of the fetch from the service</param>
        private void processSightingsList(Task<ObservableCollection<BirdSighting>> tsk)
        {
            if (tsk != null)
            {
                tsk.ContinueWith((result) =>
                {
                    ListSightings = result.Result;
                    System.Diagnostics.Trace.WriteLine("List of birds retrieved");
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// Takes a list of sightings and resolves the images to be assigned to each.
        /// </summary>
        /// <param name="lstSightings">A list of sightings</param>
        /// <returns></returns>
        private List<BirdSighting> fetchandassignimages(ObservableCollection<BirdSighting> lstSightings)
        {
            List<BirdSighting> lstMissingImages = new List<BirdSighting>();

            if (lstSightings != null)
            {
                System.Diagnostics.Trace.WriteLine("Fetching Images");

                ImageLibrarian librarian = ImageLibrarian.getInstance();

                foreach (var sighting in lstSightings)
                {
                    var bitmpsrc = librarian.getImageSource(sighting._scientific_name);                                        
                    if (bitmpsrc != null)
                    {
                        sighting.ThumbNail = bitmpsrc;
                    }
                    else
                    {
                        lstMissingImages.Add(sighting);
                    }
                }
                System.Diagnostics.Trace.WriteLine("Finished Fetching Images");
            }

            return (lstMissingImages);
        }
    }

    /// <summary>
    /// Parameters needed to find a given species within an area.
    /// </summary>
    public class find_species_at_lat_long_params
    {
        public find_species_at_lat_long_params()
        {
        }
       
        /// <summary>
        /// The name of the species that you are looking for.
        /// </summary>
        private string _species_name;
        public string species_name
        {
            get { return _species_name; }
            set {
                    if (String.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Species Name cannot be blank", "species_name");
                    }

                    _species_name = value;                        
                }
        }
                       
        /// <summary>
        /// The common name of the bird. 
        /// </summary>
        private string _species_common_name;
        public string species_common_name
        {
            get { return _species_common_name; }
            set {
                    if (String.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Common Name cannot be blank", "species_common_name");
                    }

                    _species_common_name = value; 
                }
        }       
    }
    
    /// <summary>
    /// Creates a find_species_at_location_params object by converting multiple values from the view.
    /// </summary>
    public class Species_Location_Converter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, 
                                            Type targetType, 
                                            object parameter, 
                                            System.Globalization.CultureInfo culture)
        {
            find_species_at_lat_long_params parms = new find_species_at_lat_long_params();
            parms.species_common_name = values[0].ToString();
            var rawName               = values[1].ToString().Trim();  //(Fulica americana)
            var cleanedName           = rawName.Substring(1, rawName.Length - 2);
            parms.species_name        = cleanedName;

            return (parms);
        }

        object[] IMultiValueConverter.ConvertBack(object value, 
                                                  Type[] targetTypes, 
                                                  object parameter, 
                                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }    
}   