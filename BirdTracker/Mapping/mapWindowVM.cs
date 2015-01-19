using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BirdTracker.Interfaces;
using BirdTracker.Support;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

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
        }

        private double _lat;
        public double LATT  
        {
            get { return _lat; }
            set { _lat = value; }
        }

        private double _long;
        public double LONG
        {
            get { return _long; }
            set { _long = value; }
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
                    rep.LATTITUDE = Math.Round(LATT, 2); 
                    rep.LONGITUDE = Math.Round(LONG, 2);
                    rep.REPORT_TITLE = String.Format("All Birds near Lattitude: {0} Longitude: {1}", rep.LATTITUDE, rep.LONGITUDE);
                }
                break;
                case "Notable":
                {
                    rep = new ReportRequest(ReportType.eNOTABLE_SIGHTINGS);
                    rep.LATTITUDE = Math.Round(LATT, 2);
                    rep.LONGITUDE = Math.Round(LONG, 2);
                    rep.REPORT_TITLE = String.Format("Notable Birds near Lattitude: {0} Longitude: {1}", rep.LATTITUDE, rep.LONGITUDE);
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
