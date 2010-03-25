using System;
using Jayrock.Json.Conversion;
using Riak.Client;

namespace Movies.Data
{
    /*
     * u.item     -- Information about the items (movies); this is a tab separated
              list of
              movie id | movie title | release date | video release date |
              IMDb URL | unknown | Action | Adventure | Animation |
              Children's | Comedy | Crime | Documentary | Drama | Fantasy |
              Film-Noir | Horror | Musical | Mystery | Romance | Sci-Fi |
              Thriller | War | Western |
              The last 19 fields are the genres, a 1 indicates the movie
              is of that genre, a 0 indicates it is not; movies can be in
              several genres at once.
              The movie ids are the ones used in the u.data data set.
     */
    [Flags]
    public enum Genre
    {
        NotSet = 0,
        Unknown = 1,
        Action = 2,
        Adventure = 4,
        Animation = 8,
        Childrens = 16,
        Comedy = 32,
        Crime = 64,
        Documentary = 128,
        Drama = 256,
        Fantasy = 512,
        FilmNoir = 1024,
        Horror = 2048,
        Musical = 4096,
        Mystery = 8192,
        Romance = 16384,
        SciFi = 32768,
        Thriller = 65536,
        War = 131072,
        Western = 262144,
    }

    public abstract class MovieDatabaseObject : RiakObject
    {
        public MovieDatabaseObject()
        {
            ContentType = "application/json";
        }

        public virtual string ToJson()
        {
            return JsonConvert.ExportToString(this);
        }
    }

    public class Movie : MovieDatabaseObject
    {
        public string Title { get; set; }
        public long ReleaseDate { get; set; }
        public long VideoReleaseDate { get; set; }
        public string IMDbUrl { get; set; }
        public int Genre { get; set; } // bit fields are not supported by Jayrock so punt on that for now.
    }
}
