using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Riak.Client
{
    public class RiakMockRequest : RiakRequest
    {
        private WebRequestVerb _verb;
        private Uri _uri;
        private readonly RiakResponse _response;
        private readonly MemoryStream _requestStream;

        private const string MockUriRoot = "http://mock:8098/riak";

        private RiakMockRequest(WebRequestVerb verb, Uri riakUri, RiakResponse mockResponse)
        {
            _verb = verb;
            _uri = riakUri;
            _response = mockResponse;
            _requestStream = new MemoryStream();
        }

        public static readonly Uri MockRiakRootUri = new Uri(MockUriRoot);

        public override RiakResponse GetResponse()
        {
            return _response;
        }

        public override Stream GetRequestStream()
        {
            return _requestStream;
        }

        private static RiakRequest GetBucketPropertiesCallback(WebRequestVerb verb, Uri riakUri, RegisteredMockRequests mock)
        {
            return new RiakMockRequest(verb, riakUri,
                                       new RiakMockJsonResponse(
                                           "{\"props\":{\"name\":\"getbucketprops\",\"allow_mult\":false,\"big_vclock\":50,\"chash_keyfun\":{\"mod\":\"riak_util\",\"fun\":\"chash_std_keyfun\"},\"linkfun\":{\"mod\":\"raw_link_walker_resource\",\"fun\":\"mapreduce_linkfun\"},\"n_val\":3,\"old_vclock\":86400,\"small_vclock\":10,\"young_vclock\":20},\"keys\":[]}"));
        }

        private static RiakRequest GetMultiBucketPropertiesCallback(WebRequestVerb verb, Uri riakUri, RegisteredMockRequests mock)
        {
            return new RiakMockRequest(verb, riakUri,
                                       new RiakMockJsonResponse(
                                           "{\"props\":{\"name\":\"getbucketmultiprops\",\"allow_mult\":true,\"big_vclock\":50,\"chash_keyfun\":{\"mod\":\"riak_util\",\"fun\":\"chash_std_keyfun\"},\"linkfun\":{\"mod\":\"raw_link_walker_resource\",\"fun\":\"mapreduce_linkfun\"},\"n_val\":3,\"old_vclock\":86400,\"small_vclock\":10,\"young_vclock\":20},\"keys\":[]}"));
        }

        private static RiakRequest GetHelloWorldKeyBucketCallback(WebRequestVerb verb, Uri riakUri, RegisteredMockRequests mock)
        {
            return new RiakMockRequest(verb, riakUri,
                                       new RiakMockJsonResponse(
                                           "{\"props\":{\"name\":\"say\",\"allow_mult\":false,\"big_vclock\":50,\"chash_keyfun\":{\"mod\":\"riak_util\",\"fun\":\"chash_std_keyfun\"},\"linkfun\":{\"mod\":\"raw_link_walker_resource\",\"fun\":\"mapreduce_linkfun\"},\"n_val\":3,\"old_vclock\":86400,\"small_vclock\":10,\"young_vclock\":20},\"keys\":[\"helloworld\"]}"));
        }

        private static RiakRequest GetHelloWorldKeyCallback(WebRequestVerb verb, Uri riakUri, RegisteredMockRequests mock)
        {
            return new RiakMockRequest(verb, riakUri, new RiakMockTextResponse("Hello World"));
        }

        public static bool IsMockableRequest(Uri riakUri, WebRequestVerb verb)
        {
            return Mocks.Values.Any(m => m.MockUri == riakUri && m.Verb == verb);
        }

        public static RegisteredMockRequests GetMockByUriAndVerb(Uri riakUri, WebRequestVerb verb)
        {
            RegisteredMockRequests mock = Mocks.Values.FirstOrDefault(m => m.MockUri == riakUri && m.Verb == verb);
            if (mock != null)
            {
                return mock;
            }

            throw new Exception(string.Format("Mock not found: {0} {1}", verb, riakUri.AbsoluteUri));
        }

        public static RegisteredMockRequests GetMockByName(string name)
        {
            RegisteredMockRequests mock = Mocks.Values.FirstOrDefault(m => m.MockName == name);

            if (mock != null)
            {
                return mock;
            }

            throw new Exception("Mock not found: " + name);
        }

        public static RegisteredMockRequests GetRandomMock()
        {
            Random r = new Random();
            return Mocks.Values.ElementAt(r.Next(0, Mocks.Values.Count - 1));
        }

        public static Dictionary<string, RegisteredMockRequests> Mocks = new Dictionary<string, RegisteredMockRequests>()
            {
                { 
                    GetBucketProperties,
                    new RegisteredMockRequests()
                    {
                        MockName=GetBucketProperties,
                        BucketName = "getbucketprops",
                        MockUri = getUriForBucket("getbucketprops"),
                        Verb = WebRequestVerb.GET,
                        ResponseCallback = GetBucketPropertiesCallback
                    }
                },
                {
                    GetMultiBucketProperties,
                    new RegisteredMockRequests()
                        {
                            MockName = GetMultiBucketProperties,
                            BucketName = "getbucketmultiprops",
                            MockUri = getUriForBucket("getbucketmultiprops"),
                            Verb = WebRequestVerb.GET,
                            ResponseCallback = GetMultiBucketPropertiesCallback
                        }
                },
                {
                    GetHelloWorldKey,
                    new RegisteredMockRequests()
                        {
                            MockName = GetHelloWorldKey,
                            BucketName = "say",
                            KeyName = "helloworld",
                            MockUri = getUriForBucketAndKey("say", "helloworld"),
                            Verb = WebRequestVerb.GET,
                            ResponseCallback = GetHelloWorldKeyCallback
                        }
                },
                {
                    GetHelloWorldKeyBucket,
                    new RegisteredMockRequests()
                        {
                            MockName = GetHelloWorldKeyBucket,
                            BucketName = "say",
                            KeyName = "helloworld",
                            MockUri = getUriForBucket("say"),
                            Verb = WebRequestVerb.GET,
                            ResponseCallback = GetHelloWorldKeyBucketCallback
                        }
                }

            };

        private static Uri getUriForBucket(string bucketName)
        {
            return new Uri(string.Format("{0}/{1}", MockUriRoot, bucketName));
        }

        private static Uri getUriForBucketAndKey(string bucketName, string key)
        {
            return new Uri(string.Format("{0}/{1}/{2}", MockUriRoot, bucketName, key));
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                using(_requestStream) { }
            }

            base.Dispose(disposing);
        }

        public const string GetBucketProperties = "GetBucketProperties";
        public const string GetMultiBucketProperties = "GetMultiBucketProperties";
        public const string GetHelloWorldKey = "GetHelloWorldKey";
        public const string GetHelloWorldKeyBucket = "GetHelloWorldKeyBucket";
    }
}
