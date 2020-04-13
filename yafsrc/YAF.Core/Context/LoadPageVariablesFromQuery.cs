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
    using System.Web;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;

    /// <summary>
    /// The load page variables from query.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
    public class LoadPageVariablesFromQuery : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPageVariablesFromQuery"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public LoadPageVariablesFromQuery([NotNull] IServiceLocator serviceLocator)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Order.
        /// </summary>
        public int Order => 10;

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Implemented Interfaces

        #region IHandleEvent<InitPageLoadEvent>

        /// <summary>
        /// Handles the specified @event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void Handle([NotNull] InitPageLoadEvent @event)
        {
            var queryString = this.Get<HttpRequestBase>().QueryString;

            @event.Data.CategoryID = queryString.GetFirstOrDefault("c").ToTypeOrDefault(0);
            @event.Data.ForumID = queryString.GetFirstOrDefault("f").ToTypeOrDefault(0);
            @event.Data.TopicID = queryString.GetFirstOrDefault("t").ToTypeOrDefault(0);
            @event.Data.MessageID = queryString.GetFirstOrDefault("m").ToTypeOrDefault(0);

            if (BoardContext.Current.Settings.CategoryID != 0)
            {
                @event.Data.CategoryID = BoardContext.Current.Settings.CategoryID;
            }
        }

        #endregion

        #endregion
    }
}