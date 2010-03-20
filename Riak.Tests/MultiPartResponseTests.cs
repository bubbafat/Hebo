using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riak.Client;

namespace Riak.Tests
{
    [TestClass]
    public class MultiPartResponseTests
    {
        private readonly string MultiPartBucket = Guid.NewGuid().ToString();

        private const string AllTextKeyName = "AllTextKey";
        private const string AllBinaryKeyName = "AllBinaryKey";
        private const string MixedTypeKeyName = "MixedTypeKey";
        private readonly Random _rng = new Random();

        private List<string> _randomTextData;

        [TestInitialize]
        public void AddInitialData()
        {
            _randomTextData = new List<string>();
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket(MultiPartBucket);
            bucket.SetAllowMulti(true);

            RiakObject textKey = bucket.Get(AllTextKeyName);
            _randomTextData.Add(GetRandomTextData());
            _randomTextData.Add(GetRandomTextData());
            _randomTextData.Add(GetRandomTextData());
            _randomTextData.Add(GetRandomTextData());

            foreach(string rtd in _randomTextData)
            {
                textKey.Store(rtd);
                textKey.Refresh();
            }
        }

        [TestMethod]
        public void GetAllTextData()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket(MultiPartBucket);
            bucket.SetAllowMulti(true);

            Assert.Fail("fail");
        }

        private string GetRandomTextData()
        {
            int length = _rng.Next() % (1024 * 10) + 1;
            StringBuilder b = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                b.Append(PickRandomChar());
            }

            return b.ToString();
        }

        private char PickRandomChar()
        {
            const string set = "abcdefghijklmnopqrstuvwxyz";
            return set[_rng.Next() % set.Length];
        }

    }
}
