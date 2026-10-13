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

namespace YAF.Core.Services.Import;

using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

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
    public async Task<int> BBCodeExtensionImportAsync(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BBCode>();

        // import extensions...
        var dsBBCode = new DataSet();
        dsBBCode.ReadXml(inputStream);

        if (dsBBCode.Tables["YafBBCode"]?.Columns["Name"] != null && dsBBCode.Tables["YafBBCode"].Columns["SearchRegex"] != null && dsBBCode.Tables["YafBBCode"].Columns["ExecOrder"] != null)
        {
            var bbcodeList = await repository.GetByBoardIdAsync(boardId);

            // import any extensions that don't exist...
            foreach (var row in dsBBCode.Tables["YafBBCode"].Rows.Cast<DataRow>())
            {
                var name = row["Name"].ToString();

                // an invalid search expression would break the rendering of the posts
                if (row["SearchRegex"].ToString().IsSet() && !ValidationHelper.IsValidRegex(row["SearchRegex"].ToString()))
                {
                    continue;
                }

                var bbCodeExtension = bbcodeList.Find(b => b.Name.Equals(name));

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
                        continue;
                    }

                    updateEntry.ID = bbCodeExtension.ID;

                    // update this bbcode...
                    await repository.UpdateAsync(updateEntry);
                }
                else
                {
                    // add this bbcode...
                    await repository.SaveAsync(
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
            }
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
    public async Task<int> BannedEmailAddressesImportAsync(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedEmail>();
        var existingBannedEmailList = await repository.GetAsync(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);

        while (await streamReader.ReadLineAsync() is { } line)
        {
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
    public async Task<int> BannedIpAddressesImportAsync(int boardId, int userId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedIP>();
        var existingBannedIpList = await repository.GetAsync(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);

        while (await streamReader.ReadLineAsync() is { } line)
        {
            if (line.IsNotSet())
            {
                continue;
            }

            line = line.Trim();

            // only ip addresses and masks like 192.168.1.*
            if (!IPHelper.IsValidBanMask(line) || existingBannedIpList.Exists(b => b.Mask == line))
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
    /// Import the most recent Ip Addresses from AbuseIpDb.com
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="blackListData">The black list data.</param>
    /// <returns>Returns how many address where imported.</returns>
    public async Task<int> BannedIpAddressesImportAsync(int boardId, int userId, List<BlackListEntry> blackListData)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedIP>();
        var existingBannedIpList = await repository.GetAsync(x => x.BoardID == boardId);

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
    public async Task<int> BannedNamesImportAsync(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedName>();
        var existingBannedNameList = await repository.GetAsync(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);

        while (await streamReader.ReadLineAsync() is { } line)
        {
            if (line.IsNotSet())
            {
                continue;
            }

            // the masks are regular expressions
            if (!ValidationHelper.IsValidRegex(line) || existingBannedNameList.Exists(b => b.Mask == line))
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
    public async Task<int> BannedUserAgentsImportAsync(int boardId, Stream inputStream)
    {
        var importedCount = 0;

        var repository = this.GetRepository<BannedUserAgent>();
        var existingBannedNameList = await repository.GetAsync(x => x.BoardID == boardId);

        using var streamReader = new StreamReader(inputStream);

        while (await streamReader.ReadLineAsync() is { } line)
        {
            if (line.IsNotSet())
            {
                continue;
            }

            // the user agents are regular expressions
            if (!ValidationHelper.IsValidRegex(line) || existingBannedNameList.Exists(b => b.UserAgent == line))
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
                    if (await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(Value(row, "Name")) == null)
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

            var headers = (await streamReader.ReadLineAsync())?.Split(',').Select(h => h.Trim()).ToArray() ?? [];

            if (!headers.Contains("Name", StringComparer.OrdinalIgnoreCase)
                || !headers.Contains("Email", StringComparer.OrdinalIgnoreCase))
            {
                throw new FormatException("Import stream is not expected format.");
            }

            headers.ForEach(header => usersTable.Columns.Add(header));

            var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))",
                RegexOptions.None,
                Constants.RegexTimeout);

            while (await streamReader.ReadLineAsync() is { } line)
            {
                if (line.IsNotSet())
                {
                    continue;
                }

                // rows with fewer values than headers get empty values, surrounding quotes are removed
                var values = regex.Split(line).Take(headers.Length).Select(Unquote).ToList();

                while (values.Count < headers.Length)
                {
                    values.Add(string.Empty);
                }

                var dr = usersTable.NewRow();
                dr.ItemArray = [.. values];

                usersTable.Rows.Add(dr);
            }

            streamReader.Close();

            foreach (DataRow row in usersTable.Rows)
            {
                if (await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(Value(row, "Name")) == null)
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
        // rows without user name or e-mail can not be imported
        if (Value(row, "Name").IsNotSet() || Value(row, "Email").IsNotSet())
        {
            return importCount;
        }

        // Also Check if the Email is unique and exists
        if (await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(Value(row, "Email")) != null)
        {
            return importCount;
        }

        var pass = PasswordGenerator.GeneratePassword(true, true, true, true, false, 16);

        // create empty profile just so they have one
        var userProfile = new ProfileInfo
                          {
                              // the values have to fit into the columns of the profile
                              RealName = Limited(row, "RealName", 255),
                              Blog = Limited(row, "Blog", 255),
                              Location = Limited(row, "Location", 255),
                              City = Limited(row, "City", 255),
                              Interests = Limited(row, "Interests", 4000),
                              Homepage = Limited(row, "Homepage", 255),
                              Occupation = Limited(row, "Occupation", 400)
                          };

        if (Value(row, "Gender").IsSet())
        {
            _ = int.TryParse(Value(row, "Gender"), out var gender);

            userProfile.Gender = gender;
        }

        if (Value(row, "Birthday").IsSet())
        {
            _ = DateTime.TryParse(Value(row, "Birthday"), out var userBirthdate);

            if (userBirthdate > DateTimeHelper.SqlDbMinTime())
            {
                userProfile.Birthday = userBirthdate;
            }
        }

        // only known countries (the code, e.g. "DE") and regions of the country, like the profile page
        var country = Value(row, "Country")?.Trim();

        if (country.IsSet() && StaticDataHelper.Countries().Any(c => c.Value.IsSet() && c.Value == country))
        {
            userProfile.Country = country;

            var region = Value(row, "Region")?.Trim();

            if (region.IsSet() && StaticDataHelper.Regions(country).Any(r => r.Value.IsSet() && r.Value == region))
            {
                userProfile.Region = region;
            }
        }

        var user = new AspNetUsers
        {
            Id = Guid.NewGuid().ToString(),
            ApplicationId = BoardContext.Current.BoardSettings.ApplicationId,
            UserName = Value(row, "Name"),
            LoweredUserName = Value(row, "Name").ToLower(),
            Email = Value(row, "Email"),
            IsApproved = true,

            Profile_Birthday = userProfile.Birthday,
            Profile_Blog = userProfile.Blog,
            Profile_Gender = userProfile.Gender,
            Profile_Homepage = userProfile.Homepage,
            Profile_Interests = userProfile.Interests,
            Profile_Location = userProfile.Location,
            Profile_Country = userProfile.Country,
            Profile_Region = userProfile.Region,
            Profile_City = userProfile.City,
            Profile_Occupation = userProfile.Occupation,
            Profile_RealName = userProfile.RealName
        };

        var result = await this.Get<IAspNetUsersHelper>().CreateUserAsync(user, pass);

        // refused by the identity (e.g. a name with characters that are not allowed or an invalid e-mail)
        if (!result.Succeeded)
        {
            this.Get<ILogger<DataImporter>>().Warn(
                $"User {user.UserName} not imported: {result.Errors.Select(e => e.Description).ToDelimitedString(", ")}");

            return importCount;
        }

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

        var autoWatchTopicsEnabled = BoardContext.Current.BoardSettings.DefaultNotificationSetting
                                     == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

        var newUser = await this.GetRepository<User>().GetByIdAsync(userId.Value);

        // the time zone id of the export (e.g. "W. Europe Standard Time"), otherwise the default of the new user
        var timeZone = newUser.TimeZone;

        if (Value(row, "Timezone") is { } timeZoneId
            && TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZoneInfo))
        {
            timeZone = timeZoneInfo.Id;
        }

        await this.GetRepository<User>().SaveAsync(
            userId.Value,
            newUser.UserFlags,
            timeZone,
            Value(row, "LanguageFile"),
            Value(row, "Culture"),
            Value(row, "ThemeFile"),
            false,
            true,
            5);

        // save the settings...
        await this.GetRepository<User>().SaveNotificationAsync(
            userId.Value,
            autoWatchTopicsEnabled,
            BoardContext.Current.BoardSettings.DefaultNotificationSetting.ToInt(),
            BoardContext.Current.BoardSettings.DefaultSendDigestEmail);

        importCount++;

        return importCount;
    }

    /// <summary>
    /// Gets the text of a column, <c>null</c> when the column is missing or empty.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column name.</param>
    /// <returns>Returns the value</returns>
    private static string Value(DataRow row, string column)
    {
        if (!row.Table.Columns.Contains(column) || row[column] is DBNull)
        {
            return null;
        }

        var value = row[column].ToString();

        return value.IsSet() ? value : null;
    }

    /// <summary>
    /// Gets the value of a column, shortened to the length of the column it is stored in.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <param name="maxLength">The length of the column.</param>
    /// <returns>The value or null.</returns>
    private static string Limited(DataRow row, string column, int maxLength)
    {
        var value = Value(row, column)?.Trim();

        return value.IsSet() ? value.Truncate(maxLength) : null;
    }

    /// <summary>
    /// Removes the quotes around a csv value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the value without quotes</returns>
    private static string Unquote(string value)
    {
        value = value.Trim();

        return value.Length >= 2 && value.StartsWith('"') && value.EndsWith('"')
                   ? value[1..^1].Replace("\"\"", "\"")
                   : value;
    }
}
