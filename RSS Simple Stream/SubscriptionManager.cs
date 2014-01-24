using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    public class SubscriptionManager
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

        public Category Parent
        {
            get { return this.parentCategory; }
        }

        public List<Subscription> SubscriptionList
        {
            get { return this.subscriptionList; }
        }

        #endregion

        public Subscription AddToList(int id, string url)
        {
            Subscription subscription = new Subscription(id, this, url);

            this.subscriptionList.Add(subscription);

            return subscription;
        }

        public void RemoveFromList(string url)
        {
            // Search subscription by URL
            Subscription subscription = this.subscriptionList.Find(x => x.Url.Equals(url));

            // Remove subscription from list
            subscriptionList.Remove(subscription);
        }

        public Subscription Insert(string url)
        {
            // Database connexion
            SQLiteDatabase db = SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE);

            // Dictionary with column->value
            Dictionary<String, String> data = new Dictionary<String, String>();
            data.Add("id_category", this.parentCategory.Id.ToString());
            data.Add("url_subscription", url);

            try
            {
                // Insert into database, get of the row
                int id = db.Insert("subscription", data);

                // Insert element into current list
                return this.AddToList(id, url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public void Remove(Subscription subscription)
        {
            // Database connexion
            SQLiteDatabase db = SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE);

            try
            {
                db.Delete("subscription", String.Format("id_subscription = {0}", subscription.Id));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            this.subscriptionList.Remove(subscription);
        }
    }
}
