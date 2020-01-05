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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Renders the "Last Post" part of the Forum Topics
    /// </summary>
    public partial class ForumLastPost : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The Go to last post Image ToolTip.
        /// </summary>
        private string alt;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ForumLastPost" /> class.
        /// </summary>
        public ForumLastPost()
        {
            this.PreRender += this.ForumLastPost_PreRender;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets DataRow.
        /// </summary>
        public DataRow DataRow { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event of the ForumLastPost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ForumLastPost_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.DataRow == null)
            {
                return;
            }

            var showLastLinks = true;

            if (this.DataRow["ReadAccess"].ToType<int>() == 0)
            {
                this.TopicInPlaceHolder.Visible = false;
                showLastLinks = false;
            }

            if (this.DataRow["LastPosted"] != DBNull.Value)
            {
                // Last Post Date
                this.LastPostDate.DateTime = this.DataRow["LastPosted"];

                // Topic Link
                this.topicLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "t={0}", this.DataRow["LastTopicID"]);

                this.topicLink.ToolTip = this.GetText("COMMON", "VIEW_TOPIC");

                var styles = this.Get<YafBoardSettings>().UseStyledTopicTitles
                                 ? this.Get<IStyleTransform>().DecodeStyleByString(
                                     this.DataRow["LastTopicStyles"].ToString())
                                 : string.Empty;

                if (styles.IsSet())
                {
                    this.topicLink.Attributes.Add("style", styles);
                }

                this.topicLink.Text = this.Get<IBadWordReplace>()
                    .Replace(this.HtmlEncode(this.DataRow["LastTopicName"].ToString())).Truncate(50);

                // Last Topic User
                this.ProfileUserLink.UserID = this.DataRow["LastUserID"].ToType<int>();
                this.ProfileUserLink.Style = this.Get<YafBoardSettings>().UseStyledNicks
                                                 ? this.Get<IStyleTransform>().DecodeStyleByString(
                                                     this.DataRow["Style"].ToString())
                                                 : string.Empty;
                this.ProfileUserLink.ReplaceName =
                    this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "LastUserDisplayName" : "LastUser"]
                        .ToString();

                this.LastTopicImgLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "m={0}#post{0}", this.DataRow["LastMessageID"]);

                this.ImageLastUnreadMessageLink.Visible = this.Get<YafBoardSettings>().ShowLastUnreadPost;

                this.ImageLastUnreadMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "t={0}&find=unread", this.DataRow["LastTopicID"]);

                var lastRead =
                    this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                        this.DataRow["ForumID"].ToType<int>(),
                        this.DataRow["LastTopicID"].ToType<int>(),
                        this.DataRow["LastForumAccess"].ToType<DateTime?>(),
                        this.DataRow["LastTopicAccess"].ToType<DateTime?>());

                if (this.DataRow["LastPosted"].ToType<DateTime>() > lastRead)
                {
                    this.NewMessage.Visible = true;
                    this.NewMessage.Text = $" <span class=\"badge badge-success\">{this.GetText("NEW_POSTS")}</span>";
                }
                else
                {
                    this.NewMessage.Visible = false;
                }

                this.LastPostedHolder.Visible = showLastLinks;
                this.NoPostsPlaceHolder.Visible = false;
            }
            else
            {
                // show "no posts"
                this.LastPostedHolder.Visible = false;
                this.NoPostsPlaceHolder.Visible = true;
            }
        }

        #endregion
    }
}