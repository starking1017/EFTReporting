using System;
using System.Collections;
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
using EFTReporting.Model;
using EFTReporting.ViewModels;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace EFTReporting.Forms
{
    /// <summary>
    /// Interaction logic for Payment.xaml
    /// </summary>
    public partial class frmPayment : Window
    {
        private static frmPayment _instance;

        public frmPayment()
        {
            InitializeComponent();
        }

        public static frmPayment Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new frmPayment();
                }
                return _instance;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_instance != null)
            {
                _instance = null;
            }
        }
    }
}
