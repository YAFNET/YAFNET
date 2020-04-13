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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The form for reported post complaint text.
    /// </summary>
    public partial class ReportPost : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   To save message id value.
        /// </summary>
        private int messageID;

        #endregion

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
        /// The btn cancel_ click.
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
            if (this.Report.Text.Length > this.Get<BoardSettings>().MaxReportPostChars)
            {
                this.IncorrectReportLabel.Text = this.GetTextFormatted(
                    "REPORTTEXT_TOOLONG",
                    this.Get<BoardSettings>().MaxReportPostChars);
                this.IncorrectReportLabel.DataBind();
                return;
            }

            // Save the reported message
            this.GetRepository<Message>().Report(
                this.messageID,
                this.PageContext.PageUserID,
                DateTime.UtcNow,
                this.Report.Text);

            // Send Notification to Mods about the Reported Post.
            if (this.Get<BoardSettings>().EmailModeratorsOnReportedPost)
            {
                // not approved, notify moderators
                this.Get<ISendNotification>()
                    .ToModeratorsThatMessageWasReported(
                        this.PageContext.PageForumID,
                        this.messageID,
                        this.PageContext.PageUserID,
                        this.Report.Text);
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
            this.Report.MaxLength = this.Get<BoardSettings>().MaxReportPostChars;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m").IsSet())
            {
                // We check here if the user have access to the option
                if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().ReportPostPermissions))
                {
                    BuildLink.Redirect(ForumPages.Info, "i=1");
                }

                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), out this.messageID))
                {
                    BuildLink.Redirect(ForumPages.Error, "Incorrect message value: {0}", this.messageID);
                }
            }

            if (this.IsPostBack)
            {
                return;
            }

            // Get reported message text for better quoting                    
            var messageRow = this.GetRepository<Message>().SecAsDataTable(this.messageID, this.PageContext.PageUserID);

            // Checking if the user has a right to view the message and getting data  
            if (messageRow.HasRows())
            {
                // populate the repeater with the message data row...
                this.MessageList.DataSource = messageRow;
                this.MessageList.DataBind();
            }
            else
            {
                BuildLink.Redirect(ForumPages.Info, "i=1");
            }

            // Get Forum Link
            this.PageLinks.AddRoot();

            this.LocalizedLblMaxNumberOfPost.Param0 = this.Get<BoardSettings>().MaxReportPostChars.ToString();
        }

        /// <summary>
        /// Redirects to reported post after Save or Cancel
        /// </summary>
        protected void RedirectToPost()
        {
            // Redirect to reported post
            BuildLink.Redirect(ForumPages.Posts, "m={0}#post{0}", this.messageID);
        }

        #endregion
    }
}