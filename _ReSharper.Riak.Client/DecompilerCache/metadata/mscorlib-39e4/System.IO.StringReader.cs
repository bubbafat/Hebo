// Type: System.IO.StringReader
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30128\mscorlib.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.IO
{
    [ComVisible(true)]
    [Serializable]
    public class StringReader : TextReader
    {
        [SecuritySafeCritical]
        public StringReader(string s);

        public override void Close();
        protected override void Dispose(bool disposing);

        [SecuritySafeCritical]
        public override int Peek();

        [SecuritySafeCritical]
        public override int Read();

        public override int Read([In, Out] char[] buffer, int index, int count);
        public override string ReadToEnd();

        [SecuritySafeCritical]
        public override string ReadLine();
    }
}
