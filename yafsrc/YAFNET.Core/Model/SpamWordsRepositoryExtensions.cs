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

using System.Threading.Tasks;

namespace YAF.Core.Model;

using YAF.Types.Models;

/// <summary>
///     The spam words repository extensions.
/// </summary>
public static class SpamWordsRepositoryExtensions
{
    /// <summary>
    /// Saves changes to a word.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="spamWordId">The spam word identifier.</param>
    /// <param name="spamWord">The spam word.</param>
    /// <param name="boardId">The board identifier.</param>
    public static Task SaveAsync(
        this IRepository<SpamWords> repository,
        int? spamWordId,
        string spamWord,
        int? boardId = null)
    {
        return repository.UpsertAsync(
            new SpamWords
            {
                BoardID = boardId ?? repository.BoardID,
                ID = spamWordId ?? 0,
                SpamWord = spamWord
            });
    }
}