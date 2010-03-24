using System;

namespace Riak.Tests
{
    internal static class Settings
    {
        public static Uri RiakServerUri
        {
            get
            {
                // return new Uri("http://192.168.137.50:8098/riak");
                return new Uri("http://ec2-184-73-32-105.compute-1.amazonaws.com:8098/riak");
            }
        }
    }
}
