using System;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    class RssManager
    {
        private string url;
        private string feedTitle;
        private string feedDescription;
        private Collection<RssItem> feedItems = new Collection<RssItem>();

#region Constructors

        /// <summary>
        /// Empty constructor, allowing us to
        /// instantiate our class and set our
        /// url variable to an empty string
        /// </summary>
        public RssManager() : this(string.Empty)
        {
            
        }

        /// <summary>
        /// Constructor allowing us to instantiate our class
        /// and set the _url variable to a value
        /// </summary>
        /// <param name="feedUrl">The URL of the Rss feed</param>
        public RssManager(string feedUrl)
        {
            this.url = feedUrl;
        }

#endregion

#region Properties

        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }

        public Collection<RssItem> FeedItems
        {
            get { return this.feedItems; }
        }

        public string FeedTitle
        {
            get { return this.feedTitle; }
        }

        public string FeedDescription
        {
            get { return this.feedDescription; }
        }

#endregion

        public void loadFeed()
        {
            if (String.IsNullOrEmpty(this.url))
            {
                throw new ArgumentException("URL must be provided");
            }

            using (XmlReader reader = XmlReader.Create(this.url))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);
                
                this.feedTitle = ParseElements(xmlDocument.SelectSingleNode("//channel"), "title");
                this.feedDescription = ParseElements(xmlDocument.SelectSingleNode("//channel"), "description");

                ParseItems(xmlDocument);
            }
        }

        private void ParseItems(XmlDocument xmlDocument)
        {
            this.feedItems.Clear();

            XmlNodeList nodes = xmlDocument.SelectNodes("rss/channel/item");

            foreach (XmlNode node in nodes)
            {
                RssItem item = new RssItem();
                item.Title = ParseElements(node, "title");
                item.Description = ParseElements(node, "description");
                item.Link = ParseElements(node, "link");

                DateTime date;
                DateTime.TryParse(ParseElements(node, "pubDate"), out date);
                item.Date = date;

                feedItems.Add(item);
            }
        }

        private string ParseElements(XmlNode parent, string path)
        {
            XmlNode node = parent.SelectSingleNode(path);
            
            if (node != null)
            {
                return node.InnerText;
            }
            else
            {
                return "Undefined";
            }
        }

        public override string ToString()
        {
            string toString = base.ToString();

            foreach(RssItem rssItem in this.feedItems)
            {
                toString += rssItem;
            }

            return toString;
        }
    }
}
