using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riak.Client
{
    public class LinkCollection : ICollection<Link>
    {
        private readonly List<Link> _links;

        public LinkCollection()
        {
            _links = new List<Link>();
        }

        public LinkCollection(IEnumerable<Link> links)
        {
            _links = new List<Link>(links);
        }

        public IEnumerator<Link> GetEnumerator()
        {
            return _links.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _links.GetEnumerator();
        }

        public void Add(Link item)
        {
            _links.Add(item);
        }

        public void Clear()
        {
            _links.Clear();
        }

        public bool Contains(Link item)
        {
            return _links.Contains(item);
        }

        public void CopyTo(Link[] array, int arrayIndex)
        {
            _links.CopyTo(array, arrayIndex);
        }

        public bool Remove(Link item)
        {
            return _links.Remove(item);
        }

        public int Count
        {
            get { return _links.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public static LinkCollection Create(string linkHeaderString)
        {
            LinkParser parser = new LinkParser();
            return parser.Parse(linkHeaderString);
        }

        public Link this[int index]
        {
            get
            {
                return _links[index];
            }
            set
            {
                _links[index] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            bool needComma = false;
            foreach (Link link in _links.Where(link => link.ShouldWrite))
            {
                if (needComma)
                {
                    sb.Append(", ");
                }

                sb.Append(link);
                needComma = true;
            }

            return sb.ToString();
        }
    }
}