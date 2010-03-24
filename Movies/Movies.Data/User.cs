namespace Movies.Data
{
    /*
     * u.user     -- Demographic information about the users; this is a tab
              separated list of
              user id | age | gender | occupation | zip code
              The user ids are the ones used in the u.data data set.
     */

    public enum Gender
    {
        Unknown,
        Male,
        Female,
    }

    public class User : MovieDatabaseObject
    {
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string Occupation { get; set; }
        public string ZipCode { get; set; }
    }
}
