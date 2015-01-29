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
    /// <summary>
    /// The image librarian (Singleton) is used to make the fetching and displaying of thumbnail images more
    /// efficient. It keeps a cache of images on the local machine hd and can be used to fetch a thumbnail from the web
    /// which is then stored in the local cache.
    /// </summary>
    public sealed class ImageLibrarian
    {       
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
        private static ImageLibrarian _instance = new ImageLibrarian();
        public static ImageLibrarian IMAGE_LIBRARIAN
        {
            get { return _instance; }            
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
                var key = strScientificName.Replace('/', '_');
                if (_storedThumbNails.ContainsKey(key))
                {
                    strURL = _storedThumbNails[key];
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
                    if (_LoadedThumbNails.ContainsKey(strScientificName.Replace('/', '_')))
                    {
                        src = _LoadedThumbNails[strScientificName];
                    }
                    else
                    {
                        var strPath = getImageURL(strScientificName);
                        src = load_image_from_file(strPath, strScientificName);                                               
                    }
                }
             }
            
            return (src);
        }

        /// <summary>
        /// Load an image into a bitmap source and save it in the library.
        /// </summary>
        /// <param name="strPath"></param>
        /// <param name="strScientificName"></param>
        /// <returns></returns>
        private BitmapSource load_image_from_file(String strPath, String strScientificName)
        {
            BitmapSource src = null;

            if (!String.IsNullOrEmpty(strPath))
            {
                // Load it into memory and save it so we can reuse it.
                try
                {
                    BitmapImage bi = new BitmapImage();

                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = new Uri(strPath, UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    

                    src = bi;
                    if (!_LoadedThumbNails.ContainsKey(strScientificName))
                    {
                        _LoadedThumbNails.Add(strScientificName, src);
                    }
                }
                catch (Exception e)
                {
                    Trace.Write("Caught exception: " + e.Message);
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
                (!FilePathHasInvalidChars(strScientificName)) 
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
                                            var byte_image = e.Result;
                                            if (byte_image != null)
                                            {
                                                ensure_thumbnail_directory_exists();

                                                String strExtenstion = Path.GetExtension(imageResult.MediaUrl);
                                                filenamePath = String.Format("{0}{1}{2}{3}",
                                                                             Directory.GetCurrentDirectory(),
                                                                             "\\ThumbNails\\",
                                                                             query.Replace("/", "_"), 
                                                                             strExtenstion);

                                                System.Diagnostics.Trace.WriteLine("Saving thumbnail: " + filenamePath);
                                                File.WriteAllBytes(filenamePath, byte_image);

                                                load_image_from_file(filenamePath, strScientificName);                                                
                                            }                                                                                  
                                        }
                                    };

                                    wc.DownloadDataAsync(new Uri(imageResult.MediaUrl));
                                    wc.Dispose();
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

        /// <summary>
        /// Makes sure that the thumbnail directory exists on the drive.
        /// </summary>
        private void ensure_thumbnail_directory_exists()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\ThumbNails"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\ThumbNails");
            }
        }

        /// <summary>
        /// Tests if the provided path has invalid characters in it.
        /// </summary>
        /// <param name="path">path under test.</param>
        /// <returns>True if invalid characters, false otherwise.</returns>
        public static bool FilePathHasInvalidChars(string path)
        {
            return (!string.IsNullOrEmpty(path) && path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0);
        }

        /// <summary>
        /// Tests if the provided file name has invalid characters in it.
        /// </summary>
        /// <param name="filename">file name under test.</param>
        /// <returns>True if the file name has invalid characters, false otherwise.</returns>
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
