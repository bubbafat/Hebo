using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace Riak.Client
{
    public class Bucket
    {
        private readonly List<string> _keys;

        internal Bucket(RiakClient client, string bucketName)
        {
            Client = client;
            Name = bucketName;
            _keys = new List<string>();
            Links = new LinkCollection();
        }

        public RiakClient Client
        {
            get; private set;
        }

        public LinkCollection Links
        {
            get; private set;
        }

        public void Refresh()
        {
            _keys.Clear();

            using (RiakHttpResponse response = Client.Http.Get(
                        Client.Http.BuildUri(Name, null, null), 
                        "application/json"))
            {
                Links = LinkCollection.Create(response.Headers[HttpWellKnownHeader.Link]);
                LoadFromJson(response);
            }
        }

        private void LoadFromJson(RiakHttpResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                JsonObject bucket = (JsonObject)JsonConvert.Import(sr);

                JsonArray keys = (JsonArray) bucket["keys"];
                foreach(string key in keys)
                {
                    Keys.Add(Uri.UnescapeDataString(key));
                }

                JsonObject properties = (JsonObject)bucket["props"];
                Name = Uri.UnescapeDataString((string)properties["name"]);
                AllowMulti = (bool)properties["allow_mult"];
            }
        }

        public bool AllowMulti
        {
            get; private set;
        }

        public string Name
        {
            get; private set;
        }

        public List<string> Keys
        {
            get
            {
                return _keys;
            }
        }

        public RiakObject Get(string name)
        {
            return RiakObject.Load(this, name);
        }

        public ICollection<RiakObject> GetAll(string keyName)
        {
            return GetAll(keyName, true);
        }

        public ICollection<RiakObject> GetAll(string keyName, bool lazy)
        {
            string accept = lazy ? "*/*" : "multipart/mixed";

            using(RiakHttpResponse response = Client.Http.Get(
                    Client.Http.BuildUri(Name, keyName, null),
                    accept,
                    Util.BuildListOf(HttpStatusCode.OK, HttpStatusCode.Ambiguous, HttpStatusCode.NotFound)))
            {
                switch(response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new List<RiakObject>();
                    case HttpStatusCode.Ambiguous:
                        return LoadSiblingObjects(response, keyName);
                    case HttpStatusCode.OK:
                        return new List<RiakObject> { RiakObject.Load(this, response) };
                    default:
                        throw new RiakClientException("The response code {0} was unexpected", response.StatusCode);
                }
            }
        }

        private ICollection<RiakObject> LoadSiblingObjects(RiakHttpResponse response, string keyName)
        {
            if (response.IsMultiPart)
            {
                List<RiakObject> siblings = new List<RiakObject>();

                Document doc = Document.Load(response);
                MultiPartDocument mpDoc = doc as MultiPartDocument;
                if (mpDoc != null)
                {
                    foreach (Document part in mpDoc.Parts)
                    {
                        Trace.WriteLine("Loaded document part:");
                        Trace.WriteLine(part.Dump());
                        siblings.Add(RiakObject.Load(this, keyName, part));
                    }
                }
                else
                {
                    siblings.Add(RiakObject.Load(this, keyName, doc));
                }

                return siblings;
            }
         
            return LoadConflictSiblings(response, keyName);
        }

        private ICollection<RiakObject> LoadConflictSiblings(RiakHttpResponse response, string keyName)
        {
            List<string> siblingIds = new List<string>();

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                string siblingHeader = sr.ReadLine();

                Debug.Assert(siblingHeader == "Siblings:",
                             string.Format("The header was \"{0}\" but expected \"Siblings:\"", siblingHeader));

                while (!sr.EndOfStream)
                {
                    siblingIds.Add(sr.ReadLine());
                }
            }

            List<RiakObject> siblingObjects = new List<RiakObject>(siblingIds.Count);

            siblingObjects.AddRange(siblingIds.Select(siblingId => RiakObject.Load(this, keyName, siblingId)));

            return siblingObjects;
        }

        public void SetAllowMulti(bool allowMulti)
        {
            // {"props":{"allow_mult":false}}

            string json = string.Format("{{\"props\":{{\"allow_mult\":{0}}}}}",
                                        allowMulti ? "true" : "false");

            using (Client.Http.Put(
                        Client.Http.BuildUri(Name, null, null),
                        "application/json",
                        Util.BuildListOf(HttpStatusCode.NoContent),
                        json))
            {
                AllowMulti = allowMulti;
            }
        }
    }
}
