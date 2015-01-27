/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

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
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using BirdTracker.Interfaces;

namespace BirdTracker.Mapping
{
    /// <summary>
    /// Interaction logic for mapWindow.xaml
    /// </summary>
    public partial class mapWindow : Window
    {
        public IWindowManager WindowManager { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public mapWindow(IWindowManager wm)
        {
            InitializeComponent();
            theMap.ViewChangeOnFrame += new EventHandler<Microsoft.Maps.MapControl.WPF.MapEventArgs>(theMap_ViewChangeOnFrame);
            theMap.MouseDoubleClick  += new MouseButtonEventHandler(theMap_MouseDoubleClick);
            theMap.ScaleVisibility   = System.Windows.Visibility.Visible;
            WindowManager            = wm;
        }

        void theMap_MouseDoubleClick(object sender, 
                                     MouseButtonEventArgs e)
        {
            Map map = (Map)sender;
            LocationRect bounds = map.BoundingRectangle;           
        }

        void theMap_ViewChangeOnFrame(object sender, 
                                      Microsoft.Maps.MapControl.WPF.MapEventArgs e)
        {
              //Gets the map that raised this event
               Map map = (Map) sender;
    
               //Gets the bounded rectangle for the current frame
               LocationRect bounds = map.BoundingRectangle;
    
               //Update the current latitude and longitude
               CurrentPosition.Text = String.Format("Northwest: {0:F5}, Southeast: {1:F5} (Current)",
               bounds.Northwest, bounds.Southeast);

               mapWindowVM vm = MainGrid.DataContext as mapWindowVM;
               if (vm != null)
               {
                   vm.LATT = bounds.Northwest.Latitude;
                   vm.LONG = bounds.Southeast.Longitude;
               }
        }

        /// <summary>
        /// Event handler for when the map window has been loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mapWindowVM vm = MainGrid.DataContext as mapWindowVM;
            if (vm != null)
            {
                vm.WindowManager = WindowManager;
            }

            Top    = Properties.Settings.Default.MAP_WINDOW_TOP;
            Left   = Properties.Settings.Default.MAP_WINDOW_LEFT;
            Height = Properties.Settings.Default.MAP_WINDOW_HEIGHT;
            Width  = Properties.Settings.Default.MAP_WINDOW_WIDTH;
        }

        /// <summary>
        /// Event handler for when the map window has been closed.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.MAP_WINDOW_TOP    = Top;
            Properties.Settings.Default.MAP_WINDOW_LEFT   = Left;
            Properties.Settings.Default.MAP_WINDOW_HEIGHT = Height;
            Properties.Settings.Default.MAP_WINDOW_WIDTH  = Width;
            Properties.Settings.Default.Save();
        }
    }
}
