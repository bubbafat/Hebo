using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Riak.Client
{
    public static class Util
    {
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

        public static int CopyStream(Stream input, byte[] output)
        {
            int currentIndex = 0;
            while (true)
            {
                int remaining = output.Length - currentIndex;
                int maxToRead = Math.Min(remaining, 32678);

                int read = input.Read(output, currentIndex, maxToRead);

                if (read == 0)
                {
                    break;
                }

                currentIndex += read;
            }

            return currentIndex;
        }

        public static string ReadString(byte[] bytes)
        {
            return ReadString(bytes, Encoding.UTF8);
        }

        public static string ReadString(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }

        public static ICollection<T> BuildListOf<T>(params T[] codes)
        {
            return codes;
        }

        public static bool IsMultiPart(string headerValue)
        {
            return headerValue.Trim().StartsWith("multipart/");
        }
    }
}
