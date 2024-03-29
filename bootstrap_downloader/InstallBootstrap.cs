﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

using Ionic.Zip;

namespace bootstrap_downloader
{
    static class InstallBootstrap
    {
        //Lists of existing chuncks
        /// <summary>
        /// list of chuncks that exist in local blk0001.dat
        /// </summary>
       private static Dictionary<string, string> appdata_blk001_chuncks_list = new Dictionary<string, string>();
        /// <summary>
        /// list of chuncks that exist in local bootstrap.dat
        /// </summary>
        private static Dictionary<string, string> appdata_bootstrap_dat_chuncks_list = new Dictionary<string, string>();
        /// <summary>
        /// list of chuncks that exists in local bootstrap.dat.old
        /// </summary>
        private static Dictionary<string, string> appdata_bootstrap_dat_old_chuncks_list = new Dictionary<string, string>();
        /// <summary>
        /// list of chuncks that exists on github.
        /// </summary>
        private static Dictionary<string, string> remote_git_chunk_list = new Dictionary<string, string>();

        //lists of chuncks that need manipulated
        /// <summary>
        /// list of chuncks to be copied from blk0001.dat
        /// </summary>
        private static Dictionary<string, string> to_copy_blk0001_chunks = new Dictionary<string, string>();
        /// <summary>
        /// list of chuncks to be copied from bootstrap.old
        /// </summary>
        private static Dictionary<string, string> to_copy_old_chunks = new Dictionary<string, string>();
        /// <summary>
        /// list of chuncks we dont have that need downloaded
        /// </summary>
        private static Dictionary<string, string> to_download_chunks = new Dictionary<string, string>();

        /// <summary>
        /// standard chunck size, set to 2mb
        /// </summary>
        static int chunk_size = 2097152;

        private static string blockchain_location;

        public static void run(string _blockchain_location){
            blockchain_location = _blockchain_location;

            Console.SetCursorPosition(0, Console.CursorTop);
            build_appdata_blk001_chunk_list();
            build_appdata_bootstrap_dat_chunk_list();
            build_appdata_bootstrap_dat_old_chunk_list();
            build_remote_git_file_list();


            build_required_chunk_list();
            build_bootstrap();
            Console.WriteLine("Successfully built new Bootstrap.dat! You may now launch your Radium Wallet.");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            Environment.Exit(0);
        }


