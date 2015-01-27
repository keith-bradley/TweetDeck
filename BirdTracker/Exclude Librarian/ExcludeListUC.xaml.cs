/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System.Windows;
using System.Windows.Controls;
using BirdTracker.Interfaces;
using System;

namespace BirdTracker.Exclude_Librarian
{
    /// <summary>
    /// Interaction logic for ExcludeListUC.xaml
    /// </summary>
    public partial class ExcludeListUC : UserControl
    {
        private IWindowManager _windowsManager;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="windowsManager">Window's Manager</param>
        public ExcludeListUC(IWindowManager windowsManager)
        {
            if (windowsManager == null)
                { throw new ArgumentNullException("windowsManger cannot be null.", "windowsManager"); }

            InitializeComponent();
            _windowsManager = windowsManager;
        }

        /// <summary>
        /// Refresh the contents of the column.
        /// </summary>
        public void refresh_display()
        {
            if (MainGrid.DataContext != null)
            {
                ExcludeListVM vm = MainGrid.DataContext as ExcludeListVM;
                vm.refresh();
            }
        }

        /// <summary>
        /// Event handler when the report is first loaded.
        /// </summary>
        private void ReportCtrl_Loaded(object sender,
                                       RoutedEventArgs e)
        {
            if (MainGrid.DataContext != null)
            {
                ExcludeListVM vm = MainGrid.DataContext as ExcludeListVM;
                vm.WindowManager = _windowsManager;
                vm._View = this;                
            }
        }      
    }
}
