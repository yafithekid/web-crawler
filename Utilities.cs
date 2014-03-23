using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebCrawler
{
    class Configuration
    {
        static string crawlerListLocation;
        static string ignoredExtensionLocation;
        static string indexTagListLocation;
        static string savedPagesLocation;
        static string indexLocation;
        static string crawlResultLocation;
        static string traversalMode;
        static bool enableParagraphOutput;
        static int maximumDepth;

        public static void setCrawlerListLocation(string s)
        {
            crawlerListLocation = s;
        }


        public static void setIgnoredExtensionLocation(string s)
        {
            ignoredExtensionLocation = s;
        }

        public static void setIndexTagListLocation(string s)
        {
            indexTagListLocation = s;
        }

        public static void setIndexLocation(string s)
        {
            indexLocation = s;
        }
        public static void setSavedPagesLocation(string s)
        {
            savedPagesLocation = s;
        }
        public static void setEnableParagraphOutput(bool s)
        {
            enableParagraphOutput = s;
        }

        public static void setCrawlResultLocation(string s)
        {
            crawlResultLocation = s;
        }
        public static void setTraversalMode(string s)
        {
            if (s != "DFS" && s != "BFS") s = "BFS";
            traversalMode = s;
        }
        public static void setMaximumDepth(int depth)
        {
            maximumDepth = depth;
        }
        public static string getTraversalMode()
        {
            return traversalMode;
        }
        public static int getMaximumDepth()
        {
            return maximumDepth;
        }
        public static string getCrawlerListLocation()
        {
            return crawlerListLocation;
        }

        public static string getIgnoredExtensionLocation()
        {
            return ignoredExtensionLocation;
        }

        public static string getIndexTagListLocation()
        {
            return indexTagListLocation;
        }
        public static string getIndexLocation()
        {
            return indexLocation;
        }
        public static string getSavedPagesLocation()
        {
            return savedPagesLocation;
        }

        public static string getCrawlResultLocation()
        {
            return crawlResultLocation;
        }
        public static bool getEnableParagraphOutput()
        {
            return enableParagraphOutput;
        }
        public static void setDefaultConfiguration()
        {
            Configuration.setCrawlerListLocation("config/crawlerList.conf");
            Configuration.setIgnoredExtensionLocation("config/ignoredExtension.conf");
            Configuration.setIndexTagListLocation("config/indexTagList.conf");
            Configuration.setIndexLocation("index/");
            Configuration.setSavedPagesLocation("savedpages/");
            Configuration.setCrawlResultLocation("crawlresult/");
            Configuration.enableParagraphOutput = true;
            Configuration.setMaximumDepth(2);
        }
    }
    class WordProcessor
    {
        private string currentWord;
        private System.IO.StreamReader inputFile;

        public WordProcessor(string fileName)
        {
            openFile(fileName);
            startWord();
        }

        public string getWord()
        {
            return currentWord;
        }

        public void openFile(string fileName)
        {
            try
            {
                inputFile = new System.IO.StreamReader(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void startWord()
        {
            if (!inputFile.EndOfStream)
            {
                currentWord = inputFile.ReadLine();
            }
        }

        public void advanceWord()
        {
            if (!inputFile.EndOfStream)
            {
                currentWord = inputFile.ReadLine();
            }
        }

        public bool isEOF()
        {
            return inputFile.EndOfStream;
        }

        public void closeFile()
        {
            inputFile.Close();
        }
    }
}
