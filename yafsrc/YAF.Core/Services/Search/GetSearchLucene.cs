namespace YAF.Core.Services.Search
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;

    using LuceneConsoleApplication;

    using YAF.Types.Extensions;
    using YAF.Types.Models;

    public class GetSearchLucene
    {
        public List<Message> GetSearchLuceneText(string actorName)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Message", analyzer);
            Query query = parser.Parse(actorName);


            var yafSearch = new YafSearch();

            Lucene.Net.Store.Directory dir = Lucene.Net.Store.FSDirectory.Open(new DirectoryInfo(yafSearch.LuceneIndexDirectory()));

            //Setup searcher
            IndexSearcher searcher = new IndexSearcher(dir);

            var hits_limit = 1000;
            //var parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
            //var query = ParseQuery(searchQuery, parser);



            //Do the search
            var hits = searcher.Search(query, hits_limit).ScoreDocs;
            var results = hits.Select(hit => MapLuceneData(searcher.Doc(hit.Doc))).ToList();
           // output = results.Take(5).ToList<LuceneData>();

            return results;
            
            }

        // Mapping Lucene data
        private Message MapLuceneData(Lucene.Net.Documents.Document doc)
        {
            return new Message
            {
                 ID = (doc.Get("MessageId").ToType<int>()),
                           MessageText = (doc.Get("Message"))
                       };
        }
    }
}
