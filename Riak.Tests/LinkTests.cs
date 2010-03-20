using System;
using System.Linq;
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
            Assert.AreEqual("up", links[0].Rel);
            Assert.AreEqual("</raw/hb>; rel=\"up\"", links[0].ToString());

            Assert.AreEqual("/raw/hb/fourth", links[1].UriResource);
            Assert.AreEqual("foo", links[1].RiakTag);
            Assert.AreEqual("</raw/hb/fourth>; riaktag=\"foo\"", links[1].ToString());

            // we strip out the rel="up" link if it is the only thing in the link
            Assert.AreEqual("</raw/hb/fourth>; riaktag=\"foo\"",
                links.ToString());
        }

        [TestMethod]
        public void LinkWithMultipleParameters()
        {
            LinkParser parser = new LinkParser();
            LinkCollection links = parser.Parse("</raw/hb>; rel=\"up\"; riaktag=\"foo\"");

            Assert.AreEqual(1, links.Count);
            Assert.AreEqual("/raw/hb", links[0].UriResource);
            Assert.AreEqual("up", links[0].Rel);
            Assert.AreEqual("foo", links[0].RiakTag);

            // we don't strip rel="up" out here since there is a riaktag too.
            Assert.AreEqual("</raw/hb>; rel=\"up\"; riaktag=\"foo\"",
                links.ToString());
        }

        [TestMethod]
        public void LinkWithUnknownParameters()
        {
            LinkParser parser = new LinkParser();
            LinkCollection links = parser.Parse("</raw/hb>; rel=\"up\"; riaktag=\"foo\"; unk=\"value\"");

            Assert.AreEqual(1, links.Count);
            Assert.AreEqual("/raw/hb", links[0].UriResource);
            Assert.AreEqual("up", links[0].Rel);
            Assert.AreEqual("foo", links[0].RiakTag);
            Assert.AreEqual("value", links[0].UnknownParameters["unk"]);

            // normalizes as unknown -> rel -> riaktag
            Assert.AreEqual("</raw/hb>; unk=\"value\"; rel=\"up\"; riaktag=\"foo\"",
                links.ToString());
        }

        [TestMethod]
        public void LinkRoundTripping()
        {
            LinkParser parser = new LinkParser();
            LinkCollection links = parser.Parse("</raw/hb>; rel=\"up\"; riaktag=\"foo\"; unk=\"value\"");

            LinkCollection linksNew = parser.Parse(links.ToString());

            Assert.AreEqual(1, linksNew.Count);
            Assert.AreEqual("/raw/hb", linksNew[0].UriResource);
            Assert.AreEqual("up", linksNew[0].Rel);
            Assert.AreEqual("foo", linksNew[0].RiakTag);
            Assert.AreEqual("value", linksNew[0].UnknownParameters["unk"]);

            // normalizes as unknown -> rel -> riaktag
            Assert.AreEqual("</raw/hb>; unk=\"value\"; rel=\"up\"; riaktag=\"foo\"",
                linksNew.ToString());
        }

        [TestMethod]
        public void LinkOddlyFormedIsParsedAndNormalized()
        {
            LinkParser parser = new LinkParser();
            LinkCollection links = parser.Parse(" </raw/hb>    ;rel=\"up\"     ; riaktag=\"foo\"     ;");

            Assert.AreEqual(1, links.Count);
            Assert.AreEqual("/raw/hb", links[0].UriResource);
            Assert.AreEqual("up", links[0].Rel);
            Assert.AreEqual("foo", links[0].RiakTag);

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
            Assert.AreEqual(storedValue, Util.ReadString(o1.Data()));

            // the key should have one link back to the bucket.
            o1.Refresh();
            Assert.AreEqual(1, o1.Links.Count);
            Assert.IsNotNull(o1.Links[0].Rel);
            Assert.AreEqual("up", o1.Links[0].Rel);
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

            foreach (Link link in c)
            {
                Assert.IsNotNull(link);
            }

            c[0] = link1;

            Assert.AreSame(link1, c[0]);
        }

        [TestMethod]
        public void LinkFollowing()
        {
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket artist = client.Bucket("Artist");
            DeleteAllKeys(artist);

            RiakObject miles = CreateTextObject(artist, "Miles Davis");
            
            Bucket album = client.Bucket("Album");
            DeleteAllKeys(album);

            RiakObject kindofblue = CreateTextObject(album, "Kind of Blue");
            RiakObject conception = CreateTextObject(album, "Conception");
            RiakObject blue = CreateTextObject(album, "Blue");
            RiakObject dig = CreateTextObject(album, "Dig");

            miles.AddLink(kindofblue, "album");
            miles.AddLink(conception, "album");
            miles.AddLink(blue, "album");
            miles.AddLink(dig, "album");
            miles.Store();

            kindofblue.AddLink(miles, "artist");
            kindofblue.Store();
            
            conception.AddLink(miles, "artist");
            conception.Store();
            
            blue.AddLink(miles, "artist");
            blue.Store();

            dig.AddLink(miles, "artist");
            dig.Store();

            RiakObject loadedMiles = artist.Get("Miles Davis");
             
            Assert.AreEqual("Miles Davis", loadedMiles.Name);

            // the 4 we added plus the rel="up" for the key
            Assert.AreEqual(5, loadedMiles.Links.Count);

            Assert.AreEqual(4, loadedMiles.Links.Count(l => l.RiakTag != null));
            Assert.AreEqual(0, loadedMiles.Links.Count(l => l.RiakTag != null && l.RiakTag != "album"));
        }

        private static void DeleteAllKeys(Bucket bucket)
        {
            foreach(string key in bucket.Keys)
            {
                RiakObject o = bucket.Get(key);
                o.Delete();
            }
        }

        private static RiakObject CreateTextObject(Bucket bucket, string name)
        {
            RiakObject o = bucket.Get(name);
            o.ContentType = "text/plain";
            o.Store(name);
            o.Refresh();

            return o;
        }
    }
}
