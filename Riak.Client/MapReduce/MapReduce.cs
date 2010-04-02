using System;
using System.Collections.Generic;

namespace Riak.Client
{
    public class MapReduce
    {
        private readonly List<Phase> _map = new List<Phase>();
        private readonly List<Phase> _reduce = new List<Phase>();
        private readonly List<Phase> _link = new List<Phase>();

        private Bucket _inputBucket;
        private readonly IList<KeyValuePair<RiakObject, string>> _inputKeys 
            = new List<KeyValuePair<RiakObject, string>>();

        public MapReduce Input(Bucket bucket)
        {
            _inputKeys.Clear();
            _inputBucket = bucket;
            return this;
        }

        public MapReduce Input(RiakObject riakObject)
        {
            _inputBucket = null;
            _inputKeys.Add(new KeyValuePair<RiakObject, string>(riakObject, null));
            return this;
        }

        public MapReduce Input(RiakObject riakObject, string arg)
        {
            _inputBucket = null;
            _inputKeys.Add(new KeyValuePair<RiakObject, string>(riakObject, arg));
            return this;
        }

        public MapReduce Link(Phase phase)
        {
            phase.Type = PhaseType.Link;
            _link.Add(phase);
            return this;
        }

        public MapReduce Map(Phase phase)
        {
            phase.Type = PhaseType.Map;
            _map.Add(phase);
            return this;
        }

        public MapReduce Reduce(Phase phase)
        {
            phase.Type = PhaseType.Reduce;
            _reduce.Add(phase);
            return this;
        }

        public RiakHttpResponse Run()
        {
            throw new NotImplementedException();
        }
    }
}