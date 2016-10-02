using System;
using System.Collections.Generic;
using System.Dynamic;
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

namespace EFTReporting.Forms
{
    /// <summary>
    /// Interaction logic for frmAdmin.xaml
    /// </summary>
    public partial class frmAdmin : Window
    {
        private static frmAdmin _instance;
        public frmAdmin()
        {
            InitializeComponent();
        }

        public static frmAdmin Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new frmAdmin();
                }
                return _instance;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _instance = null;

        }
    }
}
