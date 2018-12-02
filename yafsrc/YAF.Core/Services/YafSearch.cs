namespace YAF.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Lucene.Net.Search.Highlight;
    using Lucene.Net.Store;

    using YAF.Classes;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The YAF Search Functions
    /// </summary>
    /// <seealso cref="YAF.Types.Interfaces.ISearch" />
    /// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
    public class YafSearch : ISearch, IHaveServiceLocator
    {
        /// <summary>
        /// The lucene dir.
        /// </summary>
        private static readonly string LuceneDir = Path.Combine(
            AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
            "search_index");

        /// <summary>
        /// The directory temporary
        /// </summary>
        private static FSDirectory directoryTemp;

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSearch" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public YafSearch([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; protected set; }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <value>
        /// The directory.
        /// </value>
        private static FSDirectory Directory
        {
            get
            {
                if (directoryTemp == null)
                {
                    directoryTemp = FSDirectory.Open(new DirectoryInfo(LuceneDir));
                }

                if (IndexWriter.IsLocked(directoryTemp))
                {
                    IndexWriter.Unlock(directoryTemp);
                }

                var lockFilePath = Path.Combine(LuceneDir, "write.lock");

                if (File.Exists(lockFilePath))
                {
                    File.Delete(lockFilePath);
                }

                return directoryTemp;
            }
        }

        /// <summary>
        /// Optimizes the Search Index
        /// </summary>
        public void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Clears the search Index.
        /// </summary>
        /// <returns>
        /// Returns if clearing was sucessfull
        /// </returns>
        public bool ClearSearchIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                using (var writer = new IndexWriter(Directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears the search index record.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        public void ClearSearchIndexRecord(int messageId)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("MessageId", messageId.ToString()));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Adds or updates the search index
        /// </summary>
        /// <param name="messageList">The message list.</param>
        public void AddUpdateSearchIndex(IEnumerable<SearchMessage> messageList)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            var indexWriter = new IndexWriter(
                Directory,
                analyzer,
                !IndexReader.IndexExists(Directory),
                new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH));

            indexWriter.SetMergeScheduler(new ConcurrentMergeScheduler());
            indexWriter.SetMaxBufferedDocs(YafContext.Current.Get<YafBoardSettings>().ReturnSearchMax);

            foreach (var message in messageList)
            {
                AddToSearchIndex(message, indexWriter);
            }

            analyzer.Close();
            indexWriter.Dispose();
        }

        /// <summary>
        /// Adds or updates the search index
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddUpdateSearchIndex(SearchMessage message)
        {
            this.AddUpdateSearchIndex(new List<SearchMessage> { message });
        }

        /// <summary>
        /// Searches the specified user identifier.
        /// </summary>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="input">The input.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Returns the search results
        /// </returns>
        public List<SearchMessage> Search(int forumId, int userId, string input, string fieldName = "")
        {
            var totalHits = 0;

            return input.IsNotSet()
                       ? new List<SearchMessage>()
                       : this.SearchIndex(out totalHits, forumId, userId, input, fieldName);

            /*    var terms = input.Trim().Replace("-", " ").Split(' ').Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => x.Trim() + "*");
                input = string.Join(" ", terms);*/
        }

        /// <summary>
        /// Searches for similar words
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="input">The input.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Returns the list of search results.
        /// </returns>
        public List<SearchMessage> SearchSimilar(int userId, string input, string fieldName = "")
        {
            return input.IsNotSet()
                       ? new List<SearchMessage>()
                       : this.SearchSimilarIndex(userId, input, fieldName);
        }

        /// <summary>
        /// Searches the paged.
        /// </summary>
        /// <param name="totalHits">The total hits.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="input">The input.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Returns the search results
        /// </returns>
        public List<SearchMessage> SearchPaged(
            out int totalHits,
            int forumId, 
            int userId,
            string input,
            int pageIndex,
            int pageSize,
            string fieldName = "")
        {
            if (input.IsNotSet())
            {
                totalHits = 0;
                return new List<SearchMessage>();
            }

            /*var terms = input.Trim().Replace("-", " ").Split(' ').Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);*/

            return this.SearchIndex(out totalHits, forumId, userId, input, fieldName, pageIndex, pageSize);
        }

        /// <summary>
        /// Searches the default.
        /// </summary>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="input">The input.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Returns the search results
        /// </returns>
        public List<SearchMessage> SearchDefault(int forumId, int userId, string input, string fieldName = "")
        {
            var totalHits = 0;
            return string.IsNullOrEmpty(input)
                       ? new List<SearchMessage>()
                       : this.SearchIndex(out totalHits, forumId, userId, input, fieldName);
        }

        /// <summary>
        /// Adds the index of to search.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="writer">The writer.</param>
        private static void AddToSearchIndex(SearchMessage message, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("MessageId", message.MessageId.ToString()));
            writer.DeleteDocuments(searchQuery);

            var doc = new Document();

            doc.Add(new Field("MessageId", message.MessageId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Message", message.Message, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Flags", message.Flags.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Posted", message.Posted, Field.Store.YES, Field.Index.NOT_ANALYZED));

            try
            {
                doc.Add(new Field("Author", message.UserName, Field.Store.YES, Field.Index.ANALYZED));
            }
            catch (Exception)
            {
                doc.Add(new Field("Author", message.UserDisplayName, Field.Store.YES, Field.Index.ANALYZED));
            }

            doc.Add(new Field("AuthorDisplay", message.UserDisplayName, Field.Store.YES, Field.Index.ANALYZED));

            try
            {
                doc.Add(new Field("AuthorStyle", message.UserStyle, Field.Store.YES, Field.Index.ANALYZED));
            }
            catch (Exception)
            {
                doc.Add(new Field("AuthorStyle", string.Empty, Field.Store.YES, Field.Index.ANALYZED));
            }
            
            doc.Add(new Field("UserId", message.UserId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("TopicId", message.TopicId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Topic", message.Topic, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("ForumName", message.ForumName, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("ForumId", message.ForumId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            try
            {
                doc.Add(new Field("Description", message.Description, Field.Store.YES, Field.Index.ANALYZED));
            }
            catch (NullReferenceException)
            {
                doc.Add(new Field("Description", string.Empty, Field.Store.YES, Field.Index.ANALYZED));
            }

            writer.AddDocument(doc);
        }

        /// <summary>
        /// Maps the search document to data.
        /// </summary>
        /// <param name="highlighter">The highlighter.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="doc">The document.</param>
        /// <param name="userAccessList">The user access list.</param>
        /// <returns>
        /// Returns the Search Message
        /// </returns>
        private SearchMessage MapSearchDocumentToData(
            Highlighter highlighter,
            Analyzer analyzer,
            Document doc,
            List<vaccess> userAccessList)
        {
            var forumId = doc.Get("ForumId").ToType<int>();

            if (!userAccessList.Any() || !userAccessList.Exists(v => v.ForumID == forumId && v.ReadAccess))
            {
                return null;
            }

            var flags = doc.Get("Flags").ToType<int>();

            var formattedMessage = this.Get<IFormatMessage>().FormatMessage(doc.Get("Message"), new MessageFlags(flags), true);

            var message = formattedMessage;

            try
            {
                message = this.GetHighlight(highlighter, analyzer, "Message", message);
            }
            catch (Exception)
            {
                // Ignore
                message = formattedMessage;
            }
            finally
            {
                if (message.IsNotSet())
                {
                    message = formattedMessage;
                }
            }

            string topic;

            try
            {
                topic = this.GetHighlight(highlighter, analyzer, "Topic", doc.Get("Topic"));
            }
            catch (Exception)
            {
                topic = doc.Get("Topic");
            }

            return new SearchMessage
                       {
                           MessageId = doc.Get("MessageId").ToType<int>(),
                           Message = message,
                           Flags = flags,
                           Posted =
                               doc.Get("Posted").ToType<DateTime>().ToString(
                                   "yyyy-MM-ddTHH:mm:ssZ",
                                   CultureInfo.InvariantCulture),
                           UserName = doc.Get("Author"),
                           UserId = doc.Get("UserId").ToType<int>(),
                           TopicId = doc.Get("TopicId").ToType<int>(),
                           Topic = topic.IsSet() ? topic : doc.Get("Topic"),
                           ForumId = doc.Get("ForumId").ToType<int>(),
                           Description = doc.Get("Description"),
                           TopicUrl =
                               YafBuildLink.GetLink(
                                   ForumPages.posts,
                                   "t={0}",
                                   doc.Get("TopicId").ToType<int>()),
                           MessageUrl =
                               YafBuildLink.GetLink(
                                   ForumPages.posts,
                                   "m={0}#post{0}",
                                   doc.Get("MessageId").ToType<int>()),
                           ForumUrl =
                               YafBuildLink.GetLink(
                                   ForumPages.forum,
                                   "f={0}",
                                   doc.Get("ForumId").ToType<int>()),
                           UserDisplayName = doc.Get("AuthorDisplay"),
                           ForumName = doc.Get("ForumName"),
                           UserStyle = doc.Get("AuthorStyle")
                       };
        }

        /// <summary>
        /// Maps the search document to data.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="userAccessList">The user access list.</param>
        /// <returns>
        /// Returns the Search Message
        /// </returns>
        private SearchMessage MapSearchDocumentToData(Document doc, List<vaccess> userAccessList)
        {
            var forumId = doc.Get("ForumId").ToType<int>();

            if (!userAccessList.Any() || !userAccessList.Exists(v => v.ForumID == forumId && v.ReadAccess))
            {
                return null;
            }

            return new SearchMessage
                       {
                           Topic = doc.Get("Topic"),
                           TopicUrl =
                               YafBuildLink.GetLink(
                                   ForumPages.posts,
                                   "t={0}",
                                   doc.Get("TopicId").ToType<int>()),
                           Posted =
                               doc.Get("Posted").ToType<DateTime>().ToString(
                                   "yyyy-MM-ddTHH:mm:ssZ",
                                   CultureInfo.InvariantCulture),
                           UserId = doc.Get("UserId").ToType<int>(),
                           UserName = doc.Get("Author"),
                           UserDisplayName = doc.Get("AuthorDisplay"),
                           ForumName = doc.Get("ForumName"),
                           UserStyle = doc.Get("AuthorStyle")
                       };
        }

        /// <summary>
        /// Maps the search to data list.
        /// </summary>
        /// <param name="highlighter">The highlighter.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="searcher">The searcher.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="userAccessList">The user access list.</param>
        /// <returns>
        /// Returns the search list
        /// </returns>
        private List<SearchMessage> MapSearchToDataList(
            Highlighter highlighter,
            Analyzer analyzer,
            Searchable searcher,
            IEnumerable<ScoreDoc> hits,
            int pageIndex,
            int pageSize,
            List<vaccess> userAccessList)
        {
            var skip = pageSize * (pageIndex - 1);

            return hits.Select(
                    hit => this.MapSearchDocumentToData(highlighter, analyzer, searcher.Doc(hit.Doc), userAccessList))
                .OrderByDescending(s => s.MessageId).Skip(skip).Take(pageSize).ToList();
        }

        /// <summary>
        /// Maps the search to data list.
        /// </summary>
        /// <param name="searcher">The searcher.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="userAccessList">The user access list.</param>
        /// <returns>
        /// Returns the search list
        /// </returns>
        private List<SearchMessage> MapSearchToDataList(
            Searchable searcher,
            IEnumerable<ScoreDoc> hits,
            List<vaccess> userAccessList)
        {
            return hits.Select(hit => this.MapSearchDocumentToData(searcher.Doc(hit.Doc), userAccessList))
                .GroupBy(x => x.Topic).Select(y => y.FirstOrDefault()).ToList();
        }

        /// <summary>
        /// Searches the index.
        /// </summary>
        /// <param name="totalHits">The total hits.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="searchField">The search field.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// Returns the Search results
        /// </returns>
        private List<SearchMessage> SearchIndex(
            out int totalHits,
            int forumId, 
            int userId,
            string searchQuery,
            string searchField = "",
            int pageIndex = 1,
            int pageSize = 1000)
        {
            if (string.IsNullOrEmpty(searchQuery.Replace("*", string.Empty).Replace("?", string.Empty)))
            {
                totalHits = 0;
                return new List<SearchMessage>();
            }

            // Insert forum access here
            var userAccessList = this.GetRepository<vaccess>().Get(v => v.UserID == userId);

            // filter forum
            if (forumId > 0)
            {
                userAccessList = userAccessList.FindAll(v => v.ForumID == forumId);
            }

            using (var searcher = new IndexSearcher(Directory, true))
            {
                var hitsLimit = this.Get<YafBoardSettings>().ReturnSearchMax;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                var formatter = new SimpleHTMLFormatter(
                    "<mark>",
                    "</mark>");
                var fragmenter = new SimpleFragmenter(hitsLimit);
                QueryScorer scorer;

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = this.ParseQuery(searchQuery, parser);
                    scorer = new QueryScorer(query);

                    var hits = searcher.Search(query, hitsLimit).ScoreDocs;
                    totalHits = hits.Length;

                    var highlighter = new Highlighter(formatter, scorer) { TextFragmenter = fragmenter };

                    var results = this.MapSearchToDataList(
                        highlighter,
                        analyzer,
                        searcher,
                        hits,
                        pageIndex,
                        pageSize,
                        userAccessList);

                    analyzer.Close();
                    searcher.Dispose();

                    return results;
                }
                else
                {
                    var parser = new MultiFieldQueryParser(
                        Lucene.Net.Util.Version.LUCENE_30,
                        new[] { "Message", "Topic", "Author" },
                        analyzer);

                    var query = this.ParseQuery(searchQuery, parser);
                    scorer = new QueryScorer(query);

                    var hits = searcher.Search(query, null, hitsLimit, Sort.INDEXORDER).ScoreDocs;
                    totalHits = hits.Length;

                    var highlighter = new Highlighter(formatter, scorer) { TextFragmenter = fragmenter };

                    var results = this.MapSearchToDataList(
                        highlighter,
                        analyzer,
                        searcher,
                        hits,
                        pageIndex,
                        pageSize,
                        userAccessList);

                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }

        /// <summary>
        /// Searches for similar words
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="searchField">The search field.</param>
        /// <returns>
        /// Returns the Search results
        /// </returns>
        private List<SearchMessage> SearchSimilarIndex(
            int userId,
            string searchQuery,
            string searchField)
        {
            if (searchQuery.Replace("*", string.Empty).Replace("?", string.Empty).IsNotSet())
            {
                return new List<SearchMessage>();
            }

            // Insert forum access here
            var userAccessList = this.GetRepository<vaccess>().Get(v => v.UserID == userId);

            using (var searcher = new IndexSearcher(Directory, true))
            {
                var hitsLimit = this.Get<YafBoardSettings>().ReturnSearchMax;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                  var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = this.ParseQuery(searchQuery, parser);

                    var hits = searcher.Search(query, hitsLimit).ScoreDocs;

                    var results = this.MapSearchToDataList(
                        searcher,
                        hits,
                        userAccessList);

                    analyzer.Close();
                    searcher.Dispose();

                    return results;
            }
        }

        /// <summary>
        /// Parses the query.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="parser">The parser.</param>
        /// <returns>Returns the query</returns>
        private Query ParseQuery(string searchQuery, QueryParser parser)
        {
            Query query;

            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }

            return query;
        }

        /// <summary>
        /// Gets the highlighted text.
        /// </summary>
        /// <param name="highlighter">The highlighter.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="field">The field.</param>
        /// <param name="fieldContent">Content of the field.</param>
        /// <returns>
        /// Returns the highlighted text.
        /// </returns>
        private string GetHighlight(Highlighter highlighter, Analyzer analyzer, string field, string fieldContent)
        {
            var stream = analyzer.TokenStream(field, new StringReader(fieldContent));
            return highlighter.GetBestFragments(stream, fieldContent, 20, ".");
        }
    }
}