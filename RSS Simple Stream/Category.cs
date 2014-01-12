using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    class Category
    {
        private string name;
        private SubscriptionManager subscriptionManager;

        #region Constructors 

        public Category(string name)
        {
            this.name = name;
            this.subscriptionManager = new SubscriptionManager(this);
        }

        #endregion

        #region Properties

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
