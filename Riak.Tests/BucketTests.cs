using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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

            string referenceString = "Sample Stream Data";

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(referenceString));
            o1.Store(ms);

            string newString = null;

            newString = o1.GetStream<string>(
                delegate(WebHeaderCollection headers,
                         Stream stream)
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        return sr.ReadToEnd();
                    }
                });

            Assert.AreEqual(referenceString, newString);
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
    }
}
