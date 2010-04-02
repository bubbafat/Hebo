namespace Riak.Client
{
    public class ErlangFunction : Function
    {
        public ErlangFunction()
            : this(null)
        {
        }

        public ErlangFunction(string source)
            : base(source)
        {
        }
        public override Language Language
        {
            get { return Language.Erlang; }
        }
    }
}