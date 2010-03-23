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
            return Data(false);
        }

        public byte[] Data(bool force)
        {
            if (force || _cachedData == null)
            {
                using (RiakHttpResponse response = Bucket.Client.Http.Get(CreateUri(),
                                                                          Util.BuildListOf(
                                                                          HttpStatusCode.OK,
                                                                          HttpStatusCode.Ambiguous,
                                                                          HttpStatusCode.NotModified,
                                                                          HttpStatusCode.NotFound)))
                {
                    if (response.StatusCode == HttpStatusCode.Ambiguous)
                    {
                        throw new RiakUnresolvedConflictException("Error: key has conflicts that must be resolved.");
                    }

                    LoadCachedData(response);
                }
            }

            return _cachedData;
        }

        private void LoadCachedData(RiakHttpResponse response)
        {
            _cachedData = new byte[response.ContentLength];
            Util.CopyStream(response.GetResponseStream(), _cachedData);
        }

        public string DataString()
        {
            return Util.ReadString(Data());
        }

        public long ContentLength
        {
            get;
            private set;
        }

        public string SiblingId
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public string VClock
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            set;
        }

        public LinkCollection Links
        {
            get;
            private set;
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
            LoadFromResponse(response);
        }

        public RiakObject(Bucket bucket, string keyName, string siblingId)
        {
            Bucket = bucket;
            SiblingId = siblingId;
            Name = keyName;
            Refresh();
        }

        public RiakObject(Bucket bucket, string keyName, Document part)
        {
            Bucket = bucket;
            Name = keyName;
            LoadDocumentHeaders(part);
            _cachedData = part.Content;
        }

        public Bucket Bucket
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public bool Exists
        {
            get;
            private set;
        }

        public void Refresh()
        {
            using (RiakHttpResponse response = Bucket.Client.Http.Get(CreateUri(),
                                                                      Util.BuildListOf(
                                                                          HttpStatusCode.OK,
                                                                          HttpStatusCode.Ambiguous,
                                                                          HttpStatusCode.NotModified,
                                                                          HttpStatusCode.NotFound)))
            {
                LoadFromResponse(response);
            }
        }

        private void LoadFromResponse(RiakHttpResponse response)
        {
            Exists = response.StatusCode != HttpStatusCode.NotFound;

            if (Exists)
            {
                LoadHeaders(response);
                if (!HasSiblings)
                {
                    LoadCachedData(response);
                }
            }
            else
            {
                InitializeNew();
            }
        }

        private void InitializeNew()
        {
            HasSiblings = false;
            Links = new LinkCollection();
            Exists = false;
            ETag = null;
            LastModified = default(DateTime);
            VClock = null;
            ContentType = null;
            ContentLength = 0;
            _cachedData = null;
        }

        private Uri CreateUri()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(SiblingId))
            {
                parameters["vtag"] = SiblingId;
            }

            return Bucket.Client.Http.BuildUri(Bucket.Name, Name, parameters);
        }

        private Dictionary<string, string> GetHeaders(WebRequestVerb verb)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(VClock))
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
            foreach (string h in headers.Keys)
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
            if (string.IsNullOrEmpty(ContentType))
            {
                throw new InvalidOperationException("ContentType must not be null when performing a RiakObject PUT operation");
            }

            using (Bucket.Client.Http.Put(
                CreateUri(),
                ContentType,
                GetHeaders(WebRequestVerb.PUT),
                Util.BuildListOf(HttpStatusCode.OK, HttpStatusCode.NoContent),
                data))
            {
            }

            // TODO: just request the body be returned and
            Refresh();
        }

        public virtual void Store()
        {
            using (Bucket.Client.Http.Put(
                CreateUri(),
                ContentType,
                GetHeaders(WebRequestVerb.PUT),
                Util.BuildListOf(HttpStatusCode.OK, HttpStatusCode.NoContent),
                Data()))
            {
            }
        }

        public bool HasSiblings
        {
            get;
            private set;
        }

        private void LoadDocumentHeaders(Document document)
        {
            MultiPartDocument mpParent = document.Parent as MultiPartDocument;
            HasSiblings = (mpParent != null && mpParent.Parts.Count > 1);

            VClock = document.GetLocalOrParentHeader(HttpWellKnownHeader.RiakVClock);

            ContentType = document.Headers[HttpWellKnownHeader.ContentType];
            ETag = document.Headers[HttpWellKnownHeader.ETag];
            if (document.Headers.ContainsKey(HttpWellKnownHeader.LastModified))
            {
                LastModified = DateTime.Parse(document.Headers[HttpWellKnownHeader.LastModified]);
            }
            ContentLength = document.Content.Length;
            Links = LinkCollection.Create(document.Headers[HttpWellKnownHeader.Link]);
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
    }
}
