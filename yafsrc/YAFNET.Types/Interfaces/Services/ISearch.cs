/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Types.Interfaces.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using YAF.Types.Objects;

/// <summary>
/// The Search interface
/// </summary>
public interface ISearch
{
    /// <summary>
    /// Optimizes the Search Index
    /// </summary>
    void Optimize();

    /// <summary>
    /// Clears the search Index.
    /// </summary>
    /// <returns>Returns if clearing was successful</returns>
    bool ClearSearchIndex();

    /// <summary>
    /// Delete Search Index Record by Message Id.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    void DeleteSearchIndexRecordByMessageId(int messageId);

    /// <summary>
    /// Delete Search Index Record by Topic Id.
    /// </summary>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    void DeleteSearchIndexRecordByTopicId(int topicId);

    /// <summary>
    /// Adds the search index
    /// </summary>
    /// <param name="messageList">
    ///     The message list.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    Task AddSearchIndexAsync(IEnumerable<SearchMessage> messageList);

    /// <summary>
    /// The add search index item.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    void AddSearchIndexItem(SearchMessage message);

    /// <summary>
    /// Updates the Search Index Item or if not found adds it.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="dispose">
    /// Dispose IndexWriter after updating?
    /// </param>
    void UpdateSearchIndexItem(SearchMessage message, bool dispose = false);

    /// <summary>
    /// Only Get Number of Search Results (Hits)
    /// </summary>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    int CountHits(string input);

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
    List<SearchMessage> SearchSimilar(string filter, string input, string fieldName = "");

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
    Task<Tuple<List<SearchMessage>, int>> SearchPagedAsync(
        int forumId,
        string input,
        int pageIndex,
        int pageSize,
        string fieldName = "");

    /// <summary>
    /// Searches the default.
    /// </summary>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="input">The input.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>
    /// Returns the search results
    /// </returns>
    Task<Tuple<List<SearchMessage>, int>> SearchDefaultAsync(int forumId, string input, string fieldName = "");
}