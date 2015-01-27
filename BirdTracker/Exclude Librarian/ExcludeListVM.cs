/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using BirdTracker.Image_Librarian;
using BirdTracker.Interfaces;
using BirdTracker.Name_Librarian;
using BirdTracker.Support;

namespace BirdTracker.Exclude_Librarian
{
    /// <summary>
    /// ViewModel for the ExcludeList UI component
    /// </summary>
    public class ExcludeListVM : INotifyPropertyChanged
    {
        private INameLibrary named_library    = NameLibrarian.NAME_LIBRARIAN;
        private IExcudeLibary exclude_library = ExcludeLibrarian.EXCLUDE_LIBRARIAN;

        private ObservableCollection<ExcludeListItem> _exclude_list;
        public ObservableCollection<ExcludeListItem> EXCLUDE_LIST
        {
            get { return _exclude_list; }
            set { _exclude_list = value;
                  OnPropertyChanged("EXCLUDE_LIST");
                }
        }

        /// <summary>
        /// Reference to the windows manager (MainWindow.xaml).
        /// </summary>
        private IWindowManager _WindowManager;
        public IWindowManager WindowManager
        {
            get { return _WindowManager; }
            set
            {
                if (value == null)
                    { throw new ArgumentNullException("Window Manager cannot be null.", "WindowManager"); }

                _WindowManager = value;
            }
        }

        /// <summary>
        /// The view
        /// </summary>
        private ExcludeListUC _view;
        public ExcludeListUC _View
        {
            get { return _view; }
            set
            {
                if (value == null)
                    { throw new ArgumentNullException("The view cannot be null", "_View"); }

                _view = value;
            }
        }

        /// <summary>
        /// Close report CMD
        /// </summary>
        private ICommand _CloseReportCMD;
        public ICommand CLOSE_REPORT_CMD
        {
            get { return _CloseReportCMD; }
            set { _CloseReportCMD = value; }
        }

        /// <summary>
        /// Include Species CMD
        /// </summary>
        private ICommand _includeSpeciesCMD;
        public ICommand INCLUDE_SPECIES_CMD
        {
            get { return _includeSpeciesCMD; }
            set { _includeSpeciesCMD = value; }
        }
        
        /// <summary>
        /// CTOR
        /// </summary>
        public ExcludeListVM()
        {
            var lst = exclude_library.get_all_excluded_birds();
            if ((lst != null) && (lst.Count() > 0))
            {
                initialize(lst);
            }

            CLOSE_REPORT_CMD    = new RelayCommand(new Action<object>(closeReport));
            INCLUDE_SPECIES_CMD = new RelayCommand(new Action<object>(includeSpecies));
        }

        /// <summary>
        /// Refresh the excluded libraries report.
        /// </summary>
        public void refresh()
        {
            var lst = exclude_library.get_all_excluded_birds();
            if ((lst != null) && (lst.Count() > 0))
            {
                initialize(lst);
            }
        }

        /// <summary>
        /// Close the report.
        /// </summary>        
        private void closeReport(object o)
        {
            if (WindowManager != null)
                { WindowManager.closeExcludes(_View); }
        }

        /// <summary>
        /// Restore the excluded species (i.e. remove from the exclude list).
        /// </summary>
        /// <param name="o">The List Box</param>
        private void includeSpecies(object o)
        {
            var lb = o as ListBox;
            var si = lb.SelectedItems;
            if (si != null)
            {
                var item = si[0] as ExcludeListItem;
                if (WindowManager.remove_species_from_exclude_list(item._scientific_name))
                {
                    EXCLUDE_LIST.Remove(item);
                    WindowManager.refresh_columns();
                }
            }
        }

        /// <summary>
        /// Given a list of species that are on some exclusion list initialize the VM
        /// </summary>
        /// <param name="lstExclusions">A list of items to exclude.</param>
        /// <exception cref="ArgumentNullException">Thrown when the list of exclusions is null.</exception>
        private void initialize(IEnumerable<string> lstExclusions)
        {
            if (lstExclusions == null)
                { throw new ArgumentNullException("The list of exclusions cannot be null.", "initialize"); }

            ImageLibrarian librarian = ImageLibrarian.IMAGE_LIBRARIAN;
            ObservableCollection<ExcludeListItem> lstExcludeItems = new ObservableCollection<ExcludeListItem>();
            foreach (var item in lstExclusions)
            {
                ExcludeListItem eli = new ExcludeListItem();
                eli._scientific_name = item;
                eli._common_name = named_library.lookup_common_name(eli._scientific_name);
                
                var img_src = librarian.getImageSource(eli._scientific_name);
                if (img_src != null)
                {
                    eli.ThumbNail = img_src;
                }

                lstExcludeItems.Add(eli);
            }

            EXCLUDE_LIST = new ObservableCollection<ExcludeListItem>(lstExcludeItems.OrderBy(i => i._common_name));
        }

        /// <summary>
        /// 
        /// </summary>
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
