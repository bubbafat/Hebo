// Type: System.IO.TextReader
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30128\mscorlib.dll

using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace System.IO
{
    [ComVisible(true)]
    [Serializable]
    public abstract class TextReader : MarshalByRefObject, IDisposable
    {
        public static readonly TextReader Null;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected TextReader();

        #region IDisposable Members

        public void Dispose();

        #endregion

        public virtual void Close();
        protected virtual void Dispose(bool disposing);
        public virtual int Peek();
        public virtual int Read();
        public virtual int Read([In, Out] char[] buffer, int index, int count);
        public virtual string ReadToEnd();
        public virtual int ReadBlock([In, Out] char[] buffer, int index, int count);
        public virtual string ReadLine();
        public static TextReader Synchronized(TextReader reader);
    }
}
