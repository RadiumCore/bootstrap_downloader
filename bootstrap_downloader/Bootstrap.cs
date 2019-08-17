using System;
using System.Collections.Generic;
using System.Text;

namespace bootstrap_downloader
{
    static class Bootstrap
    {
        public static void Print_bootstrap()
        {
            Console.WriteLine("******        This utility will build a (nearly)          ******");
            Console.WriteLine("******    up-to-date bootstrap.dat to speed up syncing    ******");
            Console.WriteLine("******     by downloading missing chunks from github.     ******");
            Console.WriteLine("******   progress is maintained if the utility is closed  ******");
            Console.WriteLine("******                                                    ******");
            Console.WriteLine("******   This utility will require about 800mb of space   ******");
            Console.WriteLine("******          And a working Internet connection         ******");
            Console.WriteLine("******                                                    ******");
            Console.WriteLine("******           Press the Y key to continue              ******");
            Console.WriteLine("******         Press the any other key to exit            ******");
        }
    }
}
