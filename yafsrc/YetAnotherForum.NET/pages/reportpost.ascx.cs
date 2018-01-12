/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The form for reported post complaint text.
    /// </summary>
    public partial class ReportPost : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   To save messageid value.
        /// </summary>
        private int messageID;

        // message body editor

        /// <summary>
        ///   The _editor.
        /// </summary>
        private ForumEditor reportEditor;

        #endregion

        //// Class constructor

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the ReportPost class.
        /// </summary>
        public ReportPost()
            : base("REPORTPOST")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The btn cancel query_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void BtnCancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Redirect to reported post
            this.RedirectToPost();
        }

        /// <summary>
        /// The btn run query_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void BtnReport_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.reportEditor.Text.Length > this.Get<YafBoardSettings>().MaxReportPostChars)
            {
                this.IncorrectReportLabel.Text = this.GetTextFormatted(
                    "REPORTTEXT_TOOLONG",
                    this.Get<YafBoardSettings>().MaxReportPostChars);
                this.IncorrectReportLabel.DataBind();
                return;
            }

            // Save the reported message
            LegacyDb.message_report(
                this.messageID,
                this.PageContext.PageUserID,
                DateTime.UtcNow,
                this.reportEditor.Text);

            // Send Notification to Mods about the Reported Post.
            if (this.Get<YafBoardSettings>().EmailModeratorsOnReportedPost)
            {
                // not approved, notifiy moderators
                this.Get<ISendNotification>()
                    .ToModeratorsThatMessageWasReported(
                        this.PageContext.PageForumID,
                        this.messageID,
                        this.PageContext.PageUserID,
                        this.reportEditor.Text);
            }

            // Redirect to reported post
            this.RedirectToPost();
        }

        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Init([NotNull] object sender, [NotNull] EventArgs e)
        {
            // get the forum editor based on the settings
            this.reportEditor = ForumEditorHelper.GetCurrentForumEditor();

            // add editor to the page
            this.EditorLine.Controls.Add(this.reportEditor);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // set attributes of editor
            this.reportEditor.BaseDir = "{0}Scripts".FormatWith(YafForumInfo.ForumClientFileRoot);
            this.reportEditor.StyleSheet = this.Get<ITheme>().BuildThemePath("theme.css");

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m").IsSet())
            {
                // We check here if the user have access to the option
                if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ReportPostPermissions))
                {
                    YafBuildLink.Redirect(ForumPages.info, "i=1");
                }

                if (!Int32.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), out this.messageID))
                {
                    YafBuildLink.Redirect(ForumPages.error, "Incorrect message value: {0}", this.messageID);
                }
            }

            if (this.IsPostBack)
            {
                return;
            }

            // Get reported message text for better quoting                    
            var messageRow = LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID);

            // Checking if the user has a right to view the message and getting data  
            if (messageRow.HasRows())
            {
                // populate the repeater with the message datarow...
                this.MessageList.DataSource = LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID);
                this.MessageList.DataBind();
            }
            else
            {
                YafBuildLink.Redirect(ForumPages.info, "i=1");
            }

            // Get Forum Link
            this.PageLinks.AddRoot();
            this.btnReport.Attributes.Add(
                "onclick",
                "return confirm('{0}');".FormatWith(this.GetText("CONFIRM_REPORTPOST")));
        }

        /// <summary>
        /// Redirects to reported post after Save or Cancel
        /// </summary>
        protected void RedirectToPost()
        {
            // Redirect to reported post
            YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", this.messageID);
        }

        /// <summary>
        /// Binds data to data source
        /// </summary>
        private void BindData()
        {
            this.DataBind();
        }

        #endregion
    }
}