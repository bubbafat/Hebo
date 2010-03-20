using System;
using System.Net;
using System.Runtime.Serialization;

namespace Riak.Client
{
    [Serializable]
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

    [Serializable]
    public class RiakClientException : RiakException
    {
        public RiakClientException()
        {
        }

        public RiakClientException(String message)
            : base(message)
        {
        }

        public RiakClientException(String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        public RiakClientException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RiakClientException(Exception innerException, String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args), innerException)
        {
        }

        protected RiakClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class RiakUnresolvedConflictException : RiakClientException
    {
        public RiakUnresolvedConflictException()
        {
        }

        public RiakUnresolvedConflictException(String message)
            : base(message)
        {
        }

        public RiakUnresolvedConflictException(String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        public RiakUnresolvedConflictException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RiakUnresolvedConflictException(Exception innerException, String messageFormat, params object[] args)
            : base(string.Format(messageFormat, args), innerException)
        {
        }

        protected RiakUnresolvedConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class RiakServerException : RiakException
    {
        private readonly HttpStatusCode _statusCode;

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

        public RiakServerException(RiakHttpResponse response, String message)
            : base(message)
        {
            _statusCode = response.StatusCode;
        }

        public RiakServerException(RiakHttpResponse response, String messageFormat, params object[] args)
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
            _statusCode = (HttpStatusCode)info.GetInt32("StatusCode");
        }

        #region ISerializable Members

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StatusCode", StatusCode);
        }

        #endregion

        public HttpStatusCode StatusCode
        {
            get
            {
                return _statusCode;
            }
        }
    }
}
