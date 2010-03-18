using System;

namespace Riak.Client
{
    public class RegisteredMockRequests
    {
        public string MockName { get; set; }
        public string BucketName { get; set; }
        public string KeyName { get; set; }
        public Uri MockUri { get; set; }
        public WebRequestVerb Verb { get; set; }
        public GetMockResponse ResponseCallback { get; set; }
    }
}