/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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

using YAF.Utils.Helpers;

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utilities;
  using YAF.Utils;

    #endregion

  /// <summary>
  /// The last posts.
  /// </summary>
  public partial class LastPosts : BaseUserControl
  {
    #region Properties

    /// <summary>
    ///   Gets or sets TopicID.
    /// </summary>
    public long? TopicID
    {
      get
      {
        if (this.ViewState["TopicID"] != null)
        {
          return this.ViewState["TopicID"].ToType<int>();
        }

        return null;
      }

      set
      {
        this.ViewState["TopicID"] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The last post update timer_ tick.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LastPostUpdateTimer_Tick([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.BindData();
    }

      /// <summary>
      /// The on pre render.
      /// </summary>
      /// <param name="e">
      /// The e.
      /// </param>
      protected override void OnPreRender([NotNull] EventArgs e)
      {
          this.PageContext.PageElements.RegisterJsBlockStartup(
              this.LastPostUpdatePanel,
              "DisablePageManagerScrollJs",
              JavaScriptBlocks.DisablePageManagerScrollJs);

          if (this.PageContext.ForumPageType == ForumPages.postmessage)
          {
              var editorId = this.Get<YafBoardSettings>().AllowUsersTextEditor && this.PageContext.TextEditor.IsSet()
                                 ? this.PageContext.TextEditor
                                 : this.Get<YafBoardSettings>().ForumEditor;

              // Check if Editor exists, if not fallback to default editorid=1
              var forumEditor = this.Get<IModuleManager<ForumEditor>>().GetBy(editorId, false)
                                ?? this.Get<IModuleManager<ForumEditor>>().GetBy("1");

              if (forumEditor.Description.Contains("CKEditor") || forumEditor.Description.Contains("DotNetNuke"))
              {
                  this.LastPostUpdateTimer.Enabled = false;
              }
          }

          base.OnPreRender(e);
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
      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        if (this.TopicID.HasValue)
        {
            bool showDeleted = false;
            int userId = 0;

            if (this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }

            if (!showDeleted && (this.Get<YafBoardSettings>().ShowDeletedMessages &&
                                 !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
                || this.PageContext.IsAdmin ||
                this.PageContext.IsForumModerator)
            {
                userId = this.PageContext.PageUserID;
            }

             DataTable dt = LegacyDb.post_list(
                 this.TopicID,
                 this.PageContext.PageUserID,
                 userId,
                 0,
                 showDeleted,
                 false,
                 false,
                 DateTimeHelper.SqlDbMinTime(),
                 DateTime.UtcNow,
                 DateTimeHelper.SqlDbMinTime(),
                 DateTime.UtcNow,
                 0,
                 10,
                 2,
                 0,
                 0,
                 false,
                 -1);

             this.repLastPosts.DataSource = dt.AsEnumerable();
        }
        else
        {
           this.repLastPosts.DataSource = null;
        }

        this.repLastPosts.DataBind();
    }

    #endregion
  }
}