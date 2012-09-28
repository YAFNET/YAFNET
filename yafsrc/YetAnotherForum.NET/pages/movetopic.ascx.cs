/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Interfaces.Extensions;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for movetopic.
  /// </summary>
  public partial class movetopic : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "movetopic" /> class.
    /// </summary>
    public movetopic()
      : base("MOVETOPIC")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The move_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Move_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      int? linkDays = null;
      int ld = -2;
      if (this.LeavePointer.Checked && this.LinkDays.Text.IsSet() && !int.TryParse(this.LinkDays.Text, out ld))
      {
          this.PageContext.AddLoadMessage(this.GetText("POINTER_DAYS_INVALID"));
          return;
      }
      if (this.ForumList.SelectedValue.ToType<int>() <= 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("CANNOT_MOVE_TO_CATEGORY"));
        return;
      }

      // only move if it's a destination is a different forum.
      if (this.ForumList.SelectedValue.ToType<int>() != this.PageContext.PageForumID)
      {
          if (ld >= -2)
          {
              linkDays = ld;
          }
          // Ederon : 7/14/2007
          LegacyDb.topic_move(this.PageContext.PageTopicID, this.ForumList.SelectedValue, this.LeavePointer.Checked, linkDays);
      }

      YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
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
      if (this.Request.QueryString.GetFirstOrDefault("t") == null || !this.PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied();
      }

        if (this.IsPostBack)
        {
            return;
        }

        if (this.PageContext.Settings.LockedForum == 0)
        {
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.PageContext.PageCategoryName, 
                YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
        this.PageLinks.AddLink(
            this.PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID));

        this.Move.Text = this.GetText("MOVE");
        this.Move.ToolTip = "{0}: {1}".FormatWith(this.GetText("MOVE"), this.PageContext.PageTopicName);

        bool showMoved = this.Get<YafBoardSettings>().ShowMoved;
        // Ederon : 7/14/2007 - by default, leave pointer is set on value defined on host level
        this.LeavePointer.Checked = this.Get<YafBoardSettings>().ShowMoved;

        trLeaveLink.Visible = showMoved;
        trLeaveLinkDays.Visible = showMoved;
        if (showMoved)
        {
            LinkDays.Text = "1";
        }

        this.ForumList.DataSource = LegacyDb.forum_listall_sorted(this.PageContext.PageBoardID, this.PageContext.PageUserID);

        this.DataBind();

        ListItem pageItem = this.ForumList.Items.FindByValue(this.PageContext.PageForumID.ToString());
        if (pageItem != null)
        {
            pageItem.Selected = true;
        }
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private static void InitializeComponent()
    {
    }

    #endregion
  }
}