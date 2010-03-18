using System;

namespace Riak.Client
{
    public class RiakClient
    {
        private Uri _uri;

        public RiakClient(Uri riakUri)
        {
            if(!riakUri.IsAbsoluteUri)
            {
                throw new RiakException("The RiakClient uri must be absolute.");
            }
            _uri = riakUri;
        }

        public Bucket Bucket(string name)
        {
            Bucket b = new Bucket(_uri, name);
            b.Refresh();

            return b;
        }
    }
}