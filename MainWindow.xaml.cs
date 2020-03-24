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
using System.IO;
using System.IO.Compression;

namespace Encryption_Machine {
    
    public partial class MainWindow : Window {

        public MainWindow() {

            InitializeComponent();

            progress_Bar.Visibility = Visibility.Hidden;
            progress_BarL.Visibility = Visibility.Hidden;
        }

        //button handling

        public void Encrypt_Click(object sender, RoutedEventArgs e) {

            string dir_name = PathTB.Text;

            if (Directory.Exists(dir_name)) {
                if (KeyTB.Text.Length==16) {

                    encrypt_File(dir_name, KeyTB.Text);

                }
                else {
                    MessageBox.Show("Encryption key must be 16 characters long");
                    return;
                }
            }
            else {
                MessageBox.Show("Directory doesn't exist");
                return;
            }
            
        }

        public void Decrypt_Click(object sender, RoutedEventArgs e) {

            Console.WriteLine("decrypt");

        }

        public void Cancel_Click(object sender, RoutedEventArgs e) {

            Console.WriteLine("cancle");

        }

        // normal methods

        public void encrypt_File(string dir_name, string key) {

            Encryption encryption = new Encryption();

            encryption.dir_name = dir_name;
            encryption.encryption_key = key;

            //Console.WriteLine(key);
            encryption.encrypt();

        }



    }
}
