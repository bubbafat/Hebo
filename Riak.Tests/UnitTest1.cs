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
    public class RiakRequestTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            RegisteredMockRequests mock = RiakMockRequest.GetMockByName(RiakMockRequest.GetBucketProperties);
            using(RiakRequest req = RiakRequest.Create(mock.Verb, mock.MockUri))
            using (RiakResponse response = req.GetResponse())
            {
                Assert.IsNotNull(response);
                Assert.AreEqual("application/json", response.ContentType);
                Assert.IsTrue(response.ContentLength > 0);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(response.GetResponseStream());
            }
        }

        [TestMethod]
        public void GetKeyFromBucket()
        {
            RegisteredMockRequests mock = RiakMockRequest.Mocks[RiakMockRequest.GetHelloWorldKeyBucket];

            RiakClient client = new RiakClient(RiakMockRequest.MockRiakRootUri);

            Bucket b = client.Bucket(mock.BucketName);
            Assert.IsTrue(b.Keys.Contains(mock.KeyName));

            RiakObject ro = b.Get(mock.KeyName);
            Assert.IsNotNull(ro);

            string hello = ro.GetString();

            Assert.IsNotNull(ro.ETag);
            Assert.IsNotNull(ro.LastModified);
            Assert.AreEqual("text/plain", ro.ContentType);
            Assert.IsNotNull(ro.VClock);

            Assert.AreEqual("Hello World", hello);
        }
    }
}
