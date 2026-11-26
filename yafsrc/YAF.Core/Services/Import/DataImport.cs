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

namespace YAF.Core.Services.Import;

using System;
using System.Data;
using System.IO;
using System.Text;

using YAF.Core.Model;
using YAF.Types.Constants;
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
    /// <exception cref="FormatException">Import stream is not expected format.
    /// </exception>
    /// <param name="isDefaultFile">
    ///     The file is the BBCodeExtensions.xml of the forum (install, new board, upgrade): an extension the admin
    ///     changed is kept. An import of the admin replaces the extensions.
    /// </param>
    public static int BBCodeExtensionImport(int boardId, Stream inputStream, bool isDefaultFile = false)
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

                        // a bb code with an invalid search expression is not imported
                        if (row["SearchRegex"].ToString().IsSet()
                            && !ValidationHelper.IsValidRegex(row["SearchRegex"].ToString()))
                        {
                            return;
                        }

                        var bbCodeExtension = bbcodeList.FirstOrDefault(b => b.Name.Equals(name));

                        var updateEntry = new BBCode
                        {
                            BoardID = boardId,
                            Name = name,
                            Description = row["Description"].ToString(),
                            OnClickJS = row["OnClickJS"].ToString(),
                            DisplayJS = row["DisplayJS"].ToString(),
                            EditJS = row["EditJS"].ToString(),
                            DisplayCSS = row["DisplayCSS"].ToString(),
                            SearchRegex = row["SearchRegex"].ToString(),
                            ReplaceRegex = row["ReplaceRegex"].ToString(),
                            Variables = row["Variables"].ToString(),
                            UseModule = row["UseModule"].ToType<bool>(),
                            UseToolbar = row["UseToolbar"].ToType<bool>(),
                            ModuleClass = row["ModuleClass"].ToString(),
                            ExecOrder = row["ExecOrder"].ToType<int>()
                        };

                        if (bbCodeExtension != null)
                        {
                            if (BBCode.Equals(updateEntry, bbCodeExtension))
                            {
                                if (isDefaultFile)
                                {
                                    SaveDefaultHash(boardId, updateEntry);
                                }

                                return;
                            }

                            // the upgrade replaced the extensions the admin changed: only an extension that is still
                            // the installed one (the hash of the last installed version) is updated, a changed one is
                            // kept
                            if (isDefaultFile && IsChangedByAdmin(boardId, bbCodeExtension))
                            {
                                BoardContext.Current.Get<ILoggerService>().Log(
                                    $"The BBCode extension {name} was changed by an admin and is not updated. The forum has a new version of it in BBCodeExtensions.xml, import the file to use it.",
                                    EventLogTypes.Warning);

                                return;
                            }

                            updateEntry.ID = bbCodeExtension.ID;

                            // update this bbcode...
                            repository.Update(updateEntry);

                            if (isDefaultFile)
                            {
                                SaveDefaultHash(boardId, updateEntry);
                            }
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

                            if (isDefaultFile)
                            {
                                SaveDefaultHash(boardId, updateEntry);
                            }

                            importedCount++;
                        }
                    });
        }
        else
        {
            throw new FormatException("Import stream is not expected format.");
        }

        return importedCount;
    }

    /// <summary>
    /// Gets the hash of a BBCode extension (all fields of the definition, an empty field is the same as null).
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns>The hash.</returns>
    public static string DefinitionHash(BBCode code)
    {
        var definition = string.Join(
            "\u001f",
            code.Name ?? string.Empty,
            code.Description ?? string.Empty,
            code.OnClickJS ?? string.Empty,
            code.DisplayJS ?? string.Empty,
            code.EditJS ?? string.Empty,
            code.DisplayCSS ?? string.Empty,
            code.SearchRegex ?? string.Empty,
            code.ReplaceRegex ?? string.Empty,
            code.Variables ?? string.Empty,
            code.UseModule ?? false,
            code.UseToolbar ?? false,
            code.ModuleClass ?? string.Empty,
            code.ExecOrder);

        using var sha = System.Security.Cryptography.SHA256.Create();

        return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(definition))).Replace("-", string.Empty);
    }

    /// <summary>
    /// The registry name of the hash of the installed version of a BBCode extension.
    /// </summary>
    /// <param name="name">The name of the extension.</param>
    /// <returns>The registry name.</returns>
    private static string DefaultHashName(string name)
    {
        return $"bbcodedefault.{name}".ToLowerInvariant();
    }

    /// <summary>
    /// Saves the hash of the installed version of a BBCode extension (of BBCodeExtensions.xml).
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="code">The code.</param>
    private static void SaveDefaultHash(int boardId, BBCode code)
    {
        BoardContext.Current.GetRepository<Registry>().Save(DefaultHashName(code.Name), DefinitionHash(code), boardId);
    }

    /// <summary>
    /// Is the extension different from the version the forum installed (an admin changed it)? Without a hash
    /// (installed by an older version) it is not known, the extension is updated like before.
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="code">The extension in the database.</param>
    /// <returns>True if an admin changed it.</returns>
    private static bool IsChangedByAdmin(int boardId, BBCode code)
    {
        var name = DefaultHashName(code.Name);

        var installed = BoardContext.Current.GetRepository<Registry>()
            .GetSingle(r => r.Name == name && r.BoardID == boardId)?.Value;

        return installed.IsSet() && installed != DefinitionHash(code);
    }

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
    public static int BannedEmailAddressesImport(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = BoardContext.Current.GetRepository<BannedEmail>();
        var existingBannedEmailList = repository.Get(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);

        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine()?.Trim();

            // the masks are regular expressions, an invalid one would break the ban check
            if (line.IsNotSet() || !line.Contains("@") || !ValidationHelper.IsValidRegex(line))
            {
                continue;
            }

            if (existingBannedEmailList.Exists(b => b.Mask == line))
            {
                continue;
            }

            if (repository.Save(null, line, "Imported Email Address", boardId))
            {
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

        using var streamReader = new StreamReader(inputStream);

        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine()?.Trim();

            // ip addresses (v4 and v6) and ipv4 masks (e.g. 192.168.1.*), IPAddress.TryParse also accepted
            // incomplete addresses (e.g. "1.2.3" as 1.2.0.3) but no masks
            if (!IPHelper.IsValidBanMask(line))
            {
                continue;
            }

            if (existingBannedIPList.Exists(b => b.Mask == line))
            {
                continue;
            }

            if (repository.Save(null, line, "Imported IP Address", userId, boardId))
            {
                importedCount++;
            }
        }

        return importedCount;
    }

    /// <summary>
    /// Import List of Banned User Names
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>
    /// Returns the Number of Imported Items.
    /// </returns>
    /// <exception cref="Exception">
    /// Import stream is not expected format.
    /// </exception>
    public static int BannedNamesImport(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = BoardContext.Current.GetRepository<BannedName>();
        var existingBannedNameList = repository.Get(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);

        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine()?.Trim();

            // the masks are regular expressions, an invalid one would break the ban check
            if (line.IsNotSet() || !ValidationHelper.IsValidRegex(line))
            {
                continue;
            }

            if (existingBannedNameList.Exists(b => b.Mask == line))
            {
                continue;
            }

            if (repository.Save(null, line, "Imported User Name", boardId))
            {
                importedCount++;
            }
        }

        return importedCount;
    }

    /// <summary>
    /// Import List of Banned UserAgent
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>
    /// Returns the Number of Imported Items.
    /// </returns>
    /// <exception cref="Exception">
    /// Import stream is not expected format.
    /// </exception>
    public static int BannedUserAgentsImport(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = BoardContext.Current.GetRepository<BannedUserAgent>();
        var existingBannedNameList = repository.Get(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);

        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine()?.Trim();

            // the patterns are regular expressions, an invalid one would break the ban check
            if (line.IsNotSet() || !ValidationHelper.IsValidRegex(line))
            {
                continue;
            }

            if (existingBannedNameList.Exists(b => b.UserAgent == line))
            {
                continue;
            }

            if (repository.Save(null, line, boardId))
            {
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
        var repository = BoardContext.Current.GetRepository<Spam_Words>();

        // import spam words...
        var spamWords = new DataSet();
        spamWords.ReadXml(inputStream);

        if (spamWords.Tables["YafSpamWords"]?.Columns["SpamWord"] == null)
        {
            return 0;
        }

        var spamWordsList = repository.Get(x => x.BoardID == boardId);

        var imports = new List<Spam_Words>();

        // import any extensions that don't exist...
        spamWords.Tables["YafSpamWords"].Rows.Cast<DataRow>()
            .Where(row => spamWordsList.TrueForAll(s => s.SpamWord != row["SpamWord"].ToString())).ForEach(
                row =>
                    {
                        // add this...
                        imports.Add(new Spam_Words { BoardID = boardId, SpamWord = row["SpamWord"].ToString() });
                    });

        repository.BulkInsert(imports);

        // the cached spam words are loaded again
        if (imports.Count > 0)
        {
            repository.FireNew();
        }

        return imports.Count;
    }
}