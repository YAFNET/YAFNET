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
    using System.Threading.Tasks;

    using YAF.Configuration;
    using YAF.Core.Extensions;
    using YAF.Lucene.Net.Analysis;
    using YAF.Lucene.Net.Analysis.Standard;
    using YAF.Lucene.Net.Documents;
    using YAF.Lucene.Net.Index;
    using YAF.Lucene.Net.QueryParsers.Classic;
    using YAF.Lucene.Net.Search;
    using YAF.Lucene.Net.Search.Highlight;
    using YAF.Lucene.Net.Store;
    using YAF.Lucene.Net.Util;
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
        /// the Search Version
        /// </summary>
        private const LuceneVersion MatchVersion = LuceneVersion.LUCENE_48;

        /// <summary>
        /// The search index folder.
        /// </summary>
        private static readonly string SearchIndexFolder = Path.Combine(
            AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
            "search_index");

        /// <summary>
        /// The standardAnalyzer.
        /// </summary>
        private readonly StandardAnalyzer standardAnalyzer;

        /// <summary>
        /// The indexWriter.
        /// </summary>
        private IndexWriter indexWriter;

        /// <summary>
        /// The searcher manager.
        /// </summary>
        private SearcherManager searcherManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSearch" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public YafSearch([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;

            this.standardAnalyzer = new StandardAnalyzer(MatchVersion);
        }

        /// <summary>
        /// Gets the ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        private IndexWriter Writer
        {
            get
            {
                if (this.indexWriter != null && !this.indexWriter.IsClosed)
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
                    catch (Exception ex)
                    {
                        this.Get<ILogger>().Log(null, this, ex);
                    }
                }

                IndexWriter writer;

                var indexConfig = new IndexWriterConfig(MatchVersion, this.standardAnalyzer);

                try
                {
                    writer = new IndexWriter(FSDirectory.Open(SearchIndexFolder), indexConfig);
                }
                catch (LockObtainFailedException)
                {
                    var directoryInfo = new DirectoryInfo(SearchIndexFolder);
                    var directory = FSDirectory.Open(directoryInfo, new SimpleFSLockFactory(directoryInfo));
                    IndexWriter.Unlock(directory);

                    writer = new IndexWriter(FSDirectory.Open(SearchIndexFolder), indexConfig);
                }

                Thread.MemoryBarrier();
                this.indexWriter = writer;

                return this.indexWriter;
            }
        }

        /// <summary>
        /// Optimizes the Search Index
        /// </summary>
        public void Optimize()
        {
            try
            {
                var writer = this.indexWriter;

                if (writer == null || writer.IsClosed)
                {
                    return;
                }

                writer.Flush(true, true);
                writer.Commit();
                this.Dispose();
            }
            catch (Exception)
            {
                this.Dispose();
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

            this.Optimize();
        }

        /// <summary>
        /// Adds the search index
        /// </summary>
        /// <param name="messageList">
        /// The message list.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task AddSearchIndexAsync(IEnumerable<SearchMessage> messageList)
        {
            try
            {
                messageList.ForEach(message => { this.UpdateSearchIndexItemAsync(message).Wait(); });
            }
            finally
            {
                this.Optimize();
            }
        }

        /// <summary>
        /// The add search index item.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void AddSearchIndexItem(SearchMessage message)
        {
            try
            {
                var name = message.UserName ?? (message.UserDisplayName ?? string.Empty);
                var userDisplayName = message.UserDisplayName ?? string.Empty;
                var userStyle = message.UserStyle ?? string.Empty;
                var description = message.Description ?? (message.Topic ?? string.Empty);

                var doc = new Document
                              {
                                  new StringField("MessageId", message.MessageId.ToString(), Field.Store.YES),
                                  new TextField("Message", message.Message, Field.Store.YES),
                                  new StoredField("Flags", message.Flags.ToString()),
                                  new StoredField("Posted", message.Posted),
                                  new StringField("UserId", message.UserId.ToString(), Field.Store.YES),
                                  new StoredField("TopicId", message.TopicId.ToString()),
                                  new TextField("Topic", message.Topic, Field.Store.YES),
                                  new StringField("ForumName", message.ForumName, Field.Store.YES),
                                  new StoredField("ForumId", message.ForumId.ToString()),
                                  new TextField("Author", name, Field.Store.YES),
                                  new TextField("AuthorDisplay", userDisplayName, Field.Store.YES),
                                  new StoredField("AuthorStyle", userStyle),
                                  new TextField("Description", description, Field.Store.YES)
                              };
                try
                {
                    this.Writer.AddDocument(doc);
                }
                catch (OutOfMemoryException)
                {
                    this.DisposeWriter();
                    this.Writer.AddDocument(doc);
                }
            }
            finally
            {
                this.Optimize();
            }
        }

        /// <summary>
        /// Updates the Search Index Item or if not found adds it.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="dispose">
        /// Dispose IndexWriter after updating?
        /// </param>
        public void UpdateSearchIndexItem(SearchMessage message, bool dispose = false)
        {
            try
            {
                var name = message.UserName ?? (message.UserDisplayName ?? string.Empty);
                var userDisplayName = message.UserDisplayName ?? string.Empty;
                var userStyle = message.UserStyle ?? string.Empty;
                var description = message.Description ?? (message.Topic ?? string.Empty);

                var doc = new Document
                              {
                                  new StringField("MessageId", message.MessageId.ToString(), Field.Store.YES),
                                  new TextField("Message", message.Message, Field.Store.YES),
                                  new StoredField("Flags", message.Flags.ToString()),
                                  new StoredField("Posted", message.Posted),
                                  new StringField("UserId", message.UserId.ToString(), Field.Store.YES),
                                  new StoredField("TopicId", message.TopicId.ToString()),
                                  new TextField("Topic", message.Topic, Field.Store.YES),
                                  new StringField("ForumName", message.ForumName, Field.Store.YES),
                                  new StoredField("ForumId", message.ForumId.ToString()),
                                  new TextField("Author", name, Field.Store.YES),
                                  new TextField("AuthorDisplay", userDisplayName, Field.Store.YES),
                                  new StoredField("AuthorStyle", userStyle),
                                  new TextField("Description", description, Field.Store.YES)
                              };

                try
                {
                    this.Writer.UpdateDocument(
                        new Term("MessageId", message.MessageId.Value.ToString()),
                        doc,
                        this.standardAnalyzer);
                }
                catch (Exception)
                {
                    this.DisposeWriter();
                    this.Writer.UpdateDocument(
                        new Term("MessageId", message.MessageId.Value.ToString()),
                        doc,
                        this.standardAnalyzer);
                }
            }
            finally
            {
                if (dispose)
                {
                    this.Optimize();
                }
            }
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

            this.searcherManager?.Dispose();
        }

        /// <summary>
        /// Parses the query.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="parser">The parser.</param>
        /// <returns>Returns the query</returns>
        private static Query ParseQuery(string searchQuery, QueryParserBase parser)
        {
            Query query;

            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParserBase.Escape(searchQuery.Trim()));
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
            var stream = analyzer.GetTokenStream(field, new StringReader(fieldContent));
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
            IndexSearcher searcher,
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
        /// The update search index item async.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="dispose">
        /// The dispose.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UpdateSearchIndexItemAsync(SearchMessage message, bool dispose = false)
        {
            try
            {
                var name = message.UserName ?? (message.UserDisplayName ?? string.Empty);
                var userDisplayName = message.UserDisplayName ?? string.Empty;
                var userStyle = message.UserStyle ?? string.Empty;
                var description = message.Description ?? (message.Topic ?? string.Empty);

                var doc = new Document
                              {
                                  new StringField("MessageId", message.MessageId.ToString(), Field.Store.YES),
                                  new TextField("Message", message.Message, Field.Store.YES),
                                  new StoredField("Flags", message.Flags.ToString()),
                                  new StoredField("Posted", message.Posted),
                                  new StringField("UserId", message.UserId.ToString(), Field.Store.YES),
                                  new StoredField("TopicId", message.TopicId.ToString()),
                                  new TextField("Topic", message.Topic, Field.Store.YES),
                                  new StringField("ForumName", message.ForumName, Field.Store.YES),
                                  new StoredField("ForumId", message.ForumId.ToString()),
                                  new TextField("Author", name, Field.Store.YES),
                                  new TextField("AuthorDisplay", userDisplayName, Field.Store.YES),
                                  new StoredField("AuthorStyle", userStyle),
                                  new TextField("Description", description, Field.Store.YES)
                              };

                try
                {
                    this.Writer.UpdateDocument(
                        new Term("MessageId", message.MessageId.Value.ToString()),
                        doc,
                        this.standardAnalyzer);
                }
                catch (Exception ex)
                {
                    this.Get<ILogger>().Log(null, this, ex);
                    this.DisposeWriter();
                    this.Writer.UpdateDocument(
                        new Term("MessageId", message.MessageId.Value.ToString()),
                        doc,
                        this.standardAnalyzer);
                }
            }
            finally
            {
                if (dispose)
                {
                    this.Optimize();
                }
            }
        }

        /// <summary>
        /// The get searcher.
        /// </summary>
        /// <returns>
        /// The <see cref="IndexSearcher"/>.
        /// </returns>
        private IndexSearcher GetSearcher()
        {
            this.searcherManager = this.indexWriter != null
                                       ? new SearcherManager(this.indexWriter, false, null)
                                       : new SearcherManager(FSDirectory.Open(SearchIndexFolder), null);

            this.searcherManager.MaybeRefreshBlocking();

            return this.searcherManager.Acquire();
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
            var messageFlags = new MessageFlags(flags);

            var formattedMessage = this.Get<IFormatMessage>().FormatMessage(doc.Get("Message"), messageFlags);

            formattedMessage = this.Get<IBBCode>().FormatMessageWithCustomBBCode(
                formattedMessage,
                new MessageFlags(flags),
                doc.Get("UserId").ToType<int>(),
                doc.Get("MessageId").ToType<int>());

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
            IndexSearcher searcher,
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

            var analyzer = new StandardAnalyzer(MatchVersion);

            var formatter = new SimpleHTMLFormatter("<mark>", "</mark>");
            var fragmenter = new SimpleFragmenter(hitsLimit);
            QueryScorer scorer;

            // search by single field
            if (searchField.IsSet())
            {
                var parser = new QueryParser(MatchVersion, searchField, analyzer);
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

                analyzer.Dispose();

                return results;
            }
            else
            {
                var parser = new MultiFieldQueryParser(
                    MatchVersion,
                    new[]
                        {
                            "Message", "Topic",
                            this.Get<YafBoardSettings>().EnableDisplayName ? "AuthorDisplay" : "Author"
                        },
                    analyzer);

                var query = ParseQuery(searchQuery, parser);
                scorer = new QueryScorer(query);

                // sort by date
                var sort = new Sort(new SortField("MessageId", SortFieldType.STRING, true));
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

                this.searcherManager.Release(searcher);

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

            var parser = new QueryParser(MatchVersion, searchField, this.standardAnalyzer);
            var query = ParseQuery(searchQuery, parser);

            var hits = searcher.Search(query, hitsLimit).ScoreDocs;

            var results = MapSearchToDataList(searcher, hits, userAccessList);

            this.searcherManager.Release(searcher);

            return results;
        }

        /// <summary>
        /// The dispose writer.
        /// </summary>
        private void DisposeWriter()
        {
            this.indexWriter?.Dispose(true);
        }
    }
}