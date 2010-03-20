using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riak.Client;

namespace Riak.Tests
{
    [TestClass]
    public class LinkTests
    {
        [TestMethod]
        public void LinkWithRelationalUri()
        {
            LinkParser parser = new LinkParser();
            LinkCollection links = parser.Parse("</raw/hb>; rel=\"up\", </raw/hb/fourth>; riaktag=\"foo\"");

            Assert.AreEqual(2, links.Count);
            Assert.AreEqual("/raw/hb", links[0].UriResource);
            Assert.AreEqual(1, links[0].Parameters.Count);
            Assert.AreEqual("rel=\"up\"", links[0].Parameters[0]);
            Assert.AreEqual("</raw/hb>; rel=\"up\"", links[0].ToString());

            Assert.AreEqual("/raw/hb/fourth", links[1].UriResource);
            Assert.AreEqual(1, links[0].Parameters.Count);
            Assert.AreEqual("riaktag=\"foo\"", links[1].Parameters[0]);
            Assert.AreEqual("</raw/hb/fourth>; riaktag=\"foo\"", links[1].ToString());

            Assert.AreEqual("</raw/hb>; rel=\"up\", </raw/hb/fourth>; riaktag=\"foo\"",
                links.ToString());
        }

        [TestMethod]
        public void LinkWithMultipleParameters()
        {
            LinkParser parser = new LinkParser();
            LinkCollection links = parser.Parse("</raw/hb>; rel=\"up\"; riaktag=\"foo\"");

            Assert.AreEqual(1, links.Count);
            Assert.AreEqual("/raw/hb", links[0].UriResource);
            Assert.AreEqual(2, links[0].Parameters.Count);
            Assert.AreEqual("rel=\"up\"", links[0].Parameters[0]);
            Assert.AreEqual("riaktag=\"foo\"", links[0].Parameters[1]);

            Assert.AreEqual("</raw/hb>; rel=\"up\"; riaktag=\"foo\"",
                links.ToString());
        }

        [TestMethod]
        public void LinkOddlyFormedIsParsedAndNormalized()
        {
            LinkParser parser = new LinkParser();
            LinkCollection links = parser.Parse(" </raw/hb>    ;rel=\"up\"     ; riaktag=\"foo\"     ;");

            Assert.AreEqual(1, links.Count);
            Assert.AreEqual("/raw/hb", links[0].UriResource);
            Assert.AreEqual(2, links[0].Parameters.Count);
            Assert.AreEqual("rel=\"up\"", links[0].Parameters[0]);
            Assert.AreEqual("riaktag=\"foo\"", links[0].Parameters[1]);

            Assert.AreEqual("</raw/hb>; rel=\"up\"; riaktag=\"foo\"",
                links.ToString());
        }


        [TestMethod]
        public void LinkOnNewKeyExistsAndParses()
        {
            // setup a bucket with a key
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket("BucketTest");
            Assert.IsFalse(bucket.AllowMulti);
            RiakObject o1 = bucket.Get("StringKey");
            o1.ContentType = "text/plain";

            string storedValue = Guid.NewGuid().ToString();

            o1.Store(storedValue);
            Assert.AreEqual(storedValue, o1.GetString());

            // the key should have one link back to the bucket.
            o1.Refresh();
            Assert.AreEqual(1, o1.Links.Count);
            Assert.AreEqual(1, o1.Links[0].Parameters.Count);
        }

        [TestMethod]
        public void LinkCollectionOperationalTests()
        {
            LinkCollection c = new LinkCollection();
            Link link1 = new Link();
            
            c.Add(new Link());
            c.Add(link1);
            c.Add(new Link());

            Assert.AreEqual(3, c.Count);

            Assert.IsTrue(c.Contains(link1));
            c.Remove(link1);
            Assert.IsFalse(c.Contains(link1));

            Assert.AreEqual(2, c.Count);

            Link[] linkarray = new Link[2];
            c.CopyTo(linkarray, 0);

            c.Clear();
            Assert.AreEqual(0, c.Count);

            c = new LinkCollection(linkarray);
            Assert.AreEqual(2, c.Count);

            Assert.IsFalse(c.IsReadOnly);

            foreach(Link link in c)
            {
                Assert.IsNotNull(link);
            }

            c[0] = link1;

            Assert.AreSame(link1, c[0]);
        }
    }
}
