using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Riak.Client
{
    public class RiakException : Exception
    {
        public RiakException()
        {
        }
 
        public RiakException(String message)
            : base(message)
        {
        }

        public RiakException(String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }
 
        public RiakException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RiakException(Exception innerException, String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args), innerException)
        {
        }
  
        protected RiakException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class RiakServerException : RiakException
    {
        private HttpStatusCode _statusCode;

        public RiakServerException()
        {
        }

        public RiakServerException(String message)
            : base(message)
        {
        }

        public RiakServerException(String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        public RiakServerException(RiakResponse response, String message)
            : base(message)
        {
            _statusCode = response.StatusCode;
        }

        public RiakServerException(RiakResponse response, String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
            _statusCode = response.StatusCode;
        }

        public RiakServerException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RiakServerException(Exception innerException, String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args), innerException)
        {
        }

        protected RiakServerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
