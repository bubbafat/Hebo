using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Riak.Client
{
    public delegate T StreamDownloadedCallback<out T>(WebHeaderCollection headers, Stream content);
    
    public class RiakObject
    {
        public long ContentLength
        {
            get; private set;
        }

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
            Bucket = bucket;
            Name = name;
        }

        public Bucket Bucket
        {
            get; private set;
        }

        public string Name
        {
            get; private set;
        }

        public void Refresh()
        {
            using(RiakResponse response = Bucket.Client.Http.Head(
                Bucket.Client.Http.BuildUri(Bucket.Name, Name, null),
                HttpHandler.BuildListOf(HttpStatusCode.OK, HttpStatusCode.Ambiguous, HttpStatusCode.NotModified, HttpStatusCode.NotFound)))
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
            using (Bucket.Client.Http.Delete(
                Bucket.Client.Http.BuildUri(Bucket.Name, Name, null), 
                GetHeaders(),
                HttpHandler.BuildListOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound)))
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
            using(Bucket.Client.Http.Put(
                Bucket.Client.Http.BuildUri(Bucket.Name, Name, null),
                ContentType,
                GetHeaders(),
                HttpHandler.BuildListOf(HttpStatusCode.OK, HttpStatusCode.NoContent),
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

            using (RiakResponse response = Bucket.Client.Http.Get(
                Bucket.Client.Http.BuildUri(Bucket.Name, Name, null)))
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

            VClock = response.Headers[HttpWellKnownHeader.RiakVClock];
            ContentType = response.ContentType;
            ETag = response.Headers[HttpWellKnownHeader.ETag];
            LastModified = response.LastModified;
            ContentLength = response.ContentLength;
        }
    }
}
