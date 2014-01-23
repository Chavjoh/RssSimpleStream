using Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SQLite;
using System.Web;
using System.Threading;

namespace RSS_Simple_Stream
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Min size management for each part of the grid
            this.SizeChanged += (s, e) =>
            {
                double maxWidth = this.ActualWidth / 1.75;
                this.ContentGrid.ColumnDefinitions[0].MaxWidth = maxWidth;
            };

            // Automatic width size for subscription list
            this.subscriptionList.SizeChanged += (s, e) =>
            {
                ((GridViewColumn)((GridView)this.subscriptionList.View).Columns[0]).Width = e.NewSize.Width - 30;
            };

            // Automatic width size for item list
            this.itemList.SizeChanged += (s, e) =>
            {
                ((GridViewColumn)((GridView)this.itemList.View).Columns[0]).Width = e.NewSize.Width - 30;
            };

            // Create main manager
            CategoryManager categoryManager = CategoryManager.getInstance();

            try
            {
                // Database connexion
                SQLiteDatabase db = new SQLiteDatabase("database.sqlite");

                // Query to retrieve category and subscription
                string query = @"
                SELECT 
                    c.id_category, 
                    c.name_category,
                    s.id_subscription,
                    s.title_subscription,
                    s.url_subscription
                FROM 
                    category AS c
                LEFT JOIN
                    subscription AS s
                ON
                    c.id_category = s.id_category
                ORDER BY 
                    c.name_category";

                // Get result of the query in DataTable
                DataTable dataCategory = db.GetDataTable(query);

                // Storing the previous category to know when we change to the next.
                Category currentCategory = null;
                
                // For each category
                foreach (DataRow r in dataCategory.Rows)
                {
                    // When we're in a new category
                    if (currentCategory == null || currentCategory.Id != int.Parse(r["id_category"].ToString()))
                    {
                        // Add and store the new category
                        currentCategory = categoryManager.Add(
                            int.Parse(r["id_category"].ToString()), 
                            (string) r["name_category"]
                        );
                    }

                    // Add current subscription to the category
                    Subscription subscription = currentCategory.SubscriptionManager.Add((string) r["url_subscription"]);

                    // Invoke refresh category list when load is finished
                    subscription.ProgressUpdate += (s, e) => {
                        Dispatcher.Invoke((Action)delegate() { refreshCategoryList(); });
                    };

                    // Start loading in a new thread
                    Thread workerThread = new Thread(subscription.LoadFeed);
                    workerThread.Start();
                }
            }
            catch(Exception e)
            {
                // When error occurred with database loading
                String error = "The following error has occurred:\n\n";
                error += e.Message.ToString() + "\n\n";
                MessageBox.Show(error);
            }

            // Bind subscription list with ListView
            this.subscriptionList.ItemsSource = categoryManager.GetAllSubscription();

            // Indicate which properties is the group
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(this.subscriptionList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Parent.Name");
            view.GroupDescriptions.Add(groupDescription);

            // Hide specific ribbon tab
            this.RibbonTab_ItemManage.Visibility = Visibility.Collapsed;
            this.RibbonTab_ItemShare.Visibility = Visibility.Collapsed;
            this.RibbonTab_Subscription.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Refresh category list when changed is made
        /// </summary>
        public void refreshCategoryList()
        {
            this.subscriptionList.Items.Refresh();
        }

        /// <summary>
        /// Add a new tab with a browser inside
        /// </summary>
        /// <param name="title">Title of the new tab</param>
        /// <param name="url">URL to load in the browser</param>
        private void AddBrowerTab(string title, string url)
        {
            // Create context menu
            System.Windows.Controls.ContextMenu contextMenu;
            contextMenu = new System.Windows.Controls.ContextMenu();

            // Create menu items
            System.Windows.Controls.MenuItem closeItem;
            closeItem = new System.Windows.Controls.MenuItem();

            // Add menu item in context menu
            contextMenu.Items.Add(closeItem);
            closeItem.Header = "Close";
            closeItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("Images/cross_16x16.png", UriKind.Relative))
            };

            // Create tab items
            TabItem tabItem = new TabItem();
            tabItem.Header = title;

            // Define clicking event
            closeItem.Click += delegate { CloseTab(tabItem); };

            // Incorporate context menu with tab item
            tabItem.ContextMenu = contextMenu;

            // Create a new webBrowser for the tab content
            WebBrowser webBrower = new WebBrowser();
            webBrower.Source = new Uri(url);

            // Incorporate webBrowser with tabItem
            tabItem.Content = webBrower;

            // Add the new tab created
            this.contentTabControl.Items.Add(tabItem);
        }

        /// <summary>
        /// Close a tab
        /// </summary>
        /// <param name="item">Item to delete</param>
        private void CloseTab(TabItem item)
        {
            if (item != null)
            {
                // Find the parent tabControl
                TabControl tabControl = item.Parent as TabControl;

                // Remove item from tabControl
                if (tabControl != null)
                    tabControl.Items.Remove(item);
            }
        }

        private void subscriptionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if a subscription is selected
            if (this.subscriptionList.SelectedItem != null)
            {
                // Get selected subscription
                Subscription currentSubscription = (Subscription)this.subscriptionList.SelectedItem;

                // Bind item list with ListView
                this.itemList.ItemsSource = currentSubscription.ItemManager.ItemList;

                // Show tab about subscription
                this.RibbonTab_Subscription.Visibility = Visibility.Visible;
            }

            // Hide tab about items
            this.RibbonTab_ItemShare.Visibility = Visibility.Collapsed;
            this.RibbonTab_ItemManage.Visibility = Visibility.Collapsed;
        }

        private void itemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if an item is selected
            if (this.itemList.SelectedItem == null)
                return;

            // Get the current item selected
            Item rssItem = (Item)this.itemList.SelectedItem;

            // Create a browser tab with the item link
            AddBrowerTab(rssItem.Title, rssItem.Link);
        }

        private void subscriptionRefresh_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("REFRESH");
        }

        private void itemShare_Facebook_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (this.itemList.SelectedItem == null)
                return;

            // Get the current item selected
            Item rssItem = (Item)this.itemList.SelectedItem;

            // Create specific link to share on Facebook
            string url = "http://www.facebook.com/sharer/sharer.php?s=100&p[url]=" +
                HttpUtility.UrlPathEncode(rssItem.Link) + 
                "&p[images][0]=&p[title]=" +
                HttpUtility.UrlPathEncode(rssItem.Title) + 
                "&p[summary]=";

            // Create a browser tab with the share link
            AddBrowerTab("Share", url);
        }

        private void itemShare_Twitter_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (this.itemList.SelectedItem == null)
                return;

            // Get the current item selected
            Item rssItem = (Item)this.itemList.SelectedItem;

            // Create specific link to share on Twitter
            string url = "http://twitter.com/home?status=" + HttpUtility.UrlPathEncode(rssItem.Title + " : " + rssItem.Link);

            // Create a browser tab with the share link
            AddBrowerTab("Share", url);
        }

        private void itemShare_Google_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (this.itemList.SelectedItem == null)
                return;

            // Get the current item selected
            Item rssItem = (Item)this.itemList.SelectedItem;

            // Create specific link to share on Google+
            string url = "https://plus.google.com/share?url=" + HttpUtility.UrlPathEncode(rssItem.Link);

            // Create a browser tab with the share link
            AddBrowerTab("Share", url);
        }

        private void itemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If no item is selected
            if (this.itemList.SelectedItem == null)
            {
                // Hide tab about items
                this.RibbonTab_ItemManage.Visibility = Visibility.Hidden;
                this.RibbonTab_ItemShare.Visibility = Visibility.Hidden;
            }
            else
            {
                // Show tab about items
                this.RibbonTab_ItemManage.Visibility = Visibility.Visible;
                this.RibbonTab_ItemShare.Visibility = Visibility.Visible;
            }
        }

        private void contentTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When tab change to main tab "Feed"
            if (this.contentTabControl.SelectedIndex == 0)
            {
                // Hide close button in Ribbon
                this.tabClose.Visibility = Visibility.Collapsed;
            }
            // When another tab is selected
            else
            {
                // Show close button in Ribbon
                this.tabClose.Visibility = Visibility.Visible;
            }
        }

        private void tabClose_Click(object sender, RoutedEventArgs e)
        {
            // When tab change (but is NOT main "Feed" tab)
            if (this.contentTabControl.SelectedIndex > 0)
            {
                CloseTab((TabItem)this.contentTabControl.SelectedItem);
            }
        }

        private void categoryManage_Click(object sender, RoutedEventArgs e)
        {
            CategoryWindow categoryWindow = new CategoryWindow();
            categoryWindow.ShowDialog();
        }
    }
}
