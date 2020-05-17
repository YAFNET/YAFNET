/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Context
{
    #region Using

    using System;
    using System.Web;
    
    using Autofac;

    using YAF.Configuration;
    using YAF.Configuration.Pattern;
    using YAF.Core.BasePages;
    using YAF.Core.Helpers;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Identity;
    using YAF.Utils.Helpers;

    using AspNetUsers = YAF.Types.Models.Identity.AspNetUsers;

    #endregion

    /// <summary>
    /// Context class that accessible with the same instance from all locations
    /// </summary>
    public class BoardContext : UserPageBase, IDisposable, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        /// The context lifetime container.
        /// </summary>
        private readonly ILifetimeScope contextLifetimeContainer;

        /// <summary>
        /// The user.
        /// </summary>
        private AspNetUsers user;

        /// <summary>
        /// The combined user data.
        /// </summary>
        private CombinedUserDataHelper combinedUserData;

        /// <summary>
        /// The load message.
        /// </summary>
        private LoadMessage loadMessage;

        /// <summary>
        /// The page elements.
        /// </summary>
        private PageElementRegister pageElements;

        #endregion

        #region Constructors and Destructors

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

            // init context...
            this.Init?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The after init.
        /// </summary>
        public event EventHandler<EventArgs> AfterInit;

        /// <summary>
        /// The before init.
        /// </summary>
        public event EventHandler<EventArgs> BeforeInit;

        /// <summary>
        /// On BoardContext Constructor Call
        /// </summary>
        public event EventHandler<EventArgs> Init;

        /// <summary>
        /// On BoardContext Unload Call
        /// </summary>
        public event EventHandler<EventArgs> Unload;

        #endregion

        #region Public Properties

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
        /// Gets the Instance of the Combined UserData for the current user.
        /// </summary>
        public IUserData CurrentUserData =>
            this.combinedUserData ?? (this.combinedUserData = new CombinedUserDataHelper());

        /// <summary>
        /// Gets the current page as the forumPage Enumerator (for comparison)
        /// </summary>
        public ForumPages ForumPageType
        {
            get
            {
                if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g").IsNotSet())
                {
                    return ForumPages.Board;
                }

                try
                {
                    return this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g").ToEnum<ForumPages>(true);
                }
                catch (Exception)
                {
                    return ForumPages.Board;
                }
            }
        }

        /// <summary>
        /// Gets the Access to the Context Global Variable Repository Class which is a helper class that accesses BoardContext.Vars with strongly typed properties for primary variables.
        /// </summary>
        public ContextVariableRepository Globals { get; }

        /// <summary>
        /// Gets the current Page Load Message
        /// </summary>
        public LoadMessage LoadMessage => this.loadMessage ?? (this.loadMessage = new LoadMessage());

        /// <summary>
        /// Gets the Current Page Elements
        /// </summary>
        public PageElementRegister PageElements => this.pageElements ?? (this.pageElements = new PageElementRegister());

        /// <summary>
        /// Gets or sets the Current Page Query ID Helper
        /// </summary>
        public QueryStringIDHelper QueryIDs { get; set; }

        /// <summary>
        /// Gets the Provides access to the Service Locator
        /// </summary>
        public IServiceLocator ServiceLocator => this.contextLifetimeContainer.Resolve<IServiceLocator>();

        /// <summary>
        /// Gets the Current Page Control Settings from Forum Control
        /// </summary>
        public ControlSettings Settings => ControlSettings.Current;

        /// <summary>
        /// Gets the UrlBuilder
        /// </summary>
        public IUrlBuilder UrlBuilder => FactoryProvider.UrlBuilder;

        /// <summary>
        /// Gets or sets the Current Membership User
        /// </summary>
        public AspNetUsers User
        {
            get => this.user ?? (this.user = this.Get<IAspNetUsersHelper>().GetUser());

            set => this.user = value;
        }

        /// <summary>
        /// Gets the YAF Context Global Instance Variables Use for plugins or other situations where a value is needed per instance.
        /// </summary>
        public TypeDictionary Vars { get; } = new TypeDictionary();

        #endregion

        #region Public Indexers

        /// <summary>
        /// Returns a value from the BoardContext Global Instance Variables (Vars) collection.
        /// </summary>
        /// <returns>
        /// Value if it's found, null if it doesn't exist.
        /// </returns>
        public object this[[NotNull] string varName]
        {
            get => this.Vars.ContainsKey(varName) ? this.Vars[varName] : null;

            set => this.Vars[varName] = value;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Helper Function that adds a "load message" to the load message class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        public void AddLoadMessage([NotNull] string message, MessageTypes messageType)
        {
            this.LoadMessage.Add(message, messageType);
        }

        /// <summary>
        /// Helper Function that adds a "load message" to the load message class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        public void AddLoadMessage([NotNull] string message, string script, MessageTypes messageType)
        {
            this.LoadMessage.Add(message, messageType, script);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Unload?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the user data and page data...
        /// </summary>
        protected override void InitUserAndPage()
        {
            if (this.InitUserPage)
            {
                return;
            }

            this.BeforeInit?.Invoke(this, new EventArgs());

            if (this.User != null && (this.Get<HttpSessionStateBase>()["UserUpdated"] == null
                                      || this.Get<HttpSessionStateBase>()["UserUpdated"].ToString()
                                      != this.User.UserName))
            {
                AspNetRolesHelper.UpdateForumUser(this.User, this.PageBoardID);
                this.Get<HttpSessionStateBase>()["UserUpdated"] = this.User.UserName;
            }

            var pageLoadEvent = new InitPageLoadEvent();

            this.Get<IRaiseEvent>().Raise(pageLoadEvent);

            this.Page = pageLoadEvent.DataDictionary;

            this.AfterInit?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}