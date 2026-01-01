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

using System.Collections.Generic;

using YAF.Types.Objects;

namespace YAF.Types.Interfaces.Services;

using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Interface DataImporter
/// </summary>
public interface IDataImporter
{
    /// <summary>
    /// Import List of Banned Email Addresses
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>
    /// Returns the Number of Imported Items.
    /// </returns>
    /// <exception cref="Exception">
    /// Import stream is not expected format.
    /// </exception>
    Task<int> BannedEmailAddressesImportAsync(int boardId, Stream inputStream);

    /// <summary>
    /// Import List of Banned IP Addresses
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>
    /// Returns the Number of Imported Items.
    /// </returns>
    /// <exception cref="Exception">
    /// Import stream is not expected format.
    /// </exception>
    Task<int> BannedIpAddressesImportAsync(int boardId, int userId, Stream inputStream);

    /// <summary>
    /// Import the most recent Ip Addresses from AbuseIpDb.com
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="blackListData">The black list data.</param>
    /// <returns>Returns how many address where imported.</returns>
    Task<int> BannedIpAddressesImportAsync(int boardId, int userId, List<BlackListEntry> blackListData);

    /// <summary>
    /// Import List of Banned Usernames
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>
    /// Returns the Number of Imported Items.
    /// </returns>
    /// <exception cref="Exception">
    /// Import stream is not expected format.
    /// </exception>
    Task<int> BannedNamesImportAsync(int boardId, Stream inputStream);

    /// <summary>
    /// Import List of Banned UserAgents
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>
    /// Returns the Number of Imported Items.
    /// </returns>
    /// <exception cref="Exception">
    /// Import stream is not expected format.
    /// </exception>
    Task<int> BannedUserAgentsImportAsync(int boardId, Stream inputStream);

    /// <summary>
    /// The bb code extension import.
    /// </summary>
    /// <param name="boardId">
    ///     The board id.
    /// </param>
    /// <param name="inputStream">
    ///     The input stream.
    /// </param>
    /// <returns>
    /// Returns How Many Extensions where imported.
    /// </returns>
    /// <exception cref="Exception">Import stream is not expected format.
    /// </exception>
    Task<int> BBCodeExtensionImportAsync(int boardId, Stream inputStream);

    /// <summary>
    /// Import Users from the InputStream
    /// </summary>
    /// <param name="inputStream">
    ///     The input stream.
    /// </param>
    /// <param name="isXml">
    ///     Indicates if input Stream is Xml file
    /// </param>
    /// <returns>
    /// Returns How Many Users where imported.
    /// </returns>
    /// <exception cref="Exception">
    /// Import stream is not expected format.
    /// </exception>
    Task<int> ImportingUsersAsync(Stream inputStream, bool isXml);

    /// <summary>
    /// Import List of Spam Words
    /// </summary>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>Returns the Number of Imported Items.</returns>
    int SpamWordsImport(int boardId, Stream inputStream);
}