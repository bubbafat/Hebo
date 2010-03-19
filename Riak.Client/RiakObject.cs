using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Riak.Client
{
    public delegate T StreamDownloadedCallback<out T>(WebHeaderCollection headers, Stream content);
    
    public class RiakObject
    {
        private readonly Bucket _bucket;
        private readonly string _name;

        public string ContentType
        {
            get; set;
        }

        public string VClock
        {
            get; set;
        }

        public DateTime LastModified
        {
            get; private set;
        }

        public string ETag
        {
            get; set;
        }

        internal RiakObject(Bucket bucket, string name)
        {
            _bucket = bucket;
            _name = name;
        }

        public Bucket Bucket
        {
            get
            {
                return _bucket;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public void Refresh()
        {
            using(RiakResponse response = _bucket.Client.Http.Head(
                _bucket.Client.Http.BuildUri(_bucket.Name, Name, null),
                new List<HttpStatusCode>{HttpStatusCode.OK, HttpStatusCode.Ambiguous, HttpStatusCode.NotModified, HttpStatusCode.NotFound}
                ))
            {
                LoadHeaders(response);
            }
        }

        private Dictionary<string,string> GetHeaders()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if(!string.IsNullOrEmpty(VClock))
            {
                headers[HttpWellKnownHeader.RiakVClock] = VClock;
            }

            return headers;
        }

        public virtual void Delete()
        {
            using (_bucket.Client.Http.Delete(
                _bucket.Client.Http.BuildUri(_bucket.Name, Name, null), 
                GetHeaders(),
                new List<HttpStatusCode>{HttpStatusCode.NoContent, HttpStatusCode.NotFound}))
            {
            }
        }

        public virtual void Store(string data)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data.ToCharArray()), false))
            {
                Store(stream);
            }
        }

        public virtual void Store(Stream data)
        {
            using(_bucket.Client.Http.Put(
                _bucket.Client.Http.BuildUri(_bucket.Name, Name, null),
                ContentType,
                GetHeaders(),
                new List<HttpStatusCode>{HttpStatusCode.OK, HttpStatusCode.NoContent},
                data))
            {
            }
        }

        public string GetString()
        {
            if(HasSiblings)
            {
                throw new RiakUnresolvedConflictException("Error: key has conflicts that must be resolved.");
            }

            return GetStream(
                delegate(WebHeaderCollection headers,
                         Stream stream)
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            });
        }

        public T GetStream<T>(StreamDownloadedCallback<T> callback)
        {
            if (HasSiblings)
            {
                throw new RiakUnresolvedConflictException("Error: key has conflicts that must be resolved.");
            }

            using (RiakResponse response = _bucket.Client.Http.Get(
                _bucket.Client.Http.BuildUri(_bucket.Name, Name, null)))
            {
                LoadHeaders(response);
                return callback(response.Headers, response.GetResponseStream());
            }
        }

        public bool HasSiblings
        {
            get; private set;
        }

        private void LoadHeaders(RiakResponse response)
        {
            HasSiblings = response.StatusCode == HttpStatusCode.Ambiguous;

            VClock = GetOrDefault<string>(response.Headers, "X-Riak-Vclock", null);
            ContentType = GetOrDefault<string>(response.Headers, "Content-Type", null);
            ETag = GetOrDefault<string>(response.Headers, "ETag", null);
            LastModified = GetOrDefault(response.Headers, "Last-Modified", DateTime.UtcNow);
        }

        private static T GetOrDefault<T>(NameValueCollection webHeaderCollection, string name, T defaultValue)
        {
            string header = webHeaderCollection[name];
            if (!string.IsNullOrEmpty(header))
            {
                try
                {
                    return (T) Convert.ChangeType(header, typeof (T));

                }
                catch (FormatException)
                {
                    Debug.Fail(string.Format("Error converting header {0} with value \"{1}\"",
                                             name,
                                             header));

                }
            }

            return defaultValue;
        }
    }
}
