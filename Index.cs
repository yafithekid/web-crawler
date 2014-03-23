using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HAP = HtmlAgilityPack;

namespace WebCrawler
{
    public class Index
    {
        public static int counter = 0;
        public string baseURL;
        public List<IndexElement> indexes;

        public static List<string> indexTags;
        public static void initIndexTagList()
        {
            indexTags = new List<string>();

            WordProcessor wp = new WordProcessor(Configuration.getIndexTagListLocation());
            while (!wp.isEOF())
            {
                indexTags.Add(wp.getWord());
                wp.advanceWord();
            }
            indexTags.Add(wp.getWord());
            wp.closeFile();
        }
    }
    
    public class IndexElement
    {
        string url;
        string htmlFileLocation;
        string fileName;
        public string indexFileLocation = null;

        public HashSet<string> words;
        public IndexElement() { }
        public IndexElement(string _url, string _fileName) {
            this.url = _url;
            this.fileName = _fileName;
            this.htmlFileLocation = Configuration.getSavedPagesLocation() + Crawler.folder + "/" + _fileName;
            _fileName = _fileName.Remove((_fileName.Length - 5));
            Console.WriteLine(_fileName);
            this.indexFileLocation = Configuration.getIndexLocation() + Crawler.folder + "/" + _fileName + ".index";
            this.words = new HashSet<string>();
        }

        public void createWords()
        {
            if (words.Count != 0) words.Clear();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            Console.WriteLine("lokasi = " + htmlFileLocation);
            doc.Load(htmlFileLocation);

            List<HAP.HtmlNode> nodes = new List<HAP.HtmlNode>();
            //Console.WriteLine(doc.DocumentNode.InnerText);
            string innerText = doc.DocumentNode.InnerText.ToString();
            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9,. -]");
            innerText = rgx.Replace(innerText, "");
            //Console.WriteLine(innerText);
            var innerTextParts = innerText.Split(' ',',','.','-');
            foreach(var part in innerTextParts)
            {
                //Console.WriteLine(part);
                words.Add(part);
            }
        }
        public void getWords()
        {
            if (words.Count != 0) words.Clear();
            WordProcessor wp = new WordProcessor(indexFileLocation);
            while (!wp.isEOF())
            {
                words.Add(wp.getWord());
                wp.advanceWord();
            }
            words.Add(wp.getWord());
            wp.closeFile();
        }
        public void printWords()
        {
            foreach (var word in words)
            {
                Console.WriteLine(word);
            }
        }

    }
}
