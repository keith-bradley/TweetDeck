/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using BirdTracker.Exclude_Librarian;

namespace BirdTracker.Interfaces
{
    public interface IWindowManager
    {
        /// <summary>
        /// Create a report window
        /// </summary>
        /// <param name="reportInfo">The info required to create the window.</param>
        /// <exception cref="ArgumentNullException">Thrown if reportInfo is null.</exception>
        void createReportWindow(ReportRequest reportInfo);

        /// <summary>
        /// Close an existing report window.
        /// </summary>
        /// <param name="report">The report column</param>
        /// <exception cref="ArgumentNullException">Thrown if report is null.</exception>
        void closeReport(GenericBirdSightingsReport report);
        
        /// <summary>
        /// Add the provided species to the global exclude list.
        /// </summary>
        /// <param name="scientific_name">The scientific name of the species.</param>
        /// <returns>true on success, false on failure.</returns>
        /// <exception cref="ArgumentException">Thrown when the scientific_name is null or empty.</exception>
        bool add_species_to_exclude_list(String scientific_name);

        /// <summary>
        /// Restore a previously excluded bird.
        /// </summary>
        /// <param name="scientific_name">The scientific name of the bird.</param>
        /// <returns>true on success, false on failure.</returns>
        bool remove_species_from_exclude_list(String scientific_name);

        /// <summary>
        /// Call this method to have the window's manager remove the provided species from being displayed in all of the columns.
        /// </summary>        
        void refresh_excluded_species_from_all_columns();

        /// <summary>
        /// Ask all columns to re-fetch the data that they are showing.
        /// </summary>
        void refresh_columns();

        /// <summary>
        /// View the Excludes list.
        /// </summary>
        void view_excludes();

        /// <summary>
        /// Close the exclude list column.
        /// </summary>
        /// <param name="report">The exclude list column.</param>
        /// <exception cref="ArgumentNullException">Thrown when the report is null.</exception>
        void closeExcludes(ExcludeListUC report);
    }
} 
