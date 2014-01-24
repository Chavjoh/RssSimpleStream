using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RSS_Simple_Stream
{
    /// <summary>
    /// Logique d'interaction pour SubscriptionDataWindow.xaml
    /// </summary>
    public partial class SubscriptionDataWindow : Window
    {
        private Subscription insertedSubscription;

        public SubscriptionDataWindow()
        {
            InitializeComponent();

            CategoryManager categoryManager = CategoryManager.getInstance();
            this.categoryList.ItemsSource = categoryManager.CategoryList;
            this.categoryList.SelectedIndex = 1;
        }

        public Subscription InsertedSubscription
        {
            get { return this.insertedSubscription; }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.categoryList.SelectedItem == null)
            {
                MessageBox.Show("Please select a category to add your subscription.");
                return;
            }

            // Get category selected
            Category categorySelected = (Category) this.categoryList.SelectedItem;

            // Variable to check given subscription URL
            Uri uriResult;
            String uriString = this.url.Text;

            // Add protocol if not indicated
            if (!uriString.Contains("://"))
            {
                uriString = "http://" + uriString;
            }

            // Test URL validity
            bool result =
                Uri.IsWellFormedUriString(uriString, UriKind.Absolute) && 
                Uri.TryCreate(uriString, UriKind.Absolute, out uriResult) && 
                uriResult.Scheme == Uri.UriSchemeHttp &&
                testAccessUrl(uriString);
            
            if (result)
            {
                this.insertedSubscription = categorySelected.SubscriptionManager.Insert(uriString);
                this.Close();
            }
            else
            {
                MessageBox.Show("The subscription URL indicated is not valid.");
            }
        }

        private bool testAccessUrl(string url)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Timeout = 5000;
            request.Method = "HEAD";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            int statusCode = (int)response.StatusCode;

            if (statusCode >= 100 && statusCode < 400)
            {
                return true;
            }

            return false;
        }
    }
}
