using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP=HtmlAgilityPack;
namespace WebCrawler
{
    public class Explorer
    {
        /*
        private string fileName;
        private string url;
        private Dictionary<string,bool> queryWords;
        private WordProcessor wordProcessor;
        
        public Explorer(string url,string fileName, List<string> queryWords)
        {
            this.fileName = fileName;
            this.url = url;
            this.queryWords = new Dictionary<string,bool>();
            foreach (var word in queryWords)
            {
                this.queryWords.Add(word, false);
            }
            this.wordProcessor = new WordProcessor(fileName);
        }
        
        public void explore()
        {
            while (!wordProcessor.isEOF()){
                foreach(var queryWord in queryWords)
                {
                    queryWords[queryWord.Key] = queryWords[queryWord.Key] || queryWord.Key == wordProcessor.getWord();
                }
                wordProcessor.advanceWord();
            }
        }*/

        public static void getCrawlResult(string folderName,ref List<Crawler.crawlElement> retval)
        {
            retval = new List<Crawler.crawlElement>();
            WordProcessor wp = new WordProcessor(Configuration.getCrawlResultLocation() + folderName + ".result");

            retval = new List<Crawler.crawlElement>();

            while (!wp.isEOF()){
                string url = wp.getWord();
                wp.advanceWord();
                string fileName = wp.getWord();
                wp.advanceWord();
                wp.advanceWord();
                retval.Add(new Crawler.crawlElement(url,fileName));
            }

            wp.closeFile();
        }
        public static List<Crawler.crawlElement> explore(string folderName,string query,int maxSize)
        {
            List<string> qWords = query.Split(' ').ToList();
            List<Crawler.crawlElement> crawlResults = new List<Crawler.crawlElement>();
            getCrawlResult(folderName, ref crawlResults);
            for(int i = 0; i < crawlResults.Count; ++i)
            {
                crawlResults[i].initListBool(qWords.Count);
                string indexPath = Configuration.getIndexLocation() + folderName + "/" + crawlResults[i].fileName;
                indexPath = indexPath.Remove(indexPath.Length - 5); indexPath += ".index";
                
                WordProcessor wp = new WordProcessor(indexPath);
                while (!wp.isEOF())
                {
                    for (int j = 0; j < qWords.Count; ++j)
                    {
                        crawlResults[i].haveKey[j] = crawlResults[i].haveKey[j] || (wp.getWord().ToUpper() == qWords[j].ToUpper());
                    }
                    wp.advanceWord();
                }
                for (int j = 0; j < qWords.Count; ++j)
                {
                    crawlResults[i].haveKey[j] = crawlResults[i].haveKey[j] || (wp.getWord().ToUpper() == qWords[j].ToUpper());
                }
                wp.closeFile();
            }
            filterResult(ref crawlResults);
            return crawlResults;
        }
        public string getContent(ref HAP.HtmlDocument doc,ref List<string> qWords)
        {
            return "";
        }
        public static void filterResult(ref List<Crawler.crawlElement> list)
        {
            List<Crawler.crawlElement> retval = new List<Crawler.crawlElement>();
            foreach(var lElmt in list)
            {
                if (atLeastOneTrue(lElmt))
                    retval.Add(lElmt);
            }
            list = retval;
        }
        public static bool atLeastOneTrue(Crawler.crawlElement list)
        {
            foreach(var keyElmt in list.haveKey){
                if (keyElmt == true) return true;
            }
            return false;
        }

    }
    
}
