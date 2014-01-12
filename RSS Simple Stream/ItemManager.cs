using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    class ItemManager
    {
        private List<Item> itemList;
        private Category parentCategory;

        #region Constructors

        public ItemManager(Category parentCategory)
        {
            this.parentCategory = parentCategory;
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
