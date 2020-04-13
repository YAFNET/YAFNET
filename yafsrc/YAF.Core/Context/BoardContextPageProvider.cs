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

    using System.Web;

    using Autofac;

    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The board context page provider.
    /// </summary>
    internal class BoardContextPageProvider : IReadOnlyProvider<BoardContext>
    {
        #region Constants and Fields

        /// <summary>
        /// The page yaf context name.
        /// </summary>
        private const string PageBoardContextName = "YAF.BoardContext";

        /// <summary>
        /// The _container.
        /// </summary>
        private readonly ILifetimeScope _lifetimeScope;

        private readonly IInjectServices _injectServices;

        /// <summary>
        /// The _global instance.
        /// </summary>
        private static BoardContext _globalInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardContextPageProvider"/> class.
        /// </summary>
        /// <param name="lifetimeScope">
        /// The container.
        /// </param>
        /// <param name="injectServices">
        /// The inject Services.
        /// </param>
        public BoardContextPageProvider(ILifetimeScope lifetimeScope, IInjectServices injectServices)
        {
            this._lifetimeScope = lifetimeScope;
            this._injectServices = injectServices;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Instance.
        /// </summary>
        public BoardContext Instance
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return _globalInstance ?? (_globalInstance = this.CreateContextInstance());
                }

                if (HttpContext.Current.Items[PageBoardContextName] is BoardContext pageInstance)
                {
                    return pageInstance;
                }

                pageInstance = this.CreateContextInstance();

                // make sure it's put back in the page...
                HttpContext.Current.Items[PageBoardContextName] = pageInstance;

                return pageInstance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create context instance.
        /// </summary>
        /// <returns>
        /// The <see cref="BoardContext"/>.
        /// </returns>
        private BoardContext CreateContextInstance()
        {
            var lifetimeContainer = this._lifetimeScope.BeginLifetimeScope(LifetimeScope.Context);

            var instance = new BoardContext(lifetimeContainer);
            this._injectServices.Inject(instance);

            return instance;
        }

        #endregion
    }
}