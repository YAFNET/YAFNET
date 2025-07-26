/* Yet Another Forum.NET
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

using System.Collections.Generic;


namespace YAF.Core.Context;

using System;

using Microsoft.AspNetCore.Mvc.RazorPages;

using YAF.Configuration.Pattern;
using YAF.Core.BasePages;
using YAF.Types.Models;

/// <summary>
/// Context class that accessible with the same instance from all locations
/// </summary>
public class BoardContext : UserPageBase, IDisposable, IHaveServiceLocator
{
    /// <summary>
    /// The context lifetime container.
    /// </summary>
    private readonly ILifetimeScope contextLifetimeContainer;

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
    public static BoardContext Current => GlobalContainer.AutoFacContainer.Resolve<BoardContext>();

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
    /// Gets or sets the current forum controller.
    /// </summary>
    /// <value>The current forum controller.</value>
    public ForumBaseController CurrentForumController { get; set; }

    /// <summary>
    /// Gets the Access to the Context Global Variable Repository Class which is a helper class that accesses BoardContext.Vars with strongly typed properties for primary variables.
    /// </summary>
    public ContextVariableRepository Globals { get; }

    /// <summary>
    /// Gets the current Page Load Message
    /// </summary>
    public SessionMessageService SessionMessageService => field ??= new SessionMessageService();

    /// <summary>
    /// Gets the inline elements.
    /// </summary>
    /// <value>The inline elements.</value>
    public InlineElements InlineElements => field ??= new InlineElements();

    /// <summary>
    /// Gets the Provides access to the Service Locator
    /// </summary>
    public IServiceLocator ServiceLocator => this.contextLifetimeContainer.Resolve<IServiceLocator>();

    /// <summary>
    /// Gets the Current Page Control Settings from Forum Control
    /// </summary>
    public ControlSettings Settings => this.Get<ControlSettings>();

    /// <summary>
    /// Gets or sets the Current Membership User
    /// </summary>
    public AspNetUsers MembershipUser { get; set; }

    /// <summary>
    ///   Gets the current YAF PageUser.
    /// </summary>
    public User PageUser => this.PageData.Item2.Item2;

    /// <summary>
    /// Returns if user is Host PageUser or an Admin of one or more forums.
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
        get => this.Vars.GetValueOrDefault(varName);

        set => this.Vars[varName] = value;
    }

    /// <summary>
    /// Registers the JavaScript block.
    /// </summary>
    /// <param name="block">The block.</param>
    /// <returns>PageResult.</returns>
    public PageResult RegisterJsBlock(string block)
    {
        return this.CurrentForumPage.RegisterJsBlock(block).Page();
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
    public PageResult Notify(string message, MessageTypes messageType)
    {
        return this.CurrentForumPage.ToastMessage(messageType.ToString(), message).Page();
    }

    /// <summary>
    /// Add Notify message to session
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageType">Type of the message.</param>
    public void SessionNotify(string message, MessageTypes messageType)
    {
        this.SessionMessageService.AddSession(message, messageType);
    }

    /// <summary>
    /// Add Notify message to session
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageType">Type of the message.</param>
    /// <param name="redirectPage">the Page to redirect to.</param>
    public IActionResult SessionNotify(string message, MessageTypes messageType, ForumPages redirectPage)
    {
        this.SessionMessageService.AddSession(message, messageType);

        return this.CurrentForumPage.Get<ILinkBuilder>().Redirect(redirectPage);
    }

    /// <summary>
    /// Show Confirm Modal
    /// </summary>
    public void ShowConfirmModal(string title, string text,
                                 string yes,
                                 string no,
                                 string link)
    {
        this.CurrentForumPage.ConfirmModal(title, text, yes, no, link).Page();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
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

        this.MembershipUser = this.Get<IAspNetUsersHelper>().GetUser();

        this.PageLinks = [];

        this.BeforeInit?.Invoke(this, EventArgs.Empty);

        var pageLoadEvent = new InitPageLoadEvent();

        this.Get<IRaiseEvent>().Raise(pageLoadEvent);

        this.PageData = pageLoadEvent.PageData;

        this.AfterInit?.Invoke(this, EventArgs.Empty);
    }
}