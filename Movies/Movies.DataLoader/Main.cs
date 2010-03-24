using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Movies.Data;

namespace Movies.DataLoader
{
    class Program
    {
        static readonly Dictionary<int, Movie> MovieCache = new Dictionary<int, Movie>();

        static void Main(string[] args)
        {
            LoadMovieData();
        }

        private static void LoadMovieData()
        {
            using (StreamReader movieReader = new StreamReader(
                LoadResourceStream("Movies.DataLoader.Datasets.u.item")))
            {
                while (!movieReader.EndOfStream)
                {
                    string movieDescription = movieReader.ReadLine();
                    if(string.IsNullOrEmpty((movieDescription)))
                    {
                        break;
                    }

                    string[] movieParts = movieDescription.Split(new[] {'|'});
                    Movie movie = new Movie
                                      {
                                          Id = int.Parse(movieParts[0]), 
                                          Title = movieParts[1]
                                      };

                    if (!string.IsNullOrEmpty(movieParts[2]))
                    {
                        movie.ReleaseDate = DateTime.Parse(movieParts[2]);
                    }

                    if (!string.IsNullOrEmpty(movieParts[3]))
                    {
                        movie.VideoReleaseDate = DateTime.Parse(movieParts[3]);
                    }

                    if (!string.IsNullOrEmpty(movieParts[4]))
                    {
                        movie.IMDbUrl = new Uri(movieParts[4], UriKind.Absolute);
                    }

                    // index 5 is "Unknown" which is our enum default so we can skipp
                    // that.  So we want to read fields 6-24 mapping directly to genre enum 

                    const int offset = 5;
                    for (int genreOffset = 0; genreOffset < 19; genreOffset++)
                    {
                        int index = offset + genreOffset;
                        bool set = int.Parse(movieParts[index]) == 1;
                        if (set)
                        {
                            movie.Genre |= (Genre)(1 << genreOffset);
                        }
                    }

                    MovieCache[movie.Id] = movie;
                }
            }
        }

        static Stream LoadResourceStream(string filename)
        {            
            return Assembly.GetCallingAssembly().GetManifestResourceStream(filename);
        }
    }
}
