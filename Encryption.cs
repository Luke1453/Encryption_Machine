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
        public string encryption_key_str { get; set; }
        private byte[] encryption_key;

        private string storage_dir;

        private string zip_name;
        private string zip_path;

        public string encrypted_path { get; set; }
        private string encrypted_name;

        private string md5_name;
        private string md5_path;
        public string md5_string { get; set; }

        public string decrypted_storage_path { get; set; }

        public string error_msg = null;

        public Encryption(string dir_name, string encryption_key_str) {
            
            this.dir_name = dir_name;
            this.encryption_key_str = encryption_key_str;
            encryption_key = Encoding.ASCII.GetBytes(encryption_key_str);

        }

        public void encrypt() {

            //managing all filenames
            storage_dir = dir_name + "_" + DateTime.Now.ToString("s").Replace(":", "_");
            zip_name = storage_dir + ".zip";

            zip_path = Path.Combine(storage_dir, Path.GetFileName(zip_name));
            zip_name = Path.GetFileName(zip_path);

            encrypted_name = Path.ChangeExtension(zip_name, null) + "_encrypted.txt";
            encrypted_path = Path.Combine(storage_dir, encrypted_name);

            md5_name = Path.ChangeExtension(zip_name, null) + "_MD5.txt";
            md5_path = Path.Combine(storage_dir, md5_name);

            //creating directory to store encrypted file, md5 and zipping directory to be encrypted
            try {

                Directory.CreateDirectory(storage_dir);
                ZipFile.CreateFromDirectory(dir_name, zip_path);
                
            }catch(Exception ex) {

                Console.WriteLine(ex.ToString());
            }

            //reading bytes from zipfile
            byte[] plainbytes = File.ReadAllBytes(zip_path);

            //encrypting and saving bytes to txt file, setting txt file to read-only
            byte[] encrypted_byte_arr = encrypt_bytes(plainbytes, encryption_key, encryption_key);
            File.WriteAllBytes(encrypted_path, encrypted_byte_arr);
            File.SetAttributes(encrypted_path, FileAttributes.ReadOnly);

            //calculating md5 and saving it to txt file, setting txt file to read-only
            byte[] encrypted_byte_arr_hash = md5_hash(encrypted_byte_arr);
            File.WriteAllBytes(md5_path, encrypted_byte_arr_hash);
            File.SetAttributes(md5_path, FileAttributes.ReadOnly);

            //deleting zip archive
            File.Delete(zip_path);

            md5_string = byte_arr_ToString(encrypted_byte_arr_hash);

        }

        public void decrypt() {

            //getting filenames in storage directory
            storage_dir = dir_name;
            string[] storage_dir_files = Directory.GetFiles(storage_dir);

            //saving filenames
            if (storage_dir_files.Length == 2) {
                foreach (string file in storage_dir_files) {

                    if (file.Contains("MD5.txt")) {
                        md5_path = file;
                    }
                    else if (file.Contains("encrypted.txt")) {
                        encrypted_path = file;
                    }
                    else {
                        error_msg = "Wrong structure of encrypted directory";
                        return;
                    }

                }
            }
            else {
                error_msg = "Wrong structure of encrypted directory";
                return;
            }

            //reading bytes from encrypted and md5 files
            byte[] encrypted_md5 = File.ReadAllBytes(md5_path);
            byte[] encrypted_byte_arr = File.ReadAllBytes(encrypted_path);

            //hashing ecrypted byte array and checking if hashed match
            byte[] encrypted_byte_arr_md5 = md5_hash(encrypted_byte_arr);
            if (!encrypted_byte_arr_md5.SequenceEqual(encrypted_md5)) {
                error_msg = "MD5 values dont match";
                return;
            }

            //deleting garbage
            encrypted_md5 = null;
            encrypted_byte_arr_md5 = null;

            //decrypting 
            byte[] decrypted_byte_arr = decrypt_bytes(encrypted_byte_arr, encryption_key, encryption_key);

            //figuring out filenames and creating zip file
            string decrypted_zip_name = storage_dir.Substring(storage_dir.LastIndexOf("\\") + 1) + "_decrypted.zip";
            string decrypted_zip_path = Path.Combine(storage_dir, decrypted_zip_name);
            decrypted_storage_path = decrypted_zip_path.Remove(decrypted_zip_path.Length - 4, 4);
            File.WriteAllBytes(decrypted_zip_path, decrypted_byte_arr);

            //creating directory to store decrypted files, unzipping files to that directory and deleting zip file
            Directory.CreateDirectory(decrypted_storage_path);
            ZipFile.ExtractToDirectory(decrypted_zip_path, decrypted_storage_path);
            File.Delete(decrypted_zip_path);

        }


        private static byte[] encrypt_bytes(byte[] byte_arr, byte[] Key, byte[] IV) {

            byte[] encrypted_byte_arr;

            using (RijndaelManaged rijAlg = new RijndaelManaged()) {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        
                        using (BinaryWriter bwEncrypt = new BinaryWriter(csEncrypt)) {

                            bwEncrypt.Write(byte_arr);

                        }

                        encrypted_byte_arr = msEncrypt.ToArray();

                    }
                }
            }

            return encrypted_byte_arr;
        }


        private static byte[] decrypt_bytes(byte[] encrypted_byte_arr, byte[] Key, byte[] IV) {

            byte[] decrypted_byte_arr = null;

            using (RijndaelManaged rijAlg = new RijndaelManaged()) {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encrypted_byte_arr)) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (BinaryReader brDecrypt = new BinaryReader(csDecrypt)) {

                            using (MemoryStream ms = new MemoryStream()) {

                                brDecrypt.BaseStream.CopyTo(ms);
                                decrypted_byte_arr = ms.ToArray();

                            }
                        }
                    }
                }
            }

            return decrypted_byte_arr;
        }


        private static byte[] md5_hash(byte[] bytearr) {

            byte[] hash;
            using (var md5 = MD5.Create()) {
                md5.TransformFinalBlock(bytearr, 0, bytearr.Length);
                hash = md5.Hash;
            }

            return hash;
        }


        public static string byte_arr_ToString(byte[] bytes) {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString("X2"));

            return result.ToString();
        }
    }
}
