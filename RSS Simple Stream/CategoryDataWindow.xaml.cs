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
using System.Windows.Shapes;

namespace RSS_Simple_Stream
{
    /// <summary>
    /// Interaction logic for CategoryDataWindow.xaml
    /// </summary>
    public partial class CategoryDataWindow : Window
    {
        public CategoryDataWindow()
        {
            InitializeComponent();
        }

        public string NameCategory
        {
            get { return this.nameCategory.Text; }
            set { this.nameCategory.Text = value; }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            string name = this.NameCategory;

            if (this.NameCategory.Equals(""))
            {
                MessageBox.Show("Please enter a name for the category", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryManager.getInstance().SearchCategory(this.NameCategory) != null)
            {
                MessageBox.Show("The name you entered is already used.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
