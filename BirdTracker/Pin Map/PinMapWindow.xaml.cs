/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using System.Linq;

namespace BirdTracker.Pin_Map
{
    /// <summary>
    /// Interaction logic for PinMapWindow.xaml
    /// </summary>
    public partial class PinMapWindow : Window
    {        
        /// <summary>
        /// CTOR
        /// </summary>
        public PinMapWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Call initialize to pass the list of pin locations to the window.
        /// </summary>
        /// <param name="collection_of_locations_to_be_pinned">The list of locations to be pinned.</param>
        /// <exception cref="ArgumentNullException">Thrown if collection_of_locations_to_be_pinned is null.</exception>
        public void initialize(IEnumerable<BirdPinData> collection_of_locations_to_be_pinned)
        {
            if (collection_of_locations_to_be_pinned == null)
                { throw new ArgumentNullException("collection of locations cannot be null.", "initialize"); }

            if (MainGrid.DataContext != null)
            {
                var vm = MainGrid.DataContext as PinMapWindowVM;
               
                List<Pushpin> colPushPins = new List<Pushpin>();
                foreach (var pin_data in collection_of_locations_to_be_pinned)
                {
                    var pin = new Pushpin();
                    pin.Location = new Location(latitude: pin_data.Latitude, longitude: pin_data.Longitude);
                    pin.ToolTip = String.Format("{0} ({1})",pin_data.TOOL_TIP_TITLE, pin_data.DATE_REPORTED);
                    colPushPins.Add(pin);                    
                }
                vm.LIST_OF_PUSHPINS = colPushPins;

                var first = collection_of_locations_to_be_pinned.First();
                vm.CENTRE_OF_MAP = new Location(latitude: first.Latitude, longitude: first.Longitude);                               
            }            
        }

        /// <summary>
        /// Event handler for when the window is loaded.
        /// Here we want to restore previously saved settings like window width & height.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width  = Properties.Settings.Default.PIN_MAP_WIDTH;
            Height = Properties.Settings.Default.PIN_MAP_HEIGHT;
            Top    = Properties.Settings.Default.PIN_MAP_TOP;
            Left   = Properties.Settings.Default.PIN_MAP_LEFT;
        }

        /// <summary>
        /// Event handler for when the window is closed.
        /// Here we want to save the properties of the window like window width & height.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.PIN_MAP_WIDTH  = Width;
            Properties.Settings.Default.PIN_MAP_HEIGHT = Height;
            Properties.Settings.Default.PIN_MAP_TOP    = Top;
            Properties.Settings.Default.PIN_MAP_LEFT   = Left;            
            Properties.Settings.Default.Save();
        }
    }
}
