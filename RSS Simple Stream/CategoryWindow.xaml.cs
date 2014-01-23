﻿using System;
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
        public CategoryWindow()
        {
            InitializeComponent();

            CategoryManager categoryManager = CategoryManager.getInstance();

            this.categoryList.ItemsSource = categoryManager.CategoryList;
        }
    }
}
