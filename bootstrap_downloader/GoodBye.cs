using System;
using System.Collections.Generic;
using System.Text;

namespace bootstrap_downloader
{
    static class goodbye
    {
        public static void print_goodbye()
        {
            Console.Clear();
            Console.WriteLine("****************************************************************");
            Console.WriteLine("******     RadiumCore Instalation and  Update Utility     ******");
            Console.WriteLine("******                                                    ******");
            Console.WriteLine("******                                                    ******");
            Console.WriteLine("******        Goodbye! Hope to see you again soon !       ******");
            Console.WriteLine("******               Press any key to  exit               ******");
            Console.WriteLine("******                                                    ******");
            Console.WriteLine("******          Copyright 2019 RadiumCore.org             ******");
            Console.WriteLine("******                 All rights reserved                ******");
            Console.WriteLine("******                                                    ******");
            Console.WriteLine("******                                                    ******");
            Console.WriteLine("******                                                    ******");
            Console.WriteLine("****************************************************************");
            Console.ReadKey();
            Environment.Exit(0);

            }
    }
}
