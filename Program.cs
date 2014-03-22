using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP = HtmlAgilityPack;

/**
 * Warning : walaupun pake namespace, class, method dll
 * sebenarnya program ini tidak dirancang object-oriented
 * tapi prosedural. methodnya static semua.
 */
namespace WebCrawler
{
    class Program
    {
        static List<string> queryURLs;
        static string queryURLFileName;
        static bool forceCrawl = true;
        static void loadQueryURL()
        {
            queryURLs = new List<string>();
            WordProcessor wp = new WordProcessor(queryURLFileName);
            
            while (!wp.isEOF())
            {
                queryURLs.Add(wp.getWord());
                wp.advanceWord();
            }
            queryURLs.Add(wp.getWord());
        }

        static void Main(string[] args)
        {

            //Crawler.initIgnoredExtension();
            queryURLFileName = "crawlerList.conf"; loadQueryURL();
            foreach(var queryURL in queryURLs)
            {
                Console.WriteLine(queryURL);
                //Console.WriteLine(testConnection(queryURL));
                if (Program.forceCrawl || testConnection(queryURL))
                    Crawler.doCrawler(queryURL, "BFS", 3);
            }
            Console.WriteLine("ok");
            while (true) ;
        }

        public static bool testConnection(string s)
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (var stream = client.OpenRead(s))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

}
