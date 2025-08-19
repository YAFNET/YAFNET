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

namespace YAF.Core.Context;

using YAF.Types.Attributes;

/// <summary>
/// The load page lazy user data.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
public class LoadPageLazyUserData : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
{
    /// <summary>
    ///   The data broker.
    /// </summary>
    private readonly DataBroker dataBroker;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadPageLazyUserData"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="dataBroker">
    /// The data Broker.
    /// </param>
    public LoadPageLazyUserData(IServiceLocator serviceLocator, DataBroker dataBroker)
    {
        this.dataBroker = dataBroker;
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 3000;

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Handles the specified @event.
    /// </summary>
    /// <param name="event">The @event.</param>
    public void Handle(InitPageLoadEvent @event)
    {
        var activeUserLazyData = this.dataBroker.ActiveUserLazyDataAsync(@event.PageLoadData.Item1.UserID).Result;

        if (activeUserLazyData != null)
        {
            // add the lazy user data to this page data...
            @event.UserLazyData = activeUserLazyData;
        }
    }
}