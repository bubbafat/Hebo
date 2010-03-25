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
        protected RiakObject()
        {
            InitializeNew();
        }

        protected RiakObject(Bucket bucket, string name)
        {
            Bucket = bucket;
            Name = name;
        }

        public static RiakObject Load(Bucket bucket, string keyName)
        {
            return Load(bucket, keyName, true);
        }

        public static RiakObject Load(Bucket bucket, string keyName, bool autoRefresh)
        {
            RiakObject ro = new RiakObject
                                {
                                    Bucket = bucket, 
                                    Name = keyName
                                };

            ro.Refresh();

            return ro;
        }

        public static RiakObject Load(Bucket bucket, string keyName, Document part)
        {
            RiakObject ro = new RiakObject
                                {
                                    Bucket = bucket, 
                                    Name = keyName
                                };

            ro.LoadDocumentHeaders(part);
            ro._cachedData = part.Content;

            return ro;
        }

        public static RiakObject Load(Bucket bucket, RiakHttpResponse response)
        {
            RiakObject ro = new RiakObject
                                {
                                    Bucket = bucket
                                };

            ro.LoadFromResponse(response);

            return ro;
        }

        public static RiakObject Load(Bucket bucket, string keyName, string siblingId)
        {
            RiakObject ro = new RiakObject
                                {
                                    Bucket = bucket, 
                                    SiblingId = siblingId, 
                                    Name = keyName
                                };
            ro.Refresh();

            return ro;
        }

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

        public string DataString()
        {
            return Util.ReadString(Data());
        }

        public long ContentLength
        {
            get;
            protected set;
        }

        public string SiblingId
        {
            get;
            protected set;
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
            protected set;
        }

        public string ETag
        {
            get;
            set;
        }

        public LinkCollection Links
        {
            get;
            protected set;
        }

        public Bucket Bucket
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public bool Exists
        {
            get;
            protected set;
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

        public void AddLink(RiakObject remoteObject, string riakTag)
        {
            Link newLink = new Link
            {
                UriResource = remoteObject.LinkPath,
                RiakTag = riakTag
            };

            Links.Add(newLink);
        }

        public virtual string LinkPath
        {
            get
            {
                return string.Format("{0}/{1}/{2}",
                              Bucket.Client.Http.Uri.AbsolutePath,
                              Uri.EscapeDataString(Bucket.Name),
                              Uri.EscapeDataString(Name));
            }
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

        public virtual bool HasSiblings
        {
            get;
            protected set;
        }

        protected virtual void LoadFromResponse(RiakHttpResponse response)
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

        protected virtual Uri CreateUri()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(SiblingId))
            {
                parameters["vtag"] = SiblingId;
            }

            return Bucket.Client.Http.BuildUri(Bucket.Name, Name, parameters);
        }

        protected virtual Dictionary<string, string> GetHeaders(WebRequestVerb verb)
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

        protected virtual void LoadDocumentHeaders(Document document)
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


        protected virtual void LoadHeaders(RiakHttpResponse response)
        {
            HasSiblings = response.StatusCode == HttpStatusCode.Ambiguous;

            VClock = response.Headers[HttpWellKnownHeader.RiakVClock];
            ContentType = response.ContentType;
            ETag = response.Headers[HttpWellKnownHeader.ETag];
            LastModified = response.LastModified;
            ContentLength = response.ContentLength;
            Links = LinkCollection.Create(response.Headers[HttpWellKnownHeader.Link]);
        }

        protected virtual void LoadCachedData(RiakHttpResponse response)
        {
            _cachedData = new byte[response.ContentLength];
            Util.CopyStream(response.GetResponseStream(), _cachedData);
        }


        private byte[] _cachedData;
    }
}
