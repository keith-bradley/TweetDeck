/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BirdTracker.Exclude_Librarian;

namespace BirdTrackerTest
{
    [TestClass]
    public class IExcludeLibraryTests
    {
        IExcudeLibary librarian = ExcludeLibrarian.get_instance();

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_item_to_library_null_item()
        {                  
            var bItemAdded = librarian.add_item_to_library(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_item_to_library_blank_item()
        {           
            var bItemAdded = librarian.add_item_to_library("");            
        }

        [TestMethod]
        public void add_item_to_library_item_added_successfully()
        {
            var bItemAdded = librarian.add_item_to_library("bulbos vixa");
            Assert.IsTrue(bItemAdded);
            ExcludeLibrarian.get_instance().empty_library();
        }

        [TestMethod]
        public void add_item_to_library_item_already_in_library()
        {
            librarian.add_item_to_library("bulbos vixa");
            var bItemAdded = librarian.add_item_to_library("bulbos vixa");
            Assert.IsTrue(bItemAdded);
            librarian.empty_library();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void remove_item_from_library_null_item()
        {                                  
            var bItemRemoved = librarian.remove_item_from_library(null);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void remove_item_from_library_blank_item()
        {
            var bItemRemoved = librarian.remove_item_from_library("");            
        }

        [TestMethod]
        public void remove_item_from_library_successfully()
        {
            librarian.add_item_to_library("bulbos vixa");
            var bItemRemoved = librarian.remove_item_from_library("bulbos vixa");
            Assert.IsTrue(bItemRemoved);
        }

        [TestMethod]
        public void remove_item_from_library_item_not_in_library()
        {
            var bItemRemoved = librarian.remove_item_from_library("bulbos vixa");
            Assert.IsFalse(bItemRemoved);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void item_is_in_library_null_item()
        {                                  
            var bItemInLibrary = librarian.item_is_in_library(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void item_is_in_library_blank_item()
        {
            var bItemInLibrary = librarian.item_is_in_library("");            
        }

        [TestMethod]
        public void item_is_in_library_item_not_in_library()
        {
            var bItemInLibrary = librarian.item_is_in_library("bulbos vixa");
            Assert.IsFalse(bItemInLibrary);
        }

        [TestMethod]
        public void item_is_in_library_item_is_in_library()
        {
            librarian.add_item_to_library("bulbos vixa");
            var bItemInLibrary = librarian.item_is_in_library("bulbos vixa");
            Assert.IsTrue(bItemInLibrary);
            librarian.empty_library();
        }
    }
}
