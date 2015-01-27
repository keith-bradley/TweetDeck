/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BirdTracker.Pin_Map;

namespace BirdTrackerTest
{
    [TestClass]
    public class IPinMapTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void set_map_location_invalid_lat()
        {
            var map = new PinMap();
            map.set_map_location(-360, 20);        
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void set_map_location_invalid_long()
        {
            var map = new PinMap();
            map.set_map_location(20, -360);
        }

        [TestMethod]
        public void set_map_location_valid_cordinates()
        {
            var map = new PinMap();
            map.set_map_location(-90, 90);
            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void set_map_location_null_pair()
        {
            var map = new PinMap();
            map.set_map_location(null);
        }

        [TestMethod]
        public void set_map_location_valid_pair()
        {
            var map = new PinMap();
            map.set_map_location(new LatLongPair(latitude: 90, longitude: 90));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void add_pins_to_map_null_set()
        {
            var map = new PinMap();
            map.add_pins_to_map(null);
        }

        [TestMethod]
        public void add_pins_to_map_empty_set()
        {
            var map = new PinMap();
            map.add_pins_to_map(new List<LatLongPair>());
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void add_pins_to_map_valid_set()
        {
            var map = new PinMap();
            var list = new List<LatLongPair>(1);
            list.Add(new LatLongPair(latitude: 30, longitude: 50));

            bool bSuccess = map.add_pins_to_map(list);
            Assert.IsTrue(bSuccess);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void remove_pin_from_map_invalid_lat()
        {
            var map = new PinMap();
            map.remove_pin_from_map(-900, 30);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void remove_pin_from_map_invalid_long()
        {
            var map = new PinMap();
            map.remove_pin_from_map(90, -600);
        }

        [TestMethod]
        public void remove_pin_from_map_valid_coordinates()
        {
            var map = new PinMap();
            map.remove_pin_from_map(90, 90);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void remove_pin_from_map_null_pair()
        {
            var map = new PinMap();
            map.remove_pin_from_map(null);
        }

        [TestMethod]
        public void remove_pin_from_map_valid_pair()
        {
            var map = new PinMap();
            var success = map.remove_pin_from_map(new LatLongPair(latitude: 45, longitude: 30));
            Assert.IsTrue(success);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void remove_pins_from_map_null_collection()
        {
            var map = new PinMap();
            var success = map.remove_pins_from_map(null);           
        }

        [TestMethod]
        public void remove_pins_from_map_empty_collection()
        {
            var map = new PinMap();           
            var success = map.remove_pins_from_map(new List<LatLongPair>());
            Assert.IsTrue(success);
        }
    }
}
