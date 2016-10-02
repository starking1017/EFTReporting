using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EFTReporting.Forms;

namespace EFTReporting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Admin_OnClick(object sender, RoutedEventArgs e)
        {
            frmAdmin mw = frmAdmin.Instance;
            mw.Owner = this;
            mw.Show();
        }

        private void Payment_OnClick(object sender, RoutedEventArgs e)
        {
            frmPayment mw = frmPayment.Instance;
            mw.Owner = this;
            mw.Show();
        }
    }
}
