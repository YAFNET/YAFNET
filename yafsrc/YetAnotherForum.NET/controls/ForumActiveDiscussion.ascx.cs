/* Yet Another Forum.NET
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
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The forum active discussion.
  /// </summary>
  public partial class ForumActiveDiscussion : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The last post tooltip string.
    /// </summary>
    private string lastPostToolTip;

    #endregion

    #region Methods

    /// <summary>
    /// The latest posts_ item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LatestPosts_ItemDataBound([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
    {
      // populate the controls here...
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        var currentRow = (DataRowView)e.Item.DataItem;

        // make message url...
        string messageUrl = YafBuildLink.GetLinkNotEscaped(
          ForumPages.posts, "m={0}#post{0}", currentRow["LastMessageID"]);

        // get the controls
        var textMessageLink = (HyperLink)e.Item.FindControl("TextMessageLink");
        var imageMessageLink = (HyperLink)e.Item.FindControl("ImageMessageLink");
        var lastPostedImage = (ThemeImage)e.Item.FindControl("LastPostedImage");
        var lastUserLink = (UserLink)e.Item.FindControl("LastUserLink");
        var lastPostedDateLabel = (DisplayDateTime)e.Item.FindControl("LastPostDate");
        var forumLink = (HyperLink)e.Item.FindControl("ForumLink");

        // populate them...
        textMessageLink.Text = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(currentRow["Topic"].ToString()));
        textMessageLink.ToolTip = this.PageContext.Localization.GetText("COMMON", "VIEW_TOPIC");
        textMessageLink.NavigateUrl = messageUrl;

        imageMessageLink.NavigateUrl = messageUrl;
        lastPostedImage.LocalizedTitle = this.lastPostToolTip;

        // Just in case...
        if (currentRow["LastUserID"] != DBNull.Value)
        {
          lastUserLink.UserID = Convert.ToInt32(currentRow["LastUserID"]);
          lastUserLink.Style = currentRow["LastUserStyle"].ToString();
        }

        if (currentRow["LastPosted"] != DBNull.Value)
        {
          lastPostedDateLabel.DateTime = currentRow["LastPosted"];
          lastPostedImage.ThemeTag = (DateTime.Parse(currentRow["LastPosted"].ToString()) >
                                      YafContext.Current.Get<IYafSession>().GetTopicRead(
                                        Convert.ToInt32(currentRow["TopicID"])))
                                       ? "ICON_NEWEST"
                                       : "ICON_LATEST";
        }

        forumLink.Text = this.Page.HtmlEncode(currentRow["Forum"].ToString());
        forumLink.ToolTip = this.PageContext.Localization.GetText("COMMON", "VIEW_FORUM");
        forumLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.topics, "f={0}", currentRow["ForumID"]);
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // Latest forum posts
      // Shows the latest n number of posts on the main forum list page
      string cacheKey = YafCache.GetBoardCacheKey(Constants.Cache.ForumActiveDiscussions);
      DataTable activeTopics = null;

      if (this.PageContext.IsGuest)
      {
        // allow caching since this is a guest...
        activeTopics = this.PageContext.Cache[cacheKey] as DataTable;
      }

      if (activeTopics == null)
      {
        activeTopics = DB.topic_latest(
          this.PageContext.PageBoardID, 
          this.PageContext.BoardSettings.ActiveDiscussionsCount, 
          this.PageContext.PageUserID, 
          this.PageContext.BoardSettings.UseStyledNicks, 
          this.PageContext.BoardSettings.NoCountForumsInActiveDiscussions);

        // Set colorOnly parameter to true, as we get all but color from css in the place
        if (this.PageContext.BoardSettings.UseStyledNicks)
        {
          this.Get<IStyleTransform>().DecodeStyleByTable(ref activeTopics, true, "LastUserStyle");
        }

        if (this.PageContext.IsGuest)
        {
          this.PageContext.Cache.Insert(
            cacheKey, 
            activeTopics, 
            null, 
            DateTime.UtcNow.AddMinutes(this.PageContext.BoardSettings.ActiveDiscussionsCacheTimeout), 
            TimeSpan.Zero);
        }
      }

      bool groupAccess = this.Get<IPermissions>().Check(this.PageContext.BoardSettings.PostLatestFeedAccess);
      this.AtomFeed.Visible = this.PageContext.BoardSettings.ShowAtomLink && groupAccess;
      this.RssFeed.Visible = this.PageContext.BoardSettings.ShowRSSLink && groupAccess;

      this.lastPostToolTip = this.PageContext.Localization.GetText("DEFAULT", "GO_LAST_POST");
      this.LatestPosts.DataSource = activeTopics;
      this.LatestPosts.DataBind();
    }

    #endregion
  }
}