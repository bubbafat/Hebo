using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Riak.Client
{
    public class HttpHandler
    {
        private readonly string _riakRootUri;

        public HttpHandler(string riakUri)
        {
            if(string.IsNullOrEmpty(riakUri))
            {
                throw new ArgumentNullException("riakUri");
            }

            _riakRootUri = riakUri.TrimEnd('/');

            DefaultHeaders = new Dictionary<string, string>();
        }

        public Dictionary<string, string> DefaultHeaders
        {
            get; private set;
        }

        public Uri Uri
        {
            get
            {
                return new Uri(_riakRootUri);
            }
        }

        public RiakHttpResponse Get(Uri uri)
        {
            return Get(uri, "*/*");
        }

        public RiakHttpResponse Get(Uri uri, string accept)
        {
            return Get(uri, accept, Util.BuildListOf(HttpStatusCode.OK));
        }

        public RiakHttpResponse Get(Uri uri, ICollection<HttpStatusCode> allowedCodes)
        {
            return Get(uri, "*/*", allowedCodes);
        }

        public RiakHttpResponse Get(Uri uri, string accept, ICollection<HttpStatusCode> allowedCodes)
        {
            Dictionary<string, string> headers = null;
            if(!string.IsNullOrWhiteSpace(accept))
            {
                headers = new Dictionary<string, string>
                              {
                                  { "Accept", accept }
                              };
            }

            return Execute(
                WebRequestVerb.GET,
                uri,
                headers,
                allowedCodes,
                Stream.Null);
        }

        public RiakHttpResponse Head(Uri uri)
        {
            return Head(uri, null);
        }

        public RiakHttpResponse Head(Uri uri, ICollection<HttpStatusCode> allowedCodes)
        {
            return Execute(WebRequestVerb.HEAD, uri, null, allowedCodes, Stream.Null);
        }

        public RiakHttpResponse Put(Uri uri, string contentType, ICollection<HttpStatusCode> allowedCodes, string data)
        {
            return Execute(WebRequestVerb.PUT, 
                uri, 
                new Dictionary<string, string>{{HttpWellKnownHeader.ContentType, contentType}}, 
                allowedCodes, 
                data);
        }

        public RiakHttpResponse Put(Uri uri, string contentType, ICollection<HttpStatusCode> allowedCodes, Stream data)
        {
            return Execute(WebRequestVerb.PUT, 
                uri, 
                new Dictionary<string, string>{{HttpWellKnownHeader.ContentType, contentType}}, 
                allowedCodes, 
                data);
        }

        public RiakHttpResponse Put(Uri uri, string contentType, Dictionary<string, string> headers, ICollection<HttpStatusCode> allowedCodes, string data)
        {
            headers[HttpWellKnownHeader.ContentType] = contentType;

            return Execute(WebRequestVerb.PUT,
                uri,
                headers,
                allowedCodes,
                data);
        }

        public RiakHttpResponse Put(Uri uri, string contentType, Dictionary<string, string> headers, ICollection<HttpStatusCode> allowedCodes, byte[] data)
        {
            using(MemoryStream byteStream = new MemoryStream(data))
            {
                return Put(uri, contentType, headers, allowedCodes, byteStream);
            }
        }

        public RiakHttpResponse Put(Uri uri, string contentType, Dictionary<string, string> headers, ICollection<HttpStatusCode> allowedCodes, Stream data)
        {
            headers[HttpWellKnownHeader.ContentType] = contentType;

            return Execute(WebRequestVerb.PUT,
                uri,
                headers,
                allowedCodes,
                data);
        }

        public RiakHttpResponse Post(Uri uri, string contentType, Dictionary<string, string> headers, ICollection<HttpStatusCode> allowedCodes, Stream data)
        {
            headers[HttpWellKnownHeader.ContentType] = contentType;

            return Execute(WebRequestVerb.POST,
                uri,
                headers,
                allowedCodes,
                data);
        }

        public RiakHttpResponse Delete(Uri uri, Dictionary<string, string> headers, ICollection<HttpStatusCode> allowedCodes)
        {
            return Execute(WebRequestVerb.DELETE,
                uri,
                headers,
                allowedCodes,
                Stream.Null);
        }

        private RiakHttpResponse Execute(
            WebRequestVerb verb,
            Uri uri,
            Dictionary<string, string> headers,
            ICollection<HttpStatusCode> allowedCodes,
            string requestData)
        {
            if (string.IsNullOrEmpty(requestData))
            {
                return Execute(verb, uri, headers, allowedCodes, Stream.Null);
            }
         
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(requestData.ToCharArray()), false))
            {
                return Execute(verb, uri, headers, allowedCodes, stream);
            }
        }

        private RiakHttpResponse Execute(
            WebRequestVerb verb,
            Uri uri,
            Dictionary<string, string> headers,
            ICollection<HttpStatusCode> allowedCodes,
            Stream requestDataStream)
        {
            if (allowedCodes == null)
            {
                throw new ArgumentNullException("allowedCodes");
            }

            if (allowedCodes.Count == 0)
            {
                throw new ArgumentException("At least one allow HTTP response code must be provided", "allowedCodes");
            }

            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            RiakHttpRequest req = RiakHttpRequest.Create(verb, uri);
            foreach (var pair in DefaultHeaders)
            {
                req.AddHeader(pair.Key, pair.Value);
            }

            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    req.AddHeader(pair.Key, pair.Value);
                }
            }

            if (requestDataStream != null && requestDataStream != Stream.Null)
            {
                using (Stream requestStream = req.GetRequestStream())
                {
                    Util.CopyStream(requestDataStream, requestStream);
                }
            }

            RiakHttpResponse response = req.GetResponse();

            if (!allowedCodes.Contains(response.StatusCode))
            {
                var exception = new RiakServerException(response,
                                                        "HTTP error {0} performing {1} request at {2}",
                                                        response.StatusCode,
                                                        verb,
                                                        uri.AbsoluteUri);

                    using(StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        Trace.TraceError("HTTP error {0} performing {1} request at {2}", 
                            response.StatusCode, verb, uri.AbsoluteUri);

                        foreach(string key in response.Headers.Keys)
                        {
                            Trace.TraceError("{0}: {1}", key, response.Headers[key]);
                        }

                        if (response.ContentLength > 0)
                        {
                            Trace.TraceError(sr.ReadToEnd());
                        }
                    }

                response.Dispose();
                throw exception;
            }

            return response;
        }

        public Uri BuildUri(string bucket, string key, Dictionary<string,string> parameters)
        {
            if (string.IsNullOrEmpty(bucket))
            {
                throw new ArgumentNullException("bucket");
            }

            StringBuilder fullUri = new StringBuilder(_riakRootUri);
            
            if(!string.IsNullOrEmpty(bucket))
            {
                fullUri.AppendFormat("/{0}", Uri.EscapeDataString(bucket));
            }

            if (!string.IsNullOrEmpty(key))
            {
                fullUri.AppendFormat("/{0}", Uri.EscapeDataString(key));
            }

            if (parameters != null && parameters.Count > 0)
            {
                fullUri.Append("?");
                bool ampNeeded = false;

                foreach(var argument in parameters)
                {
                    if(ampNeeded)
                    {
                        fullUri.Append("&");
                    }

                    fullUri.AppendFormat("{0}={1}",
                                         Uri.EscapeDataString(argument.Key),
                                         Uri.EscapeDataString(argument.Value));
                    ampNeeded = true;

                }
            }

            return new Uri(fullUri.ToString(), UriKind.Absolute);
        }
    }
}
