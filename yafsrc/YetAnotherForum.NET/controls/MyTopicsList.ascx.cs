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
  #region Using

  using System;
  using System.Data;
  using System.Data.SqlTypes;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  public enum TopicListMode
  {
    Active,
    Favorite
  }

  /// <summary>
  /// Summary description for buddies.
  /// </summary>
  public partial class MyTopicsList : BaseUserControl
  {
    /* Data Fields */
    #region Constants and Fields

    /// <summary>
    /// the current mode.
    /// </summary>
    private TopicListMode _controlMode;

    /// <summary>
    /// The _last forum name.
    /// </summary>
    private string _lastForumName = string.Empty;

    /// <summary>
    /// default since date is now
    /// </summary>
    private DateTime sinceDate = DateTime.UtcNow;

    /// <summary>
    /// default since option is "since last visit"
    /// </summary>
    int sinceValue = 0;
   

    /// <summary>
    /// The last post image tooltip.
    /// </summary>
    protected string lastPostImageTT = string.Empty; 

    #endregion

    /* Properties */
    #region Properties

    /// <summary>
    /// Determines what is th current mode of the control.
    /// </summary>
    public TopicListMode CurrentMode
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

    #endregion

    /* Event Handlers */

    /* Methods */
    #region Public Methods

    /// <summary>
    /// The bind data.
    /// </summary>
    public void BindData()
    {
      // default since date is now
      sinceDate = DateTime.UtcNow;

      // default since option is "since last visit"
      sinceValue = 0;

      // is any "since"option selected
      if (this.Since.SelectedItem != null)
      {
        // get selected value
        sinceValue = int.Parse(this.Since.SelectedItem.Value);

        // sinceDate = DateTime.UtcNow;	// no need to do it again (look above)

        // decrypt selected option
        if (sinceValue == 9999)
        {
          // all
          // get all, from the beginning
          sinceDate = (DateTime)SqlDateTime.MinValue;
        }
        else if (sinceValue > 0)
        {
          // days
          // get posts newer then defined number of days
          sinceDate = DateTime.UtcNow - TimeSpan.FromDays(sinceValue);
        }
        else if (sinceValue < 0)
        {
          // hours
          // get posts newer then defined number of hours
          sinceDate = DateTime.UtcNow + TimeSpan.FromHours(sinceValue);
        }
      }

      // we want to filter topics since last visit
      if (sinceValue == 0)
      {
        sinceDate = YafContext.Current.Get<YafSession>().LastVisit;
      }

      // we want to page results
      var pds = new PagedDataSource();
      pds.AllowPaging = true;

      // filter by category
      object categoryIDObject = null;

      // is category set?
      if (this.PageContext.Settings.CategoryID != 0)
      {
        categoryIDObject = this.PageContext.Settings.CategoryID;
      }

      // we'll hold topics in this table
      DataTable topicList = null;

      // now depending on mode fill the table
      if (this.CurrentMode == TopicListMode.Active)
      {
        // we are getting active topics
        topicList = DB.topic_active(
          this.PageContext.PageBoardID, 
          this.PageContext.PageUserID, 
          sinceDate, 
          categoryIDObject, 
          this.PageContext.BoardSettings.UseStyledNicks);
      }
      else if (this.CurrentMode == TopicListMode.Favorite)
      {
        // we are getting favotes
        topicList = this.Get<YafFavoriteTopic>().FavoriteTopicDetails(sinceDate);
      }

      if (topicList != null)
      {
        // styled nicks
        if (this.PageContext.BoardSettings.UseStyledNicks)
        {
          new StyleTransform(this.PageContext.Theme).DecodeStyleByTable(
            ref topicList, true, "LastUserStyle", "StarterStyle");
        }

        // let's page the results
        DataView dv = topicList.DefaultView;
        pds.DataSource = dv;
        this.PagerTop.Count = dv.Count;

        // TODO : page size definable?
        this.PagerTop.PageSize = 15;
        pds.PageSize = this.PagerTop.PageSize;
        pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;

        // set datasource of repeater
        this.TopicList.DataSource = pds;
        
        // Get new Feeds links
        BindFeeds();
        // data bind controls
        this.DataBind();
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes dropdown with options to filter results by date.
    /// </summary>
    protected void InitSinceDropdown()
    {
      // value 0, for since last visted
      this.Since.Items.Add(
        new ListItem(
          this.PageContext.Localization.GetTextFormatted(
            "last_visit", this.Get<YafDateTime>().FormatDateTime(YafContext.Current.Get<YafSession>().LastVisit)), 
          "0"));

      // negative values for hours backward
      this.Since.Items.Add(new ListItem(this.PageContext.Localization.GetText("last_hour"), "-1"));
      this.Since.Items.Add(new ListItem(this.PageContext.Localization.GetText("last_two_hours"), "-2"));

      // positive values for days backward
      this.Since.Items.Add(new ListItem(this.PageContext.Localization.GetText("last_day"), "1"));
      this.Since.Items.Add(new ListItem(this.PageContext.Localization.GetText("last_two_days"), "2"));
      this.Since.Items.Add(new ListItem(this.PageContext.Localization.GetText("last_week"), "7"));
      this.Since.Items.Add(new ListItem(this.PageContext.Localization.GetText("last_two_weeks"), "14"));
      this.Since.Items.Add(new ListItem(this.PageContext.Localization.GetText("last_month"), "31"));
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
      
      lastPostImageTT = this.PageContext.Localization.GetText("DEFAULT", "GO_LAST_POST");
      if (!this.IsPostBack)
      {
        this.InitSinceDropdown();

        int? previousSince = null;

        if (this.CurrentMode == TopicListMode.Active)
        {
          // active topics mode
          previousSince = YafContext.Current.Get<YafSession>().ActiveTopicSince;

          // by default select "Last visited..." for active discussions
          this.Since.SelectedIndex = 0;
        }
        else if (this.CurrentMode == TopicListMode.Favorite)
        {
          // favorites mode
          previousSince = YafContext.Current.Get<YafSession>().FavoriteTopicSince;

          // add "Show All" option
          this.Since.Items.Add(new ListItem(this.PageContext.Localization.GetText("show_all"), "9999"));

          // by default select "Show All" for favorites topics
          this.Since.SelectedIndex = this.Since.Items.Count - 1;
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

      this.BindData();

      
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
      this.BindData();
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
      var forumName = Page.HtmlEncode((string)row["ForumName"]);
      string html = string.Empty;
      if (forumName != this._lastForumName)
      {
        html = @"<tr><td class=""header2"" colspan=""6""><a href=""{1}"">{0}</a></td></tr>".FormatWith(forumName, YafBuildLink.GetLink(ForumPages.topics, "f={0}", row["ForumID"]));
        this._lastForumName = forumName;
      }

      return html;
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
      this.PagerTop.CurrentPageIndex = 0;

      // save since option to rememver it next time
      if (this.CurrentMode == TopicListMode.Active)
      {
        // for active topics
        YafContext.Current.Get<YafSession>().ActiveTopicSince = Convert.ToInt32(this.Since.SelectedValue);
      }
      else if (this.CurrentMode == TopicListMode.Favorite)
      {
        // for favorites
        YafContext.Current.Get<YafSession>().FavoriteTopicSince = Convert.ToInt32(this.Since.SelectedValue);
      }

      // re-bind data
      this.BindData();
    }
    private void BindFeeds()
    {
        // RSS link setup 
        if (PageContext.BoardSettings.ShowRSSLink)
        {
            if (this.CurrentMode == TopicListMode.Active)
            {
                this.RssFeed.TitleLocalizedTag = "RSSICONTOOLTIPACTIVE";
                this.RssFeed.FeedType = YafRssFeeds.Active;
                this.RssFeed.AdditionalParameters =
                    "txt={0}&d={1}".FormatWith(
                        this.Server.UrlEncode(this.HtmlEncode(this.Since.Items[this.Since.SelectedIndex].Text)),
                        this.Server.UrlEncode(this.HtmlEncode(this.sinceDate.ToString())));
            }
            else if (this.CurrentMode == TopicListMode.Favorite)
            {
                this.RssFeed.TitleLocalizedTag = "RSSICONTOOLTIPFAVORITE";
                this.RssFeed.FeedType = YafRssFeeds.Favorite;
                this.RssFeed.AdditionalParameters =
                    "txt={0}&d={1}".FormatWith(
                        this.Server.UrlEncode(this.HtmlEncode(this.Since.Items[this.Since.SelectedIndex].Text)),
                        this.Server.UrlEncode(this.HtmlEncode(this.sinceDate.ToString())));
            }

            this.RssFeed.Visible = true;
        }

        // Atom link setup 
        if (PageContext.BoardSettings.ShowAtomLink)
        {
            {
                if (this.CurrentMode == TopicListMode.Active)
                {
                    this.AtomFeed.TitleLocalizedTag = "ATOMICONTOOLTIPACTIVE";
                    this.AtomFeed.FeedType = YafRssFeeds.Active;
                    this.AtomFeed.ImageThemeTag = "ATOMFEED";
                    this.AtomFeed.TextLocalizedTag = "ATOMFEED";
                    this.AtomFeed.AdditionalParameters =
                        "txt={0}&d={1}".FormatWith(
                            this.Server.UrlEncode(this.HtmlEncode(this.Since.Items[this.Since.SelectedIndex].Text)),
                            this.Server.UrlEncode(this.HtmlEncode(this.sinceDate.ToString())));
                }
                else if (this.CurrentMode == TopicListMode.Favorite)
                {
                    this.AtomFeed.TitleLocalizedTag = "ATOMICONTOOLTIPFAVORITE";
                    this.AtomFeed.FeedType = YafRssFeeds.Favorite;
                    this.AtomFeed.ImageThemeTag = "ATOMFEED";
                    this.AtomFeed.TextLocalizedTag = "ATOMFEED";
                    this.AtomFeed.AdditionalParameters =
                        "txt={0}&d={1}".FormatWith(
                            this.Server.UrlEncode(this.HtmlEncode(this.Since.Items[this.Since.SelectedIndex].Text)),
                            this.Server.UrlEncode(this.HtmlEncode(this.sinceDate.ToString())));
                }

                this.AtomFeed.IsAtomFeed = true;
                this.AtomFeed.Visible = true;
            }
        }
    }

      #endregion
  }
}