using System.Collections.Specialized;
using System.Text;

namespace Riak.Client
{
    public class Link
    {
        public Link()
        {
            UnknownParameters = new NameValueCollection();
        }

        public string UriResource { get; set; }

        public string Rel { get; set; }

        public string RiakTag { get; set; }

        public NameValueCollection UnknownParameters { get; private set; }

        public bool ShouldWrite
        {
            get
            {
                // if we have something other than '</>; rel="up"' then write it.
                return (Rel != "up" ||
                        RiakTag != null ||
                        UnknownParameters.Count > 0);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<{0}>", UriResource);
            foreach (string key in UnknownParameters.Keys)
            {
                sb.AppendFormat("; {0}=\"{1}\"", key, UnknownParameters[key]);
            }

            if (!string.IsNullOrEmpty(Rel))
            {
                sb.AppendFormat("; rel=\"{0}\"", Rel);
            }

            if (!string.IsNullOrEmpty(RiakTag))
            {
                sb.AppendFormat("; riaktag=\"{0}\"", RiakTag);
            }

            return sb.ToString();
        }
    }
}
