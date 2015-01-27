/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using BirdTracker.Interfaces;
using BirdTracker.Support;

namespace BirdTracker
{
    /// <summary>
    /// A view model for the main window.
    /// </summary>
    public class MainWindowVM : INotifyPropertyChanged
    {      
        /// <summary>
        /// Launch the report generation map command.
        /// </summary>
        private ICommand _LaunchMapCMD;
        public ICommand LAUNCH_MAP_CMD
        {
            get { return _LaunchMapCMD; }
            set { _LaunchMapCMD = value; }
        }

        /// <summary>
        /// Add the exclude list column to the main window command.
        /// </summary>
        private ICommand _view_excludes_CMD;
        public ICommand VIEW_EXCLUDES_CMD
        {
            get { return _view_excludes_CMD; }
            set { _view_excludes_CMD = value; }
        }
        
        /// <summary>
        /// Window Manager Interface.
        /// </summary>
        private IWindowManager _windowManager;
        public IWindowManager WindowManager
        {
            get { return _windowManager; }
            set {
                    if (value == null)
                        { throw new ArgumentNullException("WindowManager cannot be null.", "WindowManager"); }

                    _windowManager = value; 
                }
        }
        
        /// <summary>
        /// CTOR
        /// </summary>
        public MainWindowVM()
        {                                                                                 
            LAUNCH_MAP_CMD    = new RelayCommand(new Action<object>(LaunchMap));
            VIEW_EXCLUDES_CMD = new RelayCommand(new Action<object>(ViewExcludes));
        }

        /// <summary>
        /// View the list of birds on the exclude list.
        /// </summary>        
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
