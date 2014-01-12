using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    class SubscriptionManager
    {
        private Category parentCategory;
        private List<Subscription> subscriptionList;

        #region Constructors

        public SubscriptionManager(Category parentCategory)
        {
            this.parentCategory = parentCategory;
            this.subscriptionList = new List<Subscription>();
        }

        #endregion

        #region Properties

        public List<Subscription> SubscriptionList
        {
            get { return this.subscriptionList; }
        }

        #endregion

        private void Load()
        {
            // TODO

            // TEMP
            //this.subscriptionList.Add(new Subscription("http://www.pcinpact.com/rss/news.xml"));
        }


        public Subscription Add(string url)
        {
            // TODO: Check URL
            Subscription subscription = new Subscription(this.parentCategory, url);

            this.subscriptionList.Add(subscription);
            subscription.LoadFeed();

            return subscription;
        }

        public void Remove(string url)
        {
            // Search subscription by URL
            Subscription subscription = this.subscriptionList.Find(x => x.Url.Equals(url));

            // Remove subscription from list
            subscriptionList.Remove(subscription);
        }

    }
}
