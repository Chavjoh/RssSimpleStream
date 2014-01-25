﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RSS_Simple_Stream
{
    public class CategoryManager
    {
        private List<Category> categoryList;
        private static CategoryManager instance = null;

        #region Constructors

        private CategoryManager()
        {
            this.categoryList = new List<Category>();
        }

        public static CategoryManager getInstance()
        {
            if (instance == null)
                instance = new CategoryManager();

            return instance;
        }

        #endregion

        #region Properties

        public List<Category> CategoryList
        {
            get { return this.categoryList; }
        }

        #endregion

        public Category SearchCategory(string name)
        {
            // Search category by name
            return this.categoryList.Find(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        public Category SearchCategory(int id)
        {
            // Search category by ID
            return this.categoryList.Find(x => x.Id == id);
        }

        public Subscription SearchSubscription(int id)
        {
            return this.GetAllSubscription().Find(x => x.Id == id);
        }

        public Subscription SearchSubscription(string url)
        {
            return this.GetAllSubscription().Find(x => x.Url.ToLower().Equals(url.ToLower()));
        }

        public List<Subscription> GetAllSubscription()
        {
            List<Subscription> allSubscription = new List<Subscription>();

            foreach (Category category in this.categoryList)
            {
                allSubscription.AddRange(category.SubscriptionManager.SubscriptionList);
            }

            return allSubscription;
        }

        public Category AddToList(int id, string name)
        {
            Category category = new Category(id, name);

            this.categoryList.Add(category);

            return category;
        }

        public void RemoveFromList(string name)
        {
            // Search category by name
            Category category = this.SearchCategory(name);

            // Remove subscription from list
            this.categoryList.Remove(category);
        }

        public void LoadAll()
        {
            this.LoadCategory();
            this.LoadSubscription();
        }

        public void LoadSubscription()
        {
            // Database connexion
            SQLiteDatabase db = SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE);

            // Query to retrieve category and subscription
            string query = @"
                SELECT 
                    s.id_subscription,
                    s.id_category,
                    s.url_subscription
                FROM 
                    subscription AS s";

            // Get result of the query in DataTable
            DataTable dataSubscription = db.GetDataTable(query, null);

            // For each subscription
            foreach (DataRow r in dataSubscription.Rows)
            {
                // Add current subscription to the category
                int id_category = int.Parse(r["id_category"].ToString());
                Category category = this.SearchCategory(id_category);
                category.SubscriptionManager.AddToList(
                    int.Parse(r["id_subscription"].ToString()), 
                    (string)r["url_subscription"]
                );
            }
        }

        public void LoadCategory()
        {
            // Database connexion
            SQLiteDatabase db = SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE);

            // Query to retrieve category and subscription
            string query = @"
                SELECT 
                    c.id_category, 
                    c.name_category
                FROM 
                    category AS c
                ORDER BY 
                    c.name_category";

            // Get result of the query in DataTable
            DataTable dataCategory = db.GetDataTable(query, null);

            // For each category
            foreach (DataRow r in dataCategory.Rows)
            {
                // Add and store the new category
                this.AddToList(
                    int.Parse(r["id_category"].ToString()),
                    (string)r["name_category"]
                );
            }
        }

        public void Insert(string name)
        {
            // TODO: Check name
            // TODO: Check if exists already

            // Database connexion
            SQLiteDatabase db = SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE);

            // Dictionary with column->value
            Dictionary<String, String> data = new Dictionary<String, String>();
            data.Add("name_category", name);

            try
            {
                // Insert into database, get of the row
                int id = db.Insert("category", data);

                // Insert element into current list
                this.AddToList(id, name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Update(Category category)
        {
            // Database connexion
            SQLiteDatabase db = SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE);

            // Dictionary with column->value
            Dictionary<String, String> data = new Dictionary<String, String>();
            data.Add("name_category", category.Name);

            try
            {
                List<SQLiteParameter> parameterList = new List<SQLiteParameter>();
                parameterList.Add(new SQLiteParameter("@id_category", category.Id));
                db.Update("category", data, "category.id_category = @id_category", parameterList);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Delete(Category category)
        {
            // Database connexion
            SQLiteDatabase db = SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE);

            try
            {
                List<SQLiteParameter> parameterList = new List<SQLiteParameter>();
                parameterList.Add(new SQLiteParameter("@id_category", category.Id));
                db.Delete("subscription", "id_category = @id_category", parameterList);
                db.Delete("category", "id_category = @id_category", parameterList);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            this.categoryList.Remove(category);
        }
    }
}
