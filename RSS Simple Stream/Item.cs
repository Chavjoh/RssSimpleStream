﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    public class Item
    {
        private string title;
        private string description;
        private string link;
        private string guid;
        private DateTime date;

        #region Properties 

        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        public string Link
        {
            get { return this.link; }
            set { this.link = value; }
        }

        public DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }

        public string Guid
        {
            get { return this.guid; }
            set { this.guid = value; }
        }

        #endregion

        public override string ToString()
        {
            return base.ToString() + "(" + 
                "title=" + this.title + ", " + 
                "description=" + this.description + ", " + 
                "link=" + this.link + ", " + 
                "date=" + this.date + ")";
        }
    }
}
