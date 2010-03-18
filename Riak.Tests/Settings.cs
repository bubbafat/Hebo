using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riak.Tests
{
    internal static class Settings
    {
        public static Uri RiakServerUri
        {
            get
            {
                return new Uri("http://192.168.137.50:8098/riak");
            }
        }
    }
}
