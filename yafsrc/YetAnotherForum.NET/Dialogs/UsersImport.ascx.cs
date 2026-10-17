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

namespace YAF.Dialogs;

using System.Data;
using System.IO;
using System.Text.RegularExpressions;

using YAF.Types.Models.Identity;
using YAF.Types.Models;

/// <summary>
/// The Admin Users Import Dialog.
/// </summary>
public partial class UsersImport : BaseUserControl
{
    /// <summary>
    /// Try to Import from selected File
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Import_OnClick(object sender, EventArgs e)
    {
        try
        {
            int importedCount;

            // import selected file (if it's the proper format)...
            switch (this.importFile.PostedFile.ContentType)
            {
                // browsers send xml files as text/xml or application/xml
                case "text/xml":
                case "application/xml":
                    {
                        importedCount = this.ImportingUsers(this.importFile.PostedFile.InputStream, true);
                    }

                    break;

                // and csv files with one of these (most browsers send text/csv)
                case "text/csv":
                case "application/vnd.csv":
                case "application/vnd.ms-excel":
                case "application/csv":
                case "text/comma-separated-values":
                    {
                        importedCount = this.ImportingUsers(this.importFile.PostedFile.InputStream, false);
                    }

                    break;

                default:
                    {
                        this.PageBoardContext.Notify(
                            this.GetText("ADMIN_USERS_IMPORT", "IMPORT_FAILED_FORMAT"),
                            MessageTypes.danger);
                        return;
                    }
            }

            this.PageBoardContext.LoadMessage.AddSession(
                importedCount > 0
                    ? string.Format(this.GetText("ADMIN_USERS_IMPORT", "IMPORT_SUCESS"), importedCount)
                    : this.GetText("ADMIN_USERS_IMPORT", "IMPORT_NOTHING"),
                importedCount > 0 ? MessageTypes.success : MessageTypes.info);
        }
        catch (Exception x)
        {
            this.PageBoardContext.LoadMessage.AddSession(
                string.Format(this.GetText("ADMIN_USERS_IMPORT", "IMPORT_FAILED"), x.Message),
                MessageTypes.danger);
        }

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
    }

    /// <summary>
    /// Import Users from the InputStream
    /// </summary>
    /// <param name="inputStream">
    /// The input stream.
    /// </param>
    /// <param name="isXml">
    /// Indicates if input Stream is Xml file
    /// </param>
    /// <returns>
    /// Returns How Many Users where imported.
    /// </returns>
    /// <exception cref="Exception">
    /// Import stream is not expected format.
    /// </exception>
    private int ImportingUsers(Stream inputStream, bool isXml)
    {
        var importedCount = 0;

        if (isXml)
        {
            var usersDataSet = new DataSet();
            usersDataSet.ReadXml(inputStream);

            if (usersDataSet.Tables["User"] != null && HasRequiredColumns(usersDataSet.Tables["User"]))
            {
                importedCount = this.ImportRows(usersDataSet.Tables["User"]);
            }
            else
            {
                throw new ArgumentNullException(nameof(inputStream),"Import stream is not expected format.");
            }
        }
        else
        {
            var usersTable = new DataTable();

            var streamReader = new StreamReader(inputStream);

            var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            var headers = streamReader.ReadLine()?.Split(',');

            headers?.ForEach(header => usersTable.Columns.Add(Unquote(header)));

            // a file without the name and e-mail column is not a users file
            if (!HasRequiredColumns(usersTable))
            {
                throw new ArgumentNullException(nameof(inputStream), "Import stream is not expected format.");
            }

            while (streamReader.Peek() >= 0)
            {
                var line = streamReader.ReadLine();

                // empty lines (e.g. the last line of the file) are skipped
                if (line.IsNotSet())
                {
                    continue;
                }

                // the values without their quotes, a row with more values than columns keeps the first ones
                var values = regex.Split(line).Select(Unquote).Take(usersTable.Columns.Count).Cast<object>()
                    .ToArray();

                var dr = usersTable.NewRow();
                dr.ItemArray = values;

                usersTable.Rows.Add(dr);
            }

            streamReader.Close();

            importedCount = this.ImportRows(usersTable);
        }

        return importedCount;
    }

    /// <summary>
    /// Cuts the text to the maximum length.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <returns>Returns the text.</returns>
    private static string Cut(string text, int maxLength)
    {
        return text != null && text.Length > maxLength ? text.Substring(0, maxLength) : text;
    }

