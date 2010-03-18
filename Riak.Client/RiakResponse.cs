using System;
using System.IO;
using System.Net;

namespace Riak.Client
{
    public abstract class RiakResponse : IDisposable
    {
        protected RiakResponse()
        {
            Headers = new WebHeaderCollection();
        }

        public WebHeaderCollection Headers
        {
            get; 
            protected set;
        }

        public abstract string ContentType { get; }
        public abstract long ContentLength { get; }
        public abstract DateTime LastModified { get; }
        public abstract HttpStatusCode StatusCode { get; }

        public abstract Stream GetResponseStream();

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}