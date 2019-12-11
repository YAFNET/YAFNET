/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * https://www.yetanotherforum.net/
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
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The profile Timeline
    /// </summary>
    public partial class ProfileTimeline : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the item count.
        /// </summary>
        protected int ItemCount { get; set; }

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
            if (!this.IsPostBack)
            {
                this.BindData();
            }
        }

        /// <summary>
        /// The get first item class.
        /// </summary>
        /// <param name="itemIndex">
        /// The item index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetFirstItemClass(int itemIndex)
        {
            return itemIndex > 0 ? "border-right" : string.Empty;
        }

        /// <summary>
        /// The get last item class.
        /// </summary>
        /// <param name="itemIndex">
        /// The item index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetLastItemClass(int itemIndex)
        {
            return itemIndex == this.ItemCount - 1 ? string.Empty : "border-right";
        }

        /// <summary>
        /// The activity stream_ on item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ActivityStream_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var activity = (Activity)e.Item.DataItem;

            var iconLabel = e.Item.FindControlAs<Label>("Icon");
            var title = e.Item.FindControlAs<Literal>("Title");
            var messageHolder = e.Item.FindControlAs<PlaceHolder>("Message");
            var displayDateTime = e.Item.FindControlAs<DisplayDateTime>("DisplayDateTime");

            var message = string.Empty;
            var icon = string.Empty;

            var topicLink = new ThemeButton
                                {
                                    NavigateUrl =
                                        YafBuildLink.GetLink(
                                            ForumPages.posts,
                                            "m={0}#post{0}",
                                            activity.MessageID.Value),
                                    Type = ButtonAction.OutlineSecondary,
                                    Text = this.GetRepository<Topic>().GetById(activity.TopicID.Value)
                                        .TopicName,
                                    Icon = "comment"
                                };

            if (activity.ActivityFlags.CreatedTopic)
            {
                topicLink.NavigateUrl = YafBuildLink.GetLink(ForumPages.posts, "t={0}", activity.TopicID.Value);

                title.Text = this.GetText("CP_PROFILE", "CREATED_TOPIC");
                icon = "comment";
                message = this.GetTextFormatted("CREATED_TOPIC_MSG", topicLink.RenderToString());
            }

            if (activity.ActivityFlags.CreatedReply)
            {
                title.Text = this.GetText("CP_PROFILE", "CREATED_REPLY");
                icon = "comment";
                message = this.GetTextFormatted("CREATED_REPLY_MSG", topicLink.RenderToString());
            }

            if (activity.ActivityFlags.GivenThanks)
            {
                var userLink = new UserLink { UserID = activity.FromUserID.Value };

                title.Text = this.GetText("CP_PROFILE", "GIVEN_THANKS");
                icon = "heart";
                message = this.GetTextFormatted("GIVEN_THANKS_MSG", userLink.RenderToString(), topicLink.RenderToString());
            }

            if (activity.ActivityFlags.ReceivedThanks)
            {
                var userLink = new UserLink { UserID = activity.FromUserID.Value };

                title.Text = this.GetText("CP_PROFILE", "RECEIVED_THANKS");
                icon = "heart";
                message = this.GetTextFormatted("RECEIVED_THANKS_MSG", userLink.RenderToString(), topicLink.RenderToString());
            }

            if (activity.ActivityFlags.WasMentioned)
            {
                var userLink = new UserLink { UserID = activity.FromUserID.Value };

                title.Text = this.GetText("CP_PROFILE", "WAS_MENTIONED");
                icon = "at";
                message = this.GetTextFormatted("WAS_MENTIONED_MSG", userLink.RenderToString(), topicLink.RenderToString());
            }

            if (activity.ActivityFlags.WasQuoted)
            {
                var userLink = new UserLink { UserID = activity.FromUserID.Value };

                title.Text = this.GetText("CP_PROFILE", "WAS_QUOTED");
                icon = "quote-left";
                message = this.GetTextFormatted("WAS_QUOTED_MSG", userLink.RenderToString(), topicLink.RenderToString());
            }

            iconLabel.Text = $@"<i class=""fas fa-circle fa-stack-2x text-secondary""></i>
               <i class=""fas fa-{icon} fa-stack-1x fa-inverse""></i>;";

            displayDateTime.DateTime = activity.Created;

            messageHolder.Controls.Add(new Literal { Text = message });
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = 5;

            var stream = this.GetRepository<Activity>().GetPaged(
                x => x.UserID == this.PageContext.PageUserID,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);

            this.ActivityStream.DataSource = stream;

            this.PagerTop.Count = stream != null && stream.Any()
                                      ? this.GetRepository<Activity>()
                                          .Count(x => x.UserID == this.PageContext.PageUserID).ToType<int>()
                                      : 0;

            this.ItemCount = stream.Count;

            this.DataBind();
        }

        #endregion
    }
}