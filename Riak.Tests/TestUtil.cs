using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Riak.Client;

namespace Riak.Tests
{
    internal static class TestUtil
    {
        public static void DeleteAllKeys(Bucket bucket)
        {
            foreach (string key in bucket.Keys)
            {
                RiakObject o = bucket.Get(key);
                o.Delete();
            }
        }
    }
}
