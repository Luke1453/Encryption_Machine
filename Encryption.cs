using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Encryption_Machine {

    class Encryption {


        public string dir_name { get; set; }
        public string encryption_key { get; set; }

        private byte[] dummyIV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private string storage_dir;

        private string zip_name;
        private string zip_path;

        private string encrypted_path;
        private string encrypted_name;

        private string md5_name;
        private string md5_path;

        public void encrypt() {

            storage_dir = dir_name + "_" + DateTime.Now.ToString("s").Replace(":", "_");
            zip_name = storage_dir + ".zip";

            zip_path = Path.Combine(storage_dir, Path.GetFileName(zip_name));
            zip_name = Path.GetFileName(zip_path);

            encrypted_name = Path.ChangeExtension(zip_name, null) + "_encrypted.txt";
            encrypted_path = Path.Combine(storage_dir, encrypted_name);

            md5_name = Path.ChangeExtension(zip_name, null) + "_MD5.txt";
            md5_path = Path.Combine(storage_dir, md5_name);

            try {

                Directory.CreateDirectory(storage_dir);
                ZipFile.CreateFromDirectory(dir_name, zip_path);
                
            }catch(Exception ex) {

                Console.WriteLine(ex.ToString());
            }

            byte[] plainbytes;

            using (FileStream fs = new FileStream(zip_path, FileMode.Open, FileAccess.Read)) {
                
                // Create a byte array of file stream length
                plainbytes = File.ReadAllBytes(zip_path);
                
                //Read block of plainbytes from stream into the byte array
                fs.Read(plainbytes, 0, Convert.ToInt32(fs.Length));
                
                //Close the File Stream
                fs.Close();
               
            }

            RijndaelManaged aesAlg = new RijndaelManaged {
                    Key = Encoding.ASCII.GetBytes(encryption_key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.Zeros,
                    KeySize = 128,
                    BlockSize = 128,
                    IV = dummyIV
                };

            ICryptoTransform encryptor = aesAlg.CreateEncryptor();

            var encrypted_byte_arr = encryptor.TransformFinalBlock(plainbytes, 0, plainbytes.Length);

            File.WriteAllBytes(encrypted_path, encrypted_byte_arr);

            byte[] hash;
            using (var md5 = MD5.Create()) {
                md5.TransformFinalBlock(encrypted_byte_arr, 0, encrypted_byte_arr.Length);
                hash = md5.Hash;
            }

            using (FileStream fs = new FileStream(md5_path, FileMode.Create, FileAccess.Write)) {

                fs.Write(hash, 0, hash.Length);
                fs.Close();

            }

            File.Delete(zip_path);
            plainbytes = null;
            encrypted_byte_arr = null;
        }
    }

}
