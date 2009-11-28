/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for active.
  /// </summary>
  public partial class active : ForumPage
  {
    /// <summary>
    /// The _last forum name.
    /// </summary>
    protected string _lastForumName = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="active"/> class.
    /// </summary>
    public active()
      : base("ACTIVE")
    {
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
        this.RssFeed.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg=active");
        this.RssFeed.Text = GetText("RSSFEED");
        this.RssFeed.Visible = true;
      }
      else
      {
        this.RssFeed.Visible = false;
      }

      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(GetText("TITLE"), string.Empty);

        this.ForumJumpHolder.Visible = PageContext.BoardSettings.ShowForumJump && PageContext.Settings.LockedForum == 0;

        this.Since.Items.Add(new ListItem(GetTextFormatted("last_visit", YafServices.DateTime.FormatDateTime(Mession.LastVisit)), "0"));
        this.Since.Items.Add(new ListItem(GetText("last_hour"), "-1"));
        this.Since.Items.Add(new ListItem(GetText("last_two_hours"), "-2"));
        this.Since.Items.Add(new ListItem(GetText("last_day"), "1"));
        this.Since.Items.Add(new ListItem(GetText("last_two_days"), "2"));
        this.Since.Items.Add(new ListItem(GetText("last_week"), "7"));
        this.Since.Items.Add(new ListItem(GetText("last_two_weeks"), "14"));
        this.Since.Items.Add(new ListItem(GetText("last_month"), "31"));

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

      BindData();
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
    void BindData()
    {
      DateTime sinceDate = DateTime.Now;
      int sinceValue = 0;

      if (this.Since.SelectedItem != null)
      {
        sinceValue = int.Parse(this.Since.SelectedItem.Value);
        sinceDate = DateTime.Now;
        if (sinceValue > 0)
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

      DataTable topicActive = DB.topic_active(PageContext.PageBoardID, PageContext.PageUserID, sinceDate, categoryIDObject, PageContext.BoardSettings.UseStyledNicks);

      if (PageContext.BoardSettings.UseStyledNicks)
      {
        new StyleTransform(PageContext.Theme).DecodeStyleByTable(ref topicActive, true, "LastUserStyle", "StarterStyle");
      }

      DataView dv = topicActive.DefaultView;
      pds.DataSource = dv;
      this.PagerTop.Count = dv.Count;
      this.PagerTop.PageSize = 15;
      pds.PageSize = this.PagerTop.PageSize;

      pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
      this.TopicList.DataSource = pds;

      DataBind();
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
      var forumName = (string) row["ForumName"];
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
      this.PagerTop.CurrentPageIndex = 0;
      Mession.ActiveTopicSince = Convert.ToInt32(this.Since.SelectedValue);
      BindData();
    }
  }
}