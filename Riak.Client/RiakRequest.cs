using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Riak.Client
{
    public abstract class RiakRequest : IDisposable
    {
        public abstract void AddHeader(string name, string value);

        public abstract string ContentType { get; set; }

        public abstract string UserAgent { get; set; }

        public abstract string Accept { get; set; }

        public abstract RiakResponse GetResponse();

        public abstract Stream GetRequestStream();

        public static RiakRequest Create(WebRequestVerb verb, Uri riakUri)
        {
#if DEBUG
            if (RiakMockRequest.IsMockableRequest(riakUri, verb))
            {
                RegisteredMockRequests mock = RiakMockRequest.GetMockByUriAndVerb(riakUri, verb);
                return mock.ResponseCallback(verb, riakUri, mock);
            }
#endif

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
