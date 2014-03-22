using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebCrawler
{
    class Explorer
    {
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
        }

    }
    class WordProcessor
    {
        private string currentWord;
        private System.IO.StreamReader inputFile;

        public WordProcessor(string fileName){
            openFile(fileName);
            startWord();
        }

        public string getWord(){
            return currentWord;
        }

        public void openFile(string fileName){
            try {
                inputFile = new System.IO.StreamReader(fileName);
            } catch (Exception e){
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
    }
}
