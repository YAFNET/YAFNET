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
namespace YAF.Core.Helpers
{
    #region Using

    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The current board settings.
    /// </summary>
    public class CurrentBoardSettings : IReadWriteProvider<BoardSettings>
    {
        #region Fields

        /// <summary>
        ///     The application state base.
        /// </summary>
        private readonly HttpApplicationStateBase applicationStateBase;

        /// <summary>
        ///     The have board id.
        /// </summary>
        private readonly IHaveBoardID haveBoardId;

        /// <summary>
        ///     The inject services.
        /// </summary>
        private readonly IInjectServices injectServices;

        /// <summary>
        ///     The treat cache key.
        /// </summary>
        private readonly ITreatCacheKey treatCacheKey;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentBoardSettings"/> class.
        /// </summary>
        /// <param name="applicationStateBase">
        /// The application state base.
        /// </param>
        /// <param name="injectServices">
        /// The inject services.
        /// </param>
        /// <param name="haveBoardId">
        /// The have board id.
        /// </param>
        /// <param name="treatCacheKey">
        /// The treat Cache Key.
        /// </param>
        public CurrentBoardSettings(
            [NotNull] HttpApplicationStateBase applicationStateBase,
            [NotNull] IInjectServices injectServices,
            [NotNull] IHaveBoardID haveBoardId,
            [NotNull] ITreatCacheKey treatCacheKey)
        {
            CodeContracts.VerifyNotNull(applicationStateBase, "applicationStateBase");
            CodeContracts.VerifyNotNull(injectServices, "injectServices");
            CodeContracts.VerifyNotNull(haveBoardId, "haveBoardId");
            CodeContracts.VerifyNotNull(treatCacheKey, "treatCacheKey");

            this.applicationStateBase = applicationStateBase;
            this.injectServices = injectServices;
            this.haveBoardId = haveBoardId;
            this.treatCacheKey = treatCacheKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets Object.
        /// </summary>
        public BoardSettings Instance
        {
            get
            {
                return this.applicationStateBase.GetOrSet(
                    this.treatCacheKey.Treat(Constants.Cache.BoardSettings),
                    () =>
                        {
                            var boardSettings = (BoardSettings)new LoadBoardSettings(this.haveBoardId.BoardID);

                            // inject
                            this.injectServices.Inject(boardSettings);

                            return boardSettings;
                        });
            }

            set => this.applicationStateBase.Set(this.treatCacheKey.Treat(Constants.Cache.BoardSettings), value);
        }

        #endregion
    }
}