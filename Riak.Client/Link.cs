using System.Collections.Generic;
using System.Text;

namespace Riak.Client
{
    public class Link
    {
        public Link()
        {
            Parameters = new List<string>();
        }

        public string UriResource
        {
            get;
            set;
        }

        public List<string> Parameters
        {
            get;
            private set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<{0}>", UriResource);
            foreach(string p in Parameters)
            {
                sb.AppendFormat("; {0}", p);
            }

            return sb.ToString();
        }
    }
}
