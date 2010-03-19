using System;
using System.IO;
using System.Net;

namespace Riak.Client
{
    class RiakHttpResponse : RiakResponse
    {
        private readonly HttpWebResponse _webResponse;

        public RiakHttpResponse(HttpWebResponse webResponse)
        {
            _webResponse = webResponse;
            Headers = _webResponse.Headers;
        }

        public override string ContentType
        {
            get { return _webResponse.ContentType; }
        }

        public override long ContentLength
        {
            get { return _webResponse.ContentLength; }
        }

        public override DateTime LastModified
        {
            get { return _webResponse.LastModified; }
        }

        public override HttpStatusCode StatusCode
        {
            get { return _webResponse.StatusCode; }
        }

        public override Stream GetResponseStream()
        {
            return _webResponse.GetResponseStream();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _webResponse.Close();
            }

            base.Dispose(disposing);
        }
    }
}
