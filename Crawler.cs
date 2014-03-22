using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP = HtmlAgilityPack;

namespace WebCrawler
{
    
    class Crawler
    {
        public static int counter = 0;
        public static string root;
        public static string saveFolderName ="";

        private static Dictionary<string, bool> isVisited = new Dictionary<string, bool>();
        private static Queue<crawlElement> bfsQueue = new Queue<crawlElement>();

        public static List<crawlElement> crawlResult = new List<crawlElement>();
        public static HashSet<string> ignoredExtension = new HashSet<string>();
        
        public class crawlElement
        {
            public string url = null;
            public string fileName=null;
            public int depth;
            public crawlElement() {}
            public crawlElement(string url) { this.url = url; }
            public crawlElement(string url, int depth) { this.url = url; this.depth = depth; }
  
        }

        public static void addCrawlResult(string url)
        {
            crawlElement tmp = new crawlElement(url);
            allocateFile(ref tmp);
            crawlResult.Add(tmp);
        }

        public static void allocateFile(ref crawlElement c)
        {
            string filename = (++Crawler.counter) + ".html";
            c.fileName = filename;
        }

        /**
         * menggenerate semua link child P
         * P : (parent) link awal (format http://...)
         * C : (child) daftar link akhir
         * N : jumlah node yang diexpand
         **/
        public static void expandLink(ref crawlElement P,ref List<crawlElement> C)
        {
            try
            {
                var doc = new HAP.HtmlDocument();
                doc.Load(P.fileName);
                var a_nodes = getAllTag(doc, "a");

                C.Clear();
                foreach (var a_node in a_nodes)
                {
                    string page = getLink(P.url, a_node.GetAttributeValue("href", ""));
                    C.Add(new crawlElement(page));
                }
            }
            catch (Exception)
            {
            }
        }

        /**
         * Menghasilkan semua node (isi tag) dengan kode tag
         * doc : document yang dicari
         * tag : nilai tag
         * doc harus sudah di-load
         */

        public static List<HAP.HtmlNode> getAllTag(HAP.HtmlDocument doc,string tag)
        {
            List<HAP.HtmlNode> retval = doc.DocumentNode.Descendants(tag).ToList();
            return retval;
        }

        public static void clearData()
        {
            Crawler.counter = 0;
            Crawler.isVisited.Clear();
 
            Crawler.bfsQueue.Clear();
            Crawler.crawlResult.Clear();
        }

        public static void runCrawler(string mode,int depth)
        {
            clearData();
            crawlElement first = new crawlElement(Crawler.root, 1);
            if (mode == "DFS")
            {
                DFS(first, depth);
            }
            else
            {
                bfsQueue.Enqueue(first);
                BFS(depth);
            }
        }
        public static bool isValidLink(crawlElement ce)
        {
            bool retVal = (!isFile(ce.url)) && ce.url.StartsWith(Crawler.root);
            return retVal;
        }
        public static void DFS(crawlElement parent,int depth)
        {
            if (!isValidLink(parent)) return;

            saveContent(ref parent);
            Console.WriteLine(parent.url + "\nsaved in : " + parent.fileName);

            List<crawlElement> childs = new List<crawlElement>();
            if (depth == 0) return; else expandLink(ref parent, ref childs);
            foreach (var child in childs)
            {
                if (!isVisited.ContainsKey(child.url))
                {
                    isVisited[child.url] = true;
                    DFS(child, depth - 1);
                }
            }
        }
        public static void BFS(int depth)
        {
            Queue<crawlElement> tempQueue = new Queue<crawlElement>();
            while (bfsQueue.Count > 0)
            {
                crawlElement parent = bfsQueue.Dequeue();
                if (!isValidLink(parent)) continue;

                saveContent(ref parent);
                Console.WriteLine(parent.url + "\nsaved in : " + parent.fileName);

                List<crawlElement> childs = new List<crawlElement>();
                if (depth == 0) return; else expandLink(ref parent, ref childs);
                foreach (var child in childs)
                {
                    if (!isVisited.ContainsKey(child.url))
                    {
                        isVisited[child.url] = true;
                        tempQueue.Enqueue(child);
                    }
                }
            }

            while (tempQueue.Count > 0)
            {
                bfsQueue.Enqueue(tempQueue.Dequeue());
            }
            if (depth > 0)
                BFS(depth - 1);
        }
        /**
         * menghasilkan link absolute berdasarkan parent dan link sekarang
         * jika link sekarang bukan anggota dari link parent maka return link sekarang
         */
        public static string getLink(string parent, string relative)
        {
            if (relative.StartsWith("http:"))
                return relative;
            Uri baseUri = new Uri(parent, UriKind.Absolute);
            Uri u = new Uri(baseUri, relative);
            return u.ToString();
        }
        
        /**
         * jika status = false, maka link invalid
         */
        public static void saveContent(ref crawlElement ce)
        {
            using (var client = new System.Net.WebClient())
            {
                allocateFile(ref ce);
                try
                {
                    Console.WriteLine(saveFolderName + ce.fileName);
                    client.DownloadFile(ce.url, saveFolderName + ce.fileName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public static void initIgnoredExtension()
        {
            System.IO.StreamReader file = new System.IO.StreamReader("ignoredExtension.conf");
            string line;
            while ((line = file.ReadLine()) != null)
            {
                ignoredExtension.Add(line);
            }
        }
        public static bool isFile(string s)
        {
            s = s.ToUpper();
        
            foreach (var ext in ignoredExtension)
            {
                string extension = ("." + ext).Trim();
                if (s.Trim().EndsWith(extension))
                    return true;
            }
            return false;
        }
        public static void doCrawler(string s,string mode,int depth)
        {
            Crawler.root = s;
            Crawler.runCrawler("BFS", 2);
        }
    }
}
