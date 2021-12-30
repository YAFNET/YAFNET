/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Pages.Profile
{
    // YAF.Pages
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// User Page To Manage Email Subscriptions
    /// </summary>
    public partial class Subscriptions : ProfilePage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriptions"/> class.
        /// </summary>
        public Subscriptions()
            : base("SUBSCRIPTIONS")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageSizeTopics.DataSource = StaticDataHelper.PageEntries();
            this.PageSizeTopics.DataTextField = "Name";
            this.PageSizeTopics.DataValueField = "Value";
            this.PageSizeTopics.DataBind();

            this.PageSizeForums.DataSource = StaticDataHelper.PageEntries();
            this.PageSizeForums.DataTextField = "Name";
            this.PageSizeForums.DataValueField = "Value";
            this.PageSizeForums.DataBind();

            this.BindData();

            this.DailyDigestRow.Visible = this.PageContext.BoardSettings.AllowDigestEmail;
            this.PMNotificationRow.Visible = this.PageContext.BoardSettings.AllowPMEmailNotification;

            var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

            if (!this.PageContext.BoardSettings.AllowNotificationAllPostsAllTopics)
            {
                // remove it...
                items.Remove(UserNotificationSetting.AllTopics.ToInt());
            }

            this.rblNotificationType.Items.AddRange(
                items.Select(x => new ListItem(this.GetText(x.Value), x.Key.ToString())).ToArray());

            var setting =
                this.rblNotificationType.Items.FindByValue(
                    this.PageContext.User.NotificationSetting.ToInt().ToString())
                ?? this.rblNotificationType.Items.FindByValue(0.ToString());

            if (setting != null)
            {
                setting.Selected = true;
            }

            // update the ui...
            this.UpdateSubscribeUi(this.PageContext.User.NotificationSetting);
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.PageContext.User.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
        }

        /// <summary>
        /// Rebinds the Data After a Page Change
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTopics_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindDataTopics();
        }

        /// <summary>
        /// Rebinds the Data After a Page Change
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerForums_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindDataForums();
        }

        /// <summary>
        /// Save Preferences
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveUser_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var autoWatchTopicsEnabled = false;

            var value = this.rblNotificationType.SelectedValue.ToEnum<UserNotificationSetting>();

            if (value == UserNotificationSetting.TopicsIPostToOrSubscribeTo)
            {
                autoWatchTopicsEnabled = true;
            }

            // save the settings...
            this.GetRepository<User>().SaveNotification(
                this.PageContext.PageUserID,
                this.PMNotificationEnabled.Checked,
                autoWatchTopicsEnabled,
                this.rblNotificationType.SelectedValue.ToType<int>(),
                this.DailyDigestEnabled.Checked);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));

            this.PageContext.AddLoadMessage(this.GetText("SAVED_NOTIFICATION_SETTING"), MessageTypes.success);
        }

        /// <summary>
        /// Un-watch Forums
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UnsubscribeForums_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var ids = GetCheckedIds(this.ForumList, "unsubf", "tfid");

            if (ids.Any())
            {
                this.GetRepository<WatchForum>().DeleteByIds(ids);
                this.BindData();
            }
            else
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_SELECTFORUMS"), MessageTypes.warning);
            }
        }

        /// <summary>
        /// Un-watch Topics
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UnsubscribeTopics_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var ids = GetCheckedIds(this.TopicList, "unsubx", "ttid");

            if (ids.Any())
            {
                this.GetRepository<WatchTopic>().DeleteByIds(ids);
                this.BindData();
            }
            else
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_SELECTTOPICS"), MessageTypes.warning);
            }
        }

        /// <summary>
        /// The page size on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeForumsSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindDataForums();
        }

        /// <summary>
        /// The page size on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeTopicsSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindDataTopics();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the Notification Type control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void rblNotificationType_SelectionChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var selectedValue = this.rblNotificationType.SelectedItem.Value.ToEnum<UserNotificationSetting>();

            this.UpdateSubscribeUi(selectedValue);
        }

        /// <summary>
        /// Gets the checked ids.
        /// </summary>
        /// <param name="repeater">
        /// The repeater.
        /// </param>
        /// <param name="checkBoxId">
        /// The check box id.
        /// </param>
        /// <param name="idLabelId">
        /// The id label id.
        /// </param>
        /// <returns>
        /// Returns the List with the Checked IDs
        /// </returns>
        private static List<int> GetCheckedIds(Repeater repeater, string checkBoxId, string idLabelId)
        {
            return (from item in repeater.Items.OfType<RepeaterItem>()
                    let checkBox = item.FindControlAs<CheckBox>(checkBoxId)
                    let idLabel = item.FindControlAs<Label>(idLabelId)
                    where checkBox.Checked
                    select idLabel.Text.ToTypeOrDefault<int?>(null)
                into id
                    where id.HasValue
                    select id.Value).ToList();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.BindDataForums();

            this.BindDataTopics();

            this.PMNotificationEnabled.Checked = this.PageContext.User.PMNotification;
            this.DailyDigestEnabled.Checked = this.PageContext.User.DailyDigest;

            this.DataBind();
        }

        /// <summary>
        /// The bind data forums.
        /// </summary>
        private void BindDataForums()
        {
            this.PagerForums.PageSize = this.PageSizeForums.SelectedValue.ToType<int>();

            var list = this.GetRepository<WatchForum>().List(
                this.PageContext.PageUserID,
                this.PagerForums.CurrentPageIndex,
                this.PagerForums.PageSize);

            if (list == null)
            {
                this.UnsubscribeForums.Visible = false;

                this.ForumsHolder.Visible = false;

                return;
            }

            this.ForumList.DataSource = list;

            this.PagerForums.Count = list.Any()
                ? this.GetRepository<WatchForum>()
                    .Count(x => x.UserID == this.PageContext.PageUserID).ToType<int>()
                : 0;

            this.ForumList.DataBind();
            this.PagerForums.DataBind();
        }

        /// <summary>
        /// The bind data topics.
        /// </summary>
        private void BindDataTopics()
        {
            this.PagerTopics.PageSize = this.PageSizeTopics.SelectedValue.ToType<int>();

            var list = this.GetRepository<WatchTopic>().List(
                this.PageContext.PageUserID,
                this.PagerTopics.CurrentPageIndex,
                this.PagerTopics.PageSize);

            if (list == null)
            {
                this.UnsubscribeTopics.Visible = false;

                this.TopicsHolder.Visible = false;

                return;
            }

            this.TopicList.DataSource = list;

            this.PagerTopics.Count = list.Any()
                ? this.GetRepository<WatchTopic>()
                    .Count(x => x.UserID == this.PageContext.PageUserID).ToType<int>()
                : 0;

            this.TopicList.DataBind();
            this.PagerTopics.DataBind();
        }

        /// <summary>
        /// The update subscribe UI.
        /// </summary>
        /// <param name="selectedValue">
        /// The selected value.
        /// </param>
        private void UpdateSubscribeUi(UserNotificationSetting selectedValue)
        {
            var showSubscribe =
              !(selectedValue is UserNotificationSetting.AllTopics or UserNotificationSetting.NoNotification);

            this.SubscribeHolder.Visible = showSubscribe;
        }

        #endregion
    }
}