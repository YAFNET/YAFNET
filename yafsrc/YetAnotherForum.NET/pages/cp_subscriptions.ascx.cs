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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// User Page To Manage Email Subcriptions
    /// </summary>
    public partial class cp_subscriptions : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "cp_subscriptions" /> class.
        /// </summary>
        public cp_subscriptions()
            : base("CP_SUBSCRIPTIONS")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the forum replies.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// The format forum replies.
        /// </returns>
        protected string FormatForumReplies([NotNull] object o)
        {
            var row = o as DataRow;

            return row != null ? "{0}".FormatWith((int)row["Messages"] - (int)row["Topics"]) : string.Empty;
        }

        /// <summary>
        /// Formats the last posted.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// The format last posted.
        /// </returns>
        protected string FormatLastPosted([NotNull] object o)
        {
            var row = o as DataRow;

            if (row == null)
            {
                return string.Empty;
            }

            if (row["LastPosted"].ToString().Length == 0)
            {
                return "&nbsp;";
            }

            var displayName = this.Get<YafBoardSettings>().EnableDisplayName
                                  ? this.HtmlEncode(row["LastUserDisplayName"])
                                  : this.HtmlEncode(row["LastUserName"]);

            var link = @"<a href=""{0}"">{1}</a>".FormatWith(YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", row["LastUserID"], displayName), displayName);
            var by = this.GetTextFormatted("lastpostlink", this.Get<IDateTime>().FormatDateTime((DateTime)row["LastPosted"]), link);

            var html = @"{0} <a href=""{1}""><img src=""{2}"" alt="""" /></a>".FormatWith(
                @by,
                YafBuildLink.GetLink(ForumPages.posts, "m={0}&find=lastpost", row["LastMessageID"]),
                this.GetThemeContents("ICONS", "ICON_LATEST"));

            return html;
        }

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

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                    this.Get<YafBoardSettings>().EnableDisplayName
                        ? this.PageContext.CurrentUserData.DisplayName
                        : this.PageContext.PageUserName,
                    YafBuildLink.GetLink(ForumPages.cp_profile));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            this.UnsubscribeForums.Text = this.GetText("unsubscribe");
            this.UnsubscribeTopics.Text = this.GetText("unsubscribe");
            this.SaveUser.Text = this.GetText("Save");

            this.DailyDigestRow.Visible = this.Get<YafBoardSettings>().AllowDigestEmail;
            this.PMNotificationRow.Visible = this.Get<YafBoardSettings>().AllowPMEmailNotification;

            var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

            if (!this.Get<YafBoardSettings>().AllowNotificationAllPostsAllTopics)
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
            this.UpdateSubscribeUI(this.PageContext.CurrentUserData.NotificationSetting);
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
            LegacyDb.user_savenotification(
                this.PageContext.PageUserID,
                this.PMNotificationEnabled.Checked,
                autoWatchTopicsEnabled,
                this.rblNotificationType.SelectedValue,
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

            this.UpdateSubscribeUI(selectedValue);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var watchForums = this.GetRepository<WatchForum>().List(this.PageContext.PageUserID).AsEnumerable();

            this.ForumList.DataSource = watchForums;

            this.UnsubscribeForums.Visible = watchForums.Count() != 0;

            // we are going to page results
            var dt = this.GetRepository<WatchTopic>().List(this.PageContext.PageUserID);

            // set pager and datasource
            this.PagerTop.Count = dt.Rows.Count;

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
            
            this.PMNotificationEnabled.Checked = this.PageContext.CurrentUserData.PMNotification;
            this.DailyDigestEnabled.Checked = this.PageContext.CurrentUserData.DailyDigest;

            this.DataBind();
        }

        /// <summary>
        /// The update subscribe ui.
        /// </summary>
        /// <param name="selectedValue">
        /// The selected value.
        /// </param>
        private void UpdateSubscribeUI(UserNotificationSetting selectedValue)
        {
            var showSubscribe =
              !(selectedValue == UserNotificationSetting.AllTopics || selectedValue == UserNotificationSetting.NoNotification);

            this.SubscribeHolder.Visible = showSubscribe;
        }

        #endregion
    }
}