/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Modules
{
  #region Using

  using System;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Module that handles page permission feature
  /// </summary>
  [YafModule("Page Permission Module", "Tiny Gecko", 1)]
  public class PagePermissionForumModule : SimpleBaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    /// The _permissions.
    /// </summary>
    private readonly IPermissions permissions;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PagePermissionForumModule"/> class.
    /// </summary>
    /// <param name="permissions">
    /// The permissions.
    /// </param>
    public PagePermissionForumModule([NotNull] IPermissions permissions)
    {
      this.permissions = permissions;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      this.CurrentForumPage.Load += this.CurrentPageLoad;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The current page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void CurrentPageLoad([NotNull] object sender, [NotNull] EventArgs e)
    {
      // check access permissions for specific pages...
      switch (this.ForumPageType)
      {
        case ForumPages.activeusers:
          this.permissions.HandleRequest(this.PageContext.BoardSettings.ActiveUsersViewPermissions);
          break;
        case ForumPages.members:
          this.permissions.HandleRequest(this.PageContext.BoardSettings.MembersListViewPermissions);
          break;
        case ForumPages.profile:
        case ForumPages.albums:
        case ForumPages.album:
          this.permissions.HandleRequest(this.PageContext.BoardSettings.ProfileViewPermissions);
          break;
        case ForumPages.search:
          this.permissions.HandleRequest(this.PageContext.BoardSettings.SearchPermissions);
          break;
      }
    }

    #endregion
  }
}