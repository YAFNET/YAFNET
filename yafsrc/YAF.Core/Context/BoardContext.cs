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

namespace YAF.Core.Context;

using System;

using Autofac;

using YAF.Configuration.Pattern;
using YAF.Core.BasePages;
using YAF.Types.Constants;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

using AspNetUsers = YAF.Types.Models.Identity.AspNetUsers;

/// <summary>
/// Context class that accessible with the same instance from all locations
/// </summary>
public sealed class BoardContext : UserPageBase, IDisposable, IHaveServiceLocator
{
    /// <summary>
    /// The context lifetime container.
    /// </summary>
    private readonly ILifetimeScope contextLifetimeContainer;

    /// <summary>
    /// The user.
    /// </summary>
    private AspNetUsers membershipUser;

    /// <summary>
    /// The load message.
    /// </summary>
    private LoadMessage loadMessage;

    /// <summary>
    /// The page elements.
    /// </summary>
    private PageElementRegister pageElements;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardContext"/> class. BoardContext Constructor
    /// </summary>
    /// <param name="contextLifetimeContainer">
    /// The context Lifetime Container.
    /// </param>
    internal BoardContext(ILifetimeScope contextLifetimeContainer)
    {
        this.contextLifetimeContainer = contextLifetimeContainer;

        // init the repository
        this.Globals = new ContextVariableRepository(this.Vars);
    }

    /// <summary>
    /// The after init.
    /// </summary>
    public event EventHandler<EventArgs> AfterInit;

    /// <summary>
    /// The before init.
    /// </summary>
    public event EventHandler<EventArgs> BeforeInit;

    /// <summary>
    /// On BoardContext Unload Call
    /// </summary>
    public event EventHandler<EventArgs> Unload;

    /// <summary>
    /// Gets the instance of the Forum Context
    /// </summary>
    public static BoardContext Current => GlobalContainer.Container.Resolve<BoardContext>();

    /// <summary>
    /// Gets or sets the Current Board Settings
    /// </summary>
    public BoardSettings BoardSettings
    {
        get => this.Get<BoardSettings>();

        set => this.Get<CurrentBoardSettings>().Instance = value;
    }

    /// <summary>
    /// Gets or sets the Forum page instance of the current forum page. May not be valid until everything is initialized.
    /// </summary>
    public ForumPage CurrentForumPage { get; set; }

    /// <summary>
    /// Gets the Access to the Context Global Variable Repository Class which is a helper class that accesses BoardContext.Vars with strongly typed properties for primary variables.
    /// </summary>
    public ContextVariableRepository Globals { get; }

    /// <summary>
    /// Gets the current Page Load Message
    /// </summary>
    public LoadMessage LoadMessage => this.loadMessage ??= new LoadMessage();

    /// <summary>
    /// Gets the Current Page Elements
    /// </summary>
    public PageElementRegister PageElements => this.pageElements ??= new PageElementRegister();

    /// <summary>
    /// Gets the Provides access to the Service Locator
    /// </summary>
    public IServiceLocator ServiceLocator => this.contextLifetimeContainer.Resolve<IServiceLocator>();

    /// <summary>
    /// Gets the Current Page Control Settings from Forum Control
    /// </summary>
    public ControlSettings Settings => this.Get<ControlSettings>();

    /// <summary>
    /// Gets or sets the Current Membership PageUser
    /// </summary>
    public AspNetUsers MembershipUser => this.membershipUser ??= this.Get<IAspNetUsersHelper>().GetUser();

    /// <summary>
    ///   Gets the current YAF PageUser.
    /// </summary>
    public User PageUser => this.PageData.Item2.Item2;

    /// <summary>
    /// Returns if user is Host User or an Admin of one or more forums.
    /// </summary>
    public bool IsAdmin => this.PageUser.UserFlags.IsHostAdmin || Current.IsForumAdmin;

    /// <summary>
    /// Gets the YAF Context Global Instance Variables Use for plugins or other situations where a value is needed per instance.
    /// </summary>
    public TypeDictionary Vars { get; } = [];

    /// <summary>
    /// Returns a value from the BoardContext Global Instance Variables (Vars) collection.
    /// </summary>
    /// <returns>
    /// Value if it's found, null if it doesn't exist.
    /// </returns>
    public object this[string varName]
    {
        get => this.Vars.ContainsKey(varName) ? this.Vars[varName] : null;

        set => this.Vars[varName] = value;
    }

    /// <summary>
    /// Helper Function that adds a "load message" to the load message class.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageType">
    /// The message type.
    /// </param>
    public void Notify(string message, MessageTypes messageType)
    {
        this.LoadMessage.Add(message, messageType);
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
        this.Unload?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Initialize the user data and page data...
    /// </summary>
    override protected void InitUserAndPage()
    {
        if (this.UserPageDataLoaded)
        {
            return;
        }

        this.PageLinks = [];

        this.BeforeInit?.Invoke(this, EventArgs.Empty);

        var pageLoadEvent = new InitPageLoadEvent();

        this.Get<IRaiseEvent>().Raise(pageLoadEvent);

        this.PageData = pageLoadEvent.PageData;

        this.AfterInit?.Invoke(this, EventArgs.Empty);
    }
}