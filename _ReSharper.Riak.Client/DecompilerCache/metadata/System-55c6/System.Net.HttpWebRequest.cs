// Type: System.Net.HttpWebRequest
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.dll

using System;
using System.IO;
using System.Net.Cache;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace System.Net
{
    [Serializable]
    public class HttpWebRequest : WebRequest, ISerializable
    {
        [Obsolete("Serialization is obsoleted for this type.  http://go.microsoft.com/fwlink/?linkid=14202")]
        protected HttpWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext);

        public bool AllowAutoRedirect { get; set; }
        public bool AllowWriteStreamBuffering { get; set; }
        public bool HaveResponse { get; }

        public bool KeepAlive { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public bool Pipelined { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public override bool PreAuthenticate { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public bool UnsafeAuthenticatedConnectionSharing { get; set; }
        public bool SendChunked { get; set; }

        public DecompressionMethods AutomaticDecompression { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; set; }

        public new static RequestCachePolicy DefaultCachePolicy { get; set; }
        public static int DefaultMaximumResponseHeadersLength { get; set; }
        public static int DefaultMaximumErrorResponseLength { get; set; }

        public int MaximumResponseHeadersLength { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; set; }

        public X509CertificateCollection ClientCertificates { get; set; }

        public CookieContainer CookieContainer { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public override Uri RequestUri { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; }

        public override long ContentLength { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; set; }

        public override int Timeout { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; set; }

        public int ReadWriteTimeout { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; set; }

        public Uri Address { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; }

        public HttpContinueDelegate ContinueDelegate { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public ServicePoint ServicePoint { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; }

        public string Host { get; set; }

        public int MaximumAutomaticRedirections { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; set; }

        public override string Method { get; set; }

        public override ICredentials Credentials { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public override bool UseDefaultCredentials { get; set; }

        public override string ConnectionGroupName { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public override WebHeaderCollection Headers { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; set; }

        public override IWebProxy Proxy { get; set; }
        public Version ProtocolVersion { get; set; }

        public override string ContentType { get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public string MediaType { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public string TransferEncoding { get; set; }
        public string Connection { get; set; }

        public string Accept { get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public string Referer { get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public string UserAgent { get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public string Expect { get; set; }

        public DateTime IfModifiedSince { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        public DateTime Date { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext);

        #endregion

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state);
        public override Stream EndGetRequestStream(IAsyncResult asyncResult);
        public Stream EndGetRequestStream(IAsyncResult asyncResult, out TransportContext context);
        public override Stream GetRequestStream();
        public Stream GetRequestStream(out TransportContext context);
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state);
        public override WebResponse EndGetResponse(IAsyncResult asyncResult);
        public override WebResponse GetResponse();

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public override void Abort();

        protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext);
        public void AddRange(int from, int to);

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void AddRange(long from, long to);

        public void AddRange(int range);

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void AddRange(long range);

        public void AddRange(string rangeSpecifier, int from, int to);
        public void AddRange(string rangeSpecifier, long from, long to);
        public void AddRange(string rangeSpecifier, int range);
        public void AddRange(string rangeSpecifier, long range);
    }
}
