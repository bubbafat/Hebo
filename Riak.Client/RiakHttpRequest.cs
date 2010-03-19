using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Riak.Client
{
    public class HttpWellKnownHeader
    {
        public const string Accept = "Accept";
        public const string Connection = "Connection";
        public const string ContentLength = "Content-Length";
        public const string ContentType = "Content-Type";
        public const string Expect = "Expect";
        public const string Date = "Date";
        public const string Host = "Host";
        public const string IfModifiedSince = "If-Modified-Since";
        public const string Range = "Range";
        public const string Referer = "Referer";
        public const string TransferEncoding = "Transfer-Encoding";
        public const string UserAgent = "User-Agent";
    }

    public delegate void SetHeaderValue(string name, string value);

    class RiakHttpRequest : RiakRequest
    {
        private readonly HttpWebRequest _webRequest;
        private readonly Dictionary<string, SetHeaderValue> _headerMap;

        public RiakHttpRequest(WebRequestVerb verb, Uri riakUri)
        {
            _webRequest = (HttpWebRequest)WebRequest.Create(riakUri);
            _webRequest.Method = verb.ToString();
            _webRequest.Accept = "*/*";

            _headerMap = new Dictionary<string, SetHeaderValue>
                             {
                                 {HttpWellKnownHeader.Accept, SetAccept},
                                 {HttpWellKnownHeader.Connection, SetConnection},
                                 {HttpWellKnownHeader.ContentLength, SetContentLength},
                                 {HttpWellKnownHeader.ContentType, SetContentType},
                                 {HttpWellKnownHeader.Expect, SetExpect},
                                 {HttpWellKnownHeader.Date, SetReserved},
                                 {HttpWellKnownHeader.Host, SetReserved},
                                 {HttpWellKnownHeader.IfModifiedSince, SetIfModifiedSince},
                                 {HttpWellKnownHeader.Range, SetNotImplemented},
                                 {HttpWellKnownHeader.Referer, SetReferer},
                                 {HttpWellKnownHeader.TransferEncoding, SetTransferEncoding},
                                 {HttpWellKnownHeader.UserAgent, SetUserAgent},
                             };
        }

        void SetAccept(string name, string value) { _webRequest.Accept = value; }
        void SetConnection(string name, string value) { _webRequest.Connection = value; }
        void SetContentLength(string name, string value) { _webRequest.ContentLength = long.Parse(value); }
        void SetContentType(string name, string value) { _webRequest.ContentType = value; }
        void SetExpect(string name, string value) { _webRequest.Expect = value; }
        void SetReserved(string name, string value) { throw new RiakException("The HTTP header value {0} is reserved and cannot be set explicitly.", name); }
        void SetNotImplemented(string name, string value) { throw new RiakException("The HTTP header value {0} is not currently implemented by Riak.Client and cannot be set explicitly.", name); }
        void SetIfModifiedSince(string name, string value) { _webRequest.IfModifiedSince = DateTime.Parse(value); }
        
        void SetReferer(string name, string value) { _webRequest.Referer = value; }
        void SetTransferEncoding(string name, string value)
        {
            _webRequest.TransferEncoding = value;
            _webRequest.SendChunked = value.Length > 0;
        }

        void SetUserAgent(string name, string value) { _webRequest.UserAgent = value; }
        
        public override void AddHeader(string name, string value)
        {
            SetHeaderValue setter;
            if(_headerMap.TryGetValue(name, out setter))
            {
                setter(name, value);
            }
            else
            {
                _webRequest.Headers[name] = value;                
            }
        }

        public override string ContentType
        {
            get { return _webRequest.ContentType; }
            set { _webRequest.ContentType = value; }
        }

        public override string ClientId
        {
            get { return _webRequest.Headers["X-Riak-ClientId"]; }
            set { _webRequest.Headers["X-Riak-ClientId"] = value; }
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
                if (we.Response != null)
                {
                    using (StreamReader sr = new StreamReader(we.Response.GetResponseStream()))
                    {
                        Trace.TraceError(sr.ReadToEnd());
                    }
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
