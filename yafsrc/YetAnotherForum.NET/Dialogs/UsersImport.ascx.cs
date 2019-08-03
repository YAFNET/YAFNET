/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web.Security;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Admin Users Import Dialog.
    /// </summary>
    public partial class UsersImport : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Try to Import from selected File
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Import_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            try
            {
                int importedCount;

                // import selected file (if it's the proper format)...
                switch (this.importFile.PostedFile.ContentType)
                {
                    case "text/xml":
                        {
                            importedCount = this.ImportingUsers(inputStream: this.importFile.PostedFile.InputStream, isXml: true);
                        }

                        break;
                    case "text/csv":
                        {
                            importedCount = this.ImportingUsers(inputStream: this.importFile.PostedFile.InputStream, isXml: false);
                        }

                        break;
                    case "text/comma-separated-values":
                        {
                            importedCount = this.ImportingUsers(inputStream: this.importFile.PostedFile.InputStream, isXml: false);
                        }

                        break;
                    case "application/csv":
                        {
                            importedCount = this.ImportingUsers(inputStream: this.importFile.PostedFile.InputStream, isXml: false);
                        }

                        break;
                    case "application/vnd.csv":
                        {
                            importedCount = this.ImportingUsers(inputStream: this.importFile.PostedFile.InputStream, isXml: false);
                        }

                        break;
                    case "application/vnd.ms-excel":
                        {
                            importedCount = this.ImportingUsers(inputStream: this.importFile.PostedFile.InputStream, isXml: false);
                        }

                        break;

                    default:
                        {
                            this.PageContext.AddLoadMessage(
                                message: this.GetText(page: "ADMIN_USERS_IMPORT", tag: "IMPORT_FAILED_FORMAT"), messageType: MessageTypes.danger);
                            return;
                        }
                }

                this.PageContext.LoadMessage.AddSession(
                    message: importedCount > 0
                        ? string.Format(format: this.GetText(page: "ADMIN_USERS_IMPORT", tag: "IMPORT_SUCESS"), arg0: importedCount)
                        : this.GetText(page: "ADMIN_USERS_IMPORT", tag: "IMPORT_NOTHING"),
                    messageType: importedCount > 0 ? MessageTypes.success : MessageTypes.info);
            }
            catch (Exception x)
            {
                this.PageContext.AddLoadMessage(
                    message: string.Format(format: this.GetText(page: "ADMIN_USERS_IMPORT", tag: "IMPORT_FAILED"), arg0: x.Message), messageType: MessageTypes.danger);
            }
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
                usersDataSet.ReadXml(stream: inputStream);

                if (usersDataSet.Tables[name: "YafUser"] != null)
                {
                    importedCount =
                        usersDataSet.Tables[name: "YafUser"].Rows.Cast<DataRow>().Where(
                            predicate: row => this.Get<MembershipProvider>().GetUser(username: (string)row[columnName: "Name"], userIsOnline: false) == null)
                                                      .Aggregate(
                                                          seed: importedCount, func: (current, row) => this.ImportUser(row: row, importCount: current));
                }
                else
                {
                    throw new Exception(message: "Import stream is not expected format.");
                }
            }
            else
            {
                var usersTable = new DataTable();

                var streamReader = new StreamReader(stream: inputStream);

                var headers = streamReader.ReadLine().Split(',');

                headers.ForEach(action: header => usersTable.Columns.Add(columnName: header));

                while (streamReader.Peek() >= 0)
                {
                    var dr = usersTable.NewRow();
                    dr.ItemArray = streamReader.ReadLine().Split(',');

                    usersTable.Rows.Add(row: dr);
                }

                streamReader.Close();

                importedCount =
                    usersTable.Rows.Cast<DataRow>().Where(
                        predicate: row => this.Get<MembershipProvider>().GetUser(username: (string)row[columnName: "Name"], userIsOnline: false) == null).Aggregate(
                            seed: importedCount, func: (current, row) => this.ImportUser(row: row, importCount: current));
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
            if (this.Get<MembershipProvider>().RequiresUniqueEmail)
            {
                if (this.Get<MembershipProvider>().GetUserNameByEmail(email: (string)row[columnName: "Email"]) != null)
                {
                    return importCount;
                }
            }

            MembershipCreateStatus status;

            var pass = Membership.GeneratePassword(length: 32, numberOfNonAlphanumericCharacters: 16);
            var securityAnswer = Membership.GeneratePassword(length: 64, numberOfNonAlphanumericCharacters: 30);
            var securityQuestion = "Answer is a generated Pass";

            if (row.Table.Columns.Contains(name: "Password") && ((string)row[columnName: "Password"]).IsSet()
                && row.Table.Columns.Contains(name: "SecurityQuestion")
                && ((string)row[columnName: "SecurityQuestion"]).IsSet()
                && row.Table.Columns.Contains(name: "SecurityAnswer") && ((string)row[columnName: "SecurityAnswer"]).IsSet())
            {
                pass = (string)row[columnName: "Password"];

                securityAnswer = (string)row[columnName: "SecurityAnswer"];
                securityQuestion = (string)row[columnName: "SecurityQuestion"];
            }

            var user = YafContext.Current.Get<MembershipProvider>().CreateUser(
                username: (string)row[columnName: "Name"],
                password: pass,
                email: (string)row[columnName: "Email"],
                passwordQuestion: this.Get<MembershipProvider>().RequiresQuestionAndAnswer ? securityQuestion : null,
                passwordAnswer: this.Get<MembershipProvider>().RequiresQuestionAndAnswer ? securityAnswer : null,
                isApproved: true,
                providerUserKey: null,
                status: out status);

            // setup initial roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(pageBoardID: YafContext.Current.PageBoardID, userName: (string)row[columnName: "Name"]);

            // create the user in the YAF DB as well as sync roles...
            var userID = RoleMembershipHelper.CreateForumUser(user: user, pageBoardID: YafContext.Current.PageBoardID);

            // create empty profile just so they have one
            var userProfile = YafUserProfile.GetProfile(userName: (string)row[columnName: "Name"]);

            // Add Profile Fields to User List Table.
            if (row.Table.Columns.Contains(name: "RealName") && ((string)row[columnName: "RealName"]).IsSet())
            {
                userProfile.RealName = (string)row[columnName: "RealName"];
            }

            if (row.Table.Columns.Contains(name: "Blog") && ((string)row[columnName: "Blog"]).IsSet())
            {
                userProfile.Blog = (string)row[columnName: "Blog"];
            }

            if (row.Table.Columns.Contains(name: "Gender") && ((string)row[columnName: "Gender"]).IsSet())
            {
                int gender;

                int.TryParse(s: (string)row[columnName: "Gender"], result: out gender);

                userProfile.Gender = gender;
            }

            if (row.Table.Columns.Contains(name: "Birthday") && ((string)row[columnName: "Birthday"]).IsSet())
            {
                DateTime userBirthdate;

                DateTime.TryParse(s: (string)row[columnName: "Birthday"], result: out userBirthdate);

                if (userBirthdate > DateTimeHelper.SqlDbMinTime())
                {
                    userProfile.Birthday = userBirthdate;
                }
            }

            if (row.Table.Columns.Contains(name: "BlogServiceUsername")
                && ((string)row[columnName: "BlogServiceUsername"]).IsSet())
            {
                userProfile.BlogServiceUsername = (string)row[columnName: "BlogServiceUsername"];
            }

            if (row.Table.Columns.Contains(name: "BlogServicePassword")
                && ((string)row[columnName: "BlogServicePassword"]).IsSet())
            {
                userProfile.BlogServicePassword = (string)row[columnName: "BlogServicePassword"];
            }

            if (row.Table.Columns.Contains(name: "GoogleId") && ((string)row[columnName: "GoogleId"]).IsSet())
            {
                userProfile.GoogleId = (string)row[columnName: "GoogleId"];
            }

            if (row.Table.Columns.Contains(name: "Location") && ((string)row[columnName: "Location"]).IsSet())
            {
                userProfile.Location = (string)row[columnName: "Location"];
            }

            if (row.Table.Columns.Contains(name: "Country") && ((string)row[columnName: "Country"]).IsSet())
            {
                userProfile.Country = (string)row[columnName: "Country"];
            }

            if (row.Table.Columns.Contains(name: "Region") && ((string)row[columnName: "Region"]).IsSet())
            {
                userProfile.Region = (string)row[columnName: "Region"];
            }

            if (row.Table.Columns.Contains(name: "City") && ((string)row[columnName: "City"]).IsSet())
            {
                userProfile.City = (string)row[columnName: "City"];
            }

            if (row.Table.Columns.Contains(name: "Interests") && ((string)row[columnName: "Interests"]).IsSet())
            {
                userProfile.Interests = (string)row[columnName: "Interests"];
            }

            if (row.Table.Columns.Contains(name: "Homepage") && ((string)row[columnName: "Homepage"]).IsSet())
            {
                userProfile.Homepage = (string)row[columnName: "Homepage"];
            }

            if (row.Table.Columns.Contains(name: "Skype") && ((string)row[columnName: "Skype"]).IsSet())
            {
                userProfile.Skype = (string)row[columnName: "Skype"];
            }

            if (row.Table.Columns.Contains(name: "ICQe") && ((string)row[columnName: "ICQ"]).IsSet())
            {
                userProfile.ICQ = (string)row[columnName: "ICQ"];
            }

            if (row.Table.Columns.Contains(name: "XMPP") && ((string)row[columnName: "XMPP"]).IsSet())
            {
                userProfile.XMPP = (string)row[columnName: "XMPP"];
            }

            if (row.Table.Columns.Contains(name: "Occupation") && ((string)row[columnName: "Occupation"]).IsSet())
            {
                userProfile.Occupation = (string)row[columnName: "Occupation"];
            }

            if (row.Table.Columns.Contains(name: "Twitter") && ((string)row[columnName: "Twitter"]).IsSet())
            {
                userProfile.Twitter = (string)row[columnName: "Twitter"];
            }

            if (row.Table.Columns.Contains(name: "TwitterId") && ((string)row[columnName: "TwitterId"]).IsSet())
            {
                userProfile.TwitterId = (string)row[columnName: "TwitterId"];
            }

            if (row.Table.Columns.Contains(name: "Facebook") && ((string)row[columnName: "Facebook"]).IsSet())
            {
                userProfile.Facebook = (string)row[columnName: "Facebook"];
            }

            if (row.Table.Columns.Contains(name: "FacebookId") && ((string)row[columnName: "FacebookId"]).IsSet())
            {
                userProfile.FacebookId = (string)row[columnName: "FacebookId"];
            }

            userProfile.Save();

            if (userID == null)
            {
                // something is seriously wrong here -- redirect to failure...
                return importCount;
            }

            // send user register notification to the new users
            this.Get<ISendNotification>().SendRegistrationNotificationToUser(
                user: user, pass: pass, securityAnswer: securityAnswer, templateName: "NOTIFICATION_ON_REGISTER");

            // save the time zone...
            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(providerUserKey: user.ProviderUserKey);

            var isDst = false;

            if (row.Table.Columns.Contains(name: "IsDST") && ((string)row[columnName: "IsDST"]).IsSet())
            {
                bool.TryParse(value: (string)row[columnName: "IsDST"], result: out isDst);
            }

            var timeZone = 0;

            if (row.Table.Columns.Contains(name: "Timezone") && ((string)row[columnName: "Timezone"]).IsSet())
            {
                int.TryParse(s: (string)row[columnName: "Timezone"], result: out timeZone);
            }

            var autoWatchTopicsEnabled = this.Get<YafBoardSettings>().DefaultNotificationSetting
                                         == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            this.GetRepository<User>().Save(
                userID: userId,
                boardID: YafContext.Current.PageBoardID,
                userName: row[columnName: "Name"],
                displayName: row.Table.Columns.Contains(name: "DisplayName") ? row[columnName: "DisplayName"] : null,
                email: row[columnName: "Email"],
                timeZone: timeZone,
                languageFile: row.Table.Columns.Contains(name: "LanguageFile") ? row[columnName: "LanguageFile"] : null,
                culture: row.Table.Columns.Contains(name: "Culture") ? row[columnName: "Culture"] : null,
                themeFile: row.Table.Columns.Contains(name: "ThemeFile") ? row[columnName: "ThemeFile"] : null,
                textEditor: row.Table.Columns.Contains(name: "TextEditor") ? row[columnName: "TextEditor"] : null,
                approved: null,
                pmNotification: null,
                autoWatchTopics: this.Get<YafBoardSettings>().DefaultNotificationSetting,
                dSTUser: autoWatchTopicsEnabled,
                hideUser: isDst,
                notificationType: null,
                null);

            // save the settings...
            this.GetRepository<User>().SaveNotification(
                userID: userId,
                pmNotification: true,
                autoWatchTopics: autoWatchTopicsEnabled,
                notificationType: this.Get<YafBoardSettings>().DefaultNotificationSetting,
                dailyDigest: this.Get<YafBoardSettings>().DefaultSendDigestEmail);

            importCount++;

            return importCount;
        }

        #endregion
    }
}