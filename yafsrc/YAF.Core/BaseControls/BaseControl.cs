﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.BaseControls;

using System;
using System.Web.UI;

/// <summary>
/// The base control.
/// </summary>
public class BaseControl : Control, IRaiseControlLifeCycles, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    /// The _logger.
    /// </summary>
    private ILoggerService logger;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "BaseControl" /> class.
    /// </summary>
    public BaseControl()
    {
        this.Get<IInjectServices>().Inject(this);
    }

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => this.localization ??= this.Get<ILocalization>();

    /// <summary>
    ///   Gets or sets Logger.
    /// </summary>
    public ILoggerService Logger => this.logger ??= this.Get<ILoggerProvider>().Create(this.GetType());

    /// <summary>
    ///   Gets PageBoardContext.
    /// </summary>
    public BoardContext PageBoardContext => BoardContext.Current;

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => this.PageBoardContext.ServiceLocator;

    /// <summary>
    /// The raise init.
    /// </summary>
    void IRaiseControlLifeCycles.RaiseInit()
    {
        this.OnInit(EventArgs.Empty);
    }

    /// <summary>
    /// The raise load.
    /// </summary>
    void IRaiseControlLifeCycles.RaiseLoad()
    {
        this.OnLoad(EventArgs.Empty);
    }

    /// <summary>
    /// The raise pre render.
    /// </summary>
    void IRaiseControlLifeCycles.RaisePreRender()
    {
        this.OnPreRender(EventArgs.Empty);
    }
}