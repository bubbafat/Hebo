using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riak.Client;

namespace Riak.Tests
{
    [TestClass]
    public class BucketTests
    {
        [TestMethod]
        public void SaveAStringKeyValue()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("BucketTest");
            Assert.IsFalse(bucket.AllowMulti);
            RiakObject o1 = bucket.Get("StringKey");
            o1.ContentType = "text/html";

            string storedValue = Guid.NewGuid().ToString();

            o1.Store(storedValue);
            Assert.AreEqual(storedValue, o1.GetString());
        }

        [TestMethod]
        public void SaveABinaryKeyValue()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("BucketTest");
            Assert.IsFalse(bucket.AllowMulti);
            RiakObject o1 = bucket.Get("BinaryKey");
            o1.ContentType = "application/binary";

            const string referenceString = "Sample Stream Data";

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(referenceString));
            o1.Store(ms);

            string newString = o1.GetStream(
                delegate(WebHeaderCollection headers,
                         Stream stream)
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        return sr.ReadToEnd();
                    }
                });

            Assert.AreEqual(referenceString, newString);

            o1.Refresh();

            Assert.AreEqual("application/binary", o1.ContentType);
        }

        [TestMethod]
        public void DeleteExistingKeys()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("BucketTest");

            if (bucket.Keys.Count == 0)
            {
                RiakObject o1 = bucket.Get("SomeKey");
                o1.ContentType = "text/plain";
                o1.Store(Guid.NewGuid().ToString());
            }

            bucket.Refresh();
            Assert.IsTrue(bucket.Keys.Count > 0);

            foreach(string key in bucket.Keys)
            {
                bucket.Get(key).Delete();
            }

            bucket.Refresh();
            Assert.AreEqual(0, bucket.Keys.Count);
        }

        [TestMethod]
        public void DeleteMissingKeys()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("BucketTest");

            for (int i = 0; i < 10; i++)
            {
                RiakObject o1 = bucket.Get(i.ToString());
                o1.ContentType = "text/plain";
                o1.Store(Guid.NewGuid().ToString());
            }

            bucket.Refresh();

            for (int i = 0; i < 10; i++)
            {
                RiakObject o1 = bucket.Get(i.ToString());
                o1.Delete(); // returns 204
                o1.Delete(); // returns 404
            }

            for (int i = 0; i < 10; i++)
            {
                // allocates a new/missing key
                RiakObject o1 = bucket.Get(i.ToString());
                o1.Delete(); // returns 404
            }
        }

        [TestMethod]
        public void ConflictOnNonMultiBucket()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("NonMultiBucket");
            Assert.IsFalse(bucket.AllowMulti);

            RiakObject keyToConflictOn = bucket.Get(Guid.NewGuid().ToString());
            keyToConflictOn.ContentType = "text/plain";
            keyToConflictOn.Store("Data1");

            RiakObject conflict1 = bucket.Get(keyToConflictOn.Name);
            conflict1.GetString();

            RiakObject conflict2 = bucket.Get(keyToConflictOn.Name);
            conflict2.GetString();

            Assert.IsNotNull(conflict1.VClock);
            Assert.AreEqual(conflict1.VClock, conflict2.VClock);

            conflict1.Store("Conflict1");
            conflict2.Store("Conflict2");

            Assert.AreEqual(keyToConflictOn.GetString(), "Conflict2");
        }

        [TestMethod]
        public void SettingAllowMultiPersistsCorrectly()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("SettingAllowMultiPersistsCorrectly");

            bucket.SetAllowMulti(true);
            Assert.IsTrue(bucket.AllowMulti);
            bucket.Refresh();
            Assert.IsTrue(bucket.AllowMulti);

            bucket.SetAllowMulti(false);
            Assert.IsFalse(bucket.AllowMulti);
            bucket.Refresh();
            Assert.IsFalse(bucket.AllowMulti);

            bucket.SetAllowMulti(true);
            Assert.IsTrue(bucket.AllowMulti);
            bucket.Refresh();
            Assert.IsTrue(bucket.AllowMulti);
        }


        [TestMethod]
        [ExpectedException(typeof(RiakUnresolvedConflictException))]
        public void ConflictOnAllowMultiBucket()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("ConflictOnAllowMultiBucket");
            bucket.SetAllowMulti(true);

            RiakObject keyToConflictOn = bucket.Get(Guid.NewGuid().ToString());
            keyToConflictOn.ContentType = "text/plain";
            keyToConflictOn.Store("Data1");

            RiakObject conflict1 = bucket.Get(keyToConflictOn.Name);
            conflict1.GetString();

            RiakObject conflict2 = bucket.Get(keyToConflictOn.Name);
            conflict2.GetString();

            Assert.IsNotNull(conflict1.VClock);
            Assert.AreEqual(conflict1.VClock, conflict2.VClock);

            conflict1.Store("Conflict1");
            conflict2.Store("Conflict2");

            keyToConflictOn.Refresh();
            Assert.IsTrue(keyToConflictOn.HasSiblings);

            conflict1.Refresh();
            Assert.IsTrue(conflict1.HasSiblings);

            conflict2.Refresh();
            Assert.IsTrue(conflict2.HasSiblings);

            keyToConflictOn.GetString();
        }
    }
}
