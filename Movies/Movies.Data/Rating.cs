using System;

namespace Movies.Data
{
    /*
     * u.data     -- The full u data set, 100000 ratings by 943 users on 1682 items.
              Each user has rated at least 20 movies.  Users and items are
              numbered consecutively from 1.  The data is randomly
              ordered. This is a tab separated list of 
	         user id | item id | rating | timestamp. 
              The time stamps are unix seconds since 1/1/1970 UTC  
     */
    public class Rating : MovieDatabaseObject
    {
        public User User { get; set; }
        public Movie Movie { get; set; }
        public int Stars { get; set; }
        public DateTime When { get; set; }
    }
}
