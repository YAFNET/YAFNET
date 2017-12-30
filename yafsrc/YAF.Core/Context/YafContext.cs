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
    using System.Web;
    using System.Web.Security;

    using Autofac;

    using YAF.Classes;
    using YAF.Classes.Pattern;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Context class that accessible with the same instance from all locations
    /// </summary>
    public class YafContext : UserPageBase, IDisposable, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        /// The _context lifetime container.
        /// </summary>
        protected ILifetimeScope _contextLifetimeContainer;

        /// <summary>
        /// The _current forum page.
        /// </summary>
        protected ForumPage _currentForumPage;

        /// <summary>
        /// The _repository.
        /// </summary>
        protected ContextVariableRepository _repository;

        /// <summary>
        /// The _user.
        /// </summary>
        protected MembershipUser _user;

        /// <summary>
        /// The _variables.
        /// </summary>
        protected TypeDictionary _variables = new TypeDictionary();

        /// <summary>
        /// The _combined user data.
        /// </summary>
        private CombinedUserDataHelper _combinedUserData;

        /// <summary>
        /// The _load message.
        /// </summary>
        private LoadMessage _loadMessage;

        /// <summary>
        /// The _page elements.
        /// </summary>
        private PageElementRegister _pageElements;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafContext"/> class. YafContext Constructor
        /// </summary>
        /// <param name="contextLifetimeContainer">
        /// The context Lifetime Container.
        /// </param>
        internal YafContext(ILifetimeScope contextLifetimeContainer)
        {
            this._contextLifetimeContainer = contextLifetimeContainer;

            // init the respository
            this._repository = new ContextVariableRepository(this._variables);

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
        /// On YafContext Constructor Call
        /// </summary>
        public event EventHandler<EventArgs> Init;

        /// <summary>
        /// On YafContext Unload Call
        /// </summary>
        public event EventHandler<EventArgs> Unload;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance of the Forum Context
        /// </summary>
        public static YafContext Current => GlobalContainer.Container.Resolve<YafContext>();

        /// <summary>
        /// Gets or sets the Current Board Settings
        /// </summary>
        public YafBoardSettings BoardSettings
        {
            get
            {
                return this.Get<YafBoardSettings>();
            }

            set
            {
                this.Get<CurrentBoardSettings>().Instance = value;
            }
        }

        /// <summary>
        /// Gets or sets the Forum page instance of the current forum page. May not be valid until everything is initialized.
        /// </summary>
        public ForumPage CurrentForumPage
        {
            get
            {
                return this._currentForumPage;
            }

            set
            {
                this._currentForumPage = value;
            }
        }

        /// <summary>
        /// Gets the Instance of the Combined UserData for the current user.
        /// </summary>
        public IUserData CurrentUserData => this._combinedUserData ?? (this._combinedUserData = new CombinedUserDataHelper());

        /// <summary>
        /// Gets the current page as the forumPage Enum (for comparison)
        /// </summary>
        public ForumPages ForumPageType
        {
            get
            {
                if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g").IsNotSet())
                {
                    return ForumPages.forum;
                }

                try
                {
                    return this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g").ToEnum<ForumPages>(true);
                }
                catch (Exception)
                {
                    return ForumPages.forum;
                }
            }
        }

        /// <summary>
        /// Gets the Access to the Context Global Variable Repository Class which is a helper class that accesses YafContext.Vars with strongly typed properties for primary variables.
        /// </summary>
        public ContextVariableRepository Globals => this._repository;

        /// <summary>
        /// Gets the current Page Load Message
        /// </summary>
        public LoadMessage LoadMessage => this._loadMessage ?? (this._loadMessage = new LoadMessage());

        /// <summary>
        /// Gets the Current Page Elements
        /// </summary>
        public PageElementRegister PageElements => this._pageElements ?? (this._pageElements = new PageElementRegister());

        /// <summary>
        /// Gets the Current Page User Profile
        /// </summary>
        public YafUserProfile Profile => (YafUserProfile)HttpContext.Current.Profile;

        /// <summary>
        /// Gets or sets the Current Page Query ID Helper
        /// </summary>
        public QueryStringIDHelper QueryIDs { get; set; }

        /// <summary>
        /// Gets the Provides access to the Service Locator
        /// </summary>
        public IServiceLocator ServiceLocator => this._contextLifetimeContainer.Resolve<IServiceLocator>();

        /// <summary>
        /// Gets the Current Page Control Settings from Forum Control
        /// </summary>
        public YafControlSettings Settings => YafControlSettings.Current;

        /// <summary>
        /// Gets the UrlBuilder
        /// </summary>
        public IUrlBuilder UrlBuilder => YafFactoryProvider.UrlBuilder;

        /// <summary>
        /// Gets or sets the Current Membership User
        /// </summary>
        public MembershipUser User
        {
            get
            {
                return this._user ?? (this._user = UserMembershipHelper.GetUser(true));
            }

            set
            {
                this._user = value;
            }
        }

        /// <summary>
        /// Gets the YafContext Global Instance Variables Use for plugins or other situations where a value is needed per instance.
        /// </summary>
        public TypeDictionary Vars => this._variables;

        #endregion

        #region Public Indexers

        /// <summary>
        /// Returns a value from the YafContext Global Instance Variables (Vars) collection.
        /// </summary>
        /// <returns>
        /// Value if it's found, null if it doesn't exist.
        /// </returns>
        public object this[[NotNull] string varName]
        {
            get
            {
                return this._variables.ContainsKey(varName) ? this._variables[varName] : null;
            }

            set
            {
                this._variables[varName] = value;
            }
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
        public void AddLoadMessage([NotNull] string message, MessageTypes messageType = MessageTypes.info)
        {
            this.LoadMessage.Add(message, messageType);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Unload?.Invoke(this, new EventArgs());

            // No need to dispose the container here
            //this._contextLifetimeContainer.Dispose();
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

            if (this.User != null
                && (this.Get<HttpSessionStateBase>()["UserUpdated"] == null
                    || this.Get<HttpSessionStateBase>()["UserUpdated"].ToString() != this.User.UserName))
            {
                RoleMembershipHelper.UpdateForumUser(this.User, this.PageBoardID);
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