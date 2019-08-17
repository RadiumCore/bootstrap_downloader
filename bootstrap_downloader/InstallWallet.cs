using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace bootstrap_downloader
{
    class InstallWallet
    {
        public static void run(string _blockchain_location)
        {

        }

        private static void build_remote_git_file_list()
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla / 4.0(compatible; MSIE 6.0; Windows NT 5.2;)");
            string repo_str = client.DownloadString("https://api.github.com/repos/RadiumCore/radium-0.11/contents");
            JArray repo = JArray.Parse(repo_str);
            foreach (JObject chunk in repo)
            {
                if (chunk["name"].ToString() == "README.md")
                    continue;
                string ch = chunk["name"].ToString().Remove(chunk["name"].ToString().Length - 4);
                Console.SetCursorPosition(0, Console.CursorTop);
               

            }
            Console.WriteLine("..Complete");

        }
    }
}
