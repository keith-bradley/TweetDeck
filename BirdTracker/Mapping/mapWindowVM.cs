/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BirdTracker.Interfaces;
using BirdTracker.Support;
using BirdTracker.Pin_Map;

namespace BirdTracker.Mapping
{
    /// <summary>
    /// A view model for the map window class.
    /// </summary>
    public class mapWindowVM : INotifyPropertyChanged
    {
        public IWindowManager WindowManager { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public mapWindowVM()
        {
            initReportStrings();

            // Hook up the commands
            GENERATE_REPORT_CMD = new RelayCommand(new Action<object>(generateReport));
            _lat_long_pair = new LatLongPair();
        }

        private LatLongPair _lat_long_pair;
        public LatLongPair LAT_LONG_PAIR
        {
            get { return _lat_long_pair; }
            set { _lat_long_pair = value; }
        }
                          
        /// <summary>
        /// Generate a report CMD
        /// </summary>
        private ICommand _GenerateReportCMD;
        public ICommand GENERATE_REPORT_CMD
        {
            get { return _GenerateReportCMD; }
            set { _GenerateReportCMD = value; }
        }

        private ObservableCollection<string> _ReportStrings;            
        public ObservableCollection<string> REPORT_STRINGS
        {
            get { return _ReportStrings; }
            set { _ReportStrings = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private void initReportStrings()
        {
            REPORT_STRINGS = new ObservableCollection<string>();
            REPORT_STRINGS.Add("All");
            REPORT_STRINGS.Add("Notable");
        }

        /// <summary>
        /// Generate a bird report
        /// </summary>        
        private void generateReport(object o)
        {
            var reportType = o as string;

            ReportRequest rep = null;
            switch (reportType)
            {
                case "All":
                {
                    rep = new ReportRequest(ReportType.eSIGHTINGSNEARALOCATION);
                    rep.LAT_LONG_PAIR.Latitude = Math.Round(LAT_LONG_PAIR.Latitude, 2);
                    rep.LAT_LONG_PAIR.Longitude = Math.Round(LAT_LONG_PAIR.Longitude, 2);
                    rep.REPORT_TITLE = String.Format("All Birds near Lattitude: {0} Longitude: {1}", 
                                                     rep.LAT_LONG_PAIR.Latitude, rep.LAT_LONG_PAIR.Longitude);
                }
                break;
                case "Notable":
                {
                    rep = new ReportRequest(ReportType.eNOTABLE_SIGHTINGS);
                    rep.LAT_LONG_PAIR.Latitude = Math.Round(LAT_LONG_PAIR.Latitude, 2);
                    rep.LAT_LONG_PAIR.Longitude = Math.Round(LAT_LONG_PAIR.Longitude, 2);
                    rep.REPORT_TITLE = String.Format("Notable Birds near Lattitude: {0} Longitude: {1}", 
                                                     rep.LAT_LONG_PAIR.Latitude, rep.LAT_LONG_PAIR.Longitude);
                }
                break;
            };

            WindowManager.createReportWindow(rep); 
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
    }
}
