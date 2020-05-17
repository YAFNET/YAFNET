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

namespace YAF.Pages.Profile
{
    // YAF.Pages
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;
    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
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

            this.BindData();

            this.DailyDigestRow.Visible = this.Get<BoardSettings>().AllowDigestEmail;
            this.PMNotificationRow.Visible = this.Get<BoardSettings>().AllowPMEmailNotification;

            var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

            if (!this.Get<BoardSettings>().AllowNotificationAllPostsAllTopics)
            {
                // remove it...
                items.Remove(UserNotificationSetting.AllTopics.ToInt());
            }

            this.rblNotificationType.Items.AddRange(
                items.Select(x => new ListItem(this.GetText(x.Value), x.Key.ToString())).ToArray());

            var setting =
                this.rblNotificationType.Items.FindByValue(
                    this.PageContext.CurrentUserData.NotificationSetting.ToInt().ToString())
                ?? this.rblNotificationType.Items.FindByValue(0.ToString());

            if (setting != null)
            {
                setting.Selected = true;
            }

            // update the ui...
            this.UpdateSubscribeUi(this.PageContext.CurrentUserData.NotificationSetting);
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<BoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                BuildLink.GetLink(ForumPages.MyAccount));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
        }

        /// <summary>
        /// Rebinds the Data After a Page Change
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// Save Preferences
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveUser_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Page.Validate();

            if (!this.Page.IsValid)
            {
                return;
            }

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
        /// Gets the checked ids.
        /// </summary>
        /// <param name="repeater">The repeater.</param>
        /// <param name="checkBoxId">The check box id.</param>
        /// <param name="idLabelId">The id label id.</param>
        /// <returns></returns>
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
        /// Unwatch Forums
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
        /// Unwatch Topics
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
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var watchForums = this.GetRepository<WatchForum>().List(this.PageContext.PageUserID);

            this.ForumList.DataSource = watchForums;

            this.ForumsHolder.Visible = watchForums.Any();

            // we are going to page results
            var dt = this.GetRepository<WatchTopic>().List(this.PageContext.PageUserID);

            // set pager and data source
            this.PagerTop.Count = dt.Count;

            // page to render
            var currentPageIndex = this.PagerTop.CurrentPageIndex;

            var pageCount = this.PagerTop.Count / this.PagerTop.PageSize;

            // if we are above total number of pages, select last
            if (currentPageIndex >= pageCount)
            {
                currentPageIndex = pageCount - 1;
            }

            // bind list
            var topicList = dt.AsEnumerable().Skip(currentPageIndex * this.PagerTop.PageSize).Take(this.PagerTop.PageSize);

            this.TopicList.DataSource = topicList;

            this.UnsubscribeTopics.Visible = topicList.Count() != 0;

            this.TopicsHolder.Visible = topicList.Count() != 0;

            this.PMNotificationEnabled.Checked = this.PageContext.CurrentUserData.PMNotification;
            this.DailyDigestEnabled.Checked = this.PageContext.CurrentUserData.DailyDigest;

            this.DataBind();
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
              !(selectedValue == UserNotificationSetting.AllTopics
                || selectedValue == UserNotificationSetting.NoNotification);

            this.SubscribeHolder.Visible = showSubscribe;
        }

        #endregion
    }
}