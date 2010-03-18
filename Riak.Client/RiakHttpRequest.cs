using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Riak.Client
{
    class RiakHttpRequest : RiakRequest
    {
        private readonly HttpWebRequest _webRequest;

        public RiakHttpRequest(WebRequestVerb verb, Uri riakUri)
        {
            _webRequest = (HttpWebRequest)WebRequest.Create(riakUri);
            _webRequest.Method = verb.ToString();
            _webRequest.Accept = "*/*";
        }

        public override void AddHeader(string name, string value)
        {
            _webRequest.Headers[name] = value;
        }

        public override string ContentType
        {
            get { return _webRequest.ContentType; }
            set { _webRequest.ContentType = value; }
        }

        public override string UserAgent
        {
            get { return _webRequest.UserAgent; }
            set { _webRequest.UserAgent = value; }
        }

        public override string Accept
        {
            get { return _webRequest.Accept; }
            set { _webRequest.Accept = value; }
        }

        public override RiakResponse GetResponse()
        {
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)_webRequest.GetResponse();
                return new RiakHttpResponse(this, webResponse);
            }
            catch (WebException we)
            {
                if(we.Response != null && we.Response is HttpWebResponse)
                {
                    return new RiakHttpResponse(this, (HttpWebResponse) we.Response);
                }

#if DEBUG
                using(StreamReader sr = new StreamReader(we.Response.GetResponseStream()))
                {
                    Trace.TraceError(sr.ReadToEnd());
                }
#endif

                throw;
            }
        }

        public override Stream GetRequestStream()
        {
            return _webRequest.GetRequestStream();
        }
    }
}
