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
        /// 
        /// </summary>
        /// <param name="collection_of_locations_to_be_pinned"></param>
        public void initialize(IEnumerable<LatLongPair> collection_of_locations_to_be_pinned)
        {
            if (collection_of_locations_to_be_pinned == null)
            {
                throw new ArgumentNullException("collection of locations cannot be null.", "initialize");
            }


            if (MainGrid.DataContext != null)
            {
                foreach (var pin_location in collection_of_locations_to_be_pinned)
                {
                    var pin = new Pushpin();
                    pin.Location = new Location(latitude: pin_location.Latitude, longitude: pin_location.Longitude);
                    pin.ToolTip = "Hello world";
                    pin.ToolTipOpening += new ToolTipEventHandler(pin_ToolTipOpening);
                    pin.ToolTipClosing += new ToolTipEventHandler(pin_ToolTipClosing);
                    theMap.Children.Add(pin);
                }
            }            
        }

        void pin_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            int i = 13;
        }

        void pin_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            int i = 12;
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
