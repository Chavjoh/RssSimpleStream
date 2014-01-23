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
    /// Logique d'interaction pour CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window
    {
        private CategoryManager categoryManager;

        public CategoryWindow()
        {
            InitializeComponent();

            categoryManager = CategoryManager.getInstance();

            this.categoryList.ItemsSource = categoryManager.CategoryList;

            this.buttonEdit.IsEnabled = false;
            this.buttonDelete.IsEnabled = false;
        }

        private void categoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.buttonEdit.IsEnabled = true;
            this.buttonDelete.IsEnabled = true;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            // Create a window to enter data
            var dialog = new CategoryDataWindow();

            // When data is validated
            if (dialog.ShowDialog() == true)
            {
                categoryManager.Insert(dialog.NameCategory.ToString());
                this.categoryList.Items.Refresh();
            }
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            // Check if a category is selected
            if (this.categoryList.SelectedItem == null)
                return;

            // Get the current category selected
            Category category = (Category)this.categoryList.SelectedItem;

            // Create a window to edit data with value
            var dialog = new CategoryDataWindow();
            dialog.NameCategory = category.Name;

            // When data is edited
            if (dialog.ShowDialog() == true)
            {
                category.Name = dialog.NameCategory.ToString();
                categoryManager.Update(category);

                this.categoryList.Items.Refresh();
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            // Check if a category is selected
            if (this.categoryList.SelectedItem == null)
                return;

            // Get the current category selected
            Category category = (Category)this.categoryList.SelectedItem;

            // Confirmation message
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure ? It will destroy the category and all subscription associated.", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);

            // If user confirm to delete category
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                categoryManager.Delete(category);

                // Disable button when list is empty
                if (categoryManager.CategoryList.Count == 0)
                {
                    this.buttonEdit.IsEnabled = false;
                    this.buttonDelete.IsEnabled = false;
                }

                this.categoryList.Items.Refresh();
            }
        }
    }
}
