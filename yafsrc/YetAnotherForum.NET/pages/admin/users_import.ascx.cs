/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Users import Page.
    /// </summary>
    public partial class users_import : AdminPage
    {
        #region Methods

        /// <summary>
        /// Cancel import and Return to the Admin Users Page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Cancel_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_users);
        }

        /// <summary>
        /// Import the Users from the provided File
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
                            importedCount = this.UsersImport(this.importFile.PostedFile.InputStream, true);
                        }

                        break;
                    case "text/csv":
                        {
                            importedCount = this.UsersImport(this.importFile.PostedFile.InputStream, false);
                        }

                        break;
                    case "text/comma-separated-values":
                        {
                            importedCount = this.UsersImport(this.importFile.PostedFile.InputStream, false);
                        }

                        break;
                    case "application/csv":
                        {
                            importedCount = this.UsersImport(this.importFile.PostedFile.InputStream, false);
                        }

                        break;
                    case "application/vnd.csv":
                        {
                            importedCount = this.UsersImport(this.importFile.PostedFile.InputStream, false);
                        }

                        break;
                    case "application/vnd.ms-excel":
                        {
                            importedCount = this.UsersImport(this.importFile.PostedFile.InputStream, false);
                        }

                        break;

                    default:
                        {
                            this.PageContext.AddLoadMessage(
                                this.GetText("ADMIN_USERS_IMPORT", "IMPORT_FAILED_FORMAT"), MessageTypes.Error);
                            return;
                        }
                }

                this.PageContext.LoadMessage.AddSession(
                    importedCount > 0
                        ? this.GetText("ADMIN_USERS_IMPORT", "IMPORT_SUCESS").FormatWith(importedCount)
                        : this.GetText("ADMIN_USERS_IMPORT", "IMPORT_NOTHING"),
                    importedCount > 0 ? MessageTypes.Success : MessageTypes.Information);

                YafBuildLink.Redirect(ForumPages.admin_users);
            }
            catch (Exception x)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_USERS_IMPORT", "IMPORT_FAILED").FormatWith(x.Message), MessageTypes.Error);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);

            if (Config.IsAnyPortal)
            {
                YafBuildLink.AccessDenied();
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_users));
            this.PageLinks.AddLink(this.GetText("ADMIN_USERS_IMPORT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_USERS", "TITLE"),
                this.GetText("ADMIN_USERS_IMPORT", "TITLE"));

            this.Import.Text = this.GetText("ADMIN_USERS_IMPORT", "IMPORT");
            this.cancel.Text = this.GetText("CANCEL");
        }

        /// <summary>
        /// Import Users from the InputStream
        /// </summary>
        /// <param name="imputStream">
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
        private int UsersImport(Stream imputStream, bool isXml)
        {
            int importedCount = 0;

            if (isXml)
            {
                var usersDataSet = new DataSet();
                usersDataSet.ReadXml(imputStream);

                if (usersDataSet.Tables["YafUser"] != null)
                {
                    importedCount =
                        usersDataSet.Tables["YafUser"].Rows.Cast<DataRow>().Where(
                            row => this.Get<MembershipProvider>().GetUser((string)row["Name"], false) == null)
                                                      .Aggregate(
                                                          importedCount, (current, row) => this.ImportUser(row, current));
                }
                else
                {
                    throw new Exception("Import stream is not expected format.");
                }
            }
            else
            {
                var usersTable = new DataTable();

                var streamReader = new StreamReader(imputStream);

                string[] headers = streamReader.ReadLine().Split(',');

                foreach (string header in headers)
                {
                    usersTable.Columns.Add(header);
                }

                while (streamReader.Peek() >= 0)
                {
                    DataRow dr = usersTable.NewRow();
                    dr.ItemArray = streamReader.ReadLine().Split(',');

                    usersTable.Rows.Add(dr);
                }

                streamReader.Close();

                importedCount =
                    usersTable.Rows.Cast<DataRow>().Where(
                        row => this.Get<MembershipProvider>().GetUser((string)row["Name"], false) == null).Aggregate(
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
            if (this.Get<MembershipProvider>().RequiresUniqueEmail)
            {
                if (this.Get<MembershipProvider>().GetUserNameByEmail((string)row["Email"]) != null)
                {
                    return importCount;
                }
            }

            MembershipCreateStatus status;

            var pass = Membership.GeneratePassword(32, 16);
            var securityAnswer = Membership.GeneratePassword(64, 30);
            var securityQuestion = "Answer is a generated Pass";

            if (row.Table.Columns.Contains("Password") && !string.IsNullOrEmpty((string)row["Password"])
                && row.Table.Columns.Contains("SecurityQuestion")
                && !string.IsNullOrEmpty((string)row["SecurityQuestion"])
                && row.Table.Columns.Contains("SecurityAnswer") && !string.IsNullOrEmpty((string)row["SecurityAnswer"]))
            {
                pass = (string)row["Password"];
                securityAnswer = (string)row["SecurityAnswer"];
                securityQuestion = (string)row["SecurityQuestion"];
            }

            MembershipUser user = YafContext.Current.Get<MembershipProvider>().CreateUser(
                (string)row["Name"],
                pass,
                (string)row["Email"],
                securityQuestion,
                securityAnswer,
                true,
                null,
                out status);

            // setup inital roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, (string)row["Name"]);

            // create the user in the YAF DB as well as sync roles...
            int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

            // create empty profile just so they have one
            YafUserProfile userProfile = YafUserProfile.GetProfile((string)row["Name"]);

            // Add Profile Fields to User List Table.
            if (row.Table.Columns.Contains("RealName") && !string.IsNullOrEmpty((string)row["RealName"]))
            {
                userProfile.RealName = (string)row["RealName"];
            }

            if (row.Table.Columns.Contains("Blog") && !string.IsNullOrEmpty((string)row["Blog"]))
            {
                userProfile.Blog = (string)row["Blog"];
            }

            if (row.Table.Columns.Contains("Gender") && !string.IsNullOrEmpty((string)row["Gender"]))
            {
                int gender;

                int.TryParse((string)row["Gender"], out gender);

                userProfile.Gender = gender;
            }

            if (row.Table.Columns.Contains("Birthday") && !string.IsNullOrEmpty((string)row["Birthday"]))
            {
                DateTime userBirthdate;

                DateTime.TryParse((string)row["Birthday"], out userBirthdate);

                if (userBirthdate > DateTimeHelper.SqlDbMinTime())
                {
                    userProfile.Birthday = userBirthdate;
                }
            }

            if (row.Table.Columns.Contains("MSN") && !string.IsNullOrEmpty((string)row["MSN"]))
            {
                userProfile.MSN = (string)row["MSN"];
            }

            if (row.Table.Columns.Contains("BlogServiceUsername")
                && !string.IsNullOrEmpty((string)row["BlogServiceUsername"]))
            {
                userProfile.BlogServiceUsername = (string)row["BlogServiceUsername"];
            }

            if (row.Table.Columns.Contains("BlogServicePassword")
                && !string.IsNullOrEmpty((string)row["BlogServicePassword"]))
            {
                userProfile.BlogServicePassword = (string)row["BlogServicePassword"];
            }

            if (row.Table.Columns.Contains("AIM") && !string.IsNullOrEmpty((string)row["AIM"]))
            {
                userProfile.AIM = (string)row["AIM"];
            }

            if (row.Table.Columns.Contains("Google") && !string.IsNullOrEmpty((string)row["Google"]))
            {
                userProfile.Google = (string)row["Google"];
            }

            if (row.Table.Columns.Contains("GoogleId") && !string.IsNullOrEmpty((string)row["GoogleId"]))
            {
                userProfile.GoogleId = (string)row["GoogleId"];
            }

            if (row.Table.Columns.Contains("Location") && !string.IsNullOrEmpty((string)row["Location"]))
            {
                userProfile.Location = (string)row["Location"];
            }

            if (row.Table.Columns.Contains("Country") && !string.IsNullOrEmpty((string)row["Country"]))
            {
                userProfile.Country = (string)row["Country"];
            }

            if (row.Table.Columns.Contains("Region") && !string.IsNullOrEmpty((string)row["Region"]))
            {
                userProfile.Region = (string)row["Region"];
            }

            if (row.Table.Columns.Contains("City") && !string.IsNullOrEmpty((string)row["City"]))
            {
                userProfile.City = (string)row["City"];
            }

            if (row.Table.Columns.Contains("Interests") && !string.IsNullOrEmpty((string)row["Interests"]))
            {
                userProfile.Interests = (string)row["Interests"];
            }

            if (row.Table.Columns.Contains("Homepage") && !string.IsNullOrEmpty((string)row["Homepage"]))
            {
                userProfile.Homepage = (string)row["Homepage"];
            }

            if (row.Table.Columns.Contains("Skype") && !string.IsNullOrEmpty((string)row["Skype"]))
            {
                userProfile.Skype = (string)row["Skype"];
            }

            if (row.Table.Columns.Contains("ICQe") && !string.IsNullOrEmpty((string)row["ICQ"]))
            {
                userProfile.ICQ = (string)row["ICQ"];
            }

            if (row.Table.Columns.Contains("XMPP") && !string.IsNullOrEmpty((string)row["XMPP"]))
            {
                userProfile.XMPP = (string)row["XMPP"];
            }

            if (row.Table.Columns.Contains("YIM") && !string.IsNullOrEmpty((string)row["YIM"]))
            {
                userProfile.YIM = (string)row["YIM"];
            }

            if (row.Table.Columns.Contains("Occupation") && !string.IsNullOrEmpty((string)row["Occupation"]))
            {
                userProfile.Occupation = (string)row["Occupation"];
            }

            if (row.Table.Columns.Contains("Twitter") && !string.IsNullOrEmpty((string)row["Twitter"]))
            {
                userProfile.Twitter = (string)row["Twitter"];
            }

            if (row.Table.Columns.Contains("TwitterId") && !string.IsNullOrEmpty((string)row["TwitterId"]))
            {
                userProfile.TwitterId = (string)row["TwitterId"];
            }

            if (row.Table.Columns.Contains("Facebook") && !string.IsNullOrEmpty((string)row["Facebook"]))
            {
                userProfile.Facebook = (string)row["Facebook"];
            }

            if (row.Table.Columns.Contains("FacebookId") && !string.IsNullOrEmpty((string)row["FacebookId"]))
            {
                userProfile.FacebookId = (string)row["FacebookId"];
            }

            userProfile.Save();

            if (userID == null)
            {
                // something is seriously wrong here -- redirect to failure...
                return importCount;
            }

            // send user register notification to the new users
            this.Get<ISendNotification>().SendRegistrationNotificationToUser(
                user, pass, securityAnswer, "NOTIFICATION_ON_REGISTER");

            // save the time zone...
            int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

            bool isDST = false;

            if (row.Table.Columns.Contains("IsDST") && !string.IsNullOrEmpty((string)row["IsDST"]))
            {
                bool.TryParse((string)row["IsDST"], out isDST);
            }

            int timeZone = 0;

            if (row.Table.Columns.Contains("Timezone") && !string.IsNullOrEmpty((string)row["Timezone"]))
            {
                int.TryParse((string)row["Timezone"], out timeZone);
            }

            LegacyDb.user_save(
                userId,
                YafContext.Current.PageBoardID,
                row["Name"],
                row.Table.Columns.Contains("DisplayName") ? row["DisplayName"] : null,
                row["Email"],
                timeZone,
                row.Table.Columns.Contains("LanguageFile") ? row["LanguageFile"] : null,
                row.Table.Columns.Contains("Culture") ? row["Culture"] : null,
                row.Table.Columns.Contains("ThemeFile") ? row["ThemeFile"] : null,
                row.Table.Columns.Contains("TextEditor") ? row["TextEditor"] : null,
                null,
                null,
                null,
                null,
                isDST,
                null,
                null);

            bool autoWatchTopicsEnabled = this.Get<YafBoardSettings>().DefaultNotificationSetting
                                          == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            // save the settings...
            LegacyDb.user_savenotification(
                userId,
                true,
                autoWatchTopicsEnabled,
                this.Get<YafBoardSettings>().DefaultNotificationSetting,
                this.Get<YafBoardSettings>().DefaultSendDigestEmail);

            importCount++;

            return importCount;
        }

        #endregion
    }
}