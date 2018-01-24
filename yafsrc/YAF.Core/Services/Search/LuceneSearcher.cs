using System;
using System.Collections.Generic;
using System.Linq;

namespace LuceneConsoleApplication
{
    public class LuceneSearcher : IDisposable
    {
        public LuceneSearcher(string dirPath)
        {
            _luceneDir = dirPath;
        }
        public class LuceneData
        {
            public string Actor { get; set; }
        }

        private static string _luceneDir = null;

        private static Lucene.Net.Store.FSDirectory _directoryTemp;
        private static Lucene.Net.Store.FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(_luceneDir));
                if (Lucene.Net.Index.IndexWriter.IsLocked(_directoryTemp)) Lucene.Net.Index.IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = System.IO.Path.Combine(_luceneDir, "write.lock");
                if (System.IO.File.Exists(lockFilePath)) System.IO.File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        Lucene.Net.Search.IndexSearcher searcher = null;
        Lucene.Net.Analysis.Standard.StandardAnalyzer analyzer = null;

        public List<LuceneData> Search(string searchText, string searchField)
        {
            List<LuceneData> output = new List<LuceneData>();

            string searchQuery = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(searchText.Trim()))
                {
                    throw new Exception("You forgot to enter something to search for...");
                }

                searchQuery = searchText;

            }
            catch
            {
                throw;
            }

            try
            {
                // Open the IndexReader with readOnly=true. 
                // This makes a big difference when multiple threads are sharing the same reader, 
                // as it removes certain sources of thread contention.
                searcher = new Lucene.Net.Search.IndexSearcher(_directory, true);
                analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                output = (GetSearchResultByField(searchQuery, searchField, searcher, analyzer));
            }
            catch
            {
                throw;
            }
            finally
            {
                analyzer.Close();
                analyzer.Dispose();
                searcher.Dispose();
            }

            return output;
        }

        private List<LuceneData> GetSearchResultByField(string searchQuery, string searchField, Lucene.Net.Search.IndexSearcher searcher, Lucene.Net.Analysis.Standard.StandardAnalyzer analyzer)
        {
            try
            {
                List<LuceneData> output = new List<LuceneData>();
                var hits_limit = 1000;
                var parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                var query = ParseQuery(searchQuery, parser);
                var hits = searcher.Search(query, hits_limit).ScoreDocs;
                var results = MapLuceneDataToIDList(hits, searcher);
                output = results.Take(5).ToList<LuceneData>();
                return output;
            }
            catch
            {
                throw;
            }
        }


        private Lucene.Net.Search.Query ParseQuery(string searchQuery, Lucene.Net.QueryParsers.QueryParser parser)
        {
            Lucene.Net.Search.Query query;
            try
            {
                query = parser.Parse(searchQuery.ToLower().Trim() + "*");
            }
            catch (Lucene.Net.QueryParsers.ParseException)
            {
                query = parser.Parse(Lucene.Net.QueryParsers.QueryParser.Escape(searchQuery.Trim()));
                throw;
            }
            return query;
        }

        private IEnumerable<LuceneData> MapLuceneDataToIDList(IEnumerable<Lucene.Net.Search.ScoreDoc> hits, Lucene.Net.Search.IndexSearcher searcher)
        {
            return hits.Select(hit => MapLuceneData(searcher.Doc(hit.Doc))).ToList();
        }

        // Mapping Lucene data
        private LuceneData MapLuceneData(Lucene.Net.Documents.Document doc)
        {
            return new LuceneData
            {
                Actor = (doc.Get("Message"))
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (analyzer != null)
                    analyzer.Dispose();
                if (searcher != null)
                    searcher.Dispose();
            }
            // free native resources if there are any.
        }
    }
}
