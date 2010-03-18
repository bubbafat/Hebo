using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Riak.Client
{
    class RiakHttpResponse : RiakResponse
    {
        private RiakHttpRequest _riakHttpRequest;
        private HttpWebResponse _webResponse;

        public RiakHttpResponse(RiakHttpRequest riakHttpRequest, HttpWebResponse webResponse)
        {
            _riakHttpRequest = riakHttpRequest;
            _webResponse = webResponse;
            base.Headers = _webResponse.Headers;
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
    }
}
