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
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The forum active discussion.
  /// </summary>
  public partial class ForumActiveDiscussion : BaseUserControl
  {
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
      // Latest forum posts
      // Shows the latest n number of posts on the main forum list page
      string cacheKey = YafCache.GetBoardCacheKey(Constants.Cache.ForumActiveDiscussions);
      DataTable activeTopics = null;

      if (PageContext.IsGuest)
      {
        // allow caching since this is a guest...
        activeTopics = PageContext.Cache[cacheKey] as DataTable;
      }

      if (activeTopics == null)
      {
        activeTopics = DB.topic_latest(
          PageContext.PageBoardID, PageContext.BoardSettings.ActiveDiscussionsCount, PageContext.PageUserID, PageContext.BoardSettings.UseStyledNicks);

        // Set colorOnly parameter to true, as we get all but color from css in the place
        if (PageContext.BoardSettings.UseStyledNicks)
        {
          new StyleTransform(PageContext.Theme).DecodeStyleByTable(ref activeTopics, true, "LastUserStyle");
        }

        if (PageContext.IsGuest)
        {
          PageContext.Cache.Insert(
            cacheKey, activeTopics, null, DateTime.Now.AddMinutes(PageContext.BoardSettings.ActiveDiscussionsCacheTimeout), TimeSpan.Zero);
        }
      }

      this.LatestPosts.DataSource = activeTopics;
      this.LatestPosts.DataBind();
    }


    /// <summary>
    /// The latest posts_ item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LatestPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      // populate the controls here...
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        var currentRow = (DataRowView) e.Item.DataItem;

        // make message url...
        string messageUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", currentRow["LastMessageID"]);

        // get the controls
        var textMessageLink = (HyperLink) e.Item.FindControl("TextMessageLink");
        var imageMessageLink = (HyperLink) e.Item.FindControl("ImageMessageLink");
        var lastPostedImage = (ThemeImage) e.Item.FindControl("LastPostedImage");
        var lastUserLink = (UserLink) e.Item.FindControl("LastUserLink");
        var lastPostedDateLabel = (Label) e.Item.FindControl("LastPostedDateLabel");
        var forumLink = (HyperLink) e.Item.FindControl("ForumLink");

        // populate them...
        textMessageLink.Text = YafServices.BadWordReplace.Replace(currentRow["Topic"].ToString());
        textMessageLink.NavigateUrl = messageUrl;
        imageMessageLink.NavigateUrl = messageUrl;

        // Just in case...
        if (currentRow["LastUserID"] != DBNull.Value)
        {
          lastUserLink.UserID = Convert.ToInt32(currentRow["LastUserID"]);
          lastUserLink.Style = currentRow["LastUserStyle"].ToString();
        }

        if (currentRow["LastPosted"] != DBNull.Value)
        {
          lastPostedDateLabel.Text = YafServices.DateTime.FormatDateTimeTopic(currentRow["LastPosted"]);
          lastPostedImage.ThemeTag = (DateTime.Parse(currentRow["LastPosted"].ToString()) > Mession.GetTopicRead(Convert.ToInt32(currentRow["TopicID"])))
                                       ? "ICON_NEWEST"
                                       : "ICON_LATEST";
        }

        forumLink.Text = currentRow["Forum"].ToString();
        forumLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.topics, "f={0}", currentRow["ForumID"]);
      }
    }
  }
}