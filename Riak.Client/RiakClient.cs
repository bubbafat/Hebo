using System;

namespace Riak.Client
{
    public class RiakClient
    {
        private readonly Uri _uri;

        public RiakClient(Uri riakUri)
        {
            if(!riakUri.IsAbsoluteUri)
            {
                throw new RiakException("The RiakClient uri must be absolute.");
            }
            _uri = riakUri;

            UserAgent = "Riak Client for .NET Applications version blah";
            ClientId = Guid.NewGuid().ToString();
        }

        public Bucket Bucket(string name)
        {
            Bucket b = new Bucket(this, name);
            b.Refresh();

            return b;
        }

        public Uri Uri { get { return _uri; } }

        public string UserAgent { get; set; }
        public string ClientId { get; set; }
    }
}