using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Movies.Data;
using Riak.Client;

namespace Movies.DataLoader
{
    class Program
    {
        static readonly Dictionary<string, Movie> MovieCache = new Dictionary<string, Movie>();
        static readonly Dictionary<string, User> UserCache = new Dictionary<string, User>();
        static readonly List<Rating> RatingCache = new List<Rating>();

        const string MovieBucketName = "Movies";
        const string UsersBucketName = "Users";
        const string RatingsBucketName = "Ratings";

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintHelp();
            }
            else
            {
                string command = args[0].ToLower();
                Uri riakUri = new Uri(args[1], UriKind.Absolute);

                RiakClient client = new RiakClient(riakUri);

                switch (command)
                {
                    case "verify":
                        LoadAndVerifyData(client);
                        break;
                    case "clear":
                        ClearRiakData(client);
                        break;
                    case "upload":
                        UploadData(client);
                        break;
                    default:
                        PrintHelp();
                        break;
                }
            }
        }

        private static void ClearRiakData(RiakClient client)
        {
            ClearAllKeys(client, MovieBucketName);
            ClearAllKeys(client, UsersBucketName);
            ClearAllKeys(client, RatingsBucketName);
        }

        private static void ClearAllKeys(RiakClient client, string bucketName)
        {
            Bucket b = client.Bucket(bucketName);
            Console.WriteLine("Clearing {0} keys from {1}", b.Keys.Count, b.Name);
            Parallel.ForEach(b.Keys, key => b.Get(key, true).Delete());
        }

        private static void UploadData(RiakClient client)
        {
            LoadAndVerifyData(client);

            UploadObjects("Uploading movies", MovieCache.Values, MovieCache.Count);
            UploadObjects("Uploading ratings", RatingCache, RatingCache.Count);
            UploadObjects("Uploading users", UserCache.Values, UserCache.Count);
        }

        private static void UploadObjects(string prompt, IEnumerable<MovieDatabaseObject> keys, long count)
        {
            Console.WriteLine(prompt);
            long current = 0;
            long parallelRequests = 0;

            Parallel.ForEach(keys, mdo =>
            {
                long currentRequests = Interlocked.Increment(ref parallelRequests);

                mdo.Store(mdo.ToJson());

                long c = Interlocked.Increment(ref current);
                
                if(c % 100 == 0)
                {
                    Console.WriteLine("{0}: {1} of {2} ({3} concurrent requests)", prompt, c, count, currentRequests);                    
                }

                Interlocked.Decrement(ref parallelRequests);

            });

            Console.WriteLine("{0} complete.", prompt);            
        }

        private static void CreateMovieLinks(RiakClient client)
        {
            Bucket movies = client.Bucket(MovieBucketName, true);
            movies.SetAllowMulti(false);

            foreach(Movie m in MovieCache.Values)
            {
                m.Bucket = movies;
            }

            Bucket ratings = client.Bucket(RatingsBucketName, true);
            ratings.SetAllowMulti(false);

            foreach(Rating r in RatingCache)
            {
                r.Bucket = ratings;
            }

            Bucket users = client.Bucket(UsersBucketName, true);
            users.SetAllowMulti(false);

            foreach(User u in UserCache.Values)
            {
                u.Bucket = users;
            }

            Parallel.ForEach(RatingCache, r =>
                                              {
                                                  r.AddLink(r.Movie, "movie");
                                                  r.AddLink(r.User, "user");
                                              });
        }

        private static void PrintHelp()
        {
            Console.WriteLine("verify             : loads and verifies the sample data");
            Console.WriteLine("clear <riak uri>   : clears existing datasets from riak");
            Console.WriteLine("upload <riak uri>  : uploads the sample data to riak");
        }

        private static void LoadData(RiakClient client)
        {
            Console.WriteLine("Loading movie data...");
            LoadMovieData();

            Console.WriteLine("Loading user data...");
            LoadUserData();

            Console.WriteLine("Loading ratings data...");
            LoadRatings();

            Console.WriteLine("Building links...");
            CreateMovieLinks(client);
        }

        private static void LoadAndVerifyData(RiakClient client)
        {
            // our parsing throws on error so loading
            // is sufficent for this simple app
            LoadData(client);
        }

        private static void LoadRatings()
        {
            using (StreamReader ratingsReader = new StreamReader(
                LoadResourceStream("Movies.DataLoader.Datasets.u.data")))
            {
                while (!ratingsReader.EndOfStream)
                {
                                        string userString = ratingsReader.ReadLine();
                    if(string.IsNullOrEmpty(userString))
                    {
                        continue;
                    }

                    string[] ratingParts = userString.Split(new[] {'\t'});
                    Rating r = new Rating
                                   {
                                       User = UserCache[ratingParts[0]],
                                       Movie = MovieCache[ratingParts[1]],
                                       Stars = int.Parse(ratingParts[2]),
                                       When = DateTime.FromFileTimeUtc(long.Parse(ratingParts[3]))
                                   };

                    RatingCache.Add(r);
                }
            }
        }

        private static void LoadUserData()
        {
            using (StreamReader userReader = new StreamReader(
                LoadResourceStream("Movies.DataLoader.Datasets.u.userinfo")))
            {
                while(!userReader.EndOfStream)
                {
                    string userString = userReader.ReadLine();
                    if(string.IsNullOrEmpty(userString))
                    {
                        continue;
                    }

                    string[] userParts = userString.Split(new[] {'|'});
                    User u = new User
                                 {
                                     Name = userParts[0],
                                     Age = GetOrDefault(userParts[1], -1),
                                     Gender = GetGender(userParts[2]),
                                     Occupation = userParts[3],
                                     ZipCode = userParts[4]
                                 };

                    UserCache[u.Name] = u;
                }
            }
        }

        private static Gender GetGender(string gender)
        {
            if (!string.IsNullOrEmpty(gender))
            {
                switch (gender[0])
                {
                    case 'M':
                    case 'm':
                        return Gender.Male;
                    case 'F':
                    case 'f':
                        return Gender.Female;
                }
            }

            return Gender.Unknown;
        }

        private static T GetOrDefault<T>(string value, T defaultValue)
        {
            if(string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(value, typeof (T));
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
                        continue;
                    }

                    string[] movieParts = movieDescription.Split(new[] {'|'});
                    Movie movie = new Movie
                                      {
                                          Name = movieParts[0],
                                          Title = movieParts[1],
                                          ReleaseDate = GetOrDefault(movieParts[2], default(DateTime)),
                                          VideoReleaseDate = GetOrDefault(movieParts[3], default(DateTime)),
                                          IMDbUrl = movieParts[4],
                                      };

                    // index 5 is "Unknown" which is our enum default so we can skipp
                    // that.  So we want to read fields 6-24 mapping directly to genre enum 

                    const int offset = 5;
                    for (int genreOffset = 0; genreOffset < 19; genreOffset++)
                    {
                        int index = offset + genreOffset;
                        bool set = int.Parse(movieParts[index]) == 1;
                        if (set)
                        {
                            movie.Genre |= (1 << genreOffset);
                        }
                    }

                    MovieCache[movie.Name] = movie;
                }
            }
        }

        static Stream LoadResourceStream(string filename)
        {            
            return Assembly.GetCallingAssembly().GetManifestResourceStream(filename);
        }
    }
}
