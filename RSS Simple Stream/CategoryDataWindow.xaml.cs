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
        private Category editedCategory;

        public CategoryDataWindow()
        {
            InitializeComponent();
        }

        public CategoryDataWindow(Category category)
        {
            InitializeComponent();

            this.editedCategory = category;
            this.NameCategory = category.Name;
        }

        public string NameCategory
        {
            get { return this.nameCategory.Text; }
            set { this.nameCategory.Text = value; }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            // No modification on the name
            if (this.editedCategory != null && this.NameCategory.Equals(this.editedCategory.Name))
            {
                DialogResult = false;
                Close();
                return;
            }

            // Category name cannot be empty
            if (this.NameCategory.Equals(""))
            {
                MessageBox.Show("Please enter a name for the category", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Category name must be unique
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
