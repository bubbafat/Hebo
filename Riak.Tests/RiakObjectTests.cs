using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riak.Client;

namespace Riak.Tests
{
    [TestClass]
    public class RiakObjectTests
    {
        [TestMethod]
        public void GetAutoAssignedId()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("RiakObjectTest");

            RiakObject o1 = bucket.GetNew();
            o1.ContentType = "text/html";
            string storedValue = Guid.NewGuid().ToString();
            o1.Store(storedValue);

            // the name should be auto-assigned to something valid
            Assert.IsNotNull(o1.Name);
            Assert.AreEqual(storedValue, Util.ReadString(o1.Data()));
        }
    }
}
