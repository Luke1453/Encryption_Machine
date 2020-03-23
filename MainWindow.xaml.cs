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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace Encryption_Machine {
    
    public partial class MainWindow : Window {

        public MainWindow() {

            InitializeComponent();

            progress_Bar.Visibility = Visibility.Hidden;
            progress_BarL.Visibility = Visibility.Hidden;
        }

        public void Encrypt_Click(object sender, RoutedEventArgs e) {

            Console.WriteLine("eencrypt");
            
        }

        public void Decrypt_Click(object sender, RoutedEventArgs e) {

            Console.WriteLine("decrypt");

        }

        public void Cancel_Click(object sender, RoutedEventArgs e) {

            Console.WriteLine("cancle");

        }

    }
}
