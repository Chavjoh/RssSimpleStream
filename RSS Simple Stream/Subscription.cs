using System;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows;

namespace RSS_Simple_Stream
{
    class Subscription
    {
        private string url;
        private volatile string title;
        private volatile string description;
        private volatile ItemManager itemManager;
        private Category parentCategory;
        private event EventHandler progressUpdate;

        public const string UNDEFINED = "undefined";

        #region Constructors

        public Subscription(Category parentCategory, string feedUrl)
        {
            this.parentCategory = parentCategory;
            this.url = feedUrl;
            this.itemManager = new ItemManager(parentCategory);
        }

        #endregion

        #region Properties

        public Category Parent
        {
            get { return this.parentCategory; }
        }

        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }

        public ItemManager ItemManager
        {
            get { return this.itemManager; }
        }

        public string Title
        {
            get { return this.title; }
        }

        public string Description
        {
            get { return this.description; }
        }

        public EventHandler ProgressUpdate
        {
            get { return this.progressUpdate; }
            set { this.progressUpdate = value; }
        }

        #endregion

        public void LoadFeed()
        {
            if (String.IsNullOrEmpty(this.url))
            {
                throw new ArgumentException("URL must be provided");
            }

            try
            {
                using (XmlReader reader = XmlReader.Create(this.url))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(reader);

                    this.title = ParseElements(xmlDocument.SelectSingleNode("//channel"), "title");
                    this.description = ParseElements(xmlDocument.SelectSingleNode("//channel"), "description");

                    ParseItems(xmlDocument);
                }

                if (progressUpdate != null)
                    progressUpdate(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                // When error occurred with database loading
                String error = "The following error has occurred:\n\n";
                error += e.Message.ToString() + "\n\n";
                MessageBox.Show(error);
            }
        }

        private void ParseItems(XmlDocument xmlDocument)
        {
            this.itemManager.ItemList.Clear();

            XmlNodeList nodes = xmlDocument.SelectNodes("rss/channel/item");

            foreach (XmlNode node in nodes)
            {
                Item item = new Item();
                item.Title = ParseElements(node, "title");
                item.Description = ParseElements(node, "description");
                item.Link = ParseElements(node, "link");

                DateTime date;
                DateTime.TryParse(ParseElements(node, "pubDate"), out date);
                item.Date = date;

                string guid = ParseElements(node, "guid");
                if (guid.Equals(UNDEFINED))
                {
                    guid = Tools.ComputeHash(item.ToString(), new SHA256CryptoServiceProvider());
                }

                this.itemManager.ItemList.Add(item);
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
                return UNDEFINED;
            }
        }

        public override string ToString()
        {
            string toString = base.ToString();

            foreach(Item rssItem in this.itemManager.ItemList)
            {
                toString += rssItem;
            }

            return toString;
        }
    }
}
