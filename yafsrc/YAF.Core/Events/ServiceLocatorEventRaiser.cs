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
namespace YAF.Core.Events
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;

    #endregion

    /// <summary>
    /// The service locator event raiser.
    /// </summary>
    public class ServiceLocatorEventRaiser : IRaiseEvent
    {
        #region Fields

        /// <summary>
        ///     The _service locator.
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorEventRaiser"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service Locator.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ServiceLocatorEventRaiser([NotNull] IServiceLocator serviceLocator, ILogger logger)
        {
            this.Logger = logger;
            this._serviceLocator = serviceLocator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The event raiser.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public void Raise<T>(T eventObject) where T : IAmEvent
        {
            this.GetAggregatedAndOrderedEventHandlers<T>().ForEach(x => x.Handle(eventObject));
        }

        /// <summary>
        /// Raise all events using try/catch block.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="eventObject">
        /// </param>
        /// <param name="logExceptionAction">
        /// </param>
        public void RaiseIssolated<T>(T eventObject, [CanBeNull] Action<string, Exception> logExceptionAction)
            where T : IAmEvent
        {
            this.GetAggregatedAndOrderedEventHandlers<T>().ForEach(
                theHandler =>
                {
                    try
                    {
                        theHandler.Handle(eventObject);
                    }
                    catch (Exception ex)
                    {
                        if (logExceptionAction != null)
                        {
                            logExceptionAction(theHandler.GetType().Name, ex);
                        }
                        else
                        {
                            this.Logger.Error(ex, "Exception Raising Event to Handler: {0}", theHandler.GetType().Name);
                        }
                    }
                });
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get event handlers aggregated and ordered.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        private IList<IHandleEvent<T>> GetAggregatedAndOrderedEventHandlers<T>() where T : IAmEvent
        {
            return this._serviceLocator.Get<IEnumerable<IHandleEvent<T>>>()
                       .Concat(this._serviceLocator.Get<IEnumerable<IFireEvent<T>>>())
                       .OrderBy(x => x.Order)
                       .ToList();
        }

        #endregion
    }
}