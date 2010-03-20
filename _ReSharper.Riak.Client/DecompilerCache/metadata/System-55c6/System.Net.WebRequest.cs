// Type: System.Net.WebRequest
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.dll

using System;
using System.IO;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security.Principal;

namespace System.Net
{
    [Serializable]
    public abstract class WebRequest : MarshalByRefObject, ISerializable
    {
        protected WebRequest();

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected WebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext);

        public static RequestCachePolicy DefaultCachePolicy { get; set; }

        public virtual RequestCachePolicy CachePolicy { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public virtual string Method { get; set; }
        public virtual Uri RequestUri { get; }
        public virtual string ConnectionGroupName { get; set; }
        public virtual WebHeaderCollection Headers { get; set; }
        public virtual long ContentLength { get; set; }
        public virtual string ContentType { get; set; }
        public virtual ICredentials Credentials { get; set; }
        public virtual bool UseDefaultCredentials { get; set; }
        public virtual IWebProxy Proxy { get; set; }
        public virtual bool PreAuthenticate { get; set; }
        public virtual int Timeout { get; set; }

        public AuthenticationLevel AuthenticationLevel { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public TokenImpersonationLevel ImpersonationLevel { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public static IWebProxy DefaultWebProxy { get; set; }

        #region ISerializable Members

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext);

        #endregion

        public static WebRequest Create(string requestUriString);
        public static WebRequest Create(Uri requestUri);
        public static WebRequest CreateDefault(Uri requestUri);
        public static bool RegisterPrefix(string prefix, IWebRequestCreate creator);
        protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext);
        public virtual Stream GetRequestStream();
        public virtual WebResponse GetResponse();
        public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state);
        public virtual WebResponse EndGetResponse(IAsyncResult asyncResult);
        public virtual IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state);
        public virtual Stream EndGetRequestStream(IAsyncResult asyncResult);
        public virtual void Abort();
        public static IWebProxy GetSystemWebProxy();
    }
}
