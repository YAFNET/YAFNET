/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using System.Threading.Tasks;

namespace YAF.Core.Events;

using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

/// <summary>
/// The service locator async event raiser.
/// </summary>
public class ServiceLocatorAsyncEventRaiser : IRaiseEventAsync
{
    /// <summary>
    ///     The _service locator.
    /// </summary>
    private readonly IServiceLocator _serviceLocator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLocatorAsyncEventRaiser"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service Locator.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public ServiceLocatorAsyncEventRaiser(IServiceLocator serviceLocator, ILogger<ServiceLocatorAsyncEventRaiser> logger)
    {
        this.Logger = logger;
        this._serviceLocator = serviceLocator;
    }

    /// <summary>
    ///     Gets or sets the logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// The event raiser.
    /// </summary>
    /// <param name="eventObject">
    ///     The event object.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public async Task RaiseAsync<T>(T eventObject) where T : IAmEventAsync
    {
        foreach (var x in this.GetAggregatedAndOrderedAsyncEventHandlers<T>())
        {
            await x.HandleAsync(eventObject);
        }
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
    public async Task RaiseIsolatedAsync<T>(T eventObject, Action<string, Exception> logExceptionAction) where T : IAmEventAsync
    {
        foreach (var theAsyncHandler in this.GetAggregatedAndOrderedAsyncEventHandlers<T>())
        {
            await theAsyncHandler.HandleAsync(eventObject);

            try
            {
                await theAsyncHandler.HandleAsync(eventObject);
            }
            catch (Exception ex)
            {
                if (logExceptionAction != null)
                {
                    logExceptionAction(theAsyncHandler.GetType().Name, ex);
                }
                else
                {
                    this.Logger.Error(ex, $"Exception Raising Event to Handler: {theAsyncHandler.GetType().Name}");
                }
            }
        }
    }

    /// <summary>
    ///     The get event handlers aggregated and ordered.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    ///     The <see cref="IList" />.
    /// </returns>
    private IList<IHandleEventAsync<T>> GetAggregatedAndOrderedAsyncEventHandlers<T>() where T : IAmEventAsync
    {
        return [.. this._serviceLocator.Get<IEnumerable<IHandleEventAsync<T>>>()
            .Concat(this._serviceLocator.Get<IEnumerable<IFireEventAsync<T>>>())
            .OrderBy(x => x.Order)];
    }
}