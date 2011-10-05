﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IndiaTango.Views
{
    /// <summary>
    /// Interaction logic for LogWindowView.xaml
    /// </summary>
    public partial class LogWindowView : Window
    {
        public LogWindowView()
        {
            InitializeComponent();
        }

        private void textBoxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxLog.ScrollToEnd();
        }
    }
}