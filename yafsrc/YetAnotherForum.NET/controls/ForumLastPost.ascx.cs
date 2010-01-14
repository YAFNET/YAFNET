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
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// Renders the "Last Post" part of the Forum Topics
  /// </summary>
  public partial class ForumLastPost : BaseUserControl
  {
    /// <summary>
    /// The _data row.
    /// </summary>
    private DataRow _dataRow = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumLastPost"/> class.
    /// </summary>
    public ForumLastPost()
    {
      PreRender += new EventHandler(ForumLastPost_PreRender);
    }

    /// <summary>
    /// Gets or sets DataRow.
    /// </summary>
    public DataRow DataRow
    {
      get
      {
        return this._dataRow;
      }

      set
      {
        this._dataRow = value;
      }
    }

    /// <summary>
    /// The forum last post_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumLastPost_PreRender(object sender, EventArgs e)
    {
      if (DataRow != null)
      {
        if (int.Parse(DataRow["ReadAccess"].ToString()) == 0)
        {
          this.TopicInPlaceHolder.Visible = false;
        }

        if (DataRow["LastPosted"] != DBNull.Value)
        {
          this.LastPosted.Text = YafServices.DateTime.FormatDateTimeTopic(DataRow["LastPosted"]);
          this.topicLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "t={0}", DataRow["LastTopicID"]);
          this.topicLink.Text = StringHelper.Truncate(YafServices.BadWordReplace.Replace(DataRow["LastTopicName"].ToString()), 50);
          this.ProfileUserLink.UserID = Convert.ToInt32(DataRow["LastUserID"]);
          this.ProfileUserLink.UserName = DataRow["LastUser"].ToString();

          this.LastTopicImgLink.ToolTip = PageContext.Localization.GetText("GO_LAST_POST");
          this.LastTopicImgLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", DataRow["LastMessageID"]);
          this.Icon.ThemeTag = (DateTime.Parse(Convert.ToString(DataRow["LastPosted"])) > Mession.GetTopicRead((int) DataRow["LastTopicID"]))
                                 ? "ICON_NEWEST"
                                 : "ICON_LATEST";

          this.LastPostedHolder.Visible = true;
          this.NoPostsLabel.Visible = false;
        }
        else
        {
          // show "no posts"
          this.LastPostedHolder.Visible = false;
          this.NoPostsLabel.Visible = true;
        }
      }
    }
  }
}