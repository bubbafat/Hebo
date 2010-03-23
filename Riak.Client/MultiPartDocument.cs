using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Riak.Client
{
    public class Document
    {
        protected Document(Dictionary<string,string> headers, byte[] content)
        {
            Headers = headers;
            Content = content;
        }

        public Dictionary<string, string> Headers
        {
            get; protected set;
        }

        public byte[] Content
        {
            get; protected set;
        }

        public static Document Load(Stream stream)
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
                while (true)
                {
                    string headerline = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerline))
                    {
                        break;
                    }

                    string[] headerValues = headerline.Split(new char[] {':'}, 2);
                    headers[headerValues[0]] = headerValues[1].Trim();
                }
            }

            // if the header indicated a content length then respect that otherwise
            // read everything left over.
            long contentLength = headers.ContainsKey(HttpWellKnownHeader.ContentLength)
                                     ? long.Parse(headers[HttpWellKnownHeader.ContentLength])
                                     : Math.Max(0, stream.Length - stream.Position);

            byte[] content = new byte[contentLength];

            Util.CopyStream(stream, content);

            Document doc = Util.IsMultiPart(headers[HttpWellKnownHeader.ContentType])
                               ? new MultiPartDocument(headers, content)
                               : new Document(headers, content);

            return doc;
        }
    }

    public class MultiPartDocument : Document
    {
        private const string BoundaryPrefix = "--";
        private const char BoundaryPostfix = '\n';
        private string _boundary;
        private byte[] _content;
        private byte[] _boundaryBytes;

        private int _currentIndex;
        private bool _terminatingBoundary;

        public MultiPartDocument(Dictionary<string,string> headers, byte[] content)
            : base(headers, content)
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
                    Parts.Add(Document.Load(stream));
                }
            }
        }

        private byte[] ReadUntilBoundary()
        {
            int start = _currentIndex;
            int localCurrent = _currentIndex;
            int localEnd = _content.Length - _boundaryBytes.Length;
            bool boundaryFound = false;

            while(localCurrent < localEnd)
            {
                int immediateIndex = localCurrent;

                boundaryFound = true;
//                int index = Array.IndexOf(_content, _boundaryBytes[0], localCurrent);

                for(int i = 0; i < _boundaryBytes.Length; i++)
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

                if(boundaryFound)
                {
                    int partLength = immediateIndex - start - _boundaryBytes.Length;
                    // don't return the boundary marker
                    Debug.Assert(partLength >= 0);
                    byte[] part = new byte[partLength];
                    Array.Copy(_content, start, part, 0, partLength);

                    if (_content[immediateIndex ] == '\n')
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
                        Debug.Fail(
                            "The boundary marker was found but did not terminate with a newline or -- : this is malformed, but we'll try to work with it.");
                        _currentIndex = immediateIndex + 1; // skip past the one we just read.                        
                    }

                    return part;
                }
                else
                {
                    localCurrent++;                    
                }
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
