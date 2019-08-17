using bootstrap_downloader;
using Ionic.Zip;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static System.Environment;

namespace RadiumBootstrapper
{
    class Program
    {
       

        static bool windows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        static bool linux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        static bool osx = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        /// <summary>
        /// location of the radium blockchain
        /// </summary>
        static string blockchain_location;
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if(windows)
                blockchain_location = GetFolderPath(SpecialFolder.ApplicationData) + "/Radium";
            if (linux)
                blockchain_location = GetFolderPath(SpecialFolder.UserProfile) + "/.radium";
            if (osx)
                blockchain_location = "~/Library/Application Support/radium";


            Welcome.print_welcome();
            switch (Console.ReadKey().Key)
            {
                //case ConsoleKey.F1:
                   // InstallWallet.run(blockchain_location);
                    //break;
                case ConsoleKey.F2:
                    InstallBootstrap.run(blockchain_location);
                    break;
                default:
                    goodbye.print_goodbye();
                    break;


            }


        }

     


    }

}

