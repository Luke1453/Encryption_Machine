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
using System.Threading;

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

                    Encrypt_File_async(dir_name, KeyTB.Text);
                    
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

            string dir_name = PathTB.Text;

            if (Directory.Exists(dir_name)) {
                if (KeyTB.Text.Length == 16) {

                    Encrypt_File_async(dir_name, KeyTB.Text);

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


        public void Cancel_Click(object sender, RoutedEventArgs e) {

            Console.WriteLine("cancle");

        }


        //progress bar threading shite
        public Task Progress_Bar_work(CancellationToken cancellationToken) {

            return Task.Run(() => {

                Console.WriteLine("Progress_Bar_work() started");

                if (cancellationToken.IsCancellationRequested) {
                    Console.WriteLine("Progress_Bar_work(CancellationToken cancellationToken) canceled");
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Dispatcher.Invoke(new Action(async () => {

                    encryptBT.IsEnabled = false;
                    decryptBT.IsEnabled = false;
                    progress_BarL.Visibility = Visibility.Visible;
                    progress_Bar.Visibility = Visibility.Visible;

                    progress_Bar.Value = 0;
                    while (progress_Bar.Value < 50) {

                        var rand = new Random();
                        int time = rand.Next(250, 600);

                        if (cancellationToken.IsCancellationRequested) {
                            Console.WriteLine("Progress_Bar_work(CancellationToken cancellationToken) canceled");
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        await Task.Delay(time);
                        progress_Bar.Value++;

                    }
                }));

                Console.WriteLine("Progress_Bar_work() ended"); 
            });
        }


        //
        // normal methods
        //

       
        public Task Encrypt_File_async(string dir_name, string key) {

            return Task.Run(() => {

                Console.WriteLine("Encrypt_File_async(/*string dir_name, string key*/) started");

                //creating cancel token for progres bar
                var cancel_token_src = new CancellationTokenSource();
                var cancel_token = cancel_token_src.Token;

                //starting progress bar work
                try {
                    Progress_Bar_work(cancel_token);
                }
                catch (Exception e) {

                    Console.WriteLine(e.Message);
                }

                //handling encryption
                Console.WriteLine("Encrypt_File_async(/*string dir_name, string key*/) started encrypting");
                Encryption encryption = new Encryption();
                encryption.dir_name = dir_name;
                encryption.encryption_key = key;
                encryption.encrypt();

                //canceling progress bar thread finishing it and hiding
                cancel_token_src.Cancel();
                cancel_token_src.Dispose();

                string encrypted_path = encryption.encrypted_path;
                string md5_string = encryption.md5_string;
                
                Dispatcher.Invoke(new Action(async () => {

                    progress_Bar.Value = 49;
                    await Task.Delay(200);
                    progress_Bar.Value = 50;
                    await Task.Delay(250);

                    encryptBT.IsEnabled = true;
                    decryptBT.IsEnabled = true;
                    progress_BarL.Visibility = Visibility.Hidden;
                    progress_Bar.Visibility = Visibility.Hidden;

                    MessageBox.Show(String.Format("{0}\n\nEncrypted using Key: {1}\n\nStored as:\n{2}\n\nMD5: {3}", dir_name, key, encrypted_path, md5_string),
                        "Encryption Finished", MessageBoxButton.OK ,MessageBoxImage.Information);
                }));

                encryption = null;
                Console.WriteLine("Encrypt_File_async(string dir_name, string key) ended");

                GC.Collect();
                GC.WaitForPendingFinalizers();
            });

        }


    }
}

