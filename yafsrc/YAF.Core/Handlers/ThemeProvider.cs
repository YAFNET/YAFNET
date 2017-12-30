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
namespace YAF.Core.Handlers
{
  #region Using

  using System;

  using YAF.Core.Helpers;
  using YAF.Types.Exceptions;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The theme handler.
  /// </summary>
  public class ThemeProvider
  {
    #region Constants and Fields

    /// <summary>
    ///   The _init theme.
    /// </summary>
    private bool _initTheme;

    /// <summary>
    ///   The _theme.
    /// </summary>
    private ITheme _theme;

    #endregion

    #region Events

    /// <summary>
    ///   The after init.
    /// </summary>
    public event EventHandler<EventArgs> AfterInit;

    /// <summary>
    ///   The before init.
    /// </summary>
    public event EventHandler<EventArgs> BeforeInit;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets Theme.
    /// </summary>
    public ITheme Theme
    {
      get
      {
        if (!this._initTheme)
        {
          this.InitTheme();
        }

        return this._theme;
      }

      set
      {
        this._theme = value;
        this._initTheme = value != null;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets the theme class up for usage
    /// </summary>
    /// <exception cref="CantLoadThemeException"><c>CantLoadThemeException</c>.</exception>
    private void InitTheme()
    {
        if (this._initTheme)
        {
            return;
        }

        if (this.BeforeInit != null)
        {
            this.BeforeInit(this, new EventArgs());
        }

        string themeFile;

        if (YafContext.Current.Page != null && YafContext.Current.Page["ThemeFile"] != null &&
            YafContext.Current.BoardSettings.AllowUserTheme)
        {
            // use user-selected theme
            themeFile = YafContext.Current.Page["ThemeFile"].ToString();
        }
        else if (YafContext.Current.Page != null && YafContext.Current.Page["ForumTheme"] != null)
        {
            themeFile = YafContext.Current.Page["ForumTheme"].ToString();
        }
        else
        {
            themeFile = YafContext.Current.BoardSettings.Theme;
        }

        if (!YafTheme.IsValidTheme(themeFile))
        {
            themeFile = StaticDataHelper.Themes().Rows[0][1].ToString();
        }

        // create the theme class
        this.Theme = new YafTheme(themeFile);

        // make sure it's valid again...
        if (!YafTheme.IsValidTheme(this.Theme.ThemeFile))
        {
            // can't load a theme... throw an exception.
            throw new CantLoadThemeException(
                @"Unable to find a theme to load. Last attempted to load ""{0}"" but failed.".FormatWith(themeFile));
        }

        if (this.AfterInit != null)
        {
            this.AfterInit(this, new EventArgs());
        }
    }

    #endregion
  }
}