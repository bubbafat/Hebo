﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

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
        public const string RiakClientId = "X-Riak-ClientId";
        public const string RiakVClock = "X-Riak-Vclock";
        public const string ETag = "Etag";
        public const string Link = "Link";
        public const string LastModified = "Last-Modified";
        public const string CacheControl = "Cache-Control";
        public const string Location = "Location";
    }

    public delegate void SetHeaderValue(string name, string value);

    public class RiakHttpRequest
    {
        private readonly HttpWebRequest _webRequest;
        private readonly Dictionary<string, SetHeaderValue> _headerMap;

        public RiakHttpRequest(WebRequestVerb verb, Uri riakUri)
        {
            _webRequest = (HttpWebRequest)WebRequest.Create(riakUri);
            _webRequest.Method = verb.ToString();
            _webRequest.Accept = "*/*";
            _webRequest.AllowAutoRedirect = true;

            _webRequest.MaximumResponseHeadersLength = -1;
            _webRequest.Timeout = Timeout.Infinite;

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
                                 {HttpWellKnownHeader.RiakClientId, SetNonReservedValue },
                                 {HttpWellKnownHeader.RiakVClock, SetNonReservedValue },
                                 {HttpWellKnownHeader.CacheControl, SetNonReservedValue },
                             };
        }

        static void SetReserved(string name, string value) { throw new RiakClientException("The HTTP header value {0} is reserved and cannot be set explicitly.", name); }
        static void SetNotImplemented(string name, string value) { throw new RiakClientException("The HTTP header value {0} is not currently implemented by Riak.Client and cannot be set explicitly.", name); }
        void SetAccept(string name, string value) { _webRequest.Accept = value; }
        void SetConnection(string name, string value) { _webRequest.Connection = value; }
        void SetContentLength(string name, string value) { _webRequest.ContentLength = long.Parse(value); }
        void SetContentType(string name, string value) { _webRequest.ContentType = value; }
        void SetExpect(string name, string value) { _webRequest.Expect = value; }
        void SetIfModifiedSince(string name, string value) { _webRequest.IfModifiedSince = DateTime.Parse(value); }
        void SetUserAgent(string name, string value) { _webRequest.UserAgent = value; }
        void SetNonReservedValue(string name, string value) { _webRequest.Headers[name] = value; }
        void SetReferer(string name, string value) { _webRequest.Referer = value; }
        void SetTransferEncoding(string name, string value)
        {
            _webRequest.TransferEncoding = value;
            _webRequest.SendChunked = value.Length > 0;
        }
        
        public void AddHeader(string name, string value)
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

        public RiakHttpResponse GetResponse()
        {
            try
            {
                Trace.WriteLine(string.Format("{0}: {1}", _webRequest.Method, _webRequest.RequestUri.AbsoluteUri));
                HttpWebResponse webResponse = (HttpWebResponse)_webRequest.GetResponse();
                return new RiakHttpResponse(webResponse);
            }
            catch (WebException we)
            {
                if(we.Response != null && we.Response is HttpWebResponse)
                {
                    return new RiakHttpResponse((HttpWebResponse) we.Response);
                }

                throw;
            }
        }

        public static RiakHttpRequest Create(WebRequestVerb verb, Uri riakUri)
        {
            return new RiakHttpRequest(verb, riakUri);
        }

        public Stream GetRequestStream()
        {
            return _webRequest.GetRequestStream();
        }
    }
}
