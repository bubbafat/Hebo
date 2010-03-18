using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Riak.Client
{
    class RiakHttpRequest : RiakRequest
    {
        public RiakHttpRequest(WebRequestVerb verb, Uri riakUri)
        {
            throw new NotImplementedException();
        }

        public override RiakResponse GetResponse()
        {
            throw new NotImplementedException();
        }

        public override Stream GetRequestStream()
        {
            throw new NotImplementedException();
        }
    }
}
