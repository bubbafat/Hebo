using System;
using System.IO;
using System.Net;

namespace Riak.Client
{
    public class RiakHttpResponse : IDisposable
    {
        private readonly HttpWebResponse _webResponse;

        public RiakHttpResponse(HttpWebResponse webResponse)
        {
            _webResponse = webResponse;
            Headers = _webResponse.Headers;
        }

        public WebHeaderCollection Headers
        {
            get;
            protected set;
        }

        public string ContentType
        {
            get { return _webResponse.ContentType; }
        }

        public long ContentLength
        {
            get { return _webResponse.ContentLength; }
        }

        public DateTime LastModified
        {
            get { return _webResponse.LastModified; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _webResponse.StatusCode; }
        }

        public Stream GetResponseStream()
        {
            return _webResponse.GetResponseStream();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                _webResponse.Close();
                GC.SuppressFinalize(this);
            }
        }

        public bool IsMultiPart
        {
            get
            {
                // TODO: this feels really hokey.
                string contentType = _webResponse.ContentType;
                return contentType.StartsWith("multipart/mixed;");
            }
        }
    }
}
