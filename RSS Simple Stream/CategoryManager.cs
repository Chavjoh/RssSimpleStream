using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    class CategoryManager
    {
        private List<Category> categoryList;

        #region Constructors

        public CategoryManager()
        {
            this.categoryList = new List<Category>();
        }

        #endregion

        #region Properties

        public List<Category> CategoryList
        {
            get { return this.categoryList; }
        }

        #endregion

        public List<Subscription> GetAllSubscription()
        {
            List<Subscription> allSubscription = new List<Subscription>();

            foreach (Category category in this.categoryList)
            {
                allSubscription.AddRange(category.SubscriptionManager.SubscriptionList);
            }

            return allSubscription;
        }

        public Category Add(string name)
        {
            // TODO: Check name
            // TODO: Check if exists already
            Category category = new Category(name);

            this.categoryList.Add(category);

            return category;
        }

        public void Remove(string name)
        {
            // Search category by name
            Category category = this.categoryList.Find(x => x.Name.Equals(name));

            // Remove subscription from list
            this.categoryList.Remove(category);
        }
    }
}