    /// <summary>
    /// Gets the code of a country (the code or the name of a known country).
    /// </summary>
    /// <param name="country">The country code or name.</param>
    /// <returns>Returns the code or null.</returns>
    private static string GetCountryCode(string country)
    {
        if (country.IsNotSet())
        {
            return null;
        }

        var match = StaticDataHelper.Country().FirstOrDefault(
            c => c.Value.IsSet() && (c.Value.Equals(country, StringComparison.OrdinalIgnoreCase)
                                     || c.Name.Equals(country, StringComparison.OrdinalIgnoreCase)));

        return match?.Value.ToUpperInvariant();
    }

    /// <summary>
    /// Checks if the table has the name and e-mail column.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>Returns if the users can be imported.</returns>
    private static bool HasRequiredColumns(DataTable table)
    {
        return table.Columns.Contains("Name") && table.Columns.Contains("Email");
    }

    /// <summary>
    /// Removes the quotes of a csv value ("a ""b""" -> a "b").
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the value.</returns>
    private static string Unquote(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
        {
            value = value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
        }

        return value;
    }

    /// <summary>
    /// Gets the text of a column (null for a missing column or value).
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <returns>Returns the text.</returns>
    private static string GetValue(DataRow row, string column)
    {
        return row.Table.Columns.Contains(column) && row[column] != DBNull.Value
                   ? row[column].ToString().Trim()
                   : null;
    }

