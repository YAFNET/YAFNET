/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Lucene.Net.Search.Highlight;
    using Lucene.Net.Store;

    using YAF.Configuration;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;

    /// <summary>
    /// The YAF Search Functions
    /// </summary>
    /// <seealso cref="YAF.Types.Interfaces.ISearch" />
    /// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
    public class YafSearch : ISearch, IHaveServiceLocator, IDisposable
    {
        /// <summary>
        /// The write lock file.
        /// </summary>
        private const string WriteLockFile = "write.lock";

        /// <summary>
        /// The search index folder.
        /// </summary>
        private static readonly string SearchIndexFolder = Path.Combine(
            AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
            "search_index");

        /// <summary>
        /// The directory temp.
        /// </summary>
        private static FSDirectory directoryTemp;

        /// <summary>
        /// The standardAnalyzer.
        /// </summary>
        private readonly StandardAnalyzer standardAnalyzer;

        /// <summary>
        /// The index Searcher.
        /// </summary>
        private readonly IndexSearcher indexSearcher;

        /// <summary>
        /// The indexWriter lock.
        /// </summary>
        private readonly object indexWriterLock = new object();

        /// <summary>
        /// The indexWriter.
        /// </summary>
        private IndexWriter indexWriter;

        /// <summary>
        /// The index reader.
        /// </summary>
        private IndexReader indexReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSearch" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public YafSearch([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;

            this.standardAnalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        }

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; protected set; }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        private IndexWriter Writer
        {
            get
            {
                if (this.indexWriter != null)
                {
                    return this.indexWriter;
                }

                lock (this.indexWriterLock)
                {
                    if (this.indexWriter != null)
                    {
                        return this.indexWriter;
                    }

                    var lockFile = Path.Combine(SearchIndexFolder, WriteLockFile);

                    if (File.Exists(lockFile))
                    {
                        try
                        {
                            File.Delete(lockFile);
                        }
                        catch (IOException ex)
                        {
                            this.Get<ILogger>().Log(null, this, ex);
                        }
                    }

                    var writer = new IndexWriter(
                        FSDirectory.Open(SearchIndexFolder),
                        this.standardAnalyzer,
                        IndexWriter.MaxFieldLength.UNLIMITED);

                    this.indexReader = writer.GetReader();
                    Thread.MemoryBarrier();
                    this.indexWriter = writer;
                }

                return this.indexWriter;
            }
        }

        /// <summary>
        /// Optimizes the Search Index
        /// </summary>
        public void Optimize()
        {
            var writer = this.indexWriter;

            if (writer == null || !writer.HasDeletions())
            {
                return;
            }

            this.indexWriter.Optimize();

            this.Commit();
        }

        /// <summary>
        /// The commit.
        /// </summary>
        public void Commit()
        {
            if (this.indexWriter == null)
            {
                return;
            }

            lock (this.indexWriterLock)
            {
                this.indexWriter?.Commit();
            }
        }

        /// <summary>
        /// Clears the search Index.
        /// </summary>
        /// <returns>
        /// Returns if clearing was successful
        /// </returns>
        public bool ClearSearchIndex()
        {
            try
            {
                this.Writer.DeleteAll();
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
            // remove older index entry
            var searchQuery = new TermQuery(new Term("MessageId", messageId.ToString()));
            this.Writer.DeleteDocuments(searchQuery);
        }

        /// <summary>
        /// Adds or updates the search index
        /// </summary>
        /// <param name="messageList">The message list.</param>
        public void AddUpdateSearchIndex(IEnumerable<SearchMessage> messageList)
        {
            try
            {
                this.Writer.SetMergeScheduler(new ConcurrentMergeScheduler());
                this.Writer.SetMaxBufferedDocs(YafContext.Current.Get<YafBoardSettings>().ReturnSearchMax);

                messageList.ForEach(this.AddToSearchIndex);
            }
            catch (LockObtainFailedException ex)
            {
                this.Get<ILogger>().Log(null, this, ex);
            }
            catch (ThreadAbortException ex)
            {
                this.Get<ILogger>().Log(null, this, ex);
            }
            finally
            {
                this.Optimize();
            }
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
            return input.IsNotSet()
                       ? new List<SearchMessage>()
                       : this.SearchIndex(out _, forumId, userId, input, fieldName);

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
            return input.IsNotSet() ? new List<SearchMessage>() : this.SearchSimilarIndex(userId, input, fieldName);
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
            if (input.IsSet())
            {
                return this.SearchIndex(out totalHits, forumId, userId, input, fieldName, pageIndex, pageSize);
            }

            totalHits = 0;
            return new List<SearchMessage>();

            /*var terms = input.Trim().Replace("-", " ").Split(' ').Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);*/
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
            return input.IsNotSet()
                       ? new List<SearchMessage>()
                       : this.SearchIndex(out _, forumId, userId, input, fieldName);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.DisposeWriter();
            this.DisposeReaders();
        }

        /// <summary>
        /// The get searcher.
        /// </summary>
        /// <returns>
        /// The <see cref="IndexSearcher"/>.
        /// </returns>
        public IndexSearcher GetSearcher()
        {
            IndexSearcher searcher;

            if (this.indexReader != null)
            {
                var newReader = this.indexReader.Reopen();

                if (this.indexReader != newReader)
                {
                    Interlocked.Exchange(ref this.indexReader, newReader);
                }

                searcher = new IndexSearcher(this.indexReader);
            }
            else
            {
                searcher = new IndexSearcher(FSDirectory.Open(SearchIndexFolder), true);
            }

            return searcher;
        }

        /// <summary>
        /// Parses the query.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="parser">The parser.</param>
        /// <returns>Returns the query</returns>
        private static Query ParseQuery(string searchQuery, QueryParser parser)
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
        private static string GetHighlight(
            Highlighter highlighter,
            Analyzer analyzer,
            string field,
            string fieldContent)
        {
            var stream = analyzer.TokenStream(field, new StringReader(fieldContent));
            return highlighter.GetBestFragments(stream, fieldContent, 20, ".");
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
        private static List<SearchMessage> MapSearchToDataList(
            Searchable searcher,
            IEnumerable<ScoreDoc> hits,
            List<vaccess> userAccessList)
        {
            return hits.Select(hit => MapSearchDocumentToData(searcher.Doc(hit.Doc), userAccessList))
                .GroupBy(x => x.Topic).Select(y => y.FirstOrDefault()).ToList();
        }

        /// <summary>
        /// Maps the search document to data.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="userAccessList">The user access list.</param>
        /// <returns>
        /// Returns the Search Message
        /// </returns>
        private static SearchMessage MapSearchDocumentToData(Document doc, List<vaccess> userAccessList)
        {
            var forumId = doc.Get("ForumId").ToType<int>();

            if (!userAccessList.Any() || !userAccessList.Exists(v => v.ForumID == forumId && v.ReadAccess))
            {
                return null;
            }

            return new SearchMessage
                       {
                           Topic = doc.Get("Topic"),
                           TopicId = doc.Get("TopicId").ToType<int>(),
                           TopicUrl = YafBuildLink.GetLink(ForumPages.posts, "t={0}", doc.Get("TopicId").ToType<int>()),
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
        /// Adds the index of to search.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void AddToSearchIndex(SearchMessage message)
        {
            try
            {
                this.ClearSearchIndexRecord(message.MessageId.Value);

                var doc = new Document();

                doc.Add(
                    new Field("MessageId", message.MessageId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Message", message.Message, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Flags", message.Flags.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Posted", message.Posted, Field.Store.YES, Field.Index.NOT_ANALYZED));

                var name = message.UserName ?? (message.UserDisplayName ?? string.Empty);
                doc.Add(new Field("Author", name, Field.Store.YES, Field.Index.ANALYZED));

                name = message.UserDisplayName ?? string.Empty;
                doc.Add(new Field("AuthorDisplay", name, Field.Store.YES, Field.Index.ANALYZED));

                name = message.UserStyle ?? string.Empty;
                doc.Add(new Field("AuthorStyle", name, Field.Store.YES, Field.Index.ANALYZED));

                doc.Add(new Field("UserId", message.UserId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("TopicId", message.TopicId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Topic", message.Topic, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ForumName", message.ForumName, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ForumId", message.ForumId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                name = message.Description ?? (message.Topic ?? string.Empty);
                doc.Add(new Field("Description", name, Field.Store.YES, Field.Index.ANALYZED));

                try
                {
                    this.Writer.AddDocument(doc);
                }
                catch (OutOfMemoryException)
                {
                    lock (this.indexWriterLock)
                    {
                        this.DisposeWriter();
                        this.Writer.AddDocument(doc);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Get<ILogger>().Log(null, this, ex);
            }
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

            var formattedMessage = this.Get<IFormatMessage>().FormatMessage(
                doc.Get("Message"),
                new MessageFlags(flags));

            var message = formattedMessage;

            try
            {
                message = GetHighlight(highlighter, analyzer, "Message", message);
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
                topic = GetHighlight(highlighter, analyzer, "Topic", doc.Get("Topic"));
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
                           TopicUrl = YafBuildLink.GetLink(ForumPages.posts, "t={0}", doc.Get("TopicId").ToType<int>()),
                           MessageUrl =
                               YafBuildLink.GetLink(
                                   ForumPages.posts,
                                   "m={0}#post{0}",
                                   doc.Get("MessageId").ToType<int>()),
                           ForumUrl = YafBuildLink.GetLink(ForumPages.forum, "f={0}", doc.Get("ForumId").ToType<int>()),
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
            var skip = pageSize * pageIndex;
            return hits.Select(
                    hit => this.MapSearchDocumentToData(highlighter, analyzer, searcher.Doc(hit.Doc), userAccessList))
                .Where(item => item != null).OrderByDescending(item => item.MessageId).Skip(skip).Take(pageSize)
                .ToList();
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
            if (searchQuery.Replace("*", string.Empty).Replace("?", string.Empty).IsNotSet())
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

            var searcher = this.GetSearcher();

            var hitsLimit = this.Get<YafBoardSettings>().ReturnSearchMax;

            // 0 => Lucene error;
            if (hitsLimit == 0)
            {
                hitsLimit = pageSize;
            }

            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            var formatter = new SimpleHTMLFormatter("<mark>", "</mark>");
            var fragmenter = new SimpleFragmenter(hitsLimit);
            QueryScorer scorer;

            // search by single field
            if (searchField.IsSet())
            {
                var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                var query = ParseQuery(searchQuery, parser);
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

                var query = ParseQuery(searchQuery, parser);
                scorer = new QueryScorer(query);

                // sort by date
                var sort = new Sort(new SortField("Posted", SortField.STRING, true));
                var hits = searcher.Search(query, null, hitsLimit, sort).ScoreDocs;

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

        /// <summary>
        /// Searches for similar words
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="searchField">The search field.</param>
        /// <returns>
        /// Returns the Search results
        /// </returns>
        private List<SearchMessage> SearchSimilarIndex(int userId, string searchQuery, string searchField)
        {
            if (searchQuery.Replace("*", string.Empty).Replace("?", string.Empty).IsNotSet())
            {
                return new List<SearchMessage>();
            }

            // Insert forum access here
            var userAccessList = this.GetRepository<vaccess>().Get(v => v.UserID == userId);

            var searcher = this.GetSearcher();

            var hitsLimit = this.Get<YafBoardSettings>().ReturnSearchMax;

            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, this.standardAnalyzer);
            var query = ParseQuery(searchQuery, parser);

            var hits = searcher.Search(query, hitsLimit).ScoreDocs;

            var results = MapSearchToDataList(searcher, hits, userAccessList);

            searcher.Dispose();

            return results;
        }

        /// <summary>
        /// The dispose writer.
        /// </summary>
        /// <param name="commit">
        /// The commit.
        /// </param>
        private void DisposeWriter(bool commit = true)
        {
            if (this.indexWriter == null)
            {
                return;
            }

            lock (this.indexWriterLock)
            {
                if (this.indexWriter == null)
                {
                    return;
                }

                this.indexReader.Dispose();
                this.indexReader = null;

                if (commit)
                {
                    this.indexWriter.Commit();
                }

                this.indexWriter.Dispose();
                this.indexWriter = null;
            }
        }

        /// <summary>
        /// The dispose readers.
        /// </summary>
        private void DisposeReaders()
        {
            this.indexReader = null;
        }
    }
}