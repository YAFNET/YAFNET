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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Linq;
  using System.Web.UI;

  using YAF.Core;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Core.Services;
  using YAF.Types.Interfaces.Data;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Provides an Online/Offline status for a YAF User
  /// </summary>
  public class OnlineStatusImage : ThemeImage
  {
    #region Properties

    /// <summary>
    ///   The userid of the user.
    /// </summary>
    public int UserID
    {
      get
      {
        return ViewState["UserID"].ToType<int>();
      }

      set
      {
        ViewState["UserID"] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter output)
    {
      this.LocalizedTitlePage = "POSTS";

      if (Visible)
      {
        DataTable activeUsers = this.Get<IDataCache>().GetOrSet(
          Constants.Cache.UsersOnlineStatus,
          () => this.Get<YafDbBroker>().GetActiveList(false, YafContext.Current.BoardSettings.ShowCrawlersInActiveList),
          TimeSpan.FromMilliseconds((double)YafContext.Current.BoardSettings.OnlineStatusCacheTimeout));

        if (activeUsers.AsEnumerable().Any(x => x.Field<int>("UserId") == this.UserID && !x.Field<bool>("IsHidden")))
        {
          // online
          this.LocalizedTitleTag = "USERONLINESTATUS";
          this.ThemeTag = "USER_ONLINE";
          this.Alt = "Online";
        }
        else
        {
          // offline
          this.LocalizedTitleTag = "USEROFFLINESTATUS";
          this.ThemeTag = "USER_OFFLINE";
          this.Alt = "Offline";
        }
      }

      base.Render(output);
    }

    #endregion
  }
}