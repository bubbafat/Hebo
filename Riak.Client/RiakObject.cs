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

        private Dictionary<string,string> GetHeaders()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
                                                     {
                                                         {"X-Riak-Vclock", VClock},
                                                     };

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

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
        }

        public virtual void Store(Stream data)
        {
            using(RiakRequest req = RiakRequest.Create(WebRequestVerb.PUT, Uri))
            {
                req.UserAgent = _bucket.UserAgent;

                if(string.IsNullOrEmpty(ContentType))
                {
                    throw new RiakException("ContentType must be set when storing a riak object");
                }

                req.ContentType = ContentType;
                
                if (!string.IsNullOrEmpty(VClock))
                {
                    req.AddHeader("X-Riak-Vclock", VClock);
                }

                using (Stream requestStream = req.GetRequestStream())
                {
                    CopyStream(data, requestStream);
                }

                using(RiakResponse response = req.GetResponse())
                {
                    switch(response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        case HttpStatusCode.NoContent:
                            break;
                        default:
                            throw new RiakServerException(response,
                                "Error storing key {0} at {1}",
                                _name,
                                Uri);
                    }
                }
            }
        }

        public virtual Uri Uri
        {
            get
            {
                return new Uri(string.Format("{0}/{1}",
                    _bucket.Uri.AbsoluteUri,
                    Uri.EscapeUriString(Name)));
            }
        }

        public string GetString()
        {
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
            using (RiakRequest req = RiakRequest.Create(WebRequestVerb.GET, Uri))
            using (RiakResponse response = req.GetResponse())
            {
                LoadHeaders(response.Headers);

                if(response.StatusCode != HttpStatusCode.OK)
                {
                    throw new RiakServerException(response, 
                        "Error downloading content for key {0} at {1}",
                        Name,
                        Uri);
                }

                return callback(response.Headers, response.GetResponseStream());
            }
        }

        private void LoadHeaders(NameValueCollection webHeaderCollection)
        {
            VClock = GetOrDefault<string>(webHeaderCollection, "X-Riak-Vclock", null);
            ContentType = GetOrDefault<string>(webHeaderCollection, "Content-Type", null);
            ETag = GetOrDefault<string>(webHeaderCollection, "ETag", null);
            LastModified = GetOrDefault(webHeaderCollection, "Last-Modified", DateTime.UtcNow);
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
