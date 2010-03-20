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
            TestUtil.DeleteAllKeys(bucket);
            bucket.SetAllowMulti(true);

            RiakObject key1 = bucket.Get(AllTextKeyName);
            RiakObject key2 = bucket.Get(AllTextKeyName);
            RiakObject key3 = bucket.Get(AllTextKeyName);
            RiakObject key4 = bucket.Get(AllTextKeyName);

            SetAndStoreTextKey(key1);
            SetAndStoreTextKey(key2);
            SetAndStoreTextKey(key3);
            SetAndStoreTextKey(key4);
        }

        private void SetAndStoreTextKey(RiakObject textKey)
        {
            textKey.ContentType = "text/plain";
            string data = GetRandomTextData();
            textKey.Store(data);
            _randomTextData.Add(data);
        }

        [TestMethod]
        public void GetAllTextData()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket(MultiPartBucket);

            ICollection<RiakObject> conflicts = bucket.GetAll(AllTextKeyName, false);
            Assert.AreEqual(4, conflicts.Count);

            foreach(RiakObject ro in conflicts)
            {
                Assert.AreEqual("text/plain", ro.ContentType);
                string content = ro.DataString();
                Assert.IsNotNull(content);
                Assert.IsTrue(_randomTextData.Contains(content));
                Assert.AreEqual(ro.ContentLength, content.Length);
            }
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
