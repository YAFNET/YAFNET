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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web.UI;

  using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// Summary description for BaseControl.
  /// </summary>
  public class BaseControl : Control, IRaiseControlLifeCycles, IHaveServiceLocator, IHaveLocalization
  {
    #region Constants and Fields

    /// <summary>
    ///   The _localization.
    /// </summary>
    private ILocalization _localization;

    /// <summary>
    /// The _logger.
    /// </summary>
    private ILogger _logger;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "BaseControl" /> class.
    /// </summary>
    public BaseControl()
    {
      this.Get<IInjectServices>().Inject(this);
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization
    {
      get
      {
        return this._localization ?? (this._localization = this.Get<ILocalization>());
      }
    }

    /// <summary>
    ///   Gets or sets Logger.
    /// </summary>
    public ILogger Logger
    {
      get
      {
        return this._logger ?? (this._logger = this.Get<ILoggerProvider>().Create(this.GetType()));
      }
    }

    /// <summary>
    ///   Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        return YafContext.Current;
      }
    }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator
    {
      get
      {
        return this.PageContext.ServiceLocator;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IRaiseControlLifeCycles

    /// <summary>
    /// The raise init.
    /// </summary>
    void IRaiseControlLifeCycles.RaiseInit()
    {
      this.OnInit(new EventArgs());
    }

    /// <summary>
    /// The raise load.
    /// </summary>
    void IRaiseControlLifeCycles.RaiseLoad()
    {
      this.OnLoad(new EventArgs());
    }

    /// <summary>
    /// The raise pre render.
    /// </summary>
    void IRaiseControlLifeCycles.RaisePreRender()
    {
      this.OnPreRender(new EventArgs());
    }

    #endregion

    #endregion
  }
}