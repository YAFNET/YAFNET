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
    // YAF.Pages
    #region Using

    using System;
    using System.Data;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Print topic Page.
    /// </summary>
    public partial class printtopic : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "printtopic" /> class.
        /// </summary>
        public printtopic()
            : base("PRINTTOPIC")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the print body.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// The get print body.
        /// </returns>
        protected string GetPrintBody([NotNull] object o)
        {
            var row = (DataRow)o;

            var message = row["Message"].ToString();

            message = this.Get<IFormatMessage>().FormatMessage(message, new MessageFlags(row["Flags"].ToType<int>()));

            // Remove HIDDEN Text
            message = this.Get<IFormatMessage>().RemoveHiddenBBCodeContent(message);

            message = this.Get<IFormatMessage>().RemoveCustomBBCodes(message);

            return message;
        }

        /// <summary>
        /// Gets the print header.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// The get print header.
        /// </returns>
        protected string GetPrintHeader([NotNull] object o)
        {
            var row = (DataRow)o;
            return "<strong>{2}: {0}</strong> - {1}".FormatWith(this.Get<YafBoardSettings>().EnableDisplayName ? row["DisplayName"] : row["UserName"], this.Get<IDateTime>().FormatDateTime((DateTime)row["Posted"]), this.GetText("postedby"));
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Request.QueryString.GetFirstOrDefault("t") == null || !this.PageContext.ForumReadAccess)
            {
                YafBuildLink.AccessDenied();
            }

            this.ShowToolBar = false;

            if (this.IsPostBack)
            {
                return;
            }

            if (this.PageContext.Settings.LockedForum == 0)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(
                    this.PageContext.PageCategoryName,
                    YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
            }

            this.PageLinks.AddForum(this.PageContext.PageForumID);
            this.PageLinks.AddLink(
                this.PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID));
            var showDeleted = false;
            var userId = 0;
            if (this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }

            if (!showDeleted && ((this.Get<YafBoardSettings>().ShowDeletedMessages &&
                                  !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
                                 || this.PageContext.IsAdmin ||
                                 this.PageContext.ForumModeratorAccess))
            {
                userId = this.PageContext.PageUserID;
            }

            var dt = LegacyDb.post_list(
                this.PageContext.PageTopicID,
                this.PageContext.PageUserID,
                userId,
                !PageContext.IsCrawler ? 1 : 0,
                showDeleted,
                false,
                false,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                0,
                500,
                2,
                0,
                0,
                false,
                -1);

            this.Posts.DataSource = dt.AsEnumerable();

            this.DataBind();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}