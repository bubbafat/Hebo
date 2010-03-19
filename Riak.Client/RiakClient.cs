using System;

namespace Riak.Client
{
    public class RiakClient
    {
        public RiakClient(Uri riakUri)
        {
            if(!riakUri.IsAbsoluteUri)
            {
                throw new RiakException("The RiakClient uri must be absolute.");
            }

            Http = new HttpHandler(riakUri.AbsoluteUri);

            UserAgent = "Riak Client for .NET Applications version blah";
            ClientId = Guid.NewGuid().ToString();
        }

        public HttpHandler Http
        {
            get; private set;
        }

        public Bucket Bucket(string name)
        {
            Bucket b = new Bucket(this, name);
            b.Refresh();

            return b;
        }

        public Uri Uri { get { return Http.Uri; } }

        public string UserAgent { get; set; }
        public string ClientId { get; set; }
    }
}