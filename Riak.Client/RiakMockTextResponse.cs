using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Riak.Client
{
    public abstract class RiakMockResponse : RiakResponse
    {
        protected RiakMockResponse()
        {
            Headers.Add(new NameValueCollection
                            {
                                { "X-Riak-Vclock", "a85hYGBgzGDKBVIsbLvm1WYwJTLmsTLcjeE5ypcFAA==" },
                                { "Last-Modified", "Wed, 10 Mar 2010 18:11:41 GMT" },
                                { "ETag", "6dQBm9oYA1mxRSH0e96l5W" },
                            }
                );
        }
    }

    public class RiakMockTextResponse : RiakMockResponse
    {
        private readonly MemoryStream _stream;

        public RiakMockTextResponse(string responseContent)
        {
            _stream = new MemoryStream(Encoding.UTF8.GetBytes(responseContent.ToCharArray()), false);
            Headers.Add("Content-Type", "text/plain");
        }

        public override string ContentType
        {
            get { return "text/plain"; }
        }

        public override long ContentLength
        {
            get { return _stream.Length; }
        }

        public override DateTime LastModified
        {
            // 20 minutes old
            get { return DateTime.Now.Subtract(TimeSpan.FromMinutes(20)); }
        }

        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.OK; }
        }

        public override Stream GetResponseStream()
        {
            return _stream;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                using(_stream) { }
            }
        }
    }

    public class RiakMockJsonResponse : RiakMockTextResponse
    {
        public RiakMockJsonResponse(string responseContent)
            : base(responseContent)
        {
            Headers.Add("Content-Type", "application/json");
        }

        public override string ContentType
        {
            get { return "application/json"; }
        }
    }
}