using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Riak.Client
{
    public delegate T StreamDownloadedCallback<out T>(WebHeaderCollection headers, Stream content);
    
    public class RiakObject
    {
        private byte[] _cachedData;

        public byte[] Data()
        {
            if (HasSiblings)
            {
                throw new RiakUnresolvedConflictException("Error: key has conflicts that must be resolved.");
            }

            if (_cachedData == null)
            {
                using (RiakHttpResponse response = Bucket.Client.Http.Get(
                    CreateUri()))
                {
                    _cachedData = new byte[response.ContentLength];
                    Util.CopyStream(response.GetResponseStream(), _cachedData);
                }
            }

            return _cachedData;
        }

        public string DataString()
        {
            return Util.ReadString(Data());
        }

        public long ContentLength
        {
            get; private set;
        }

        public string SiblingId
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

        public LinkCollection Links
        {
            get; private set;
        }

        internal RiakObject(Bucket bucket, string name)
        {
            Bucket = bucket;
            Name = name;
            Refresh();
        }

        public RiakObject(Bucket bucket, RiakHttpResponse response)
        {
            Bucket = bucket;
            LoadHeaders(response);
        }

        public RiakObject(Bucket bucket, string keyName, string siblingId)
        {
            Bucket = bucket;
            SiblingId = siblingId;
            Name = keyName;
            Refresh();
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
            using (RiakHttpResponse response = Bucket.Client.Http.Head(CreateUri(),
                                                Util.BuildListOf(
                                                    HttpStatusCode.OK, 
                                                    HttpStatusCode.Ambiguous, 
                                                    HttpStatusCode.NotModified, 
                                                    HttpStatusCode.NotFound)))
            {
                LoadHeaders(response);
            }
        }

        private Uri CreateUri()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if(!string.IsNullOrEmpty(SiblingId))
            {
                parameters["vtag"] = SiblingId;
            }

            return Bucket.Client.Http.BuildUri(Bucket.Name, Name, parameters);
        }

        private Dictionary<string,string> GetHeaders(WebRequestVerb verb)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if(!string.IsNullOrEmpty(VClock))
            {
                headers[HttpWellKnownHeader.RiakVClock] = VClock;
            }

            if (verb == WebRequestVerb.POST || verb == WebRequestVerb.PUT)
            {
                if (Links.Count > 0)
                {
                    headers[HttpWellKnownHeader.Link] = Links.ToString();
                }
            }

#if TRACE
            foreach(string h in headers.Keys)
            {
                Trace.WriteLine(string.Format("{0}: {1}", h, headers[h]));
            }
#endif
            return headers;
        }

        public virtual void Delete()
        {
            using (Bucket.Client.Http.Delete(
                CreateUri(), 
                GetHeaders(WebRequestVerb.DELETE),
                Util.BuildListOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound)))
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
                CreateUri(),
                ContentType,
                GetHeaders(WebRequestVerb.PUT),
                Util.BuildListOf(HttpStatusCode.OK, HttpStatusCode.NoContent),
                data))
            {
            }
        }

        public bool HasSiblings
        {
            get; private set;
        }

        private void LoadHeaders(RiakHttpResponse response)
        {
            HasSiblings = response.StatusCode == HttpStatusCode.Ambiguous;

            VClock = response.Headers[HttpWellKnownHeader.RiakVClock];
            ContentType = response.ContentType;
            ETag = response.Headers[HttpWellKnownHeader.ETag];
            LastModified = response.LastModified;
            ContentLength = response.ContentLength;
            Links = LinkCollection.Create(response.Headers[HttpWellKnownHeader.Link]);
        }

        public void AddLink(RiakObject remoteObject, string riakTag)
        {
            Link newLink = new Link
                           {
                               UriResource = remoteObject.LinkPath, 
                               RiakTag = riakTag
                           };

            Links.Add(newLink);
        }

        public string LinkPath
        {
            get
            {
                return string.Format("{0}/{1}/{2}",
                              Bucket.Client.Http.Uri.AbsolutePath,
                              Uri.EscapeDataString(Bucket.Name),
                              Uri.EscapeDataString(Name));
            }
        }

        public void Store()
        {
            Store(string.Empty);
        }
    }
}
