using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    class Category
    {
        private int id;
        private string name;
        private SubscriptionManager subscriptionManager;

        #region Constructors 

        public Category(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.subscriptionManager = new SubscriptionManager(this);
        }

        #endregion

        #region Properties

        public int Id
        {
            get { return this.id; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public SubscriptionManager SubscriptionManager
        {
            get { return this.subscriptionManager; }
        }

        #endregion

    }
}
