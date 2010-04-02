namespace Riak.Client
{
    public enum Language
    {
        JavaScript,
        Erlang
    }

    public abstract class Function
    {
        protected Function()
            : this(null)
        {
        }

        protected Function(string source)
        {
            Source = source;
        }

        public abstract Language Language { get; }
        public string Source { get; set; }        
    }
}