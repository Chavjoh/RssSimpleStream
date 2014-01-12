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

            this.SizeChanged += (s, e) =>
            {
                double maxWidth = this.ActualWidth / 1.5;
                this.ContentGrid.ColumnDefinitions[0].MaxWidth = maxWidth;
            };

            CategoryManager categoryManager = new CategoryManager();
            Category category = categoryManager.Add("Main");
            SubscriptionManager subscriptionManager = category.SubscriptionManager;
            Subscription subscription = subscriptionManager.Add("http://www.pcinpact.com/rss/news.xml");
            ItemManager itemManager = subscription.ItemManager;
            subscription.LoadFeed();

            this.subscriptionList.ItemsSource = categoryManager.GetAllSubscription();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(this.subscriptionList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Parent.Name");
            view.GroupDescriptions.Add(groupDescription);

            this.RibbonTab_Item.Visibility = Visibility.Collapsed;
            this.RibbonTab_Subscription.Visibility = Visibility.Collapsed;
        }

        private void subscriptionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.subscriptionList.SelectedItems != null)
            {
                Subscription currentSubscription = (Subscription)this.subscriptionList.SelectedItem;
                MessageBox.Show(currentSubscription.Title);

                this.RibbonTab_Subscription.Visibility = Visibility.Visible;

                this.itemList.ItemsSource = currentSubscription.ItemManager.ItemList;
            }
        }
    }
}
