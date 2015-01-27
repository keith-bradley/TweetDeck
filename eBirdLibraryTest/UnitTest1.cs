/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using eBirdLibrary;
using System.Collections.ObjectModel;

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
        [ExpectedException(typeof(ArgumentException))]
        public void fetch_observations_near_a_location_async_Invalid_Negative_Lattitude()
        {
            var fetcher = new eBirdDataFetcher();                       
            Task<ObservableCollection<BirdSighting>> tsk = fetcher.fetch_observations_near_a_location_async(-91, 25);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void fetch_observations_near_a_location_async_Invalid_Positive_Lattitude()
        {
            var fetcher = new eBirdDataFetcher();
            Task<ObservableCollection<BirdSighting>> tsk = fetcher.fetch_observations_near_a_location_async(91, 25);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void fetch_observations_near_a_location_async_Invalid_Negative_longitude()
        {
            var fetcher = new eBirdDataFetcher();
            Task<ObservableCollection<BirdSighting>> tsk = fetcher.fetch_observations_near_a_location_async(45, -181);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void fetch_observations_near_a_location_async_Invalid_Positive_longitude()
        {
            var fetcher = new eBirdDataFetcher();                        
            Task<ObservableCollection<BirdSighting>> tsk = fetcher.fetch_observations_near_a_location_async(45, 181);
        }
    }
}
