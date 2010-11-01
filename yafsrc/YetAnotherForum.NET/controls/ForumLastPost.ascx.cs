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

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Renders the "Last Post" part of the Forum Topics
  /// </summary>
  public partial class ForumLastPost : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The Go to last post Image ToolTip.
    /// </summary>
    private string _alt;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForumLastPost" /> class.
    /// </summary>
    public ForumLastPost()
    {
      this.PreRender += this.ForumLastPost_PreRender;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets Alt.
    /// </summary>
    [NotNull]
    public string Alt
    {
      get
      {
        if (string.IsNullOrEmpty(this._alt))
        {
          return string.Empty;
        }

        return this._alt;
      }

      set
      {
        this._alt = value;
      }
    }

    /// <summary>
    ///   Gets or sets DataRow.
    /// </summary>
    public DataRow DataRow { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// The forum last post_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumLastPost_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.DataRow != null)
      {
        if (int.Parse(this.DataRow["ReadAccess"].ToString()) == 0)
        {
          this.TopicInPlaceHolder.Visible = false;
        }

        if (this.DataRow["LastPosted"] != DBNull.Value)
        {
          this.LastPostDate.DateTime = this.DataRow["LastPosted"];
          this.topicLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
            ForumPages.posts, "t={0}", this.DataRow["LastTopicID"]);
          this.topicLink.Text =
            StringHelper.Truncate(
              this.Get<YafBadWordReplace>().Replace(this.HtmlEncode(this.DataRow["LastTopicName"].ToString())), 50);
          this.ProfileUserLink.UserID = Convert.ToInt32(this.DataRow["LastUserID"]);
          if (string.IsNullOrEmpty(this.Alt))
          {
            this.Alt = this.PageContext.Localization.GetText("GO_LAST_POST");
          }

          this.LastTopicImgLink.ToolTip = this.Alt;
          this.LastTopicImgLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
            ForumPages.posts, "m={0}#post{0}", this.DataRow["LastMessageID"]);
          this.Icon.ThemeTag = (DateTime.Parse(Convert.ToString(this.DataRow["LastPosted"])) >
                                YafContext.Current.Get<YafSession>().GetTopicRead((int)this.DataRow["LastTopicID"]))
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

    #endregion
  }
}