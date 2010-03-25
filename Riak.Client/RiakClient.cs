using System;

namespace Riak.Client
{
    public class RiakClient
    {
        public RiakClient(Uri riakUri)
        {
            if(!riakUri.IsAbsoluteUri)
            {
                throw new RiakClientException("The RiakClient uri must be absolute.");
            }

            Http = new HttpHandler(riakUri.AbsoluteUri);

            Http.DefaultHeaders[HttpWellKnownHeader.UserAgent] = "Riak Client for .NET";
            Http.DefaultHeaders[HttpWellKnownHeader.RiakClientId] = Guid.NewGuid().ToString();
            Http.DefaultHeaders[HttpWellKnownHeader.CacheControl] = "no-cache";
        }

        public HttpHandler Http
        {
            get; private set;
        }

        public Bucket Bucket(string name)
        {
            return Bucket(name, false);
        }

        public Bucket Bucket(string name, bool lazy)
        {
            Bucket b = new Bucket(this, name);

            if (!lazy)
            {
                b.Refresh();
            }

            return b;
        }

    }
}