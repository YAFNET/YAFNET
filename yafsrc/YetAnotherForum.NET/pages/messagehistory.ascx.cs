/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Post Message History Page.
    /// </summary>
    public partial class messagehistory : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   To save single report value.
        /// </summary>
        protected bool singleReport;

        /// <summary>
        ///   To save forumid value.
        /// </summary>
        private int forumID;

        /// <summary>
        ///   To save messageid value.
        /// </summary>
        private int messageID;

        /// <summary>
        ///   To save originalRow value.
        /// </summary>
        private DataTable originalRow;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "messagehistory" /> class. 
        /// </summary>
        public messagehistory()
            : base("MESSAGEHISTORY")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.IsGuest)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m").IsSet())
            {
                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), out this.messageID))
                {
                    this.Response.Redirect(
                        YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
                }

                this.ReturnBtn.Visible = true;
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f").IsSet())
            {
                // We check here if the user have access to the option
                if (this.PageContext.IsGuest)
                {
                    this.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.info, "i=4"));
                }

                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out this.forumID))
                {
                    this.Response.Redirect(
                        YafBuildLink.GetLink(ForumPages.error, "Incorrect forum value: {0}", this.forumID));
                }

                this.ReturnModBtn.Visible = true;
            }

            this.originalRow = LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID);

            if (this.originalRow.Rows.Count <= 0)
            {
                this.Response.Redirect(
                    YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            this.BindData();
        }

        /// <summary>
        /// Redirect to the changed post
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReturnBtn_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", this.messageID));
        }

        /// <summary>
        /// Redirect to the changed post
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReturnModBtn_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Response.Redirect(
                YafBuildLink.GetLinkNotEscaped(ForumPages.moderate_reportedposts, "f={0}", this.forumID));
        }

        /// <summary>
        /// Binds data to data source
        /// </summary>
        private void BindData()
        {
            // Fill revisions list repeater.
            DataTable dt = LegacyDb.messagehistory_list(
                this.messageID,
                this.PageContext.BoardSettings.MessageHistoryDaysToLog);
            this.RevisionsList.DataSource = dt.AsEnumerable();

            this.singleReport = dt.Rows.Count <= 1;

            // Fill current message repeater
            this.CurrentMessageRpt.DataSource =
                LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID).AsEnumerable();
            this.CurrentMessageRpt.Visible = true;

            this.DataBind();
        }

        #endregion
    }
}