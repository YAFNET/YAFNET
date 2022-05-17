/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
using System.Web.UI.WebControls;

/// <summary>
/// Control derived from Panel that includes a reference to the <see cref="BoardContext"/>.
/// </summary>
public class BasePanel : Panel, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    /// The logger.
    /// </summary>
    private ILoggerService logger;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "BasePanel" /> class.
    /// </summary>
    public BasePanel()
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
}