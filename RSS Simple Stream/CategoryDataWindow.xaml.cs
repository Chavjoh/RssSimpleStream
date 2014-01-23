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
    /// Logique d'interaction pour CategoryDataWindow.xaml
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
            DialogResult = true;
        }
    }
}
