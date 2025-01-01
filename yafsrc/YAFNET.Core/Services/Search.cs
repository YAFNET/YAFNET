/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Queries;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Lucene.Net.Util;

using Microsoft.Extensions.Logging;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Types.Constants;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Search Functions
/// </summary>
/// <seealso cref="ISearch" />
/// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
public class Search : ISearch, IHaveServiceLocator, IDisposable
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
    private readonly static string SearchIndexFolder = AppDomain.CurrentDomain.GetData("SearchDataDirectory").ToString();

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
    /// Initializes a new instance of the <see cref="Services.Search" /> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    public Search(IServiceLocator serviceLocator)
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
            if (this.indexWriter is { IsClosed: false })
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
                    this.Get<ILogger<Search>>().Log(null, this, ex);
                }
            }

            IndexWriter writer;

            try
            {
                var indexConfig = new IndexWriterConfig(MatchVersion, this.standardAnalyzer);
                writer = new IndexWriter(FSDirectory.Open(SearchIndexFolder), indexConfig);
            }
            catch (LockObtainFailedException)
            {
                var directoryInfo = new DirectoryInfo(SearchIndexFolder);
                var directory = FSDirectory.Open(directoryInfo, new SimpleFSLockFactory(directoryInfo));
                IndexWriter.Unlock(directory);

                var indexConfig = new IndexWriterConfig(MatchVersion, this.standardAnalyzer);
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
    /// Delete Search Index Record by Message Id.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    public void DeleteSearchIndexRecordByMessageId(int messageId)
    {
        // remove older index entry
        var searchQuery = new TermQuery(new Term("MessageId", messageId.ToString()));

        this.Writer.DeleteDocuments(searchQuery);

        this.Optimize();
    }

    /// <summary>
    /// Delete Search Index Record by Topic Id.
    /// </summary>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    public void DeleteSearchIndexRecordByTopicId(int topicId)
    {
        // remove older index entry
        var searchQuery = new TermQuery(new Term("TopicId", topicId.ToString()));

        this.Writer.DeleteDocuments(searchQuery);

        this.Optimize();
    }

    /// <summary>
    /// Adds the search index
    /// </summary>
    /// <param name="messageList">
    ///     The message list.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task AddSearchIndexAsync(IEnumerable<SearchMessage> messageList)
    {
        try
        {
            messageList.ForEach(message => this.UpdateSearchIndexItemAsync(message));
        }
        finally
        {
            this.Optimize();
        }

        return Task.CompletedTask;
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
            var name = message.UserName ?? message.UserDisplayName ?? string.Empty;
            var userDisplayName = message.UserDisplayName ?? string.Empty;
            var userStyle = message.UserStyle ?? string.Empty;
            var description = message.Description ?? message.Topic ?? string.Empty;

            var doc = new Document
                          {
                              new StringField("MessageId", message.MessageId.ToString(), Field.Store.YES),
                              new TextField("Message", message.Message, Field.Store.YES),
                              new StoredField("Flags", message.Flags.ToString()),
                              new StoredField("Posted", message.Posted),
                              new StringField("UserId", message.UserId.ToString(), Field.Store.YES),
                              new StringField("TopicId", message.TopicId.ToString(), Field.Store.YES),
                              new TextField("Topic", message.Topic, Field.Store.YES),
                              new TextField("TopicTags", message.TopicTags, Field.Store.YES),
                              new StringField("ForumName", message.ForumName, Field.Store.YES),
                              new StringField("ForumId", message.ForumId.ToString(), Field.Store.YES),
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
            var name = message.UserName ?? message.UserDisplayName ?? string.Empty;
            var userDisplayName = message.UserDisplayName ?? string.Empty;
            var userStyle = message.UserStyle ?? string.Empty;
            var description = message.Description ?? message.Topic ?? string.Empty;

            var doc = new Document
                          {
                              new StringField("MessageId", message.MessageId.ToString(), Field.Store.YES),
                              new TextField("Message", message.Message, Field.Store.YES),
                              new StoredField("Flags", message.Flags.ToString()),
                              new StoredField("Posted", message.Posted),
                              new StringField("UserId", message.UserId.ToString(), Field.Store.YES),
                              new StringField("TopicId", message.TopicId.ToString(), Field.Store.YES),
                              new TextField("Topic", message.Topic, Field.Store.YES),
                              new StringField("ForumName", message.ForumName, Field.Store.YES),
                              new StringField("ForumId", message.ForumId.ToString(), Field.Store.YES),
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
    /// Only Get Number of Search Results (Hits)
    /// </summary>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public int CountHits(string input)
    {
        var searcher = this.GetSearcher();

        if (searcher == null)
        {
            return 0;
        }

        var parser = new QueryParser(MatchVersion, "Message", this.standardAnalyzer);
        var query = ParseQuery(input, parser);

        return searcher.Search(query, 1000).TotalHits;
    }

    /// <summary>
    /// Searches for similar words
    /// </summary>
    /// <param name="filter">
    /// The filter.
    /// </param>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <param name="fieldName">
    /// Name of the field.
    /// </param>
    /// <returns>
    /// Returns the list of search results.
    /// </returns>
    public List<SearchMessage> SearchSimilar(string filter, string input, string fieldName = "")
    {
        return input.IsNotSet()
                   ? []
                   : this.SearchSimilarIndex(filter, input, fieldName);
    }

    /// <summary>
    /// Searches the paged.
    /// </summary>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="input">The input.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>
    /// Returns the search results
    /// </returns>
    public async Task<Tuple<List<SearchMessage>, int>> SearchPagedAsync(
        int forumId,
        string input,
        int pageIndex,
        int pageSize,
        string fieldName = "")
    {
        if (!input.IsSet())
        {
            return new Tuple<List<SearchMessage>, int>([], 0);
        }

        try
        {
            return await this.SearchIndexAsync(forumId, input, fieldName, pageIndex, pageSize);
        }
        catch (Exception exception)
        {
            this.Get<ILogger<Search>>().Error(exception, "Search Error");
        }

        return new Tuple<List<SearchMessage>, int>([], 0);
    }

    /// <summary>
    /// Searches the default.
    /// </summary>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="input">The input.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>
    /// Returns the search results
    /// </returns>
    public async Task<Tuple<List<SearchMessage>, int>> SearchDefaultAsync(int forumId, string input, string fieldName = "")
    {
        return input.IsNotSet()
                   ? new Tuple<List<SearchMessage>, int>([], 0)
                   : await this.SearchIndexAsync(forumId, input, fieldName);
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
    private List<SearchMessage> MapSearchToDataList(
        IndexSearcher searcher,
        IEnumerable<ScoreDoc> hits,
        List<VAccess> userAccessList)
    {
        var results = hits.Select(hit => this.MapSearchDocumentToData(searcher.Doc(hit.Doc), userAccessList))
            .ToList();

        return results.Count != 0
                   ? results.Where(item => item != null).GroupBy(x => x.Topic).Select(y => y.FirstOrDefault()).ToList()
                   : [];
    }

    /// <summary>
    /// Maps the search document to data.
    /// </summary>
    /// <param name="doc">The document.</param>
    /// <param name="userAccessList">The user access list.</param>
    /// <returns>
    /// Returns the Search Message
    /// </returns>
    private SearchMessage MapSearchDocumentToData(Document doc, List<VAccess> userAccessList)
    {
        var forumId = doc.Get("ForumId").ToType<int>();

        if (userAccessList.Count == 0 || !userAccessList.Exists(v => v.ForumID == forumId && v.ReadAccess > 0))
        {
            return null;
        }

        return new SearchMessage
                   {
                       Topic = doc.Get("Topic"),
                       TopicId = doc.Get("TopicId").ToType<int>(),
                       TopicUrl = this.Get<LinkBuilder>().GetTopicLink(doc.Get("TopicId").ToType<int>(), doc.Get("Topic")),
                       Posted = doc.Get("Posted"),
                       UserId = doc.Get("UserId").ToType<int>(),
                       UserName = HttpUtility.HtmlEncode(doc.Get("Author")),
                       UserDisplayName = HttpUtility.HtmlEncode(doc.Get("AuthorDisplay")),
                       ForumName = doc.Get("ForumName"),
                       ForumUrl = this.Get<LinkBuilder>().GetForumLink(
                           doc.Get("ForumId").ToType<int>(),
                           doc.Get("ForumName")),
                       UserStyle = doc.Get("AuthorStyle")
                   };
    }

    /// <summary>
    /// The update search index item async.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="dispose">
    ///     The dispose.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    private void UpdateSearchIndexItemAsync(SearchMessage message, bool dispose = false)
    {
        try
        {
            var name = message.UserName ?? message.UserDisplayName ?? string.Empty;
            var userDisplayName = message.UserDisplayName ?? string.Empty;
            var userStyle = message.UserStyle ?? string.Empty;
            var description = message.Description ?? message.Topic ?? string.Empty;
            var topicTags = message.TopicTags.IsNotSet() ? string.Empty : message.TopicTags;

            var doc = new Document
                          {
                              new StringField("MessageId", message.MessageId.ToString(), Field.Store.YES),
                              new TextField("Message", message.Message, Field.Store.YES),
                              new StoredField("Flags", message.Flags.ToString()),
                              new StoredField("Posted", message.Posted),
                              new StringField("UserId", message.UserId.ToString(), Field.Store.YES),
                              new StringField("TopicId", message.TopicId.ToString(), Field.Store.YES),
                              new TextField("Topic", message.Topic, Field.Store.YES),
                              new StringField("ForumName", message.ForumName, Field.Store.YES),
                              new StringField("ForumId", message.ForumId.ToString(), Field.Store.YES),
                              new TextField("Author", name, Field.Store.YES),
                              new TextField("AuthorDisplay", userDisplayName, Field.Store.YES),
                              new StoredField("AuthorStyle", userStyle),
                              new TextField("Description", description, Field.Store.YES),
                              new TextField(
                                  "TopicTags",
                                  topicTags,
                                  Field.Store.YES)
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
                this.Get<ILogger<Search>>().Log(null, this, ex);
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
        if (!DirectoryReader.IndexExists(FSDirectory.Open(SearchIndexFolder)))
        {
            return null;
        }

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
    private async Task<SearchMessage> MapSearchDocumentToDataAsync(
        Highlighter highlighter,
        Analyzer analyzer,
        Document doc,
        List<VAccess> userAccessList)
    {
        var forumId = doc.Get("ForumId").ToType<int>();

        if (userAccessList.Count == 0 || !userAccessList.Exists(v => v.ForumID == forumId && v.ReadAccess > 0))
        {
            return null;
        }

        var flags = doc.Get("Flags").ToType<int>();
        var messageFlags = new MessageFlags(flags);

        var message = doc.Get("Message");

        try
        {
            message = GetHighlight(highlighter, analyzer, "Message", message);
        }
        catch (Exception)
        {
            // Ignore
            message = doc.Get("Message");
        }
        finally
        {
            if (message.IsNotSet())
            {
                message = doc.Get("Message");
            }
        }

        var formattedMessage = this.Get<IFormatMessage>().Format(
            doc.Get("MessageId").ToType<int>(),
            message,
            messageFlags);

        formattedMessage = await this.Get<IBBCodeService>().FormatMessageWithCustomBBCodeAsync(
                               formattedMessage,
                               new MessageFlags(flags),
                               doc.Get("UserId").ToType<int>(),
                               doc.Get("MessageId").ToType<int>());

        message = formattedMessage;

        string topic;

        try
        {
            topic = GetHighlight(highlighter, analyzer, "Topic", doc.Get("Topic"));
        }
        catch (Exception)
        {
            topic = doc.Get("Topic");
        }

        string posted;

        try
        {
            var postedDateTime = doc.Get("Posted").ToType<DateTime>();

            posted = this.Get<BoardSettings>().ShowRelativeTime
                         ? postedDateTime.ToRelativeTime()
                         : this.Get<IDateTimeService>().Format(DateTimeFormat.BothTopic, postedDateTime);
        }
        catch (Exception)
        {
            posted = doc.Get("Posted");
        }

        return new SearchMessage {
            MessageId = doc.Get("MessageId").ToType<int>(),
            Message = message,
            Flags = flags,
            Posted = posted,
            UserName = HttpUtility.HtmlEncode(doc.Get("Author")),
            UserId = doc.Get("UserId").ToType<int>(),
            TopicId = doc.Get("TopicId").ToType<int>(),
            Topic = topic.IsSet() ? topic : doc.Get("Topic"),
            TopicTags = doc.Get("TopicTags"),
            ForumId = doc.Get("ForumId").ToType<int>(),
            Description = doc.Get("Description"),
            TopicUrl = this.Get<LinkBuilder>().GetTopicLink(
                doc.Get("TopicId").ToType<int>(),
                doc.Get("Topic")),
            MessageUrl = this.Get<LinkBuilder>().GetMessageLink(doc.Get("Topic"),
                doc.Get("MessageId").ToType<int>()),
            ForumUrl = this.Get<LinkBuilder>().GetForumLink(
                doc.Get("ForumId").ToType<int>(),
                doc.Get("ForumName")),
            UserDisplayName = HttpUtility.HtmlEncode(doc.Get("AuthorDisplay")),
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
    private async Task<List<SearchMessage>> MapSearchToDataListAsync(
        Highlighter highlighter,
        Analyzer analyzer,
        IndexSearcher searcher,
        IEnumerable<ScoreDoc> hits,
        int pageIndex,
        int pageSize,
        List<VAccess> userAccessList)
    {
        var skip = pageSize * pageIndex;
        return await hits.ToAsyncEnumerable()
                   .SelectAwait(
                       async hit => await this.MapSearchDocumentToDataAsync(
                                        highlighter,
                                        analyzer,
                                        searcher.Doc(hit.Doc),
                                        userAccessList)).Where(item => item != null)
                   .OrderByDescending(item => item.MessageId).Skip(skip).Take(pageSize).ToListAsync();
    }

    /// <summary>
    /// Searches the index.
    /// </summary>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="searchQuery">The search query.</param>
    /// <param name="searchField">The search field.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <returns>
    /// Returns the Search results
    /// </returns>
    private async Task<Tuple<List<SearchMessage>, int>> SearchIndexAsync(
        int forumId,
        string searchQuery,
        string searchField = "",
        int pageIndex = 1,
        int pageSize = 1000)
    {
        if (searchQuery.Replace("*", string.Empty).Replace("?", string.Empty).IsNotSet())
        {
            return new Tuple<List<SearchMessage>, int>([], 0);
        }

        // Insert forum access here
        var userAccessList = await this.GetRepository<VAccess>().GetAsync(v => v.UserID == BoardContext.Current.PageUserID);

        // filter forum
        if (forumId > 0)
        {
            userAccessList = userAccessList.FindAll(v => v.ForumID == forumId);
        }

        var searcher = this.GetSearcher();

        if (searcher == null)
        {
            return new Tuple<List<SearchMessage>, int>([], 0);
        }

        var hitsLimit = this.Get<BoardSettings>().ReturnSearchMax;

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

            var highlighter = new Highlighter(formatter, scorer) { TextFragmenter = fragmenter };

            var results = await this.MapSearchToDataListAsync(
                highlighter,
                analyzer,
                searcher,
                hits,
                pageIndex,
                pageSize,
                userAccessList);

            analyzer.Dispose();

            return new Tuple<List<SearchMessage>, int>(results, hits.Length);
        }
        else
        {
            var parser = new MultiFieldQueryParser(
                MatchVersion,
                [
                    "Message", "Topic",
                        this.Get<BoardSettings>().EnableDisplayName ? "AuthorDisplay" : "Author", "TopicTags"
                ],
                analyzer);

            var query = ParseQuery(searchQuery, parser);
            scorer = new QueryScorer(query);

            // sort by date
            var sort = new Sort(new SortField("MessageId", SortFieldType.STRING, true));

            var fil = new BooleanFilter();

            // search this forum
            if (forumId > 0)
            {
                fil.Add(new FilterClause(new TermsFilter(new Term("ForumId", forumId.ToString())), Occur.SHOULD));
            }
            else
            {
                // filter user access
                if (userAccessList.HasItems())
                {
                    userAccessList.Where(a => a.ReadAccess == 0).ForEach(
                        access => fil.Add(
                            new FilterClause(
                                new TermsFilter(new Term("ForumId", access.ForumID.ToString())),
                                Occur.MUST_NOT)));
                }
            }

            var hits = searcher.Search(query, fil.Any() ? fil : null, hitsLimit, sort).ScoreDocs;

            var highlighter = new Highlighter(formatter, scorer) { TextFragmenter = fragmenter };

            var results = await this.MapSearchToDataListAsync(
                highlighter,
                analyzer,
                searcher,
                hits,
                pageIndex,
                pageSize,
                userAccessList);

            this.searcherManager.Release(searcher);

            return new Tuple<List<SearchMessage>, int>(results, hits.Length);
        }
    }

    /// <summary>
    /// Searches for similar words
    /// </summary>
    /// <param name="filter">
    /// The filter.
    /// </param>
    /// <param name="searchQuery">
    /// The search query.
    /// </param>
    /// <param name="searchField">
    /// The search field.
    /// </param>
    /// <returns>
    /// Returns the Search results
    /// </returns>
    private List<SearchMessage> SearchSimilarIndex(
        string filter,
        string searchQuery,
        string searchField)
    {
        if (searchQuery.Replace("*", string.Empty).Replace("?", string.Empty).IsNotSet())
        {
            return [];
        }

        // Insert forum access here
        var userAccessList = this.GetRepository<VAccess>().Get(v => v.UserID == BoardContext.Current.PageUserID);

        var searcher = this.GetSearcher();

        if (searcher == null)
        {
            return [];
        }

        var booleanFilter = new BooleanFilter
                                {
                                    new FilterClause(new TermsFilter(new Term("TopicId", filter)), Occur.MUST_NOT)
                                };

        var hitsLimit = this.Get<BoardSettings>().ReturnSearchMax;

        var parser = new QueryParser(MatchVersion, searchField, this.standardAnalyzer);
        var query = ParseQuery(searchQuery, parser);

        var hits = filter.IsSet()
                       ? searcher.Search(query, booleanFilter, hitsLimit).ScoreDocs
                       : searcher.Search(query, hitsLimit).ScoreDocs;

        var results = this.MapSearchToDataList(searcher, hits, userAccessList);

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