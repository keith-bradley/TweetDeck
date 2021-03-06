﻿/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BirdTracker.Name_Librarian;

namespace BirdTrackerTest
{
    [TestClass]
    public class INameLibraryTest
    {
        private INameLibrary librarian = NameLibrarian.NAME_LIBRARIAN;

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_name_pair_null_common_name()
        {
            librarian.add_name_pair(null, "viva loca");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_name_pair_blank_common_name()
        {                       
            librarian.add_name_pair("", "viva loca");    
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_name_pair_null_scientific_name()
        {
            librarian.add_name_pair("crazy bird", null);             
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_name_pair_empty_scientific_name()
        {                        
            librarian.add_name_pair("crazy bird", "");
        }

        [TestMethod]
        public void add_name_pair_good_pair()
        {
            var bAdded = librarian.add_name_pair("crazy bird", "viva loca");
            Assert.IsTrue(bAdded);
        }

        [TestMethod]
        public void add_name_pair_pair_already_exists()
        {
            librarian.add_name_pair("crazy bird 2", "viva loca 2");
            var bAdded = librarian.add_name_pair("crazy bird 2", "viva loca 2");
            Assert.IsFalse(bAdded);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void lookup_scientific_name_null_common_name()
        {
            librarian.lookup_scientific_name(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void lookup_scientific_name_blank_common_name()
        {                       
            librarian.lookup_scientific_name("");
        }

        [TestMethod]
        public void lookup_scientific_name_exists()
        {
            librarian.add_name_pair("Blue Bird", "Da Bird Blue");
            var common_name = librarian.lookup_scientific_name("Blue Bird");
            Assert.IsTrue(common_name == "Da Bird Blue");
        }

        [TestMethod]
        public void lookup_scientific_name_does_not_exist()
        {
            var common_name = librarian.lookup_common_name("black hawk down"); 
            Assert.IsTrue(common_name == "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void lookup_common_name_null_scientific_name()
        {
            librarian.lookup_common_name(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void lookup_common_name_blank_scientific_name()
        {
            librarian.lookup_common_name("");                       
        }

        [TestMethod]
        public void lookup_common_name_name_exists()
        {
            librarian.add_name_pair("black bird", "da bird black");
            var common_name = librarian.lookup_common_name("da bird black");
            Assert.IsTrue(common_name == "black bird");
        }

        [TestMethod]
        public void lookup_common_name_does_not_exist()
        {
            var common_name = librarian.lookup_common_name("footed blue bird");
            Assert.IsTrue(common_name == "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void scientific_name_exists_null_name()
        {
            librarian.scientific_name_exists(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void scientific_name_exists_blank_name()
        {
            librarian.scientific_name_exists("");    
        }

        [TestMethod]
        public void scientific_name_exists_exists()
        {
            librarian.add_name_pair("chickadee", "the chickadee");
            var exists = librarian.scientific_name_exists("the chickadee");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void scientific_name_exists_does_not_exist()
        {
            var exists = librarian.scientific_name_exists("BMW");
            Assert.IsFalse(exists);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void common_name_exists_null_name()
        {           
            librarian.common_name_exists(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void common_name_exists_blank_name()
        {           
            librarian.common_name_exists("");
        }

        [TestMethod]
        public void common_name_exists_exists()
        {
            librarian.add_name_pair("dove", "the dove");
            var exists = librarian.common_name_exists("dove");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void common_name_exists_does_not_exist()
        {
            var exists = librarian.common_name_exists("pinto");
            Assert.IsFalse(exists);
        }
    }
}
