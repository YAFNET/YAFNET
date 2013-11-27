/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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

            string link = @"<a href=""{0}"">{1}</a>".FormatWith(YafBuildLink.GetLink(ForumPages.profile, "u={0}", row["LastUserID"]), this.Get<YafBoardSettings>().EnableDisplayName ? this.HtmlEncode(row["LastUserDisplayName"]) : this.HtmlEncode(row["LastUserName"]));
            string by = this.GetTextFormatted("lastpostlink", this.Get<IDateTime>().FormatDateTime((DateTime)row["LastPosted"]), link);

            string html = @"{0} <a href=""{1}""><img src=""{2}"" alt="""" /></a>".FormatWith(
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

            bool autoWatchTopicsEnabled = false;

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

            this.PageContext.AddLoadMessage(this.GetText("SAVED_NOTIFICATION_SETTING"), MessageTypes.Success);
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
                this.GetRepository<WatchForum>().DeleteByIDs(ids);
                this.BindData();
            }
            else
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_SELECTFORUMS"), MessageTypes.Warning);
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
                this.GetRepository<WatchTopic>().DeleteByIDs(ids);
                this.BindData();
            }
            else
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_SELECTTOPICS"), MessageTypes.Warning);
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
            bool showSubscribe =
              !(selectedValue == UserNotificationSetting.AllTopics || selectedValue == UserNotificationSetting.NoNotification);

            this.SubscribeHolder.Visible = showSubscribe;
        }

        #endregion
    }
}