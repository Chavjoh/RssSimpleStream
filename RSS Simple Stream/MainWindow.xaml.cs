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
using System.ComponentModel;

namespace RSS_Simple_Stream
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private Item currentItem;

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
                // Load all categories and subscriptions
                categoryManager.LoadAll();

                // Load all RSS items
                categoryManager.GetAllSubscription().ForEach(delegate(Subscription subscription)
                {
                    // Invoke refresh category list when load is finished
                    subscription.ProgressUpdate += (s, e) =>
                    {
                        Dispatcher.Invoke((Action)delegate() { refreshSubscriptionList(); });
                    };

                    // Start loading in a new thread
                    Thread workerThread = new Thread(subscription.LoadFeed);
                    workerThread.Start();
                });
            }
            catch(Exception e)
            {
                // When error occurred with database loading
                Console.WriteLine("The following error has occurred:\n\n" + e.Message.ToString() + "\n\n");
            }

            refreshSubscriptionList();

            // Hide specific ribbon tab
            this.RibbonTab_ItemShare.Visibility = Visibility.Collapsed;
            this.RibbonTab_Subscription.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Refresh subscription list when changed is made
        /// </summary>
        public void refreshSubscriptionList()
        {
            // Bind subscription list with ListView
            this.subscriptionList.ItemsSource = CategoryManager.getInstance().GetAllSubscription();

            // Indicate which properties is the group
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(this.subscriptionList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Manager.Parent.Name");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void ChangeCurrentTab(TabItem tabItem)
        {
            this.ChangeCurrentTab(this.contentTabControl.Items.IndexOf(tabItem), tabItem);
        }

        private void ChangeCurrentTab(int index, TabItem tabItem)
        {
            // Change selection
            this.contentTabControl.SelectedIndex = index;

            // Change item
            //this.contentTabControl.SelectedItem = tabItem;

            // Item state changed
            //tabItem.IsSelected = true;
        }

        private Item GetCurrentRssItem()
        {
            // When we're on other tab
            if (this.contentTabControl.SelectedIndex > 0)
            {
                return this.currentItem;
            }
            // When we're on Feed tab
            else
            {
                // Check if an item is selected
                if (this.itemList.SelectedItem != null)
                {
                    // Get the current item selected
                    return (Item)this.itemList.SelectedItem;
                }
            }

            return null;
        }

        private TabItem AddBrowserTab(Item rssItem)
        {
            TabItem browserTab = this.AddBrowserTab(rssItem.Title, rssItem.Link);
            browserTab.Tag = rssItem;
            return browserTab;
        }

        /// <summary>
        /// Add a new tab with a browser inside
        /// </summary>
        /// <param name="title">Title of the new tab</param>
        /// <param name="url">URL to load in the browser</param>
        private TabItem AddBrowserTab(string title, string url)
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

            return tabItem;
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

        #region RIBBON TAB - HOME

        private void categoryManage_Click(object sender, RoutedEventArgs e)
        {
            CategoryWindow categoryWindow = new CategoryWindow();
            categoryWindow.ShowDialog();

            refreshSubscriptionList();
        }

        private void subscriptionAdd_Click(object sender, RoutedEventArgs e)
        {
            CategoryManager categoryManager = CategoryManager.getInstance();

            // Check if at least one category exists
            if (categoryManager.CategoryList.Count > 0)
            {
                // Open a new window to add a new subscription
                SubscriptionDataWindow subscriptionWindow = new SubscriptionDataWindow();
                subscriptionWindow.ShowDialog();

                // Get inserted subscription from the window
                Subscription insertedSubscription = subscriptionWindow.InsertedSubscription;

                if (insertedSubscription != null)
                {
                    // Invoke refresh category list when load is finished
                    insertedSubscription.ProgressUpdate += (s, ev) =>
                    {
                        Dispatcher.Invoke((Action)delegate() { refreshSubscriptionList(); });
                    };

                    // Start loading in a new thread
                    Thread workerThread = new Thread(insertedSubscription.LoadFeed);
                    workerThread.Start();
                }
            }
            else
            {
                MessageBox.Show("You need at least one category to add a subscription.");
            }
        }

        private void AppAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void AppQuit_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            //TODO: TEMP
            MessageBox.Show(this.contentTabControl.SelectedIndex + this.contentTabControl.SelectedItem.ToString());
        }

        #endregion 

        #region RIBBON TAB - SUBSCRIPTION

        private void subscriptionRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Check if a subscription is selected
            if (this.subscriptionList.SelectedItem == null)
                return;

            // Get the current subscription selected
            Subscription subscription = (Subscription)this.subscriptionList.SelectedItem;

            // Start loading in a new thread
            Thread workerThread = new Thread(subscription.LoadFeed);
            workerThread.Start();
        }

        private void subscriptionDelete_Click(object sender, RoutedEventArgs e)
        {
            // Check if a subscription is selected
            if (this.subscriptionList.SelectedItem == null)
                return;

            // Get the current subscription selected
            Subscription subscription = (Subscription)this.subscriptionList.SelectedItem;

            // Confirmation message
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to want to delete the subscription " + subscription.Title + " ?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);

            // If user confirm to delete subscription
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                // Delete subscription
                subscription.Manager.Remove(subscription);

                // Refresh subscription list
                this.refreshSubscriptionList();

                // Clear items list
                this.itemList.ItemsSource = null;
                this.itemList.Items.Refresh();

                // Hide tab
                this.RibbonTab_Subscription.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region RIBBON TAB - SHARE

        private void itemShare_Facebook_Click(object sender, RoutedEventArgs e)
        {
            Item rssItem = this.GetCurrentRssItem();

            // Create specific link to share on Facebook
            string url = "http://www.facebook.com/sharer/sharer.php?s=100&p[url]=" +
                HttpUtility.UrlPathEncode(rssItem.Link) + 
                "&p[images][0]=&p[title]=" +
                HttpUtility.UrlPathEncode(rssItem.Title) + 
                "&p[summary]=";

            // Create a browser tab with the share link
            TabItem browserTab = AddBrowserTab("Share", url);

            // Select new tab
            ChangeCurrentTab(browserTab);
        }

        private void itemShare_Twitter_Click(object sender, RoutedEventArgs e)
        {
            Item rssItem = this.GetCurrentRssItem();

            // Create specific link to share on Twitter
            string url = "http://twitter.com/home?status=" + HttpUtility.UrlPathEncode(rssItem.Title + " : " + rssItem.Link);

            // Create a browser tab with the share link
            TabItem browserTab = AddBrowserTab("Share", url);

            // Select new tab
            ChangeCurrentTab(browserTab);
        }

        private void itemShare_Google_Click(object sender, RoutedEventArgs e)
        {
            Item rssItem = this.GetCurrentRssItem();

            // Create specific link to share on Google+
            string url = "https://plus.google.com/share?url=" + HttpUtility.UrlPathEncode(rssItem.Link);

            // Create a browser tab with the share link
            TabItem browserTab = AddBrowserTab("Share", url);

            // Select new tab
            ChangeCurrentTab(browserTab);
        }

        #endregion

        #region Window Event

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
        }

        private void itemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if an item is selected
            if (this.itemList.SelectedItem == null)
                return;

            // Get the current item selected
            Item rssItem = (Item)this.itemList.SelectedItem;

            // Create a browser tab with the item link
            AddBrowserTab(rssItem);
        }

        private void itemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If no item is selected
            if (this.itemList.SelectedItem == null)
            {
                // Hide tab about items
                this.RibbonTab_ItemShare.Visibility = Visibility.Hidden;
            }
            else
            {
                // Show tab about items
                this.RibbonTab_ItemShare.Visibility = Visibility.Visible;
            }
        }

        private void contentTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                // When tab change to main tab "Feed"
                if (this.contentTabControl.SelectedIndex == 0)
                {
                    // Hide close button in Ribbon
                    this.tabClose.Visibility = Visibility.Collapsed;

                    // Show subscription ribbon tab
                    this.RibbonTab_Subscription.Visibility = Visibility.Visible;

                    // Show item ribbon tab only if item selected on feed tab
                    if (this.itemList.SelectedItem != null)
                    {
                        this.RibbonTab_ItemShare.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.RibbonTab_ItemShare.Visibility = Visibility.Collapsed;
                    }
                }
                // When another tab is selected
                else
                {
                    // Get current tab selected 
                    TabItem currentTab = (TabItem)this.contentTabControl.SelectedItem;

                    // Show item ribbon tab only if it's not a share tab
                    if (currentTab != null && currentTab.Tag != null)
                    {
                        // Save item associated
                        this.currentItem = ((Item)currentTab.Tag);

                        // Show share ribbon tab
                        this.RibbonTab_ItemShare.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.RibbonTab_ItemShare.Visibility = Visibility.Collapsed;
                    }

                    // Hide subscription ribbon tab
                    this.RibbonTab_Subscription.Visibility = Visibility.Collapsed;

                    // Show close button in Ribbon
                    this.tabClose.Visibility = Visibility.Visible;
                }
            }

            e.Handled = true;
        }

        private void tabClose_Click(object sender, RoutedEventArgs e)
        {
            // When tab change (but is NOT main "Feed" tab)
            if (this.contentTabControl.SelectedIndex > 0)
            {
                CloseTab((TabItem)this.contentTabControl.SelectedItem);
            }
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Close connection
            SQLiteDatabase.getInstance(Settings.SQLITE_DATABASE).closeConnection();
        }

        #endregion
    }
}
