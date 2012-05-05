/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj?rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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

using YAF.Utils.Helpers;

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The topic list mode.
    /// </summary>
    public enum TopicListMode
    {
        /// <summary>
        ///   The active topics list.
        /// </summary>
        Active,

        /// <summary>
        ///   The unanswered topics list.
        /// </summary>
        Unanswered,

        /// <summary>
        ///   The favorite topics list.
        /// </summary>
        Favorite,

        /// <summary>
        ///   The Unread topics list.
        /// </summary>
        Unread,

        /// <summary>
        ///   The User topics list.
        /// </summary>
        User
    }

    /// <summary>
    /// My Topics List Control.
    /// </summary>
    public partial class MyTopicsList : BaseUserControl
    {
        /* Data Fields */

        #region Constants and Fields

        /// <summary>
        ///   The last post image tooltip.
        /// </summary>
        protected string lastPostImageTT = string.Empty;

        /// <summary>
        ///   The _last forum name.
        /// </summary>
        private string _lastForumName = string.Empty;

        /// <summary>
        ///   default since date is now
        /// </summary>
        private DateTime sinceDate = DateTime.UtcNow;

        /// <summary>
        ///   default since option is "since last visit"
        /// </summary>
        private int sinceValue;

        /// <summary>
        ///   The Topic List Data Table
        /// </summary>
        private DataTable topics;

        #endregion

        /* Properties */

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether Auto Databind.
        /// </summary>
        public bool AutoDatabind { get; set; }

        /// <summary>
        ///   Gets or sets Determines what is th current mode of the control.
        /// </summary>
        public TopicListMode CurrentMode { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The bind data.
        /// </summary>
        public void BindData()
        {
            // default since date is now
            this.sinceDate = DateTime.UtcNow;

            // default since option is "since last visit"
            this.sinceValue = 0;

            // is any "since"option selected
            if (this.Since.SelectedItem != null)
            {
                // get selected value
                this.sinceValue = int.Parse(this.Since.SelectedItem.Value);

                // decrypt selected option
                if (this.sinceValue == 9999)
                {
                    // all
                    // get all, from the beginning
                    this.sinceDate = DateTimeHelper.SqlDbMinTime();
                }
                else if (this.sinceValue > 0)
                {
                    // days
                    // get posts newer then defined number of days
                    this.sinceDate = DateTime.UtcNow - TimeSpan.FromDays(this.sinceValue);
                }
                else if (this.sinceValue < 0)
                {
                    // hours
                    // get posts newer then defined number of hours
                    this.sinceDate = DateTime.UtcNow + TimeSpan.FromHours(this.sinceValue);
                }
            }

            // we want to filter topics since last visit
            if (this.sinceValue == 0)
            {
                this.sinceDate = this.Get<IYafSession>().LastVisit ?? DateTime.UtcNow;

                if (this.CurrentMode.Equals(TopicListMode.Unread))
                {
                    this.sinceDate = this.Get<IReadTrackCurrentUser>().LastRead;
                }
            }

            // filter by category
            object categoryIdObject = null;

            // is category set?
            if (this.PageContext.Settings.CategoryID != 0)
            {
                categoryIdObject = this.PageContext.Settings.CategoryID;
            }

            // we'll hold topics in this table
            DataTable topicList = null;
            
            if (this.sinceDate == DateTime.MinValue)
            {
                this.sinceDate = DateTimeHelper.SqlDbMinTime();
            }

            // set the page size here
            int basePageSize = this.Get<YafBoardSettings>().MyTopicsListPageSize;
            this.PagerTop.PageSize = basePageSize;

            // page index in db which is returned back  is +1 based!
            int nCurrentPageIndex = this.PagerTop.CurrentPageIndex;

            // now depending on mode fill the table
            switch (this.CurrentMode)
            {
                case TopicListMode.Active:
                    topicList = LegacyDb.topic_active(
                        this.PageContext.PageBoardID,
                        categoryIdObject,
                        this.PageContext.PageUserID,
                        this.sinceDate,
                        DateTime.UtcNow,
                        nCurrentPageIndex,
                        basePageSize,
                        this.Get<YafBoardSettings>().UseStyledNicks,
                        this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                    break;
                case TopicListMode.Unanswered:
                    topicList = LegacyDb.topic_unanswered(
                        this.PageContext.PageBoardID,
                        categoryIdObject,
                        this.PageContext.PageUserID,
                        this.sinceDate,
                        DateTime.UtcNow,
                        nCurrentPageIndex,
                        basePageSize,
                        this.Get<YafBoardSettings>().UseStyledNicks,
                        this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                    break;
                case TopicListMode.Unread:
                    topicList = LegacyDb.topic_unread(
                        this.PageContext.PageBoardID,
                        categoryIdObject,
                        this.PageContext.PageUserID,
                        this.sinceDate,
                        DateTime.UtcNow,
                        nCurrentPageIndex,
                        basePageSize,
                        this.Get<YafBoardSettings>().UseStyledNicks,
                        this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                    break;
                case TopicListMode.User:
                    topicList = LegacyDb.Topics_ByUser(
                        this.PageContext.PageBoardID,
                        categoryIdObject,
                        this.PageContext.PageUserID,
                        this.sinceDate,
                        DateTime.UtcNow,
                        nCurrentPageIndex,
                        basePageSize,
                        this.Get<YafBoardSettings>().UseStyledNicks,
                        this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                    break;
                case TopicListMode.Favorite:
                    topicList = LegacyDb.topic_favorite_details(
                        this.PageContext.PageBoardID,
                        (YafContext.Current.Settings.CategoryID == 0)
                            ? null
                            : (object)YafContext.Current.Settings.CategoryID,
                        this.PageContext.PageUserID,
                        this.sinceDate,
                        DateTime.UtcNow,
                        nCurrentPageIndex,
                        basePageSize,
                        this.Get<YafBoardSettings>().UseStyledNicks,
                        this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                    break;
            }

             if (topicList == null)
            {
                this.PagerTop.Count = 0;
                return;
            }
            if ( topicList.Rows.Count <=0)
            {
                this.PagerTop.Count = 0;
                this.TopicList.DataSource = null;
                this.TopicList.DataBind();
                return;
            }

            this.topics = topicList;

            DataTable topicsNew = topicList.Copy();

            foreach (var thisTableRow in topicsNew.Rows.Cast<DataRow>().Where(thisTableRow => thisTableRow["LastPosted"] != DBNull.Value
                                                                                                  && thisTableRow["LastPosted"].ToType<DateTime>() <= this.sinceDate))
            {
                thisTableRow.Delete();
            }

            // styled nicks
            topicsNew.AcceptChanges();
            if (this.Get<YafBoardSettings>().UseStyledNicks)
            {
                this.Get<IStyleTransform>().DecodeStyleByTable(ref topicsNew, false, "LastUserStyle", "StarterStyle");
            }

            // let's page the results
            this.PagerTop.Count = topicsNew.Rows.Count > 0
                                      ? topicsNew.AsEnumerable().First().Field<int>("TotalRows")
                                      : 0;

            this.TopicList.DataSource = topicsNew;

            // Get new Feeds links
            this.BindFeeds();

            // data bind controls
            this.DataBind();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes dropdown with options to filter results by date.
        /// </summary>
        protected void InitSinceDropdown()
        {
            var lastVisit = this.Get<IYafSession>().LastVisit;

            // value 0, for since last visted
            this.Since.Items.Add(
                new ListItem(
                    this.GetTextFormatted(
                        "last_visit",
                        !this.PageContext.IsMobileDevice
                            ? this.Get<IDateTime>().FormatDateTime(
                                lastVisit.HasValue && lastVisit.Value != DateTime.MinValue
                                    ? lastVisit.Value
                                    : DateTime.UtcNow)
                            : string.Empty),
                    "0"));

            // negative values for hours backward
            this.Since.Items.Add(new ListItem(this.GetText("last_hour"), "-1"));
            this.Since.Items.Add(new ListItem(this.GetText("last_two_hours"), "-2"));
            this.Since.Items.Add(new ListItem(this.GetText("last_eight_hours"), "-8"));

            // positive values for days backward
            this.Since.Items.Add(new ListItem(this.GetText("last_day"), "1"));
            this.Since.Items.Add(new ListItem(this.GetText("last_two_days"), "2"));
            this.Since.Items.Add(new ListItem(this.GetText("last_week"), "7"));
            this.Since.Items.Add(new ListItem(this.GetText("last_two_weeks"), "14"));
            this.Since.Items.Add(new ListItem(this.GetText("last_month"), "31"));
        }

        /// <summary>
        /// Mark all Topis in the List as Read
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void MarkAll_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindData();

            if (this.topics.Rows.Count.Equals(0))
            {
                return;
            }

            foreach (DataRow row in this.topics.Rows)
            {
                this.Get<IReadTrackCurrentUser>().SetTopicRead(row["TopicID"].ToType<int>());
            }

            // Rebind
            this.BindData();
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.lastPostImageTT = this.GetText("DEFAULT", "GO_LAST_POST");
            this.MarkAll.Text = this.GetText("DEFAULT", "MARK_ALL_ASREAD");

            if (!this.IsPostBack)
            {
                this.InitSinceDropdown();

                int? previousSince = null;

                switch (this.CurrentMode)
                {
                    case TopicListMode.User:
                        previousSince = this.Get<IYafSession>().UserTopicSince;
                        this.Since.Items.Add(new ListItem(this.GetText("show_all"), "9999"));
                        this.Since.SelectedIndex = this.Since.Items.Count - 1;
                        break;
                    case TopicListMode.Unread:
                        previousSince = this.Get<IYafSession>().UnreadTopicSince;
                        this.Since.Items.Clear();
                        this.Since.Items.Add(new ListItem(this.GetText("SHOW_UNREAD_ONLY"), "0"));
                        this.Since.SelectedIndex = 0;
                        break;
                    case TopicListMode.Active:
                        previousSince = this.Get<IYafSession>().ActiveTopicSince;
                        this.Since.SelectedIndex = 0;
                        break;
                    case TopicListMode.Unanswered:
                        previousSince = this.Get<IYafSession>().UnansweredTopicSince;
                        this.Since.Items.Add(new ListItem(this.GetText("show_all"), "9999"));
                        this.Since.SelectedIndex = 2;
                        break;
                    case TopicListMode.Favorite:
                        previousSince = this.Get<IYafSession>().FavoriteTopicSince;
                        this.Since.Items.Add(new ListItem(this.GetText("show_all"), "9999"));
                        this.Since.SelectedIndex = 2;
                        break;
                }

                // has user selected any "since" value before we can remember now?
                if (previousSince.HasValue)
                {
                    // look for value previously selected
                    ListItem sinceItem = this.Since.Items.FindByValue(previousSince.Value.ToString());

                    // and select it if found
                    if (sinceItem != null)
                    {
                        this.Since.SelectedIndex = this.Since.Items.IndexOf(sinceItem);
                    }
                }
            }

            if (this.AutoDatabind)
            {
                this.BindData();
            }
        }

        /// <summary>
        /// Handles the PageChange event of the Pager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The print forum name.
        /// </summary>
        /// <param name="row">
        /// The row. 
        /// </param>
        /// <returns>
        /// Returns the forum name output. 
        /// </returns>
        protected string PrintForumName([NotNull] DataRowView row)
        {
            var forumName = this.Page.HtmlEncode(row["ForumName"]);
            string html = string.Empty;
            if (forumName != this._lastForumName)
            {
                html =
                    @"<tr><td class=""header2"" colspan=""6""><a href=""{1}"" alt=""{2}"" title=""{2}"" >{0}</a></td></tr>"
                        .FormatWith(
                            forumName,
                            YafBuildLink.GetLink(ForumPages.topics, "f={0}", row["ForumID"]),
                            this.GetText("COMMON", "VIEW_FORUM"));
                this._lastForumName = forumName;
            }

            return html;
        }

        /// <summary>
        /// Reloads the Topic Last Based on the Selected Since Value
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Since_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Set the controls' pager index to 0.
            this.PagerTop.CurrentPageIndex = 0;

            // save since option to rememver it next time
            switch (this.CurrentMode)
            {
                case TopicListMode.User:
                    this.Get<IYafSession>().UserTopicSince = this.Since.SelectedValue.ToType<int>();
                    break;
                case TopicListMode.Unread:
                    this.Get<IYafSession>().UnreadTopicSince = this.Since.SelectedValue.ToType<int>();
                    break;
                case TopicListMode.Active:
                    this.Get<IYafSession>().ActiveTopicSince = this.Since.SelectedValue.ToType<int>();
                    break;
                case TopicListMode.Unanswered:
                    this.Get<IYafSession>().UnansweredTopicSince = this.Since.SelectedValue.ToType<int>();
                    break;
                case TopicListMode.Favorite:
                    this.Get<IYafSession>().FavoriteTopicSince = this.Since.SelectedValue.ToType<int>();
                    break;
            }

            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Bind the Feeds
        /// </summary>
        private void BindFeeds()
        {
            bool accessActive = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ActiveTopicFeedAccess);
            bool accessFavorite = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().FavoriteTopicFeedAccess);

            // RSS link setup 
            if (this.Get<YafBoardSettings>().ShowRSSLink)
            {
                switch (this.CurrentMode)
                {
                    case TopicListMode.User:
                        this.RssFeed.Visible = false;
                        break;
                    case TopicListMode.Unread:
                        this.RssFeed.Visible = false;
                        break;
                    case TopicListMode.Unanswered:
                        this.RssFeed.Visible = false;
                        break;
                    case TopicListMode.Active:
                        this.RssFeed.TitleLocalizedTag = "RSSICONTOOLTIPACTIVE";
                        this.RssFeed.FeedType = YafRssFeeds.Active;
                        this.RssFeed.AdditionalParameters =
                            "txt={0}&d={1}".FormatWith(
                                this.Server.UrlEncode(this.HtmlEncode(this.Since.Items[this.Since.SelectedIndex].Text)),
                                this.Server.UrlEncode(this.HtmlEncode(this.sinceDate.ToString())));

                        this.RssFeed.Visible = accessActive;
                        break;
                    case TopicListMode.Favorite:
                        this.RssFeed.TitleLocalizedTag = "RSSICONTOOLTIPFAVORITE";
                        this.RssFeed.FeedType = YafRssFeeds.Favorite;
                        this.RssFeed.AdditionalParameters =
                            "txt={0}&d={1}".FormatWith(
                                this.Server.UrlEncode(this.HtmlEncode(this.Since.Items[this.Since.SelectedIndex].Text)),
                                this.Server.UrlEncode(this.HtmlEncode(this.sinceDate.ToString())));
                        this.RssFeed.Visible = accessFavorite;
                        break;
                }
            }

            // Atom link setup 
            if (!this.Get<YafBoardSettings>().ShowAtomLink)
            {
                return;
            }

            switch (this.CurrentMode)
            {
                case TopicListMode.User:
                    this.AtomFeed.Visible = false;
                    break;
                case TopicListMode.Unread:
                    this.AtomFeed.Visible = false;
                    break;
                case TopicListMode.Unanswered:
                    this.AtomFeed.Visible = false;
                    break;
                case TopicListMode.Active:
                    this.AtomFeed.TitleLocalizedTag = "ATOMICONTOOLTIPACTIVE";
                    this.AtomFeed.FeedType = YafRssFeeds.Active;
                    this.AtomFeed.ImageThemeTag = "ATOMFEED";
                    this.AtomFeed.TextLocalizedTag = "ATOMFEED";
                    this.AtomFeed.AdditionalParameters =
                        "txt={0}&d={1}".FormatWith(
                            this.Server.UrlEncode(this.HtmlEncode(this.Since.Items[this.Since.SelectedIndex].Text)),
                            this.Server.UrlEncode(this.HtmlEncode(this.sinceDate.ToString())));
                    this.AtomFeed.Visible = accessActive;
                    break;
                case TopicListMode.Favorite:
                    this.AtomFeed.TitleLocalizedTag = "ATOMICONTOOLTIPFAVORITE";
                    this.AtomFeed.FeedType = YafRssFeeds.Favorite;
                    this.AtomFeed.ImageThemeTag = "ATOMFEED";
                    this.AtomFeed.TextLocalizedTag = "ATOMFEED";
                    this.AtomFeed.AdditionalParameters =
                        "txt={0}&d={1}".FormatWith(
                            this.Server.UrlEncode(this.HtmlEncode(this.Since.Items[this.Since.SelectedIndex].Text)),
                            this.Server.UrlEncode(this.HtmlEncode(this.sinceDate.ToString())));

                    this.AtomFeed.Visible = accessFavorite;
                    break;
            }

            // We should set token to show a common control to handlw it as an Atom feed.
            this.AtomFeed.IsAtomFeed = true;
        }

        #endregion
    }
}