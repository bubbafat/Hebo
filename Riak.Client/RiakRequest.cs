using System;
using System.IO;

namespace Riak.Client
{
    public abstract class RiakRequest : IDisposable
    {
        public abstract void AddHeader(string name, string value);

        public abstract RiakResponse GetResponse();

        public abstract Stream GetRequestStream();

        public static RiakRequest Create(WebRequestVerb verb, Uri riakUri)
        {
            return new RiakHttpRequest(verb, riakUri);
        }

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
