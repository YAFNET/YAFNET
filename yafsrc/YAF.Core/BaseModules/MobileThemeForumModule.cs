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
namespace YAF.Core.BaseModules
{
  #region Using

  using System;
  using System.Web;

  using YAF.Core.Handlers;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The mobile theme module.
  /// </summary>
  [YafModule("Mobile Theme Module", "Tiny Gecko", 1)]
  public class MobileThemeForumModule : BaseForumModule
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MobileThemeForumModule"/> class.
    /// </summary>
    /// <param name="yafSession">
    /// The YAF session.
    /// </param>
    /// <param name="httpRequestBase">
    /// The http request base.
    /// </param>
    public MobileThemeForumModule([NotNull] IYafSession yafSession, [NotNull] HttpRequestBase httpRequestBase)
    {
      this.YafSession = yafSession;
      this.HttpRequestBase = httpRequestBase;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets HttpRequestBase.
    /// </summary>
    public HttpRequestBase HttpRequestBase { get; set; }

    /// <summary>
    /// Gets or sets YafSession.
    /// </summary>
    public IYafSession YafSession { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The init.
    /// </summary>
    public override void Init()
    {
      YafContext.Current.AfterInit += this.Current_AfterInit;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The current_ after init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Current_AfterInit([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafContext.Current.Vars["IsMobile"] = false;

      // see if this is a mobile device...
      if (!UserAgentHelper.IsMobileDevice(this.HttpRequestBase.UserAgent) &&
          !this.HttpRequestBase.Browser.IsMobileDevice)
      {
        // make sure to shut off mobile theme usage if the user agent is not mobile.
        if (this.YafSession.UseMobileTheme ?? false)
        {
          this.YafSession.UseMobileTheme = false;
        }

        return;
      }

      if (!YafContext.Current.IsGuest)
      {
        // return if the user has mobile themes shut off in their profile.
        var userData = new CombinedUserDataHelper(YafContext.Current.PageUserID);
        if (!userData.UseMobileTheme)
        {
          return;
        }
      }

      this.UpdateUseMobileThemeFromQueryString();

      // use the mobile theme?
      var useMobileTheme = this.YafSession.UseMobileTheme ?? true;

      // get the current mobile theme...
      var mobileTheme = YafContext.Current.BoardSettings.MobileTheme;

      if (mobileTheme.IsSet())
      {
        // create a new theme object...
        var theme = new YafTheme(mobileTheme);

        // make sure it's valid...
        if (YafTheme.IsValidTheme(theme.ThemeFile))
        {
          YafContext.Current.Vars["IsMobile"] = true;

          // set new mobile theme...
          if (useMobileTheme)
          {
            YafContext.Current.Get<ThemeProvider>().Theme = theme;
            this.YafSession.UseMobileTheme = true;
          }

          return;
        }
      }

      // make sure to shut off mobile theme usage if there was no valid mobile theme found...
      if (this.YafSession.UseMobileTheme ?? false)
      {
        this.YafSession.UseMobileTheme = false;
      }
    }

    /// <summary>
    /// Updates the use mobile theme session variable from the query string.
    /// </summary>
    private void UpdateUseMobileThemeFromQueryString()
    {
      var fullSite = this.HttpRequestBase.QueryString.GetFirstOrDefault("fullsite");
      if (fullSite.IsSet() && fullSite.Equals("true"))
      {
        this.YafSession.UseMobileTheme = false;
      }

      var mobileSite = this.HttpRequestBase.QueryString.GetFirstOrDefault("mobileSite");
      if (mobileSite.IsSet() && mobileSite.Equals("true"))
      {
        this.YafSession.UseMobileTheme = true;
      }
    }

    #endregion
  }
}