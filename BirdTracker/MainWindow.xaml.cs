/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using BirdTracker.Exclude_Librarian;
using BirdTracker.Interfaces;
using BirdTracker.Name_Librarian;
using BirdTracker.Support;
using BirdTracker.Pin_Map;

// Possible features/to do for this application
// - Schedule the refresh and do a popup if an interesting bird pops up.
// - Sorting of the results.
// - info on the status bar.
// - Allow the user to select the location.
// - Create columns to watch:
//      - specific species of bird (or birds)      
//      - specific location
//      - defined hotspots
// Bing maps key: AlgGlvdk8YG15kLf44UFdyq18MtBB890r0NnItZb3187ryBDswn9xuq3h58Oo5vX

namespace BirdTracker 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowManager
    {
        private IExcudeLibary exclude_library = ExcludeLibrarian.get_instance();
        private INameLibrary  name_library    = NameLibrarian.get_instance();  
     
        /// <summary>
        /// CTOR
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();                      
        }
    
        /// <summary>
        /// Create a report.
        /// </summary>
        /// <param name="reportInfo">Container for the information needed to create the report.</param>
        /// <exception cref="ArgumentNullException">Thrown in reportInfo is null.</exception>
        public void createReportWindow(ReportRequest reportInfo)
        {
            if (reportInfo == null)
                { throw new ArgumentNullException("Report request cannot be null", "createReportWindow"); }
                                       
            var report = new GenericBirdSightingsReport(reportInfo, this);
            DockingPanel.Children.Add(report);
            save_existing_reports();                       
        }

        /// <summary>
        /// Save the state of the main window (a series of reports really) so that we can load them again.
        /// </summary>
        public void save_existing_reports()
        {
            StringBuilder sbXML = new StringBuilder("<main_window_state>");
            if ((DockingPanel.Children != null) && 
                (DockingPanel.Children.Count > 0))
            {
                sbXML.Append("<columns>");

                int column_position = 0;
                foreach (var col in DockingPanel.Children)
                {
                    if (col.GetType() == typeof(GenericBirdSightingsReport))
                    {
                        var bgsr = col as GenericBirdSightingsReport;                        
                        sbXML.Append("<column>");
                        sbXML.AppendFormat("<column_position>{0}</column_position>", column_position++ );
                        var report_request = bgsr.REPORT_REQUEST;
                        if (report_request != null)
                        {
                            sbXML.Append(report_request.to_xml());
                        }
                        sbXML.Append("</column>");
                    }
                }                
            }

            sbXML.Append("</columns>");
            sbXML.Append("</main_window_state>");

            Properties.Settings.Default.MAIN_WINDOW_STATE = sbXML.ToString();
            Properties.Settings.Default.Save();                       
        }

        /// <summary>
        /// Load the existing reports.
        ///     <main_window_state>
        ///         <columns>
        ///               <column>
        ///                 <column_position>0</column_position>
        ///                     <Report_Request>
        ///                         <report_title>All Birds near Lattitude: 45.42  Longitude: -75.43</report_title>
        ///                         <report_type>eSIGHTINGSNEARALOCATION</report_type>
        ///                         <lattitude>45.42</lattitude>
        ///                         <longitude>-75.43</longitude>
        ///                         <species></species>
        ///                    </Report_Request>
        ///                </column>
        ///         </columns>
        ///      </main_window_state>                      
        /// </summary>
        public void load_existing_reports()
        {
            String saved_state = Properties.Settings.Default.MAIN_WINDOW_STATE;
            if (!String.IsNullOrEmpty(saved_state))
            {
                try
                {
                    var xdoc = Utilities.load_xml_from_string(saved_state);
                    if (xdoc != null)
                    {
                        List<column_report_requests> lstColumns = (from _rep in xdoc.Element("main_window_state").Elements("columns").Elements("column")
                                                                   select new column_report_requests
                                                                   {
                                                                       _column_position = (string)_rep.Element("column_position"),
                                                                       _report_request = (from _rep_req in _rep.Elements("Report_Request")
                                                                                          select new ReportRequest
                                                                                          {
                                                                                              REPORT_TITLE  = (string)_rep_req.Element("report_title"),
                                                                                              REPORT_TYPE   = (ReportType)Enum.Parse(typeof(ReportType), ((string)_rep_req.Element("report_type"))),
                                                                                              LAT_LONG_PAIR  = new LatLongPair
                                                                                              {
                                                                                                Latitude    = Double.Parse((string)_rep_req.Element("lattitude")),
                                                                                                Longitude     = Double.Parse((string)_rep_req.Element("longitude")),
                                                                                              },
                                                                                              HOT_SPOTS     = _rep_req.Elements("hot_spots").Elements("spot").Select(xe => xe.Value).ToList(),
                                                                                              SPECIES       = (string) _rep_req.Element("species")
                                                                                          }).SingleOrDefault()
                                                                   }).ToList();


                        foreach (var col_rep in lstColumns)
                        {
                            var report = new GenericBirdSightingsReport(col_rep._report_request, this);
                            DockingPanel.Children.Add(report);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }         
        }

        private class column_report_requests
        {
            public column_report_requests()
            {
            }

            public string _column_position       { get; set; }
            public ReportRequest _report_request { get; set; }
        }

        /// <summary>
        /// Close a report.
        /// </summary>
        /// <param name="report">Container for the information needed to create the report.</param>
        /// <exception cref="ArgumentNullException">Thrown if report is null.</exception>
        public void closeReport(GenericBirdSightingsReport report)
        {
            if (report == null)
                { throw new ArgumentNullException("Report cannot be null.", "closeReport"); }

            DockingPanel.Children.Remove(report);
            save_existing_reports();
        }

        /// <summary>
        /// Close the Excludes report.
        /// </summary>
        /// <param name="report">The exclude list column.</param>
        /// <exception cref="ArgumentNullException">Thrown when the report is null.</exception>
        public void closeExcludes(ExcludeListUC report)
        {
            if (report == null)
                { throw new ArgumentNullException("Report control cannot be null.", "closeExcludes"); }

            DockingPanel.Children.Remove(report);
        }

        /// <summary>
        /// Show the list of excluded birds.
        /// </summary>
        public void view_excludes()
        {            
            var report = new ExcludeListUC(this);            
            DockingPanel.Children.Add(report);
        }

        /// <summary>
        /// Event handler for when the window is first loaded.
        /// </summary>
        private void main_Loaded(object sender, 
                                 RoutedEventArgs e)
        {
            // Set the windows location to where is was last. 
            Top  = Properties.Settings.Default.MAIN_WINDOW_TOP;
            Left = Properties.Settings.Default.MAIN_WINDOW_LEFT;

            // Set the windows Height/Width based on last time it was displayed
            this.Height = Properties.Settings.Default.MAIN_WINDOW_HEIGHT;
            this.Width  = Properties.Settings.Default.MAIN_WINDOW_WIDTH;

            MainWindowVM vm = MainGrid.DataContext as MainWindowVM;
            if (vm != null)
            {
                vm.WindowManager = this;
            }

            exclude_library.load_library();
            name_library.load_library();
            load_existing_reports();

            // Launch the map dialog
            var map = new Mapping.mapWindow(vm.WindowManager);
            map.Show();
        }

        /// <summary>
        /// Event handler for when the main window is closing.
        /// Save window location, size etc.
        /// </summary>
        private void main_Closing(object sender, 
                                  System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.MAIN_WINDOW_TOP    = Top;
            Properties.Settings.Default.MAIN_WINDOW_LEFT   = Left;
            Properties.Settings.Default.MAIN_WINDOW_HEIGHT = Height;
            Properties.Settings.Default.MAIN_WINDOW_WIDTH  = Width;            
            Properties.Settings.Default.Save();

            exclude_library.save_library();
            name_library.save_library();
        }

        /// <summary>
        /// Add the provided species to the global exclude list.
        /// </summary>
        /// <param name="scientific_name">The scientific name of the species.</param>
        /// <returns>true on success, false on failure.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific_name is null or empty.</exception>
        public bool add_species_to_exclude_list(String scientific_name)
        {
            if (String.IsNullOrEmpty(scientific_name))
                { throw new ArgumentException("The scientific name cannot be blank.", "add_species_to_exclude_list"); }

            bool added = exclude_library.add_item_to_library(scientific_name);
            return (added);
        }

        /// <summary>
        /// Restore a previously excluded bird.
        /// </summary>
        /// <param name="scientific_name">The scientific name of the bird.</param>
        /// <returns>true on success, false on failure.</returns>
        /// <exception cref="ArgumentException">Thrown when scientific name is null or empty.</exception>
        public bool remove_species_from_exclude_list(String scientific_name)
        {
            if (String.IsNullOrEmpty(scientific_name))
                { throw new ArgumentException("The scientific name cannot be blank.", "remove_species_from_exclude_list"); }

            bool removed = exclude_library.remove_item_from_library(scientific_name);
            return (removed);
        }

        /// <summary>
        /// Call this method to have the window's manager remove the provided species from being displayed in all of the columns.
        /// </summary>        
        public void refresh_excluded_species_from_all_columns()
        {
            if (DockingPanel.Children.Count > 0)
            {
                foreach (var column in DockingPanel.Children)
                {
                    if (column.GetType() == typeof(GenericBirdSightingsReport))
                    {
                        var report = column as GenericBirdSightingsReport;                                       
                        report.refresh_due_to_new_exclude();
                    }
                    else if (column.GetType() == typeof(ExcludeListUC))
                    {
                        var exclude_column = column as ExcludeListUC;
                        exclude_column.refresh_display();
                    }
                }
            }
        }

        public void refresh_columns()
        {
            if (DockingPanel.Children.Count > 0)
            {
                foreach (var column in DockingPanel.Children)
                {
                    var report = column as GenericBirdSightingsReport;
                    if (report != null)
                    {
                        report.refresh_column();
                    }
                }
            }
        }
    }
}