        private static void build_appdata_blk001_chunk_list()
        {
            appdata_blk001_chuncks_list.Clear();
            String file_path = blockchain_location + "/blk0001.dat";
            appdata_blk001_chuncks_list = List_Builder(file_path, "Indexing local blockchain, stage 1 of 3...");
        }
        private static void build_appdata_bootstrap_dat_chunk_list()
        {
            appdata_bootstrap_dat_chuncks_list.Clear();
            String file_path = blockchain_location + "/bootstrap.dat";
            appdata_bootstrap_dat_chuncks_list = List_Builder(file_path, "Indexing local blockchain, stage 2 of 3...");
        }
        private static void build_appdata_bootstrap_dat_old_chunk_list()
        {
            appdata_bootstrap_dat_old_chuncks_list.Clear();
            String file_path = blockchain_location + "/bootstrap.dat.old";
            appdata_bootstrap_dat_old_chuncks_list = List_Builder(file_path, "Indexing local blockchain, stage 3 of 3...");
        }
        private static void build_remote_git_file_list()
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla / 4.0(compatible; MSIE 6.0; Windows NT 5.2;)");
            string repo_str = client.DownloadString("https://api.github.com/repos/JJ12880/Radium_Bootstrap_Chunks/contents");
            JArray repo = JArray.Parse(repo_str);
            foreach (JObject chunk in repo)
            {
                if (chunk["name"].ToString() == "README.md")
                    continue;
                remote_git_chunk_list.Add(chunk["name"].ToString().Remove(chunk["name"].ToString().Length - 4), "");
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Indexing remote git files....................processing file {0} of {1}", remote_git_chunk_list.Count, repo.Count);


            }
            Console.WriteLine("..Complete");

        }
        private static void build_required_chunk_list()
        {

            int to_copy = 0;
            int to_download = 0;

            foreach (string remote_chunk in remote_git_chunk_list.Keys)
            {
                //check if chunk allready exists in local bootstrap.dat
                if (Have_bootstrap_chunk(remote_chunk))
                    continue;
                //if chunk exists in bootstrap.old, add it to work que to copy later. 
                if (Have_bootstrap_old_chunk(remote_chunk))
                {
                    to_copy += 1;
                    to_copy_old_chunks.Add(remote_chunk, "");
                    continue;
                }
                //if chunk exists in blk0001, add it to work que to copy later. 
                if (Have_blk001_chunk(remote_chunk))
                {
                    to_copy += 1;
                    to_copy_blk0001_chunks.Add(remote_chunk, "");
                    continue;
                }

                //Uh-oh, we dont have a local copy of the chunk we need. 
                to_download += 1;
                to_download_chunks.Add(remote_chunk, "");
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Building worklist.......{0} local chunks to copy, {1} to download", to_copy, to_download);
            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Building worklist.......{0} local chunks to copy, {1} to download. COMPLETE!", to_copy, to_download);

        }

        private static void build_bootstrap()
        {
            int count = 0;
            int to_copy = to_copy_blk0001_chunks.Count + to_copy_old_chunks.Count;
            foreach (string chunk in to_copy_blk0001_chunks.Keys)
            {
                int chunk_offset = int.Parse(chunk.ToString().Substring(0, chunk.ToString().IndexOf("_")));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Copying local chunk {0} of {1} {2}", count, to_copy, chunk.ToString());
                WriteChunk(get_chunk(chunk, "/blk0001.dat"), chunk_offset);
                count += 1;

            }



            foreach (string chunk in to_copy_old_chunks.Keys)
            {
                int chunk_offset = int.Parse(chunk.ToString().Substring(0, chunk.ToString().IndexOf("_")));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Copying chunk {0} of {1} {2}", count, to_copy, chunk.ToString());
                WriteChunk(get_chunk(chunk, "/bootstrap.dat.old"), chunk_offset);

            }

            count = 0;
            Console.WriteLine("Copied {0} local chunks.................................. COMPLETE! ", count);

            foreach (string chunk in to_download_chunks.Keys)
            {
                int chunk_offset = int.Parse(chunk.ToString().Substring(0, chunk.ToString().IndexOf("_")));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Downloading remote chunk {0} of {1} {2}", count, to_download_chunks.Count, chunk.ToString());
                WriteChunk(get_remote_chunk(chunk), chunk_offset);
                count += 1;

            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Copied {0} chunks.................................. COMPLETE!                                             ", count);
            count = 0;

        }

        private  static byte[] get_remote_chunk(string chunk)
        {
            byte[] chunk_bytes;
            string chunk_hash = chunk.Remove(0, chunk.IndexOf("_") + 1);
            WebClient wc = new WebClient();
            SHA256CryptoServiceProvider md5prov = new SHA256CryptoServiceProvider();

            using (MemoryStream compressed_MS = new MemoryStream(wc.DownloadData("https://raw.githubusercontent.com/JJ12880/Radium_Bootstrap_Chunks/master/" + chunk + ".zip")))
            {
                
                //extract downloaded file
                using (ZipFile zip = ZipFile.Read(compressed_MS))
                    zip.ExtractAll(blockchain_location, ExtractExistingFileAction.OverwriteSilently);
            }

            //open downloaded file, read it to byte[]
            using (FileStream fs = File.OpenRead(blockchain_location + "/" + chunk))
            {
                chunk_bytes = new byte[fs.Length];
                fs.Read(chunk_bytes, 0, (int)fs.Length);

            }

            //check downloaded chunk matches expected hash
            if (chunk_hash != ByteArrayToString(md5prov.ComputeHash(chunk_bytes)))
                throw new Exception("Chunk hash does not match expected");
            File.Delete(blockchain_location + "/" + chunk);
            return chunk_bytes;
        }



        /// <summary>
        /// returns a chunk 
        /// </summary>
        /// <param name="chunk">chunk requested</param>
        /// <param name="file">file to get chunk from</param>
        /// <returns>byte[] of requested chunk</returns>
        private  static byte[] get_chunk(string chunk, string file)
        {

            byte[] chunk_bytes = new byte[chunk_size - 1];
            int chunk_offset = int.Parse(chunk.ToString().Substring(0, chunk.ToString().IndexOf("_")));
            string expected_hash = chunk.Remove(0, chunk.IndexOf("_") + 1);
            string actual_hash;
            WebClient wc = new WebClient();
            SHA256CryptoServiceProvider md5prov = new SHA256CryptoServiceProvider();
            byte[] input = new byte[chunk_size];

            using (FileStream fs = File.OpenRead(blockchain_location + file))
            {
                BigInteger filelenght = fs.Length;
                fs.Seek(chunk_offset, SeekOrigin.Begin);
                fs.Read(input, 0, chunk_size);
                actual_hash = ByteArrayToString(md5prov.ComputeHash(input));

            }

            if (expected_hash == actual_hash)
                return input;

            //something went wrong, and we did not get the chunk we expected
            //we will just have to download this chunk, as we cant copy it
            to_download_chunks.Add(chunk, "");
            throw new Exception("Chunk hash does not match expected");


        }
        private static void WriteChunk(byte[] chunk, int offset)
        {
            if (!File.Exists(blockchain_location + "/bootstrap.dat"))
                File.Create(blockchain_location + "/bootstrap.dat").Close();
            using (FileStream fs = File.OpenWrite(blockchain_location + "/bootstrap.dat"))
            {
                fs.Seek(offset, SeekOrigin.Begin);
                fs.Write(chunk, 0, chunk.Length);
            }

        }




        private static Dictionary<string, string> List_Builder(string filepath, string message)
        {
            int count = 0;
            Dictionary<string, string> list = new Dictionary<string, string>();
            if (!File.Exists(filepath))
            {
                Console.WriteLine(message + "found {0} chunks. COMPLETE!", count);
                return new Dictionary<string, string>();
            }

            using (FileStream fs = File.OpenRead(filepath))
            {
                BigInteger filelength = fs.Length;
                BigInteger offset = 0;

                SHA256CryptoServiceProvider md5prov = new SHA256CryptoServiceProvider();
                string hash;
                byte[] input;
                while (filelength - offset >= chunk_size)
                {
                    input = new byte[chunk_size];
                    fs.Read(input, 0, chunk_size);
                    hash = ByteArrayToString(md5prov.ComputeHash(input));
                    list.Add(offset.ToString() + "_" + hash, "");

                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(message + "found {0} chunks so far", count);

                    offset = offset + chunk_size;
                    count += 1;
                }

                input = new byte[(int)(filelength - offset)];
                fs.Read(input, 0, (int)(filelength - offset));
                hash = ByteArrayToString(md5prov.ComputeHash(input));
                list.Add(offset.ToString() + "_" + hash, "");
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine(message + "found {0} chunks. COMPLETE!", count);



            }
            return list;
        }

        private static string ByteArrayToString(byte[] data)
        {
            string hex = BitConverter.ToString(data);
            return hex.Replace("-", "").ToLower();
        }



        /// <summary>
        /// checks if chunk exists in local blk0001.dat
        /// </summary>
        /// <param name="chunk">sha256 hash of a chunk</param>
        /// <returns></returns>
        private static bool Have_blk001_chunk(string chunk)
        {
            if (appdata_blk001_chuncks_list.ContainsKey(chunk))
                return true;
            return false;

        }
        /// <summary>
        /// checks if chunk exists in local bootstrap.dat
        /// </summary>
        /// <param name="chunk">sha256 hash of a chunk</param>
        /// <returns></returns>
        private static bool Have_bootstrap_chunk(string chunk)
        {
            if (appdata_bootstrap_dat_chuncks_list.ContainsKey(chunk))
                return true;
            return false;

        }
        /// <summary>
        /// checks if chunk exists in local bootstrap.dat.old
        /// </summary>
        /// <param name="chunk">sha256 hash of a chunk</param>
        /// <returns></returns>
        private static bool Have_bootstrap_old_chunk(string chunk)
        {
            if (appdata_bootstrap_dat_old_chuncks_list.ContainsKey(chunk))
                return true;
            return false;

        }

        
    }
}
