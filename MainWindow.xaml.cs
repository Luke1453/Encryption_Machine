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




                    //Task.Run(Encrypt_File_async(dir_name, KeyTB.Text));
                    //var encryption_task =  Encrypt_File_async(dir_name, KeyTB.Text);
                    //var task = await Encrypt_File_async(dir_name, KeyTB.Text);
                    //task.

                    //var task = Encrypt_File_async(dir_name, KeyTB.Text);
                    //await task;

                    //await Encrypt_File_async(dir_name, KeyTB.Text);

                    //await Encrypt_File_async(dir_name, KeyTB.Text);

                    //Task.Run(() => Encrypt_File_async(/*dir_name, KeyTB.Text*/));

                    Encrypt_File_async();
              
                    for (int i = 0; i<50000; i++) {
                        Console.WriteLine(i);
                    }
                    
                    





                    //await Task.Run(() => {

                    //    Dispatcher.Invoke(new Action(() => {

                    //        encryptBT.IsEnabled = false;
                    //        decryptBT.IsEnabled = false;
                    //        progress_BarL.Visibility = Visibility.Visible;
                    //        progress_Bar.Visibility = Visibility.Visible;
                    //    }));

                    //    Dispatcher.Invoke(new Action(async () => {

                    //        progress_Bar.Value = 0;
                    //        while (progress_Bar.Value < 50) {

                    //            var rand = new Random();
                    //            int time = rand.Next(100, 250);

                    //            await Task.Delay(time);
                    //            progress_Bar.Value++;

                    //        }
                    //    }));

                    //    Task.Delay(2000);
                    //    Encrypt_File(dir_name, KeyTB.Text);
                    //    Task.Delay(2000);

                    //    Dispatcher.Invoke(new Action(() => {

                    //        encryptBT.IsEnabled = true;
                    //        decryptBT.IsEnabled = true;
                    //        progress_BarL.Visibility = Visibility.Hidden;
                    //        progress_Bar.Visibility = Visibility.Hidden;
                    //    }));

                    //});

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

        //progress bar threading shite
        public Task Progress_Bar_work(CancellationToken cancellationToken) {

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
                    int time = rand.Next(500, 1000);

                    if (cancellationToken.IsCancellationRequested) {
                        Console.WriteLine("Progress_Bar_work(CancellationToken cancellationToken) canceled");
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    await Task.Delay(time);
                    progress_Bar.Value++;

                }

                //progress_Bar.IsIndeterminate = true;
            }));

            return Task.Run(() => { Console.WriteLine("Progress_Bar_work() ended"); });
        }

        //
        // normal methods
        //

        //public void Encrypt_File_async(/*string dir_name, string key*/) {

        //    string dir_name = "R:\\Encrypt\\test";

        //    string key = "1234567890qwerty";
        //     //creating cancel token for progres bar
        //     //var cancel_token_src = new cancellationtokensource();
        //     //var cancel_token = cancel_token_src.token;

        //     ////starting progress bar work

        //     //try {
        //     //    var progress_bar_work = progress_bar_work(cancel_token);
        //     //}
        //     //catch (exception e) {

        //     //    console.writeline(e.message);
        //     //}

        //     Encryption encryption = new Encryption();
        //    encryption.dir_name = dir_name;
        //    encryption.encryption_key = key;
        //    encryption.encrypt();

        //    ////canceling progress bar thread finishing it and hiding
        //    //cancel_token_src.cancel();
        //    //cancel_token_src.dispose();
        //    //dispatcher.invoke(new action(async () => {

        //    //    progress_bar.value = 49;
        //    //    await task.delay(200);
        //    //    progress_bar.value = 50;
        //    //    await task.delay(250);

        //    //    encryptbt.isenabled = true;
        //    //    decryptbt.isenabled = true;
        //    //    progress_barl.visibility = visibility.hidden;
        //    //    progress_bar.visibility = visibility.hidden;
        //    //    //progress_bar.isindeterminate = false;

        //    //}));

        //    //return Task.Run(() => { Console.WriteLine("encrypt_file_async(string dir_name, string key) ended"); });
        //}

        public Task Encrypt_File_async(/*string dir_name, string key*/) {

            return Task.Run(() => {

                Console.WriteLine("public Task Encrypt_File_async(/*string dir_name, string key*/) started");

                string dir_name = "R:\\Encrypt\\test";

                string key = "1234567890qwerty";

                //creating cancel token for progres bar
                //var cancel_token_src = new CancellationTokenSource();
                //var cancel_token = cancel_token_src.Token;

                ////starting progress bar work

                //try {
                //    var progress_bar_work = Progress_Bar_work(cancel_token);
                //}
                //catch (Exception e) {

                //    Console.WriteLine(e.Message);
                //}

                Encryption encryption = new Encryption();
                encryption.dir_name = dir_name;
                encryption.encryption_key = key;
                encryption.encrypt();

                ////canceling progress bar thread finishing it and hiding
                //cancel_token_src.Cancel();
                //cancel_token_src.Dispose();
                //Dispatcher.Invoke(new Action(async () => {

                //    progress_Bar.Value = 49;
                //    await Task.Delay(200);
                //    progress_Bar.Value = 50;
                //    await Task.Delay(250);

                //    encryptBT.IsEnabled = true;
                //    decryptBT.IsEnabled = true;
                //    progress_BarL.Visibility = Visibility.Hidden;
                //    progress_Bar.Visibility = Visibility.Hidden;
                //    //progress_Bar.IsIndeterminate = false;

                //}));

                Console.WriteLine("Encrypt_File_async(string dir_name, string key) ended");

            });

        }


    }
}

