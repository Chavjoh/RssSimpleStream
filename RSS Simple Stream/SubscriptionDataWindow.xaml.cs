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
    /// Interaction logic for SubscriptionDataWindow.xaml
    /// </summary>
    public partial class SubscriptionDataWindow : Window
    {
        private Subscription insertedSubscription;

        public SubscriptionDataWindow()
        {
            InitializeComponent();

            // Load the list of categories
            CategoryManager categoryManager = CategoryManager.getInstance();
            this.categoryList.ItemsSource = categoryManager.CategoryList;
            this.categoryList.SelectedIndex = 1;
        }

        /// <summary>
        /// Tries to access the URL to test its validity
        /// </summary>
        /// <param name="url">URL to validate</param>
        /// <returns>Validity state (True or False)</returns>
        private bool testAccessUrl(string url)
        {
            // Prepare request (5sec timeout, HEAD request only)
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Timeout = 5000;
            request.Method = "HEAD";

            // Try to get response
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            // Get status code
            int statusCode = (int)response.StatusCode;

            if (statusCode >= 100 && statusCode < 400)
            {
                return true;
            }

            return false;
        }

        #region Properties

        public Subscription InsertedSubscription
        {
            get { return this.insertedSubscription; }
        }

        #endregion

        #region Window Event

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.categoryList.SelectedItem == null)
            {
                MessageBox.Show("Please select a category to add your subscription.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                // Save inserted subscription to get it with parent window
                this.insertedSubscription = categorySelected.SubscriptionManager.Insert(uriString);

                // Close window
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("The subscription URL indicated is not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            // Close window
            this.DialogResult = false;
            this.Close();
        }

        #endregion

    }
}
