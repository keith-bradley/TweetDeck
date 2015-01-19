using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using BirdTracker.Interfaces;
using BirdTracker.Support;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace BirdTracker
{
    /// <summary>
    /// A view model for the main window.
    /// </summary>
    public class MainWindowVM : INotifyPropertyChanged
    {
        private List<String> _reportTypes;
        public List<String> REPORT_TYPES
        {
            get { return _reportTypes; }
            set { 
                    _reportTypes = value;
                    OnPropertyChanged("REPORT_TYPES");           
                }
        }

        private String _selectedReport;
        public String SELECTED_REPORT
        {
            get { return _selectedReport; }
            set { _selectedReport = value; }
        }
        
        private double _Lattitude;
        public double LATTITUDE
        {
            get { return _Lattitude; }
            set {
                    if ((value < -90) || (value > 90))
                    {
                        throw new ArgumentOutOfRangeException("Lattitude must be between -90 and 90", "Lattitude");
                    }

                    _Lattitude = value; 
                }
        }

        private double _Longitude;
        public double LONGITUDE
        {
            get { return _Longitude; }
            set {
                    if ((value < -180) || (value > 180))
                    {
                        throw new ArgumentOutOfRangeException("Longitude must be between -180 and 180", "Longitude");
                    }

                    _Longitude = value; 
                }
        }

        private ICommand _GenerateReportCMD;
        public ICommand GENERATE_REPORT_CMD
        {
            get { return _GenerateReportCMD; }
            set { _GenerateReportCMD = value; }
        }

        private ICommand _LaunchMapCMD;
        public ICommand LAUNCH_MAP_CMD
        {
            get { return _LaunchMapCMD; }
            set { _LaunchMapCMD = value; }
        }

        private ICommand _view_excludes_CMD;
        public ICommand VIEW_EXCLUDES_CMD
        {
            get { return _view_excludes_CMD; }
            set { _view_excludes_CMD = value; }
        }
        
        private IWindowManager _windowManager;
        public IWindowManager WindowManager
        {
            get { return _windowManager; }
            set {
                    if (value == null)
                    {
                        throw new ArgumentNullException("WindowManager cannot be null", "WindowManager");
                    }

                    _windowManager = value; 
                }
        }
        
        /// <summary>
        /// CTOR
        /// </summary>
        public MainWindowVM()
        {
            REPORT_TYPES = new List<String>() { "All", "Notable" };
            SELECTED_REPORT = "All";
            
            LATTITUDE = 45.42;
            LONGITUDE = -75.43;
            
            GENERATE_REPORT_CMD = new RelayCommand(new Action<object>(generateReport));
            LAUNCH_MAP_CMD      = new RelayCommand(new Action<object>(LaunchMap));
            VIEW_EXCLUDES_CMD = new RelayCommand(new Action<object>(ViewExcludes));
        }

        private void ViewExcludes(object o)
        {
            WindowManager.view_excludes();
        }



        /// <summary>
        /// Launch a map window
        /// </summary>        
        private void LaunchMap(object o)
        {
            var map = new Mapping.mapWindow(WindowManager);            
            map.Show();
        }

        /// <summary>
        /// Generate a report based on the values set in the toolbar.
        /// </summary>
        /// <param name="o"></param>
        private void generateReport(object o)
        {
            // Note: Because this is the viewmodel, it knows nothing about creating a gui component
            //  Zo we pass the info to the Service yet to be created
            // http://stackoverflow.com/questions/10094265/wpf-mvvm-opening-one-view-from-another
            // Do data validation and build a report object
            // Then you call the window manager (to be created) 

            if (!String.IsNullOrEmpty(SELECTED_REPORT) && (WindowManager != null))
            {
                ReportRequest rep = null;

                switch (SELECTED_REPORT)
                {
                    case "All":
                    {
                        rep = new ReportRequest(ReportType.eSIGHTINGSNEARALOCATION);
                        rep.LATTITUDE = LATTITUDE;
                        rep.LONGITUDE = LONGITUDE;
                        rep.REPORT_TITLE = String.Format("All Birds near Lattitude: {0}  Longitude: {1}", rep.LATTITUDE, rep.LONGITUDE);
                    }
                    break;
                    case "Notable":
                    {
                        rep = new ReportRequest(ReportType.eNOTABLE_SIGHTINGS);
                        rep.LATTITUDE = LATTITUDE;
                        rep.LONGITUDE = LONGITUDE;
                        rep.REPORT_TITLE = String.Format("Notable Birds near Lattitude: {0}  Longitude: {1}", rep.LATTITUDE, rep.LONGITUDE);
                    }
                    break;
                };
                                             
                WindowManager.createReportWindow(rep);                      
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
    }
}
