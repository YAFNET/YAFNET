/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj?rnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.Controls
{
    using System;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.UI;
    using YAF.Classes.Utils;
    using YAF.Utilities;

    /// <summary>
    /// Summary description for buddies.
    /// </summary>
    public partial class MyTopicsList : BaseUserControl
    {
        /// <summary>
        /// The _last forum name.
        /// </summary>
        protected string _lastForumName = string.Empty;

        /// <summary>
        /// the control mode.
        /// </summary>
        private int _controlMode;

        /// <summary>
        /// Determines what is th current mode of the control.
        /// </summary>
        public int Mode
        {
            get
            {
                return this._controlMode;
            }
            set
            {
                this._controlMode = value;
            }

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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PageContext.BoardSettings.ShowRSSLink)
            {
                if (this.Mode == 1)
                {
                    this.RssFeed.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg=active");
                }
                else
                {
                    this.RssFeed.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg=favorite");
                }
                this.RssFeed.Text = PageContext.Localization.GetText("RSSFEED");
                this.RssFeed.Visible = true;
            }
            else
            {
                this.RssFeed.Visible = false;
            }
            if (!IsPostBack)
            {
                this.Since.Items.Add(new ListItem(PageContext.Localization.GetTextFormatted("last_visit", YafServices.DateTime.FormatDateTime(Mession.LastVisit)), "0"));
                this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_hour"), "-1"));
                this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_two_hours"), "-2"));
                this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_day"), "1"));
                this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_two_days"), "2"));
                this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_week"), "7"));
                this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_two_weeks"), "14"));
                this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_month"), "31"));

                if (this.Mode == 1)
                {
                    if (Mession.ActiveTopicSince.HasValue)
                    {
                        ListItem activeTopicItem = this.Since.Items.FindByValue(Mession.ActiveTopicSince.Value.ToString());
                        if (activeTopicItem != null)
                        {
                            activeTopicItem.Selected = true;
                        }
                    }
                    else
                    {
                        this.Since.SelectedIndex = 0;
                    }
                }
                else
                {
                    this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("show_all"), "365"));
                    if (Mession.FavoriteTopicSince.HasValue)
                    {
                        ListItem favoriteTopicItem = this.Since.Items.FindByValue(Mession.FavoriteTopicSince.Value.ToString());
                        if (favoriteTopicItem != null)
                        {
                            favoriteTopicItem.Selected = true;
                        }
                    }
                    else
                    {
                        this.Since.SelectedIndex = 0;
                    }
                }
            }
            BindData();
        }

        /// <summary>
        /// The since_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Since_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set the controls' pager index to 0.
            PagerTop.CurrentPageIndex = 0;
            if (Mode == 1)
            {
                Mession.ActiveTopicSince = Convert.ToInt32(this.Since.SelectedValue);
            }
            else
            {
                Mession.FavoriteTopicSince = Convert.ToInt32(this.Since.SelectedValue);
            }
            BindData();
        }

        /// <summary>
        /// The print forum name.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The print forum name.
        /// </returns>
        protected string PrintForumName(DataRowView row)
        {
            var forumName = (string)row["ForumName"];
            string html = string.Empty;
            if (forumName != this._lastForumName)
            {
                html = String.Format(
                  @"<tr><td class=""header2"" colspan=""6""><a href=""{1}"">{0}</a></td></tr>",
                  forumName,
                  YafBuildLink.GetLink(ForumPages.topics, "f={0}", row["ForumID"]));
                this._lastForumName = forumName;
            }

            return html;
        }

        /// <summary>
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Pager_PageChange(object sender, EventArgs e)
        {
            BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        public void BindData()
        {
            DateTime sinceDate = DateTime.Now;
            int sinceValue = 0;

            if (this.Since.SelectedItem != null)
            {
                sinceValue = int.Parse(this.Since.SelectedItem.Value);
                sinceDate = DateTime.Now;
                if (sinceValue == 365)
                {
                    sinceDate = (DateTime)(System.Data.SqlTypes.SqlDateTime.MinValue);
                }

                else if (sinceValue > 0)
                {
                    sinceDate = DateTime.Now - TimeSpan.FromDays(sinceValue);
                }
                else if (sinceValue < 0)
                {
                    sinceDate = DateTime.Now + TimeSpan.FromHours(sinceValue);
                }
            }

            if (sinceValue == 0)
            {
                sinceDate = Mession.LastVisit;
            }

            var pds = new PagedDataSource();
            pds.AllowPaging = true;

            object categoryIDObject = null;

            if (PageContext.Settings.CategoryID != 0)
            {
                categoryIDObject = PageContext.Settings.CategoryID;
            }

            DataTable topicList;
            if (this.Mode == 1)
            {
                topicList = DB.topic_active(PageContext.PageBoardID, PageContext.PageUserID, sinceDate, categoryIDObject, PageContext.BoardSettings.UseStyledNicks);
            }
            else
            {
                topicList = YafServices.FavoriteTopic.FavoriteTopicDetails(sinceDate);
            }
            if (PageContext.BoardSettings.UseStyledNicks)
            {
                new StyleTransform(PageContext.Theme).DecodeStyleByTable(ref topicList, true, "LastUserStyle", "StarterStyle");
            }

            DataView dv = topicList.DefaultView;
            pds.DataSource = dv;
            this.PagerTop.Count = dv.Count;
            this.PagerTop.PageSize = 15;
            pds.PageSize = this.PagerTop.PageSize;

            pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
            this.TopicList.DataSource = pds;

            DataBind();
        }
    }
}
