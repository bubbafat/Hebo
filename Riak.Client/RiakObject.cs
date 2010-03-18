using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Riak.Client
{
    public delegate T StreamDownloadedCallback<T>(WebHeaderCollection headers, Stream content);
    
    public class RiakObject
    {
        private Bucket _bucket;
        private string _name;

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

        public virtual void Delete()
        {
            throw new NotImplementedException();
        }

        public virtual void Store(string data)
        {
            Store(new MemoryStream(Encoding.UTF8.GetBytes(data.ToCharArray()), false));
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
                CopyStream(data, req.GetRequestStream());

                using(RiakResponse response = req.GetResponse())
                {
                    if(response.StatusCode != HttpStatusCode.OK)
                    {
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
                    System.Uri.EscapeUriString(Name)));
            }
        }

        public string GetString()
        {
            return GetStream<string>(
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

        private void LoadHeaders(WebHeaderCollection webHeaderCollection)
        {
            VClock = GetOrDefault<string>(webHeaderCollection, "X-Riak-Vclock", null);
            ContentType = GetOrDefault<string>(webHeaderCollection, "Content-Type", null);
            ETag = GetOrDefault<string>(webHeaderCollection, "ETag", null);
            LastModified = GetOrDefault<DateTime>(webHeaderCollection, "Last-Modified", DateTime.UtcNow);
        }

        private T GetOrDefault<T>(WebHeaderCollection webHeaderCollection, string name, T defaultValue)
        {
            string header = webHeaderCollection[name];
            if(!string.IsNullOrEmpty(header))
            {
                try
                {
                    return (T)Convert.ChangeType(header, typeof(T));

                }
                catch (FormatException)
                {
                    Debug.Fail(string.Format("Error converting header {0} with value \"{1}\"",
                                              name,
                                              header));

                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
