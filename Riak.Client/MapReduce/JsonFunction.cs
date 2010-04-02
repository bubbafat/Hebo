namespace Riak.Client
{
    public class JsonFunction : Function
    {
        public JsonFunction()
            : this(null)
        {
        }

        public JsonFunction(string source)
            : base(source)
        {
        }

        public override Language Language
        {
            get { return Language.JavaScript; }
        }
    }
}