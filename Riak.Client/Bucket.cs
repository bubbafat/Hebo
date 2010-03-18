using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace Riak.Client
{
    public class Bucket
    {
        private Uri _bucketUri;
        private string _name;
        private bool _allowMulti;
        private List<string> _keys;

        internal Bucket(Uri riakRootUri, string bucketName)
        {
            _bucketUri = new Uri(string.Format("{0}/{1}",
                                               riakRootUri.AbsoluteUri.TrimEnd('/'),
                                               Uri.EscapeUriString(bucketName)));
            _name = bucketName;
            _keys = new List<string>();
        }

        public Uri Uri
        {
            get { return _bucketUri;  }
        }

        public void Refresh()
        {
            _keys.Clear();

            using(RiakRequest req = RiakRequest.Create(WebRequestVerb.GET, _bucketUri))
            using(RiakResponse response = req.GetResponse())
            {
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    throw new RiakServerException("HTTP Response {0} while loading bucket {1} ({2})",
                                                  response.StatusCode, 
                                                  _name, 
                                                  _bucketUri.AbsoluteUri);
                }

                if(response.ContentType != "application/json")
                {
                    throw new RiakServerException("Error while loading bucket {0} ({1}): Expected content type {2} but received {3}",
                        _name,
                        _bucketUri.AbsoluteUri,
                        "application/json",
                        response.ContentType);
                }

                loadFromJson(response.GetResponseStream());
            }
        }

        private void loadFromJson(Stream stream)
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
                _name = (string)properties["name"];
                _allowMulti = (bool)properties["allow_mult"];
            }
        }

        public bool AllowMulti
        {
            get { return _allowMulti; }
            set { _allowMulti = value; }
        }

        public string Name
        {
            get { return _name; }
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
    }
}
