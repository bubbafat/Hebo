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
            : this(client.Uri, bucketName)
        {
            UserAgent = client.UserAgent;
            ClientId = client.ClientId;
        }

        internal Bucket(Uri riakRootUri, string bucketName)
        {
            _bucketUri = new Uri(string.Format("{0}/{1}",
                                               riakRootUri.AbsoluteUri.TrimEnd('/'),
                                               Uri.EscapeUriString(bucketName)));
            Name = bucketName;
            _keys = new List<string>();
        }

        public Uri Uri
        {
            get { return _bucketUri;  }
        }

        public void Refresh()
        {
            _keys.Clear();

            using (RiakRequest req = RiakRequest.Create(WebRequestVerb.GET, _bucketUri))
            {
                req.UserAgent = UserAgent;

                using (RiakResponse response = req.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new RiakServerException("HTTP Response {0} while loading bucket {1} ({2})",
                                                      response.StatusCode,
                                                      Name,
                                                      _bucketUri.AbsoluteUri);
                    }

                    if (response.ContentType != "application/json")
                    {
                        throw new RiakServerException(
                            "Error while loading bucket {0} ({1}): Expected content type {2} but received {3}",
                            Name,
                            _bucketUri.AbsoluteUri,
                            "application/json",
                            response.ContentType);
                    }

                    LoadFromJson(response.GetResponseStream());
                }
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

            using(RiakRequest req = RiakRequest.Create(WebRequestVerb.PUT, Uri))
            {
                req.ClientId = ClientId;
                req.UserAgent = UserAgent;
                req.ContentType = "application/json";

                using(StreamWriter sw = new StreamWriter(req.GetRequestStream()))
                {
                    // value must be lower cased (default is upper)
                    string json = string.Format("{{\"props\":{{\"allow_mult\":{0}}}}}", 
                        allowMulti ? "true" : "false");

                    sw.Write(json);
                }

                using(RiakResponse response = req.GetResponse())
                {
                    if(response.StatusCode == HttpStatusCode.NoContent)
                    {
                        AllowMulti = allowMulti;
                    }
                    else
                    {
                        throw new RiakServerException(response, 
                            "Error setting allow_mutl property on bucket {0}", _bucketUri.AbsoluteUri);
                    }
                }
            }
        }
    }
}
