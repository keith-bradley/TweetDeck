using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BirdTracker.Location_Manager;
using BirdTracker.Interfaces;

namespace BirdTrackerTest
{
    [TestClass]
    public class ILocationManagerTest
    {
        ILocationManager lm = LocationManager.getInstance();

        [TestMethod]
        public void add_mapping_null_ebird_location()
        {
            var bExceptionThrown = false;
            try
                { lm.add_mapping(null, "mud lake"); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);        
        }

        [TestMethod]
        public void add_mapping_null_real_world_location()
        {
            var bExceptionThrown = false;
            try 
                { lm.add_mapping("L12345", null); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void add_mapping_blank_ebird_location()
        {
            var bExceptionThrown = false;
            try
                { lm.add_mapping("", "mud lake"); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void add_mapping_blank_real_world_location()
        {
            var bExceptionThrown = false;
            try
                { lm.add_mapping("L12345", ""); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void add_mapping_good_values()
        {
            var location_added = lm.add_mapping("L123456", "Mud Lake");
            Assert.IsTrue(location_added);
        }

        [TestMethod]
        public void add_mapping_location_already_exists()
        {
            lm.add_mapping("L123456", "Mud Lake");
            var location_added = lm.add_mapping("L123456", "Mud Lake");
            Assert.IsFalse(location_added);
        }

        [TestMethod]
        public void remove_mapping_null_ebird_location()
        {
            var bExceptionThrown = false;
            try
                { lm.remove_mapping(null, "Mud Lake"); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void remove_mapping_blank_ebird_location()
        {
            var bExceptionThrown = false;
            try
                { lm.remove_mapping("", "Mud Lake"); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void remove_mapping_null_real_world_location()
        {
            var bExceptionThrown = false;
            try
                { lm.remove_mapping("L12345", null); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void remove_mapping_blank_real_world_location()
        {
            var bExceptionThrown = false;
            try
                { lm.remove_mapping("L12345", ""); }
            catch
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void remove_mapping_successful_remove()
        {
            lm.add_mapping("L98765", "Mud Lake");
            var bRemoved = lm.remove_mapping("L98765", "Mud Lake");
            Assert.IsTrue(bRemoved);
        }

        [TestMethod]
        public void remove_mapping_item_does_not_exist()
        {
            var bRemoved = lm.remove_mapping("L99999", "Pink Lake");
            Assert.IsFalse(bRemoved);
        }

        [TestMethod]
        public void get_real_world_location_null_ebird_location()
        {
            var bExceptionThrown = false;
            try
                { lm.get_real_world_location(null); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void get_real_world_location_empty_ebird_location()
        {
            var bExceptionThrown = false;
            try
                { lm.get_real_world_location(""); }
            catch (ArgumentException)
                { bExceptionThrown = true; }

            Assert.IsTrue(bExceptionThrown);
        }

        [TestMethod]
        public void get_real_world_location_exists()
        {
            lm.add_mapping("L23456", "Purple Lake");
            var strLocation = lm.get_real_world_location("L23456");
            Assert.IsTrue(strLocation == "Purple Lake");
        }

        [TestMethod]
        public void get_real_world_location_does_not_exist()
        {
            var strLocation = lm.get_real_world_location("L7896");
            Assert.IsTrue(strLocation == "");
        }

    }
}
