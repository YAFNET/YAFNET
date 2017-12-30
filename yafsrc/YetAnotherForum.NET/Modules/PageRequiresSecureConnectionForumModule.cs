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
  using System.Web;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;
  

  #endregion

  /// <summary>
  /// The page requires secure connection module.
  /// </summary>
  [YafModule("Page Requires Secure Connection Module", "Tiny Gecko", 1)]
  public class PageRequiresSecureConnectionForumModule : SimpleBaseForumModule
  {
    #region Public Methods

    /// <summary>
    /// The init forum.
    /// </summary>
    public override void InitForum()
    {
      this.ForumControl.Load += this.ForumControl_Load;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The forum control_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumControl_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool accessDenied = false;

      switch (this.ForumPageType)
      {
        case ForumPages.login:
          if (!HttpContext.Current.Request.IsSecureConnection & this.PageContext.BoardSettings.UseSSLToLogIn)
          {
            accessDenied = true;
          }

          break;

        case ForumPages.register:
          if (!HttpContext.Current.Request.IsSecureConnection & this.PageContext.BoardSettings.UseSSLToRegister)
          {
            accessDenied = true;
          }

          break;
      }

      if (accessDenied)
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
      }
    }

    #endregion
  }
}