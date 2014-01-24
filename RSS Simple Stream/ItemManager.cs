using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    public class ItemManager
    {
        private List<Item> itemList;
        private Subscription parentSubscription;

        #region Constructors

        public ItemManager(Subscription parentSubscription)
        {
            this.parentSubscription = parentSubscription;
            this.itemList = new List<Item>();
        }

        #endregion

        #region Properties

        public List<Item> ItemList
        {
            get { return this.itemList; }
        }

        #endregion

    }
}
