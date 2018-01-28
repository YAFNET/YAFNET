/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Types.Interfaces
{
    using System.Collections.Generic;

    using YAF.Types.Objects;

    /// <summary>
    /// The Search interface
    /// </summary>
    public interface ISearch
    {
        #region Public Methods

        /// <summary>
        /// Optimizes the Search Index
        /// </summary>
        void Optimize();

        /// <summary>
        /// Clears the search Index.
        /// </summary>
        /// <returns>Returns if clearing was sucessfull</returns>
        bool ClearSearchIndex();

        /// <summary>
        /// Clears the search index record.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        void ClearSearchIndexRecord(int messageId);

        /// <summary>
        /// Adds or updates the search index
        /// </summary>
        /// <param name="messageList">The message list.</param>
        void AddUpdateSearchIndex(IEnumerable<SearchMessage> messageList);

        /// <summary>
        /// Adds or updates the search index
        /// </summary>
        /// <param name="message">The message.</param>
        void AddUpdateSearchIndex(SearchMessage message);

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
        List<SearchMessage> Search(int forumId, int userId, string input, string fieldName = "");

        /// <summary>
        /// Searches for similar words
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="input">The input.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Returns the list of search results.</returns>
        List<SearchMessage> SearchSimilar(int userId, string input, string fieldName = "");

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
        List<SearchMessage> SearchPaged(out int totalHits, int forumId, int userId, string input, int pageIndex, int pageSize, string fieldName = "");

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
        List<SearchMessage> SearchDefault(int forumId, int userId, string input, string fieldName = "");

        #endregion
    }
}