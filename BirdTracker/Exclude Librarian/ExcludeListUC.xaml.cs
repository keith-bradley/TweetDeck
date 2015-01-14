using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BirdTracker.Interfaces;

namespace BirdTracker.Exclude_Librarian
{
    /// <summary>
    /// Interaction logic for ExcludeListUC.xaml
    /// </summary>
    public partial class ExcludeListUC : UserControl
    {
        private IWindowManager _windowsManager;

        public ExcludeListUC(IWindowManager windowsManager)
        {
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
