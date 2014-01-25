using System;
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

        /// <summary>
        /// Search category by name
        /// </summary>
        /// <param name="name">Name of the category</param>
        /// <returns>Category found</returns>
        public Category SearchCategory(string name)
        {
            return this.categoryList.Find(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        /// <summary>
        /// Search category by ID
        /// </summary>
        /// <param name="id">ID of the category</param>
        /// <returns>Category found</returns>
        public Category SearchCategory(int id)
        {
            return this.categoryList.Find(x => x.Id == id);
        }

        /// <summary>
        /// Search subscription by ID
        /// </summary>
        /// <param name="id">ID of the subscription</param>
        /// <returns>Subscription found</returns>
        public Subscription SearchSubscription(int id)
        {
            return this.GetAllSubscription().Find(x => x.Id == id);
        }

        /// <summary>
        /// Search subscription by URL
        /// </summary>
        /// <param name="url">URL of the subscription</param>
        /// <returns>Subscription found</returns>
        public Subscription SearchSubscription(string url)
        {
            return this.GetAllSubscription().Find(x => x.Url.ToLower().Equals(url.ToLower()));
        }

        /// <summary>
        /// Create a subscription list of all categories
        /// </summary>
        /// <returns>Subscription list</returns>
        public List<Subscription> GetAllSubscription()
        {
            List<Subscription> allSubscription = new List<Subscription>();

            foreach (Category category in this.categoryList)
            {
                allSubscription.AddRange(category.SubscriptionManager.SubscriptionList);
            }

            return allSubscription;
        }

        /// <summary>
        /// Add a category to the list
        /// </summary>
        /// <param name="id">ID of the category</param>
        /// <param name="name">Name of the category</param>
        /// <returns>New category inserted</returns>
        public Category AddToList(int id, string name)
        {
            Category category = new Category(id, name);

            this.categoryList.Add(category);

            return category;
        }

        /// <summary>
        /// Remove a category from the list
        /// </summary>
        /// <param name="name">Name of the category</param>
        public void RemoveFromList(string name)
        {
            // Search category by name
            Category category = this.SearchCategory(name);

            // Remove subscription from list
            this.categoryList.Remove(category);
        }

        /// <summary>
        /// Load category and subscription list
        /// </summary>
        public void LoadAll()
        {
            this.LoadCategory();
            this.LoadSubscription();
        }

        /// <summary>
        /// Load subscriptions (all subscription)
        /// </summary>
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

        /// <summary>
        /// Load categories
        /// </summary>
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

        /// <summary>
        /// Insert a new category into the list and database
        /// </summary>
        /// <param name="name">Name of the category</param>
        public void Insert(string name)
        {
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

        /// <summary>
        /// Update a category from database
        /// </summary>
        /// <param name="category">Category object already edited</param>
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

        /// <summary>
        /// Delete a category from the list and database
        /// </summary>
        /// <param name="category">Category object to delete</param>
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

            // Remove from the list
            this.categoryList.Remove(category);
        }
    }
}
