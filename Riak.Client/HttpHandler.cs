using System;
using System.Collections.Generic;
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

        public RiakResponse Get(Uri uri)
        {
            return Get(uri, "*/*");
        }

        public RiakResponse Get(Uri uri, string accept)
        {
            return Get(uri, accept, new List<HttpStatusCode> {HttpStatusCode.OK});
        }

        public RiakResponse Get(Uri uri, IList<HttpStatusCode> allowedCodes)
        {
            return Get(uri, "*/*", allowedCodes);
        }

        public RiakResponse Get(Uri uri, string accept, IList<HttpStatusCode> allowedCodes)
        {
            return Execute(
                WebRequestVerb.GET,
                uri,
                null,
                allowedCodes,
                Stream.Null);
        }

        public RiakResponse Put(Uri uri, string contentType, IList<HttpStatusCode> allowedCodes, string data)
        {
            return Execute(WebRequestVerb.PUT, 
                uri, 
                new Dictionary<string, string>{{HttpWellKnownHeader.ContentType, contentType}}, 
                allowedCodes, 
                data);
        }

        public RiakResponse Put(Uri uri, string contentType, IList<HttpStatusCode> allowedCodes, Stream data)
        {
            return Execute(WebRequestVerb.PUT, 
                uri, 
                new Dictionary<string, string>{{HttpWellKnownHeader.ContentType, contentType}}, 
                allowedCodes, 
                data);
        }

        public RiakResponse Put(Uri uri, string contentType, Dictionary<string, string> headers, IList<HttpStatusCode> allowedCodes, string data)
        {
            headers[HttpWellKnownHeader.ContentType] = contentType;

            return Execute(WebRequestVerb.PUT,
                uri,
                headers,
                allowedCodes,
                data);
        }

        public RiakResponse Put(Uri uri, string contentType, Dictionary<string, string> headers, IList<HttpStatusCode> allowedCodes, Stream data)
        {
            headers[HttpWellKnownHeader.ContentType] = contentType;

            return Execute(WebRequestVerb.PUT,
                uri,
                headers,
                allowedCodes,
                data);
        }


        public RiakResponse Delete(Uri uri, Dictionary<string, string> headers, IList<HttpStatusCode> allowedCodes)
        {
            return Execute(WebRequestVerb.DELETE,
                uri,
                headers,
                allowedCodes,
                Stream.Null);
        }

        public RiakResponse Execute(
            WebRequestVerb verb,
            Uri uri,
            Dictionary<string, string> headers,
            IList<HttpStatusCode> allowedCodes,
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

        public RiakResponse Execute(
            WebRequestVerb verb, 
            Uri uri, 
            Dictionary<string,string> headers,
            IList<HttpStatusCode> allowedCodes,
            Stream requestDataStream)
        {
            if(allowedCodes == null)
            {
                throw new ArgumentNullException("allowedCodes");
            }
            
            if(allowedCodes.Count == 0)
            {
                throw new ArgumentException("At least one allow HTTP response code must be provided", "allowedCodes");
            }

            if(uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            using(RiakRequest req = RiakRequest.Create(verb, uri))
            {
                foreach(var pair in DefaultHeaders)
                {
                    req.AddHeader(pair.Key, pair.Value);
                }

                if(headers != null)
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
                        CopyStream(requestDataStream, requestStream);
                    }
                }

                RiakResponse response = req.GetResponse();

                if(!allowedCodes.Contains(response.StatusCode))
                {
                    var exception = new RiakServerException(response, 
                        "HTTP error {0} performing {1} request at {2}",
                        response.StatusCode,
                        verb,
                        uri.AbsoluteUri);

                    response.Dispose();

                    throw exception;
                }

                return response;
            }
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
                fullUri.AppendFormat("/{0}", Uri.EscapeUriString(bucket));
            }

            if (!string.IsNullOrEmpty(key))
            {
                fullUri.AppendFormat("/{0}", Uri.EscapeUriString(key));
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
                                         Uri.EscapeUriString(argument.Key),
                                         Uri.EscapeUriString(argument.Value));
                    ampNeeded = true;

                }
            }

            return new Uri(fullUri.ToString(), UriKind.Absolute);
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
        }
    }
}
