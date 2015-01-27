/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BirdTracker.Image_Librarian
{
    // To do: The images are stored as thumbnails of their scientific name, however this leads to a problem when
    //        the scientific name has invalid file characters in it. Code currently try/catches this problem but its
    //        a ugly solution which leads to no images being displayed in this scenario. Holding my nose for now.

    /// <summary>
    /// The image librarian (Singleton) is used to make the fetching and displaying of thumbnail images more
    /// efficient. It keeps a cache of images on the local machine hd and can be used to fetch a thumbnail from the web
    /// which is then stored in the local cache.
    /// </summary>
    public sealed class ImageLibrarian
    {
        private static ImageLibrarian _instance = new ImageLibrarian();

        /// <summary>
        /// A dictionary of saved thumbnails of birds so we don't keep fetching the same images over and over again.
        /// </summary>        
        private Dictionary<string, string>       _storedThumbNails                 = new Dictionary<string, string>();
        private Dictionary<string, BitmapSource> _LoadedThumbNails                 = new Dictionary<string, BitmapSource>();
        private Dictionary<string, object>       _images_currently_being_retrieved = new Dictionary<string, object>();

        /// <summary>
        /// CTOR - will load images from local cache when first created. Private call getInstance() to get reference to librarian.
        /// </summary>
        private ImageLibrarian()
        {
            loadThumbNailDictionary();        
        }

        /// <summary>
        /// Returns a reference to the librarian.
        /// </summary>        
        public static ImageLibrarian getInstance()
        {           
            return (_instance);
        }

        /// <summary>
        /// Load the existing saved thumbnails.
        /// </summary>
        private void loadThumbNailDictionary()
        {
            String strThumbNailDirectory = Directory.GetCurrentDirectory() + "\\ThumbNails";
            if (Directory.Exists(strThumbNailDirectory))
            {
                var listThumbnails = Directory.GetFiles(strThumbNailDirectory);
                if (listThumbnails != null) 
                {
                    foreach (var strFilePath in listThumbnails)
                    {
                        var FileName = Path.GetFileNameWithoutExtension(strFilePath);       // The scientific name
                        if (!_storedThumbNails.ContainsKey(FileName))
                        {
                            _storedThumbNails.Add(FileName, strFilePath);                    // Scientific Name / Local file.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the url for a stored image.
        /// </summary>
        /// <param name="strScientificName">The scientific name of the bird.</param>
        /// <returns>URL or an empty string.</returns>
        public string getImageURL(String strScientificName)
        {
            Trace.Assert(!String.IsNullOrEmpty(strScientificName));

            String strURL = "";
            if (!String.IsNullOrEmpty(strScientificName))
            {
                if (_storedThumbNails.ContainsKey(strScientificName))
                {
                    strURL = _storedThumbNails[strScientificName];
                }
            }

            return (strURL);
        }

        /// <summary>
        /// Get a bitmap of the image, given the scientific name of the species.
        /// Thread safe.
        /// </summary>
        private object lck = new object();
        public BitmapSource getImageSource(String strScientificName)
        {
            Trace.Assert(!String.IsNullOrEmpty(strScientificName));

            BitmapSource src = null;

            if (!String.IsNullOrEmpty(strScientificName))
            {
                lock (lck)
                {
                    if (_LoadedThumbNails.ContainsKey(strScientificName))
                    {
                        src = _LoadedThumbNails[strScientificName];
                    }
                    else
                    {
                        var strPath = getImageURL(strScientificName);
                        if (!String.IsNullOrEmpty(strPath))
                        {
                            // Load it into memory and save it so we can reuse it.
                            try
                            {
                                BitmapImage bi = new BitmapImage();

                                bi.BeginInit();
                                bi.UriSource = new Uri(strPath, UriKind.RelativeOrAbsolute);
                                bi.EndInit();

                                src = bi;
                                _LoadedThumbNails.Add(strScientificName, src);
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                        }
                    }
                }
             }
            
            return (src);
        }

        /// <summary>
        /// Asynchronously fetch the url to a thumbnail. 
        /// </summary>
        /// <param name="strScientificName">Scientific name of the bird</param>
        /// <returns></returns>
        public Task<BitmapSource> getImageURL_async(String strScientificName)
        {
            Trace.Assert(!String.IsNullOrEmpty(strScientificName));

            Task<BitmapSource> tsk = null;
            if (!String.IsNullOrEmpty(strScientificName) &&
                (!_images_currently_being_retrieved.ContainsKey(strScientificName)) &&
                (!FilePathHasInvalidChars(strScientificName)) &&
                (!fileNameHasInvalidChars(strScientificName))
            )
            {
                BitmapImage bi = new BitmapImage();

                _images_currently_being_retrieved.Add(strScientificName, null);

                tsk = new Task<BitmapSource>(() =>
                {
                    String filenamePath = "";
                    try
                    {
                        string query = strScientificName;
                        System.Diagnostics.Trace.WriteLine("Fetching image for: " + query + " " + DateTime.Now.ToString());

                        // Create a Bing container.
                        string rootUri = "https://api.datamarket.azure.com/Bing/Search";
                        var bingContainer = new Bing.BingSearchContainer(new Uri(rootUri));

                        // Replace this value with your account key.
                        var accountKey = "EANiF0CYdv1M93VFgqvH42GJP09XZWptf0HN2P3suds=";

                        // Configure bingContainer to use your credentials.
                        bingContainer.Credentials = new NetworkCredential(accountKey, accountKey);

                        // Build the query.
                        var imageQuery = bingContainer.Image(query, null, null, null, null, null, null);

                        //Query Bing Asynchonously
                        QueryExtensions.QueryAsync(imageQuery).ContinueWith((result) =>
                        {                           
                            if ((result != null) && (result.Result != null))
                            {
                                Bing.ImageResult imageResult = result.Result.FirstOrDefault();
                                if (imageResult != null && (imageResult.MediaUrl != null))
                                {
                                    System.Diagnostics.Trace.WriteLine("Query finished for" + DateTime.Now + imageResult.Title);

                                    // Now download the data asynchronously
                                    var wc = new WebClient();

                                    //object sender, DownloadDataCompletedEventArgs e
                                    wc.DownloadDataCompleted += (obj, args) =>
                                    {
                                        DownloadDataCompletedEventArgs e = args;
                                        if (e != null)
                                        {
                                            try
                                            {
                                                var byte_image = e.Result;
                                                if (byte_image != null)
                                                {
                                                    if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\ThumbNails"))
                                                    {
                                                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\ThumbNails");
                                                    }

                                                    String strExtenstion = Path.GetExtension(imageResult.MediaUrl);
                                                    filenamePath = Directory.GetCurrentDirectory() + "\\ThumbNails\\" + query.Replace("/", "_") + strExtenstion;

                                                    try
                                                    {
                                                        System.Diagnostics.Trace.WriteLine("Saving thumbnail: " + filenamePath);
                                                        File.WriteAllBytes(filenamePath, byte_image);

                                                        bi.BeginInit();
                                                        bi.UriSource = new Uri(filenamePath, UriKind.RelativeOrAbsolute);
                                                        bi.EndInit();

                                                        _LoadedThumbNails.Add(strScientificName, bi);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        System.Diagnostics.Trace.WriteLine(ex.Message);
                                                    }
                                                }
                                            }
                                            catch (Exception e2)
                                            {
                                                System.Diagnostics.Trace.WriteLine(e2.Message);
                                            }
                                        }
                                    };

                                    wc.DownloadDataAsync(new Uri(imageResult.MediaUrl));
                                }
                            }
                        });                        
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                    finally
                    {
                        _images_currently_being_retrieved.Remove(strScientificName);
                    }

                    return (bi);
                });

                tsk.Start();                
            }
                        
            return (tsk);
        }
     
        public static bool FilePathHasInvalidChars(string path)
        {
            return (!string.IsNullOrEmpty(path) && path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0);
        }

        public static bool fileNameHasInvalidChars(string filename)
        {
            return (!string.IsNullOrEmpty(filename) && filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class QueryExtensions
    {
        public static Task<IEnumerable<TResult>> QueryAsync<TResult>(this DataServiceQuery<TResult> query)
        {
            return Task<IEnumerable<TResult>>.Factory.FromAsync(query.BeginExecute, query.EndExecute, null);
        }
    }
}
