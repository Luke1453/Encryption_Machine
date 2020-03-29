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


        private static CancellationTokenSource cancel_token_src = new CancellationTokenSource();
        private CancellationToken cancellationToken = cancel_token_src.Token;
        private Task task;
        

        public MainWindow() {

            InitializeComponent();

            progress_Bar.Visibility = Visibility.Hidden;
            progress_BarL.Visibility = Visibility.Hidden;
        }


        //
        //button handling
        //


        public void Encrypt_Click(object sender, RoutedEventArgs e) {

            string dir_name = PathTB.Text;
            string key = KeyTB.Text;

            if (Directory.Exists(dir_name)) {
                if (key.Length==16) {

                    try {
                        Encrypt_File_async(dir_name, key, cancellationToken);
                    }
                    catch (Exception ex) {

                        Console.WriteLine(ex.Message);
                    }

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
            string key = KeyTB.Text;

            if (Directory.Exists(dir_name)) {
                if (key.Length == 16) {

                    Decrypt_File_async(dir_name, key, cancellationToken);
                    
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

            cancel_token_src.Cancel();
            System.Windows.Application.Current.Shutdown();

        }


        //
        //additional methods
        //

       
        public Task Encrypt_File_async(string dir_name, string key, CancellationToken cancellationToken) {

            return Task.Run(() => {

                if (cancellationToken.IsCancellationRequested) {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                //Console.WriteLine("Encrypt_File_async(/*string dir_name, string key*/) started");

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
                //Console.WriteLine("Encrypt_File_async(/*string dir_name, string key*/) started encrypting");
                Encryption encryption = new Encryption(dir_name,key);
                encryption.encrypt();

                //if (cancellationToken.IsCancellationRequested) {
                //    cancellationToken.ThrowIfCancellationRequested();
                //}

                //canceling progress bar thread finishing it and hiding
                cancel_token_src.Cancel();
                cancel_token_src.Dispose();

                string encrypted_path = encryption.encrypted_path;
                string md5_string = encryption.md5_string;
                
                //hiding progress bar and showing message window
                Dispatcher.Invoke(new Action(async () => {

                    progress_Bar.Value = 49;
                    await Task.Delay(200);
                    progress_Bar.Value = 50;
                    await Task.Delay(250);

                    encryptBT.IsEnabled = true;
                    decryptBT.IsEnabled = true;
                    progress_BarL.Visibility = Visibility.Hidden;
                    progress_Bar.Visibility = Visibility.Hidden;

                    MessageBox.Show(String.Format("{0}\nEncrypted using Key: {1}\n\nStored as:\n{2}\n\nMD5: {3}", dir_name, key, encrypted_path, md5_string),
                        "Encryption Finished", MessageBoxButton.OK ,MessageBoxImage.Information);
                }));

                //Console.WriteLine("Encrypt_File_async(string dir_name, string key) ended");
            });

        }


        public Task Decrypt_File_async(string dir_name, string key, CancellationToken cancellationToken) {

            return Task.Run(() => {

                if (cancellationToken.IsCancellationRequested) {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Console.WriteLine("Decrypt_File_async(string dir_name, string key) started");

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

                //handling decryption
                Console.WriteLine("Decrypt_File_async(string dir_name, string key) started decrypting");
                Encryption encryption = new Encryption(dir_name, key);
                encryption.decrypt();

                if (cancellationToken.IsCancellationRequested) {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (encryption.error_msg == null) {

                    //canceling progress bar thread finishing it and hiding
                    cancel_token_src.Cancel();
                    cancel_token_src.Dispose();

                    string decrypted_storage_path = encryption.decrypted_storage_path;
                    
                    //hiding progress bar and showing message window
                    Dispatcher.Invoke(new Action(async () => {

                        progress_Bar.Value = 49;
                        await Task.Delay(200);
                        progress_Bar.Value = 50;
                        await Task.Delay(250);

                        encryptBT.IsEnabled = true;
                        decryptBT.IsEnabled = true;
                        progress_BarL.Visibility = Visibility.Hidden;
                        progress_Bar.Visibility = Visibility.Hidden;

                        MessageBox.Show(String.Format("{0}\nDecrypted using Key: {1}\n\nStored in:\n{2}", dir_name, key, decrypted_storage_path),
                            "Encryption Finished", MessageBoxButton.OK, MessageBoxImage.Information);
                    }));

                }
                else {

                    MessageBox.Show(encryption.error_msg, "Decryption Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }

            });
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


    }
}

