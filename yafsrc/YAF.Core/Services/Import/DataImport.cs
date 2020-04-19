/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Services.Import
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    /// <summary>
    /// The data import.
    /// </summary>
    public static class DataImport
    {
        /// <summary>
        /// The bb code extension import.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="inputStream">
        /// The input stream.
        /// </param>
        /// <returns>
        /// Returns How Many Extensions where imported.
        /// </returns>
        /// <exception cref="Exception">Import stream is not expected format.
        /// </exception>
        public static int BBCodeExtensionImport(int boardId, Stream inputStream)
        {
            var importedCount = 0;

            var repository = BoardContext.Current.GetRepository<BBCode>();

            // import extensions...
            var dsBBCode = new DataSet();
            dsBBCode.ReadXml(inputStream);

            if (dsBBCode.Tables["YafBBCode"]?.Columns["Name"] != null && dsBBCode.Tables["YafBBCode"].Columns["SearchRegex"] != null && dsBBCode.Tables["YafBBCode"].Columns["ExecOrder"] != null)
            {
                var bbcodeList = repository.GetByBoardId(boardId);

                // import any extensions that don't exist...
                dsBBCode.Tables["YafBBCode"].Rows.Cast<DataRow>().ForEach(
                    row =>
                        {
                            var name = row["Name"].ToString();

                            var bbCodeExtension = bbcodeList.FirstOrDefault(b => b.Name.Equals(name));

                            if (bbCodeExtension != null)
                            {
                                // update this bbcode...
                                repository.Save(
                                    bbCodeExtension.ID,
                                    row["Name"].ToString(),
                                    row["Description"].ToString(),
                                    row["OnClickJS"].ToString(),
                                    row["DisplayJS"].ToString(),
                                    row["EditJS"].ToString(),
                                    row["DisplayCSS"].ToString(),
                                    row["SearchRegex"].ToString(),
                                    row["ReplaceRegex"].ToString(),
                                    row["Variables"].ToString(),
                                    row["UseModule"].ToType<bool>(),
                                    row["UseToolbar"].ToType<bool>(),
                                    row["ModuleClass"].ToString(),
                                    row["ExecOrder"].ToType<int>(),
                                    boardId);
                            }
                            else
                            {
                                // add this bbcode...
                                repository.Save(
                                    null,
                                    row["Name"].ToString(),
                                    row["Description"].ToString(),
                                    row["OnClickJS"].ToString(),
                                    row["DisplayJS"].ToString(),
                                    row["EditJS"].ToString(),
                                    row["DisplayCSS"].ToString(),
                                    row["SearchRegex"].ToString(),
                                    row["ReplaceRegex"].ToString(),
                                    row["Variables"].ToString(),
                                    row["UseModule"].ToType<bool>(),
                                    row["UseToolbar"].ToType<bool>(),
                                    row["ModuleClass"].ToString(),
                                    row["ExecOrder"].ToType<int>(),
                                    boardId);

                                importedCount++;
                            }
                        });
            }
            else
            {
                throw new Exception("Import stream is not expected format.");
            }

            return importedCount;
        }

        /// <summary>
        /// Import List of Banned Email Addresses
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
        public static int BannedEmailAddressesImport(int boardId, int userId, Stream inputStream)
        {
            var importedCount = 0;

            var repository = BoardContext.Current.GetRepository<BannedEmail>();
            var existingBannedEmailList = repository.Get(x => x.BoardID == boardId);

            using (var streamReader = new StreamReader(inputStream))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();

                    if (line.IsNotSet() || !line.Contains("@"))
                    {
                        continue;
                    }

                    if (existingBannedEmailList.Any(b => b.Mask == line))
                    {
                        continue;
                    }

                    repository.Save(null, line, "Imported Email Adress", boardId);
                    importedCount++;
                }
            }

            return importedCount;
        }

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
        public static int BannedIpAddressesImport(int boardId, int userId, Stream inputStream)
        {
            var importedCount = 0;

            var repository = BoardContext.Current.GetRepository<BannedIP>();
            var existingBannedIPList = repository.Get(x => x.BoardID == boardId);

            using (var streamReader = new StreamReader(inputStream))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();

                    if (line.IsNotSet() || !IPAddress.TryParse(line, out var importAddress))
                    {
                        continue;
                    }

                    if (existingBannedIPList.Any(b => b.Mask == importAddress.ToString()))
                    {
                        continue;
                    }

                    repository.Save(null, importAddress.ToString(), "Imported IP Adress", userId, boardId);
                    importedCount++;
                }
            }

            return importedCount;
        }

        /// <summary>
        /// Import List of Banned User Names
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
        public static int BannedNamesImport(int boardId, int userId, Stream inputStream)
        {
            var importedCount = 0;

            var repository = BoardContext.Current.GetRepository<BannedName>();
            var existingBannedNameList = repository.Get(x => x.BoardID == boardId);

            using (var streamReader = new StreamReader(inputStream))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();

                    if (line.IsNotSet())
                    {
                        continue;
                    }

                    if (existingBannedNameList.Any(b => b.Mask == line))
                    {
                        continue;
                    }

                    repository.Save(null, line, "Imported User Name", boardId);
                    importedCount++;
                }
            }

            return importedCount;
        }

        /// <summary>
        /// Import List of Spam Words
        /// </summary>
        /// <param name="boardId">The board identifier.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>Returns the Number of Imported Items.</returns>
        public static int SpamWordsImport(int boardId, Stream inputStream)
        {
            var importedCount = 0;

            var repository = BoardContext.Current.GetRepository<Spam_Words>();

            // import spam words...
            var spamWords = new DataSet();
            spamWords.ReadXml(inputStream);

            if (spamWords.Tables["YafSpamWords"]?.Columns["spamword"] == null)
            {
                return importedCount;
            }

            var spamWordsList = repository.Get(x => x.BoardID == boardId);

            // import any extensions that don't exist...
            spamWords.Tables["YafSpamWords"].Rows.Cast<DataRow>()
                .Where(row => spamWordsList.Any(s => s.SpamWord == row["spamword"])).ForEach(
                    row =>
                        {
                            // add this...
                            repository.Save(null, row["spamword"].ToString());
                            importedCount++;
                        });

            return importedCount;
        }
    }
}