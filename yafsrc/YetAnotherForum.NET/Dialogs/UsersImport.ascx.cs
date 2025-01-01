﻿/* Yet Another Forum.NET
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
                case "text/xml":
                    {
                        importedCount = this.ImportingUsers(this.importFile.PostedFile.InputStream, true);
                    }

                    break;

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

            if (usersDataSet.Tables["User"] != null)
            {
                importedCount =
                    usersDataSet.Tables["User"].Rows.Cast<DataRow>().Where(
                            row => this.Get<IAspNetUsersHelper>().GetUserByName((string)row["Name"]) == null)
                        .Aggregate(
                            importedCount, (current, row) => this.ImportUser(row, current));
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

            var headers = streamReader.ReadLine()?.Split(',');

            headers.ForEach(header => usersTable.Columns.Add(header));

            while (streamReader.Peek() >= 0)
            {
                var dr = usersTable.NewRow();
                var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                dr.ItemArray = (string[])regex.Split(streamReader.ReadLine());

                usersTable.Rows.Add(dr);
            }

            streamReader.Close();

            importedCount =
                usersTable.Rows.Cast<DataRow>().Where(
                    row => this.Get<IAspNetUsersHelper>().GetUserByName((string)row["Name"]) == null).Aggregate(
                    importedCount, (current, row) => this.ImportUser(row, current));
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
        if (this.Get<IAspNetUsersHelper>().GetUserByEmail((string)row["Email"]) != null)
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
            int.TryParse((string)row["Gender"], out var gender);

            userProfile.Gender = gender;
        }

        if (row.Table.Columns.Contains("Birthday") && ((string)row["Birthday"]).IsSet())
        {
            DateTime.TryParse((string)row["Birthday"], out var userBirthdate);

            if (userBirthdate > DateTimeHelper.SqlDbMinTime())
            {
                userProfile.Birthday = userBirthdate;
            }
        }

        if (row.Table.Columns.Contains("GoogleId") && ((string)row["GoogleId"]).IsSet())
        {
            userProfile.GoogleId = (string)row["GoogleId"];
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

        if (row.Table.Columns.Contains("Skype") && ((string)row["Skype"]).IsSet())
        {
            userProfile.Skype = (string)row["Skype"];
        }

        if (row.Table.Columns.Contains("XMPP") && ((string)row["XMPP"]).IsSet())
        {
            userProfile.XMPP = (string)row["XMPP"];
        }

        if (row.Table.Columns.Contains("Occupation") && ((string)row["Occupation"]).IsSet())
        {
            userProfile.Occupation = (string)row["Occupation"];
        }

        if (row.Table.Columns.Contains("Twitter") && ((string)row["Twitter"]).IsSet())
        {
            userProfile.Twitter = (string)row["Twitter"];
        }

        if (row.Table.Columns.Contains("TwitterId") && ((string)row["TwitterId"]).IsSet())
        {
            userProfile.TwitterId = (string)row["TwitterId"];
        }

        if (row.Table.Columns.Contains("Facebook") && ((string)row["Facebook"]).IsSet())
        {
            userProfile.Facebook = (string)row["Facebook"];
        }

        if (row.Table.Columns.Contains("FacebookId") && ((string)row["FacebookId"]).IsSet())
        {
            userProfile.FacebookId = (string)row["FacebookId"];
        }

        var user = new AspNetUsers
                       {
                           Id = Guid.NewGuid().ToString(),
                           ApplicationId = this.PageBoardContext.BoardSettings.ApplicationId,
                           UserName = (string)row["Name"],
                           LoweredUserName = (string)row["Name"],
                           Email = (string)row["Email"],
                           IsApproved = true,

                           Profile_Birthday = userProfile.Birthday,
                           Profile_Blog = userProfile.Blog,
                           Profile_Gender = userProfile.Gender,
                           Profile_GoogleId = userProfile.GoogleId,
                           Profile_Homepage = userProfile.Homepage,
                           Profile_Facebook = userProfile.Facebook,
                           Profile_FacebookId = userProfile.FacebookId,
                           Profile_Twitter = userProfile.Twitter,
                           Profile_TwitterId = userProfile.TwitterId,
                           Profile_Interests = userProfile.Interests,
                           Profile_Location = userProfile.Location,
                           Profile_Country = userProfile.Country,
                           Profile_Region = userProfile.Region,
                           Profile_City = userProfile.City,
                           Profile_Occupation = userProfile.Occupation,
                           Profile_RealName = userProfile.RealName,
                           Profile_Skype = userProfile.Skype,
                           Profile_XMPP = userProfile.XMPP
                       };

        this.Get<IAspNetUsersHelper>().Create(user, pass);

        // setup initial roles (if any) for this user
        this.Get<IAspNetRolesHelper>().SetupUserRoles(this.PageBoardContext.PageBoardID, user);

        // create the user in the YAF DB as well as sync roles...
        var userId = this.Get<IAspNetRolesHelper>().CreateForumUser(user, this.PageBoardContext.PageBoardID);

        if (userId == null)
        {
            // something is seriously wrong here -- redirect to failure...
            return importCount;
        }

        // send user register notification to the new users
        this.Get<ISendNotification>().SendRegistrationNotificationToUser(
            user, pass, "NOTIFICATION_ON_REGISTER");

        var timeZone = 0;

        if (row.Table.Columns.Contains("Timezone") && ((string)row["Timezone"]).IsSet())
        {
            int.TryParse((string)row["Timezone"], out timeZone);
        }

        var autoWatchTopicsEnabled = this.PageBoardContext.BoardSettings.DefaultNotificationSetting
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
            true,
            autoWatchTopicsEnabled,
            this.PageBoardContext.BoardSettings.DefaultNotificationSetting.ToInt(),
            this.PageBoardContext.BoardSettings.DefaultSendDigestEmail);

        importCount++;

        return importCount;
    }
}