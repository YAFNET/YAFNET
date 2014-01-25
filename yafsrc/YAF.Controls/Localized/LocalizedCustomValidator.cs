/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
  using System.Web.UI.WebControls;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;

  #endregion

  /// <summary>
  /// The localized custom validator.
  /// </summary>
  public class LocalizedCustomValidator : CustomValidator, ILocalizationSupport
  {
    #region Constants and Fields

    /// <summary>
    /// The _enable bb code.
    /// </summary>
    private bool _enableBbCode;

    /// <summary>
    /// The _localized page.
    /// </summary>
    private string _localizedPage;

    /// <summary>
    /// The _localized tag.
    /// </summary>
    private string _localizedTag;

    /// <summary>
    /// The _param 0.
    /// </summary>
    private string _param0;

    /// <summary>
    /// The _param 1.
    /// </summary>
    private string _param1;

    /// <summary>
    /// The _param 2.
    /// </summary>
    private string _param2;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether EnableBBCode.
    /// </summary>
    public bool EnableBBCode
    {
      get
      {
        return this._enableBbCode;
      }

      set
      {
        this._enableBbCode = value;
      }
    }

    /// <summary>
    /// Gets or sets LocalizedPage.
    /// </summary>
    public string LocalizedPage
    {
      get
      {
        return this._localizedPage;
      }

      set
      {
        this._localizedPage = value;
      }
    }

    /// <summary>
    /// Gets or sets LocalizedTag.
    /// </summary>
    public string LocalizedTag
    {
      get
      {
        return this._localizedTag;
      }

      set
      {
        this._localizedTag = value;
      }
    }

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        if (this.Site != null && this.Site.DesignMode)
        {
          // design-time, return null...
          return null;
        }

        return YafContext.Current;
      }
    }

    /// <summary>
    /// Gets or sets Param0.
    /// </summary>
    public string Param0
    {
      get
      {
        return this._param0;
      }

      set
      {
        this._param0 = value;
      }
    }

    /// <summary>
    /// Gets or sets Param1.
    /// </summary>
    public string Param1
    {
      get
      {
        return this._param1;
      }

      set
      {
        this._param1 = value;
      }
    }

    /// <summary>
    /// Gets or sets Param2.
    /// </summary>
    public string Param2
    {
      get
      {
        return this._param2;
      }

      set
      {
        this._param2 = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on load.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      // localize ErrorMessage, ToolTip
      this.ErrorMessage = this.Localize(this);
      this.ToolTip = this.Localize(this);
    }

    #endregion
  }
}