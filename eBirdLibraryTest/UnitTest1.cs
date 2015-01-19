using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using eBirdLibrary;
using System.Collections.ObjectModel;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace eBirdLibraryTest
{
    /// <summary>
    /// Unit tests for the eBirdLibary. Because the library makes calls to the web.
    /// We only write unit tests to validate the parameter's being passed into the library.
    /// </summary>
    [TestClass]
    public class eBirdLibrary_Unit_Tests
    {
        [TestMethod]
        public void fetch_observations_near_a_location_async_Invalid_Negative_Lattitude()
        {
            var fetcher = new eBirdDataFetcher();

            try
            {
                Task<ObservableCollection<BirdSighting>> tsk = fetcher.fetch_observations_near_a_location_async(-91, 25);          //This should throw!
                Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);    // We expect to end up here as the smallest lattitude is -90
            }           
        }

        [TestMethod]
        public void fetch_observations_near_a_location_async_Invalid_Positive_Lattitude()
        {
            var fetcher = new eBirdDataFetcher();

            try
            {
                Task<ObservableCollection<BirdSighting>> tsk = fetcher.fetch_observations_near_a_location_async(91, 25);          //This should throw!
                Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);    // We expect to end up here as the largest lattitude is 90 
            }
        }

        [TestMethod]
        public void fetch_observations_near_a_location_async_Invalid_Negative_longitude()
        {
            var fetcher = new eBirdDataFetcher();
            try
            {
                Task<ObservableCollection<BirdSighting>> tsk = fetcher.fetch_observations_near_a_location_async(45, -181);          //This should throw!
                Assert.IsTrue(false);

            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);    // We expect to end up here as the smallest longitude is -180
            }
        }

        [TestMethod]
        public void fetch_observations_near_a_location_async_Invalid_Positive_longitude()
        {
            var fetcher = new eBirdDataFetcher();
            try
            {
                Task<ObservableCollection<BirdSighting>> tsk = fetcher.fetch_observations_near_a_location_async(45, 181);          //This should throw!
                Assert.IsTrue(false);

            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);    // We expect to end up here as the biggest longitude is 180
            }           
        }










    }
}
