using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riak.Client;

namespace Riak.Tests
{
    [TestClass]
    public class MultiPartResponseTests
    {
        [TestMethod]
        public void GetAllTextData()
        {
            string _multiPartBucket = Guid.NewGuid().ToString();
            const string AllTextKeyName = "AllTextKey";

            _randomTextData = new List<string>();
            RiakClient client = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = client.Bucket(_multiPartBucket);
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

            client = new RiakClient(Settings.RiakServerUri);
            bucket = client.Bucket(_multiPartBucket);

            ICollection<RiakObject> conflicts = bucket.GetAll(AllTextKeyName, false);

            // whoa!  Why 2?  Because we're using the same client ID for all
            // of these objects so the Riak server is going to throw away all
            // but the last two conflicts.
            Assert.AreEqual(2, conflicts.Count);

            foreach(RiakObject ro in conflicts)
            {
                Assert.AreEqual("text/plain", ro.ContentType);
                string content = ro.DataString();
                Assert.IsNotNull(content);
                Assert.IsTrue(_randomTextData.Contains(content));
                Assert.AreEqual(ro.ContentLength, content.Length);
            }
        }

        [TestMethod]
        public void ConflictsFromUniqueClientsWithVClock()
        {
            string BucketName = Guid.NewGuid().ToString();
            string KeyName = Guid.NewGuid().ToString();

            RiakClient startClient = new RiakClient(Settings.RiakServerUri);
            Bucket startBucket = startClient.Bucket(BucketName);
            startBucket.SetAllowMulti(true);
            RiakObject startKey = startBucket.Get(KeyName);
            startKey.ContentType = "text/plain";
            startKey.Store(Guid.NewGuid().ToString());

            List<RiakClient> clients = new List<RiakClient>();
            clients.Add(new RiakClient(Settings.RiakServerUri));
            clients.Add(new RiakClient(Settings.RiakServerUri));
            clients.Add(new RiakClient(Settings.RiakServerUri));
            clients.Add(new RiakClient(Settings.RiakServerUri));

            List<RiakObject> conflicts = new List<RiakObject>();
            foreach(RiakClient client in clients)
            {
                Bucket b = client.Bucket(BucketName);
                Assert.IsTrue(b.AllowMulti);
                conflicts.Add(b.Get(KeyName));
            }

            foreach(RiakObject key in conflicts)
            {
                Assert.IsNotNull(key.VClock);
                key.Store(Guid.NewGuid().ToString());
            }

            RiakClient verifyClient = new RiakClient(Settings.RiakServerUri);
            Bucket bucket = verifyClient.Bucket(BucketName);

            ICollection<RiakObject> verifyConflicts = bucket.GetAll(KeyName, false);
            Assert.AreEqual(4, verifyConflicts.Count);

            foreach(RiakObject ro in verifyConflicts)
            {
                Assert.AreEqual("text/plain", ro.ContentType);
                string content = ro.DataString();
                Assert.IsNotNull(content);
                Assert.AreEqual(ro.ContentLength, content.Length);
            }

        }

        [TestMethod]
        public void LoadSimpleMultiPartDocument()
        {
            // this file is saved in CodePage 28591 (iso-8859-1 Western European (ISO))
            // this file uses unix style line endings (like riak would return)
            using (Stream response = LoadDocumentStream("Riak.Tests.SimpleMultiPartDocument.dat"))
            {
                Document d = Document.Load(response);
                Assert.IsNotNull(d);
                Assert.IsNotNull(d.Headers);
                Assert.IsNotNull(d.Content);

                MultiPartDocument mpd = d as MultiPartDocument;
                Assert.IsNotNull(mpd);
                Assert.IsNotNull(mpd.Parts);
                Assert.AreEqual(4, mpd.Parts.Count);

                CheckData(mpd.Parts[0], "{\"bar\":\"baz\"}");
                CheckData(mpd.Parts[1], "{\"baz\":\"bar\"}");
                CheckData(mpd.Parts[2], "{\"bap\":\"boz\"}");
                CheckData(mpd.Parts[3], "{\"foo\":\"boo\"}");
            }
        }

        private static void CheckData(Document document, string expectedString)
        {
            byte[] expected = Encoding.GetEncoding(28591).GetBytes(expectedString);
            Assert.AreEqual(expected.Length, document.Content.Length);

            for(int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], document.Content[i]);
            }
        }

        private Stream LoadDocumentStream(string streamName)
        {
            return Assembly.GetCallingAssembly().GetManifestResourceStream(streamName);
        }

        private string GetRandomTextData()
        {
            int length = _rng.Next() % (20) + 1;
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

        private void SetAndStoreTextKey(RiakObject textKey)
        {
            textKey.ContentType = "text/plain";
            string data = GetRandomTextData();
            textKey.Store(data);
            _randomTextData.Add(data);
        }

        private readonly Random _rng = new Random();
        private List<string> _randomTextData;
    }
}
