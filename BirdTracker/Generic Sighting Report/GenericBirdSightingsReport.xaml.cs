/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Windows;
using System.Windows.Controls;
using BirdTracker.Interfaces;

namespace BirdTracker
{
    /// <summary>
    /// Interaction logic for NotableBirdSightingsReport.xaml
    /// </summary>
    public partial class GenericBirdSightingsReport : UserControl
    {       
        private ReportRequest _report_request;
        public ReportRequest REPORT_REQUEST
        {
            get { return _report_request; }
            set {
                    if (value == null)
                    {
                        throw new ArgumentNullException("Report_Request cannot be set to null", "REPORT_REQUEST");
                    }

                    _report_request = value; 
                }
        }
                       
        private IWindowManager  _windowsManager;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="reportInfo">The information required to generate the report.</param>
        /// <param name="windowsManager">A class that immplements the IWindowManager interface.</param>
        public GenericBirdSightingsReport(ReportRequest reportInfo, 
                                          IWindowManager windowsManager)
        {
            InitializeComponent();
            REPORT_REQUEST = reportInfo;
            _windowsManager = windowsManager;
        }
       
        /// <summary>
        /// Event handler for when the report is first loaded.
        /// We pass the REPORT_REQUEST to the VM so that it can fetch the data.
        /// </summary>   
        private void ReportCtrl_Loaded(object sender, 
                                       RoutedEventArgs e)
        {
            if (MainGrid.DataContext != null)
            {
                GenericSightingReportVM vm = MainGrid.DataContext as GenericSightingReportVM;
                vm.WindowManager = _windowsManager;
                vm._View = this;
                vm.setReportRequestAndFetchData(REPORT_REQUEST);
            }
        }

        /// <summary>
        /// Processes the existing columns data, removing those species that are on the exclude list.
        /// </summary>
        public void refresh_due_to_new_exclude()
        {
            GenericSightingReportVM vm = MainGrid.DataContext as GenericSightingReportVM;
            vm.stop_showing_excluced_species();
        }

        /// <summary>
        /// Refresh the data in the column.
        /// </summary>
        public void refresh_column()
        {
            GenericSightingReportVM vm = MainGrid.DataContext as GenericSightingReportVM;
            vm.refresh();            
        }
    }
}
