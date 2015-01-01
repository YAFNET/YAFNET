/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Controls;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
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
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.PageContext.PageCategoryName, 
                YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
        }

        this.PageLinks.AddForum(this.PageContext.PageForumID);
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