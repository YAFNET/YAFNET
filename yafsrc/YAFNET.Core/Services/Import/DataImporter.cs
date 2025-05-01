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

using System.Collections.Generic;

using YAF.Types.Objects;

namespace YAF.Core.Services.Import;

using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The data importer service.
/// </summary>
public class DataImporter : IHaveServiceLocator, IDataImporter
{
    /// <summary>
    ///     Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeService"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public DataImporter(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

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
    public int BBCodeExtensionImport(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BBCode>();

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

                        var updateEntry = new BBCode {
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
                                return;
                            }

                            updateEntry.ID = bbCodeExtension.ID;

                            // update this bbcode...
                            repository.Update(updateEntry);
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
            throw new FormatException("Import stream is not expected format.");
        }

        return importedCount;
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
    public int BannedEmailAddressesImport(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedEmail>();
        var existingBannedEmailList = repository.Get(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();

            if (line.IsNotSet() || !line.Contains('@'))
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
    public int BannedIpAddressesImport(int boardId, int userId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedIP>();
        var existingBannedIpList = repository.Get(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();

            if (line.IsNotSet() || !IPAddress.TryParse(line, out var importAddress))
            {
                continue;
            }

            if (existingBannedIpList.Exists(b => b.Mask == importAddress.ToString()))
            {
                continue;
            }

            if (repository.Save(null, importAddress.ToString(), "Imported IP Address", userId, boardId))
            {
                importedCount++;
            }
        }

        return importedCount;
    }

    /// <summary>
    /// Import the most recent Ip Addresses from AbuseIpDb.com
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="blackListData">The black list data.</param>
    /// <returns>Returns how many address where imported.</returns>
    public int BannedIpAddressesImport(int boardId, int userId, List<BlackListEntry> blackListData)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedIP>();
        var existingBannedIpList = repository.Get(x => x.BoardID == boardId);

        foreach (var data in blackListData)
        {
            if (!IPAddress.TryParse(data.IpAddress, out var importAddress))
            {
                continue;
            }

            if (existingBannedIpList.Exists(b => b.Mask == importAddress.ToString()))
            {
                continue;
            }

            if (repository.Save(null, importAddress.ToString(), $"Imported IP Address from AbuseIpDb.com with {data.AbuseConfidenceScore} confidence", userId, boardId))
            {
                importedCount++;
            }
        }

        return importedCount;
    }

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
    public int BannedNamesImport(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedName>();
        var existingBannedNameList = repository.Get(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();

            if (line.IsNotSet())
            {
                continue;
            }

            if (existingBannedNameList.Exists(b => b.Mask == line))
            {
                continue;
            }

            if (repository.Save(null, line, "Imported PageUser Name", boardId))
            {
                importedCount++;
            }
        }

        return importedCount;
    }

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
    public int BannedUserAgentsImport(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedUserAgent>();
        var existingBannedNameList = repository.Get(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();

            if (line.IsNotSet())
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
    public int SpamWordsImport(int boardId, Stream inputStream)
    {
        var repository = this.GetRepository<SpamWords>();

        // import spam words...
        var spamWords = new DataSet();
        spamWords.ReadXml(inputStream);

        if (spamWords.Tables["YafSpamWords"]?.Columns["spamword"] == null)
        {
            return 0;
        }

        var spamWordsList = repository.Get(x => x.BoardID == boardId);

        var imports = new List<SpamWords>();

        // import any extensions that don't exist...
        spamWords.Tables["YafSpamWords"].Rows.Cast<DataRow>()
            .Where(row => spamWordsList.TrueForAll(s => s.SpamWord != row["SpamWord"].ToString())).ForEach(
                row =>
                    {
                        // add this...
                        imports.Add(new SpamWords { BoardID = boardId, SpamWord = row["SpamWord"].ToString() });
                    });

        repository.BulkInsert(imports);

        return imports.Count;
    }

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
    public async Task<int> ImportingUsersAsync(Stream inputStream, bool isXml)
    {
        var importedCount = 0;

        if (isXml)
        {
            var usersDataSet = new DataSet();
            usersDataSet.ReadXml(inputStream);

            if (usersDataSet.Tables["User"] != null)
            {
                foreach (DataRow row in usersDataSet.Tables["User"].Rows)
                {
                    if (await this.Get<IAspNetUsersHelper>().GetUserByNameAsync((string)row["Name"]) == null)
                    {
                        importedCount = await this.ImportUserAsync(row, importedCount);
                    }
                }
            }
            else
            {
                throw new FormatException("Import stream is not expected format.");
            }
        }
        else
        {
            var usersTable = new DataTable();

            var streamReader = new StreamReader(inputStream);

            var headers = (await streamReader.ReadLineAsync())?.Split(',');

            headers.ForEach(header => usersTable.Columns.Add(header));

            while (streamReader.Peek() >= 0)
            {
                var dr = usersTable.NewRow();
                var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))",
                    RegexOptions.None,
                    TimeSpan.FromMilliseconds(100));
                dr.ItemArray = regex.Split(await streamReader.ReadLineAsync());

                usersTable.Rows.Add(dr);
            }

            streamReader.Close();

            foreach (DataRow row in usersTable.Rows)
            {
                if (await this.Get<IAspNetUsersHelper>().GetUserByNameAsync((string)row["Name"]) == null)
                {
                    importedCount = await this.ImportUserAsync(row, importedCount);
                }
            }
        }

        return importedCount;
    }

