using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Riak.Client
{
    public class Document
    {
        protected Document(Dictionary<string,string> headers, byte[] content, Document parent)
        {
            Headers = headers;
            Content = content;
            Parent = parent;
        }

        public Dictionary<string, string> Headers
        {
            get; protected set;
        }

        public byte[] Content
        {
            get; protected set;
        }

        public Document Parent
        {
            get; private set;
        }

        public static Document Load(RiakHttpResponse response)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
                                                     {
                                                         { HttpWellKnownHeader.ContentLength, response.ContentLength.ToString() },
                                                         { HttpWellKnownHeader.ContentType, response.ContentType },
                                                     };

            foreach(string headerKey in response.Headers.AllKeys)
            {
                headers[headerKey] = response.Headers[headerKey];
            }

            return Load(response.GetResponseStream(), headers, null);
        }

        public static Document Load(Stream stream)
        {
            return Load(stream, null);
        }

        protected static Document Load(Stream stream, Dictionary<string,string> headers, Document parent)
        {
            // if the header indicated a content length then respect that otherwise
            // read everything left over.
            long contentLength = headers.ContainsKey(HttpWellKnownHeader.ContentLength)
                                     ? long.Parse(headers[HttpWellKnownHeader.ContentLength])
                                     : Math.Max(0, stream.Length - stream.Position);

            byte[] content = new byte[contentLength];

            Util.CopyStream(stream, content);

            Document doc = Util.IsMultiPart(headers[HttpWellKnownHeader.ContentType])
                               ? new MultiPartDocument(headers, content, parent)
                               : new Document(headers, content, parent);

            return doc;            
        }

        protected static Document Load(Stream stream, Document parent)
        {
            Dictionary<string, string> headers = LoadHeadersFromStream(stream);
            return Load(stream, headers, parent);
        }

        private static Dictionary<string, string> LoadHeadersFromStream(Stream stream)
        {
            List<byte> bytes = new List<byte>();
            bool headerBoundaryFound = false;

            while (true)
            {
                int read = stream.ReadByte();
                if (read == -1)
                {
                    break;
                }

                bytes.Add((byte) read);

                if(read == '\r')
                {
                    // ignore to handle CR/LF or just LF
                }
                else if (read == '\n')
                {
                    if (headerBoundaryFound)
                    {
                        break;
                    }

                    headerBoundaryFound = true;
                }
                else
                {
                    headerBoundaryFound = false;
                }
            }

            Dictionary<string, string> headers = new Dictionary<string, string>();

            // CodePage 28591 is iso-8859-1 ISO 8859-1 Latin 1; Western European (ISO)
            using (MemoryStream ms = new MemoryStream(bytes.ToArray()))
            using (StreamReader reader = new StreamReader(ms, Encoding.GetEncoding(28591)))
            {
                while (!reader.EndOfStream)
                {
                    string headerline = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerline))
                    {
                        continue;
                    }

                    string[] headerValues = headerline.Split(new[] {':'}, 2);
                    headers[headerValues[0]] = headerValues[1].Trim();
                }
            }

            return headers;
        }

        public string GetLocalOrParentHeader(string name)
        {
            string value;

            if(!Headers.TryGetValue(name, out value))
            {
                return Parent != null 
                    ? Parent.GetLocalOrParentHeader(name) 
                    : null;
            }

            return value;
        }

        internal string Dump()
        {
            StringBuilder sb = new StringBuilder();
            foreach(string key in Headers.Keys)
            {
                sb.AppendFormat("{0}: {1}{2}", key, Headers[key], Environment.NewLine);
            }

            sb.Append(Environment.NewLine);

            sb.Append(Encoding.GetEncoding(28591).GetString(Content));

            return sb.ToString();
        }
    }

    public class MultiPartDocument : Document
    {
        private const string BoundaryPrefix = "\n--";
        private readonly string _boundary;
        private readonly byte[] _content;
        private readonly byte[] _boundaryBytes;

        private int _currentIndex;
        private bool _terminatingBoundary;

        public MultiPartDocument(Dictionary<string,string> headers, byte[] content, Document parent)
            : base(headers, content, parent)
        {
            _content = content;
            _boundary = LoadBoundary();
            _boundaryBytes = Encoding.GetEncoding(28591).GetBytes(BoundaryPrefix + _boundary);
            LoadParts();
        }

        public List<Document> Parts
        {
            get; private set;
        }

        private void LoadParts()
        {
            Parts = new List<Document>();

            // read through the first boundary marker
            ReadUntilBoundary();

            while(true)
            {
                if(_terminatingBoundary)
                {
                    break;
                }

                byte[] part = ReadUntilBoundary();
                using (MemoryStream stream = new MemoryStream(part))
                {
                    Parts.Add(Load(stream, this));
                }
            }
        }

        private byte[] ReadUntilBoundary()
        {
            int start = _currentIndex;
            int localCurrent = _currentIndex;
            int localEnd = _content.Length - _boundaryBytes.Length;

            while (localCurrent < localEnd)
            {
                int immediateIndex = localCurrent;

                bool boundaryFound = true;

                for (int i = 0; i < _boundaryBytes.Length; i++)
                {
                    if (localCurrent + i > _content.Length)
                    {
                        boundaryFound = false;
                        Trace.TraceError("Unexpected stream end found in multipart document.");
                        break;
                    }

                    if (_content[localCurrent + i] != _boundaryBytes[i])
                    {
                        boundaryFound = false;
                        break;
                    }

                    immediateIndex++;
                }

                if (boundaryFound)
                {
                    int partLength = immediateIndex - start - _boundaryBytes.Length;
                    // don't return the boundary marker
                    Debug.Assert(partLength >= 0);
                    byte[] part = new byte[partLength];
                    Array.Copy(_content, start, part, 0, partLength);

                    if (_content[immediateIndex] == '\n')
                    {
                        // we're at a non-terminating boundary
                        _currentIndex = immediateIndex + 1; // skip past the one we just read.
                    }
                    else if (_content[immediateIndex] == '-' && _content[immediateIndex + 1] == '-')
                    {
                        _terminatingBoundary = true;
                        _currentIndex = immediateIndex + 1; // skip past the one we just read.
                    }
                    else
                    {
                        _currentIndex = immediateIndex + 1; // skip past the one we just read.  

                        Debug.Fail(
                            "The boundary marker was found but did not terminate with a newline or -- : this is malformed, but we'll try to work with it.");
                    }

                    return part;
                }

                localCurrent++;
            }

            throw new RiakServerException("The returned multipart document was malformed.");
        }

        private string LoadBoundary()
        {
            string boundaryString = null;

            const string boundaryMark = "boundary=";

            string contentTypeHeader = Headers[HttpWellKnownHeader.ContentType];

            string[] parts = contentTypeHeader.Split(';');
            foreach(string part in parts)
            {
                string trimmed = part.Trim();
                if (trimmed.StartsWith(boundaryMark))
                {
                    if (trimmed.Length > boundaryMark.Length)
                    {
                        boundaryString = trimmed.Substring("boundary=".Length);
                    }
                    else
                    {
                        throw new RiakServerException("The Content-Type header boundary marker was malformed: {0}", contentTypeHeader);
                    }
                }
            }

            if (string.IsNullOrEmpty(boundaryString))
            {
                throw new RiakServerException("The Content-Type header was missing or empty.");
            }

            return boundaryString;
        }
    }
}
