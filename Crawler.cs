using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using HAP = HtmlAgilityPack;

namespace WebCrawler
{
    
    public class Crawler
    {
        private static int counter = 0;
        public static string root;
        public static string folder;

        private static Dictionary<string, bool> isVisited = new Dictionary<string, bool>();
        private static Queue<crawlElement> bfsQueue = new Queue<crawlElement>();
        private static HashSet<string> ignoredExtension = new HashSet<string>();

        public static List<crawlElement> crawlResult;

        
        
        public class crawlElement
        {
            public string url = null;
            public string fileName=null;
            public string pageTitle = null;
            public string content = null;
            public List<bool> haveKey;

            public crawlElement() {}
            public crawlElement(string url) { this.url = url; }
            public crawlElement(string url, string fileName) { this.url = url; this.fileName = fileName; }
            public void initListBool(int n) {
                haveKey = new List<bool>();
                for (int i = 0; i < n; i++)
                {
                    haveKey.Add(false);
                }
            }
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
                doc.Load(Configuration.getSavedPagesLocation() + Crawler.folder + "/" + P.fileName);

                var a_nodes = getAllTag(doc, "a");
                C.Clear();
                foreach (var a_node in a_nodes)
                {
                    string page = getLink(P.url, a_node.GetAttributeValue("href", ""));
                    //Console.WriteLine("Here" + a_node.InnerText.ToString());
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
            Crawler.crawlResult = new List<crawlElement>();
        }

        public static void runCrawler(string mode,int depth)
        {
            clearData();
            crawlElement first = new crawlElement(Crawler.root);
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

            if (saveContent(ref parent)) Crawler.crawlResult.Add(parent);
            Console.WriteLine(parent.url + "\nsaved in : " + parent.fileName);

            List<crawlElement> childs = new List<crawlElement>();

            if (depth > 1){
                expandLink(ref parent, ref childs);
                foreach (var child in childs)
                {
                    if (!isVisited.ContainsKey(child.url))
                    {
                        isVisited[child.url] = true;
                        DFS(child, depth - 1);
                    }
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

                if (saveContent(ref parent)) Crawler.crawlResult.Add(parent);
                Console.WriteLine(parent.url + "\n at depth = " + depth +" saved in : " + parent.fileName);

                List<crawlElement> childs = new List<crawlElement>();
                if (depth == 0) return; else expandLink(ref parent, ref childs);
                foreach (var child in childs)
                {
                    //Console.WriteLine("continue");
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
            if (depth > 1)
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
        public static bool saveContent(ref crawlElement ce)
        {
            using (var client = new System.Net.WebClient())
            {
                allocateFile(ref ce);
                try
                {
                    client.DownloadFile(ce.url, Configuration.getSavedPagesLocation() + Crawler.folder + "/" + ce.fileName);
                    HAP.HtmlDocument doc = new HAP.HtmlDocument();
                    doc.Load(Configuration.getSavedPagesLocation() + Crawler.folder + "/" + ce.fileName);
                    
                    List<HAP.HtmlNode> t_nodes = getAllTag(doc, "title");
                    if (t_nodes.Count == 0) t_nodes = getAllTag(doc, "h1");
                    if (t_nodes.Count == 0) t_nodes = getAllTag(doc, "h2");
                    if (t_nodes.Count == 0) t_nodes = getAllTag(doc, "h3");
                    ce.pageTitle = "-Untitled Pages-";
                    foreach (var t_node in t_nodes)
                    {
                        ce.pageTitle = t_node.InnerHtml.ToString();

                    }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
        public static void initIgnoredExtension()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(Configuration.getIgnoredExtensionLocation());
            string line;
            while ((line = file.ReadLine()) != null)
            {
                ignoredExtension.Add(line);
            }
            file.Close();
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
        public static void doCrawler(string s,string mode,string savefoldername,int depth)
        {
            Crawler.root = s;
            Crawler.folder = savefoldername;
            Crawler.createDirectory(Configuration.getSavedPagesLocation() + savefoldername);
            Crawler.runCrawler(mode, depth);

        }
        public static void createDirectory(string s)
        {
            if (!Directory.Exists(s))
            {
                Directory.CreateDirectory(s);
            }
        }

        public static void saveToIndex()
        {
            string saveFolderName = Configuration.getIndexLocation() + Crawler.folder +"/";
            if (!Directory.Exists(saveFolderName))
            {
                Directory.CreateDirectory(saveFolderName);
            }
            foreach (var ce in Crawler.crawlResult)
            {
                IndexElement ie = new IndexElement(ce.url, ce.fileName);
                ie.createWords();

                System.IO.StreamWriter printer = new System.IO.StreamWriter(ie.indexFileLocation);
                foreach (var word in ie.words)
                {
                    printer.WriteLine(word);
                }
                printer.Close();

                Console.WriteLine("saved to " + ie.indexFileLocation);
            }
        }
        public static string getAbsolutePath(string relative)
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(dir, relative);
        }
        public static void saveResultToFile()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(Configuration.getCrawlResultLocation() + Crawler.folder + ".result");
            foreach (var celmt in Crawler.crawlResult)
            {
                Console.WriteLine(celmt.url);
                celmt.url = celmt.url.Replace("\n", "");  sw.WriteLine(celmt.url);
                celmt.fileName = celmt.fileName.Replace("\n", ""); sw.WriteLine(celmt.fileName);
                celmt.pageTitle = celmt.pageTitle.Replace("\n", ""); sw.WriteLine(celmt.pageTitle);
            }
            sw.Close();
        }
    }
}
