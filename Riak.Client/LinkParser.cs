﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * This class parses Link headers (loosely) based on the draft spec
 * http://tools.ietf.org/html/draft-nottingham-http-link-header-06
 */

namespace Riak.Client
{
    public class LinkParser
    {
        private Stack<char> _chars;

        public LinkCollection Parse(string linkString)
        {
            LinkCollection links = new LinkCollection();

            if (!string.IsNullOrEmpty(linkString))
            {
                _chars = new Stack<char>(linkString.Reverse());

                while (CouldBeALink())
                {
                    SkipWhiteSpaceAndAnother(',');

                    Link link = new Link();
                    DemandRelationalUri(link);
                    SkipWhiteSpaceAndAnother(';');

                    while (!AtLinkEdge())
                    {                        
                        if (!RequestParameter(link))
                        {
                            break;
                        }

                        SkipWhiteSpaceAndAnother(';');
                    }

                    links.Add(link);

                    SkipWhiteSpaceAndAnother(',');
                }
            }

            return links;
        }

        private bool AtLinkEdge()
        {
            SkipWhiteSpace();
            return Eof() || Peek() == ',';
        }

        private void SkipWhiteSpaceAndAnother(params char[] another)
        {
            while(!Eof())
            {
                char c = Peek();
                if(char.IsWhiteSpace(c) || another.Contains(c))
                {
                    ReadChar();
                }
                else
                {
                    break;
                }
            }
        }

        private bool CouldBeALink()
        {
            return _chars.Count > 0;
        }

        private bool RequestParameter(Link link)
        {
            if (!Eof())
            {
                // if we ever need to support non-quoted attributes (e.g. title*) fix this method.
                string key = ReadUntil('=');
                Skip(); // skip the '='
                SkipWhiteSpaceAndAnother('"'); // read past the opening "
                string value = ReadUntil('"'); // read the value;
                Skip(); // skip the '"'
                SkipWhiteSpaceAndAnother(';');

                switch(key)
                {
                    case "rel":
                        link.Rel = value;
                        break;
                    case "riaktag":
                        link.RiakTag = value;
                        break;
                    default:
                        link.UnknownParameters.Add(key, value);
                        break;
                }

                return true;
            }

            return false;
        }

        private void DemandRelationalUri(Link link)
        {
            SkipWhiteSpace();
            DemandChar('<');
            Skip();
            link.UriResource = ReadUntil('>');
            DemandChar('>');
            Skip();
        }

        private string ReadUntil(params char[] chars)
        {
            StringBuilder data = new StringBuilder();
            while(!Eof() && !chars.Contains(Peek()))
            {
                data.Append(ReadChar());
            }

            return data.ToString();
        }

        private void DemandChar(char demand)
        {
            char peek = Peek();
            if(peek != demand)
            {
                throw new RiakClientException("Parser error.  Expected \'{0}\' but encountered \'{1}\'.", demand, peek);
            }
        }

        private void Skip()
        {
            if(Eof())
            {
                throw new RiakClientException("Parser error: Unexpected end of stream.");
            }

            ReadChar();
        }

        private void SkipWhiteSpace()
        {
            while(!Eof())
            {
                if(char.IsWhiteSpace(Peek()))
                {
                    ReadChar();
                }
                else
                {
                    break;
                }
            }
        }

        private char ReadChar()
        {
            return _chars.Pop();
        }

        private char Peek()
        {
            return _chars.Peek();
        }

        private bool Eof()
        {
            return _chars.Count == 0;
        }
    }
}