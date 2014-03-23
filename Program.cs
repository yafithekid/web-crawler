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
        struct queryElmt
        {
            public string url;
            public string folderName;

        }
        static List<queryElmt> queryURLs;
        static bool forceCrawl = true;
        static void loadQueryURL()
        {
            queryURLs = new List<queryElmt>();
            WordProcessor wp = new WordProcessor(Configuration.getCrawlerListLocation());
            while (!wp.isEOF())
            {
                queryElmt tmp;
                tmp.url = wp.getWord();
                wp.advanceWord();
                tmp.folderName = wp.getWord();
                wp.advanceWord();
                queryURLs.Add(tmp);
            }
            wp.closeFile();
        }        

        static void Main(string[] args)
        {
            Configuration.setDefaultConfiguration();
            Crawler.initIgnoredExtension();
            Index.initIndexTagList();
            //Configuration.setTraversalMode("DFS");
            Configuration.setMaximumDepth(2);

            loadQueryURL();
            foreach(var queryURL in queryURLs)
            {
                break;
                Crawler.doCrawler(queryURL.url, Configuration.getTraversalMode(), queryURL.folderName,Configuration.getMaximumDepth());
                Crawler.saveToIndex();
                Crawler.saveResultToFile();
            }
            Console.WriteLine("crawler complete");
            List<Crawler.crawlElement> explorerResults =  Explorer.explore("hello", "kuliah", 10);
            foreach (var explorerResult in explorerResults)
            {
                Console.WriteLine(explorerResult.url);
            }
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
