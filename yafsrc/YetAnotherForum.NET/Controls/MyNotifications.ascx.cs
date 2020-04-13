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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The profile Timeline
    /// </summary>
    public partial class MyNotifications : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the item count.
        /// </summary>
        protected int ItemCount { get; set; }

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlock("dropDownToggleJs", JavaScriptBlocks.DropDownToggleJs());

            base.OnPreRender(e);
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
            this.WasMentioned.Text = this.GetText("WAS_MENTIONED");
            this.ReceivedThanks.Text = this.GetText("RECEIVED_THANKS");
            this.WasQuoted.Text = this.GetText("WAS_QUOTED");

            if (this.IsPostBack)
            {
                return;
            }

            var previousPageSize = this.Get<ISession>().UserActivityPageSize;

            if (previousPageSize.HasValue)
            {
                // look for value previously selected
                var sinceItem = this.PageSize.Items.FindByValue(previousPageSize.Value.ToString());

                // and select it if found
                if (sinceItem != null)
                {
                    this.PageSize.SelectedIndex = this.PageSize.Items.IndexOf(sinceItem);
                }
            }

            this.BindData();
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

            var messageHolder = e.Item.FindControlAs<PlaceHolder>("Message");
            var displayDateTime = e.Item.FindControlAs<DisplayDateTime>("DisplayDateTime");
            var markRead = e.Item.FindControlAs<ThemeButton>("MarkRead");
            var iconLabel = e.Item.FindControlAs<Label>("Icon");

            var message = string.Empty;
            var icon = string.Empty;

            var topicLink = new ThemeButton
                                {
                                    NavigateUrl =
                                        BuildLink.GetLink(
                                            ForumPages.Posts,
                                            "m={0}#post{0}",
                                            activity.MessageID.Value),
                                    Type = ButtonStyle.None,
                                    Text = this.GetRepository<Topic>().GetById(activity.TopicID.Value).TopicName,
                                    Icon = "comment"
                                };

            if (activity.ActivityFlags.ReceivedThanks)
            {
                var userLink = new UserLink { UserID = activity.FromUserID.Value };

                icon = "heart";
                message = this.GetTextFormatted(
                    "RECEIVED_THANKS_MSG",
                    userLink.RenderToString(),
                    topicLink.RenderToString());
            }

            if (activity.ActivityFlags.WasMentioned)
            {
                var userLink = new UserLink { UserID = activity.FromUserID.Value };

                icon = "at";
                message = this.GetTextFormatted(
                    "WAS_MENTIONED_MSG",
                    userLink.RenderToString(),
                    topicLink.RenderToString());
            }

            if (activity.ActivityFlags.WasQuoted)
            {
                var userLink = new UserLink { UserID = activity.FromUserID.Value };

                icon = "quote-left";
                message = this.GetTextFormatted(
                    "WAS_QUOTED_MSG",
                    userLink.RenderToString(),
                    topicLink.RenderToString());
            }

            var notify = activity.Notification ? "text-success" : "text-secondary";

            iconLabel.Text = $@"<i class=""fas fa-circle fa-stack-2x {notify}""></i>
               <i class=""fas fa-{icon} fa-stack-1x fa-inverse""></i>";

            displayDateTime.DateTime = activity.Created;

            messageHolder.Controls.Add(new Literal { Text = message });

            if (!activity.Notification)
            {
                return;
            }

            markRead.CommandArgument = activity.MessageID.Value.ToString();
            markRead.Visible = true;
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
        /// Mark all Activity as read
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void MarkAll_Click(object sender, EventArgs e)
        {
            this.GetRepository<Activity>().MarkAllAsRead(this.PageContext.PageUserID);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));

            this.BindData();
        }

        /// <summary>
        /// The activity stream_ on item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ActivityStream_OnItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "read")
            {
                return;
            }

            this.GetRepository<Activity>().UpdateNotification(
                this.PageContext.PageUserID,
                e.CommandArgument.ToType<int>());

            this.BindData();
        }

        /// <summary>
        /// The update filter click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void UpdateFilterClick(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Reset Filter
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ResetClick(object sender, EventArgs e)
        {
            this.WasMentioned.Checked = true;
            this.ReceivedThanks.Checked = true;
            this.WasQuoted.Checked = true;

            this.BindData();
        }

        /// <summary>
        /// The page size selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.Get<ISession>().UserActivityPageSize = this.PageSize.SelectedValue.ToType<int>();

            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
            private void BindData()
        {
            this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

            var stream = this.GetRepository<Activity>().Get(x => x.UserID == this.PageContext.PageUserID && x.FromUserID.HasValue);

            if (!this.WasMentioned.Checked)
            {
                stream.RemoveAll(a => a.WasMentioned);
            }

            if (!this.ReceivedThanks.Checked)
            {
                stream.RemoveAll(a => a.ReceivedThanks);
            }

            if (!this.WasQuoted.Checked)
            {
                stream.RemoveAll(a => a.WasQuoted);
            }

            var paged = stream.OrderByDescending(item => item.ID)
                .Skip(this.PagerTop.CurrentPageIndex * this.PagerTop.PageSize).Take(this.PagerTop.PageSize).ToList();

            this.ActivityStream.DataSource = paged;

            if (paged.Any())
            {
                this.PagerTop.Count = stream.Count;

                this.ItemCount = paged.Count;

                this.NoItems.Visible = false;
            }
            else
            {
                this.PagerTop.Count = 0;
                this.ItemCount = 0;

                this.NoItems.Visible = true;
            }

            this.DataBind();
        }

        #endregion
    }
}