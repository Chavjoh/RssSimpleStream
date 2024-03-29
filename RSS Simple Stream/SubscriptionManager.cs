﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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

        /// <summary>
        /// Search a subscription by ID
        /// </summary>
        /// <param name="id">ID of the subscription</param>
        /// <returns>Subscription found</returns>
        public Subscription Search(int id)
        {
            return this.subscriptionList.Find(x => x.Id == id);
        }

        /// <summary>
        /// Search a subscription by URL
        /// </summary>
        /// <param name="url">URL of the subscription</param>
        /// <returns>Subscription found</returns>
        public Subscription Search(string url)
        {
            return this.subscriptionList.Find(x => x.Url.ToLower().Equals(url.ToLower()));
        }

        /// <summary>
        /// Add a subscription to the current category
        /// </summary>
        /// <param name="id">ID of the subscription</param>
        /// <param name="url">URL of the subscription</param>
        /// <returns>New subscription added</returns>
        public Subscription AddToList(int id, string url)
        {
            Subscription subscription = new Subscription(id, this, url);

            this.subscriptionList.Add(subscription);

            return subscription;
        }

        /// <summary>
        /// Remove a subscription from the current category
        /// </summary>
        /// <param name="url">URL of the subscription</param>
        public void RemoveFromList(string url)
        {
            // Search subscription by URL
            Subscription subscription = this.Search(url);

            // Remove subscription from list
            subscriptionList.Remove(subscription);
        }

        /// <summary>
        /// Insert a new subscription into the list and database
        /// </summary>
        /// <param name="url">URL of the subscription</param>
        /// <returns>New subscription added</returns>
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

        /// <summary>
        /// Delete a subscription from the list and database
        /// </summary>
        /// <param name="subscription">Subscription object to delete</param>
        public void Remove(Subscription subscription)
        {
            // Database connexion
            SQLiteDatabase db = SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE);

            try
            {
                List<SQLiteParameter> parameterList = new List<SQLiteParameter>();
                parameterList.Add(new SQLiteParameter("@id_subscription", subscription.Id));
                db.Delete("subscription", "id_subscription = @id_subscription", parameterList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Remove from the list
            this.subscriptionList.Remove(subscription);
        }
    }
}
