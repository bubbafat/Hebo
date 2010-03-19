using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace Riak.Client
{
    public class Bucket
    {
        private readonly Uri _bucketUri;
        private readonly List<string> _keys;

        private bool _allowMulti;

        internal Bucket(RiakClient client, string bucketName)
        {
            Client = client;
            _bucketUri = Client.Http.BuildUri(bucketName, null, null);

            Name = bucketName;
            _keys = new List<string>();
        }

        public RiakClient Client
        {
            get; private set;
        }

        public Uri Uri
        {
            get { return _bucketUri;  }
        }

        public void Refresh()
        {
            _keys.Clear();

            using (RiakResponse response = Client.Http.Get(
                        Client.Http.BuildUri(Name, null, null), 
                        "application/json"))
            {
                LoadFromJson(response.GetResponseStream());
            }
        }

        private void LoadFromJson(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                JsonObject bucket = (JsonObject)JsonConvert.Import(sr);

                JsonArray keys = (JsonArray) bucket["keys"];
                foreach(string key in keys)
                {
                    Keys.Add(key);
                }

                JsonObject properties = (JsonObject)bucket["props"];
                Name = (string)properties["name"];
                _allowMulti = (bool)properties["allow_mult"];
            }
        }

        public bool AllowMulti
        {
            get { return _allowMulti; }
            set { _allowMulti = value; }
        }

        public string UserAgent { get; set; }
        public string ClientId { get; set; }

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
            return new RiakObject(this, name);
        }

        public void SetAllowMulti(bool allowMulti)
        {
            // {"props":{"allow_mult":false}}

            string json = string.Format("{{\"props\":{{\"allow_mult\":{0}}}}}",
                                        allowMulti ? "true" : "false");

            using (Client.Http.Put(
                        Client.Http.BuildUri(Name, null, null),
                        "application/json",
                        new List<HttpStatusCode> {HttpStatusCode.NoContent},
                        json))
            {
                AllowMulti = allowMulti;
            }
        }
    }
}