    /// <summary>
    /// Import the User From the Current Table Row
    /// </summary>
    /// <param name="row">
    /// The row with the User Information.
    /// </param>
    /// <param name="importCount">
    /// The import Count.
    /// </param>
    /// <returns>
    /// Returns the Imported User Count.
    /// </returns>
    private async Task<int> ImportUserAsync(DataRow row, int importCount)
    {
        // Also Check if the Email is unique and exists
        if (await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync((string)row["Email"]) != null)
        {
            return importCount;
        }

        var pass = PasswordGenerator.GeneratePassword(true, true, true, true, false, 16);

        // create empty profile just so they have one
        var userProfile = new ProfileInfo();

        // Add Profile Fields to User List Table.
        if (row.Table.Columns.Contains("RealName") && ((string)row["RealName"]).IsSet())
        {
            userProfile.RealName = (string)row["RealName"];
        }

        if (row.Table.Columns.Contains("Blog") && ((string)row["Blog"]).IsSet())
        {
            userProfile.Blog = (string)row["Blog"];
        }

        if (row.Table.Columns.Contains("Gender") && ((string)row["Gender"]).IsSet())
        {
            _ = int.TryParse((string)row["Gender"], out var gender);

            userProfile.Gender = gender;
        }

        if (row.Table.Columns.Contains("Birthday") && ((string)row["Birthday"]).IsSet())
        {
            _ = DateTime.TryParse((string)row["Birthday"], out var userBirthdate);

            if (userBirthdate > DateTimeHelper.SqlDbMinTime())
            {
                userProfile.Birthday = userBirthdate;
            }
        }

        if (row.Table.Columns.Contains("Location") && ((string)row["Location"]).IsSet())
        {
            userProfile.Location = (string)row["Location"];
        }

        if (row.Table.Columns.Contains("Country") && ((string)row["Country"]).IsSet())
        {
            userProfile.Country = (string)row["Country"];
        }

        if (row.Table.Columns.Contains("Region") && ((string)row["Region"]).IsSet())
        {
            userProfile.Region = (string)row["Region"];
        }

        if (row.Table.Columns.Contains("City") && ((string)row["City"]).IsSet())
        {
            userProfile.City = (string)row["City"];
        }

        if (row.Table.Columns.Contains("Interests") && ((string)row["Interests"]).IsSet())
        {
            userProfile.Interests = (string)row["Interests"];
        }

        if (row.Table.Columns.Contains("Homepage") && ((string)row["Homepage"]).IsSet())
        {
            userProfile.Homepage = (string)row["Homepage"];
        }

        if (row.Table.Columns.Contains("XMPP") && ((string)row["XMPP"]).IsSet())
        {
            userProfile.XMPP = (string)row["XMPP"];
        }

        if (row.Table.Columns.Contains("Occupation") && ((string)row["Occupation"]).IsSet())
        {
            userProfile.Occupation = (string)row["Occupation"];
        }

        if (row.Table.Columns.Contains("Facebook") && ((string)row["Facebook"]).IsSet())
        {
            userProfile.Facebook = (string)row["Facebook"];
        }

        var user = new AspNetUsers
        {
            Id = Guid.NewGuid().ToString(),
            ApplicationId = BoardContext.Current.BoardSettings.ApplicationId,
            UserName = (string)row["Name"],
            LoweredUserName = (string)row["Name"],
            Email = (string)row["Email"],
            IsApproved = true,

            Profile_Birthday = userProfile.Birthday,
            Profile_Blog = userProfile.Blog,
            Profile_Gender = userProfile.Gender,
            Profile_Homepage = userProfile.Homepage,
            Profile_Facebook = userProfile.Facebook,
            Profile_Interests = userProfile.Interests,
            Profile_Location = userProfile.Location,
            Profile_Country = userProfile.Country,
            Profile_Region = userProfile.Region,
            Profile_City = userProfile.City,
            Profile_Occupation = userProfile.Occupation,
            Profile_RealName = userProfile.RealName,
            Profile_XMPP = userProfile.XMPP
        };

        await this.Get<IAspNetUsersHelper>().CreateUserAsync(user, pass);

        // setup initial roles (if any) for this user
        await this.Get<IAspNetRolesHelper>().SetupUserRolesAsync(BoardContext.Current.PageBoardID, user);

        // create the user in the YAF DB as well as sync roles...
        var userId = await this.Get<IAspNetRolesHelper>().CreateForumUserAsync(user, BoardContext.Current.PageBoardID);

        if (userId == null)
        {
            // something is seriously wrong here -- redirect to failure...
            return importCount;
        }

        // send user register notification to the new users
        await this.Get<ISendNotification>().SendRegistrationNotificationToUserAsync(
            user, pass, "NOTIFICATION_ON_REGISTER");

        var timeZone = 0;

        if (row.Table.Columns.Contains("Timezone") && ((string)row["Timezone"]).IsSet())
        {
            _ = int.TryParse((string)row["Timezone"], out timeZone);
        }

        var autoWatchTopicsEnabled = BoardContext.Current.BoardSettings.DefaultNotificationSetting
                                     == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

        this.GetRepository<User>().Save(
            userId.Value,
            timeZone.ToString(),
            row.Table.Columns.Contains("LanguageFile") ? row["LanguageFile"].ToString() : null,
            row.Table.Columns.Contains("Culture") ? row["Culture"].ToString() : null,
            row.Table.Columns.Contains("ThemeFile") ? row["ThemeFile"].ToString() : null,
            false,
            true,
            5);

        // save the settings...
        this.GetRepository<User>().SaveNotification(
            userId.Value,
            autoWatchTopicsEnabled,
            BoardContext.Current.BoardSettings.DefaultNotificationSetting.ToInt(),
            BoardContext.Current.BoardSettings.DefaultSendDigestEmail);

        importCount++;

        return importCount;
    }
}