    /// <summary>
    /// Imports the rows with a name and e-mail of users that don't exist yet, a failing row does not stop the import.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>Returns how many users where imported.</returns>
    private int ImportRows(DataTable table)
    {
        var importedCount = 0;

        foreach (DataRow row in table.Rows)
        {
            var name = GetValue(row, "Name");

            if (name.IsNotSet() || GetValue(row, "Email").IsNotSet()
                                || this.Get<IAspNetUsersHelper>().GetUserByName(name) != null)
            {
                continue;
            }

            try
            {
                importedCount = this.ImportUser(row, importedCount);
            }
            catch (Exception x)
            {
                this.Get<ILoggerService>().Error(x, $"Import of the user {name} failed");
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
    private int ImportUser(DataRow row, int importCount)
    {
        // Also Check if the Email is unique and exists
        if (this.Get<IAspNetUsersHelper>().GetUserByEmail(GetValue(row, "Email")) != null)
        {
            return importCount;
        }

        var pass = PasswordGenerator.GeneratePassword(true, true, true, true, false, 16);

        // create empty profile just so they have one
        var userProfile = new ProfileInfo();

        // Add Profile Fields to User List Table.
        if (row.Table.Columns.Contains("RealName") && GetValue(row, "RealName").IsSet())
        {
            userProfile.RealName = GetValue(row, "RealName");
        }

        if (row.Table.Columns.Contains("Blog") && GetValue(row, "Blog").IsSet())
        {
            userProfile.Blog = GetValue(row, "Blog");
        }

        if (row.Table.Columns.Contains("Gender") && GetValue(row, "Gender").IsSet())
        {
            int.TryParse(GetValue(row, "Gender"), out var gender);

            userProfile.Gender = gender;
        }

        if (row.Table.Columns.Contains("Birthday") && GetValue(row, "Birthday").IsSet())
        {
            DateTime.TryParse(GetValue(row, "Birthday"), out var userBirthdate);

            if (userBirthdate > DateTimeHelper.SqlDbMinTime())
            {
                userProfile.Birthday = userBirthdate;
            }
        }

        if (row.Table.Columns.Contains("Location") && GetValue(row, "Location").IsSet())
        {
            userProfile.Location = GetValue(row, "Location");
        }

        if (row.Table.Columns.Contains("Country") && GetValue(row, "Country").IsSet())
        {
            userProfile.Country = GetValue(row, "Country");
        }

        if (row.Table.Columns.Contains("Region") && GetValue(row, "Region").IsSet())
        {
            userProfile.Region = GetValue(row, "Region");
        }

        if (row.Table.Columns.Contains("City") && GetValue(row, "City").IsSet())
        {
            userProfile.City = GetValue(row, "City");
        }

        if (row.Table.Columns.Contains("Interests") && GetValue(row, "Interests").IsSet())
        {
            userProfile.Interests = GetValue(row, "Interests");
        }

        if (row.Table.Columns.Contains("Homepage") && GetValue(row, "Homepage").IsSet())
        {
            userProfile.Homepage = GetValue(row, "Homepage");
        }

        if (row.Table.Columns.Contains("XMPP") && GetValue(row, "XMPP").IsSet())
        {
            userProfile.XMPP = GetValue(row, "XMPP");
        }

        if (row.Table.Columns.Contains("Occupation") && GetValue(row, "Occupation").IsSet())
        {
            userProfile.Occupation = GetValue(row, "Occupation");
        }

        if (row.Table.Columns.Contains("Facebook") && GetValue(row, "Facebook").IsSet())
        {
            userProfile.Facebook = GetValue(row, "Facebook");
        }

        // the values have to fit into their columns (the import failed with an error on SQL Server, MySQL and
        // PostgreSQL), the country column contains the code of the country (a name is converted, unknown are dropped)
        userProfile.RealName = Cut(userProfile.RealName, 255);
        userProfile.Blog = Cut(userProfile.Blog, 255);
        userProfile.Location = Cut(userProfile.Location, 255);
        userProfile.Region = Cut(userProfile.Region, 255);
        userProfile.City = Cut(userProfile.City, 255);
        userProfile.Interests = Cut(userProfile.Interests, 4000);
        userProfile.Homepage = Cut(userProfile.Homepage, 255);
        userProfile.XMPP = Cut(userProfile.XMPP, 255);
        userProfile.Occupation = Cut(userProfile.Occupation, 400);
        userProfile.Facebook = Cut(userProfile.Facebook, 400);
        userProfile.Country = GetCountryCode(userProfile.Country);

        var user = new AspNetUsers
                       {
                           Id = Guid.NewGuid().ToString(),
                           ApplicationId = this.PageBoardContext.BoardSettings.ApplicationId,
                           UserName = GetValue(row, "Name"),
                           LoweredUserName = GetValue(row, "Name"),
                           Email = GetValue(row, "Email"),
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

        var result = this.Get<IAspNetUsersHelper>().Create(user, pass);

        // a user refused by the login system (e.g. a name with characters that are not allowed, an invalid e-mail)
        // was created as forum user without login and got the registration mail with a password that did not work
        if (!result.Succeeded)
        {
            this.Get<ILoggerService>().Log(
                $"Import of the user {user.UserName} refused: {result.Errors.ToDelimitedString(", ")}",
                EventLogTypes.Warning);

            return importCount;
        }

        // setup initial roles (if any) for this user
        this.Get<IAspNetRolesHelper>().SetupUserRoles(this.PageBoardContext.PageBoardID, user);

        // create the user in the YAF DB as well as sync roles...
        var userId = this.Get<IAspNetRolesHelper>().CreateForumUser(user, this.PageBoardContext.PageBoardID);

        if (userId == null)
        {
            // something is seriously wrong here -- redirect to failure...
            return importCount;
        }

        // send user register notification to the new users (a mail error does not stop the import)
        try
        {
            this.Get<ISendNotification>().SendRegistrationNotificationToUser(
                user, pass, "NOTIFICATION_ON_REGISTER");
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Error(x, $"Registration mail to {user.Email} failed");
        }

        // the export contains the id of the time zone (e.g. "W. Europe Standard Time"), it was parsed as number
        var timeZone = GetValue(row, "TimeZone");

        try
        {
            timeZone = timeZone.IsSet() ? TimeZoneInfo.FindSystemTimeZoneById(timeZone).Id : TimeZoneInfo.Local.Id;
        }
        catch (Exception)
        {
            timeZone = TimeZoneInfo.Local.Id;
        }

        var autoWatchTopicsEnabled = this.PageBoardContext.BoardSettings.DefaultNotificationSetting
                                     == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

        this.GetRepository<User>().Save(
            userId.Value,
            timeZone,
            row.Table.Columns.Contains("LanguageFile") ? row["LanguageFile"].ToString() : null,
            row.Table.Columns.Contains("Culture") ? row["Culture"].ToString() : null,
            row.Table.Columns.Contains("ThemeFile") ? row["ThemeFile"].ToString() : null,
            false,
            true,
            5);

        // save the settings...
        this.GetRepository<User>().SaveNotification(
            userId.Value,
            true,
            autoWatchTopicsEnabled,
            this.PageBoardContext.BoardSettings.DefaultNotificationSetting.ToInt(),
            this.PageBoardContext.BoardSettings.DefaultSendDigestEmail);

        importCount++;

        return importCount;
    }
}