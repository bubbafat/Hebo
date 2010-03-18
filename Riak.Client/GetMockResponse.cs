using System;

namespace Riak.Client
{
    public delegate RiakRequest GetMockResponse(WebRequestVerb verb, Uri riakUri, RegisteredMockRequests request);
}