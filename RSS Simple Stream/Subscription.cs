﻿using System;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows;
using System.Text.RegularExpressions;

namespace RSS_Simple_Stream
{
    public class Subscription
    {
        private int id;
        private string url;
        private SubscriptionManager manager;

        private volatile string title;
        private volatile string description;
        private volatile ItemManager itemManager;

        private event EventHandler progressUpdate;

        public const string UNDEFINED = "undefined";

        #region Constructors

        public Subscription(int id, SubscriptionManager manager, string feedUrl)
        {
            this.id = id;
            this.manager = manager;
            this.url = feedUrl;
            this.itemManager = new ItemManager(this);
        }

        #endregion

        #region Properties

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public SubscriptionManager Manager
        {
            get { return this.manager; }
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

                    this.title = Regex.Replace(this.title.Trim(), @"\t|\n|\r", "");
                    this.description = Regex.Replace(this.description.Trim(), @"\t|\n|\r", "");

                    ParseItems(xmlDocument);
                }

                if (progressUpdate != null)
                    progressUpdate(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                // When error occurred with database loading
                String error = "The following error has occurred during loading of subscription " + this.url + ":\n\n" + e.Message.ToString() + "\n\n";
                MessageBox.Show(error);

                this.title = this.url;
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

                item.Title = Regex.Replace(item.Title.Trim(), @"\t|\n|\r", "");
                item.Description = Regex.Replace(item.Description.Trim(), @"\t|\n|\r", "");
                item.Link = Regex.Replace(item.Link.Trim(), @"\t|\n|\r", "");

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
