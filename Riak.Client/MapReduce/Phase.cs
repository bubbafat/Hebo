using System.Collections.Generic;

namespace Riak.Client
{
    public enum PhaseType
    {
        Map,
        Reduce,
        Link
    }

    public class Phase
    {
        public Phase(Function function)
        {
            Args = new List<string>();
            Keep = false;
            Function = function;
        }

        public PhaseType Type { get; set; }
        public bool Keep { get; set; }
        public IList<string> Args { get; private set; }
        public Function Function { get; set; }
    }
}