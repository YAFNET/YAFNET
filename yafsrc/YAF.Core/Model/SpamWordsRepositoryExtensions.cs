/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2017 Ingo Herbote
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
namespace YAF.Core.Model
{
    using System.Collections.Generic;
    using System.Data;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The spam words repository extensions.
    /// </summary>
    public static class SpamWordsRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets a list of spam words
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="spamWordID">The spam word identifier.</param>
        /// <param name="boardId">The board identifier.</param>
        /// <returns>
        /// DataTable with spam words
        /// </returns>
        public static DataTable List(
            this IRepository<Spam_Words> repository,
            int? spamWordID = null,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.spam_words_list(BoardID: boardId ?? repository.BoardID, ID: spamWordID);
        }

        /// <summary>
        /// Gets a list of spam words
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="spamWordID">The spam word identifier.</param>
        /// <param name="boardId">The board identifier.</param>
        /// <returns>
        /// List with spam words
        /// </returns>
        public static IList<Spam_Words> ListTyped(
            this IRepository<Spam_Words> repository,
            int? spamWordID = null,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return
                    session.GetTyped<Spam_Words>(
                        r => r.spam_words_list(BoardID: boardId ?? repository.BoardID, ID: spamWordID));
            }
        }

        /// <summary>
        /// Saves changes to a word.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="spamWordID">The spam word identifier.</param>
        /// <param name="spamWord">The spam word.</param>
        /// <param name="boardId">The board identifier.</param>
        public static void Save(
            this IRepository<Spam_Words> repository,
            int? spamWordID,
            string spamWord,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.spam_words_save(
                ID: spamWordID,
                BoardID: boardId ?? repository.BoardID,
                spamword: spamWord);

            if (spamWordID.HasValue)
            {
                repository.FireUpdated(spamWordID);
            }
            else
            {
                repository.FireNew();
            }
        }

        #endregion
    }
}