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

namespace YAF.Core.BaseModules;

using YAF.Types.Attributes;

/// <summary>
/// Stop watch start/stop
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerScope)]
public class StartStopWatch : IHandleEvent<ForumPageInitEvent>
{
    /// <summary>
    /// The _stop watch.
    /// </summary>
    private readonly IStopWatch stopWatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartStopWatch"/> class.
    /// </summary>
    /// <param name="stopWatch">
    /// The stop watch.
    /// </param>
    public StartStopWatch(IStopWatch stopWatch)
    {
        this.stopWatch = stopWatch;
    }

    /// <summary>
    /// Gets Order.
    /// </summary>
    public int Order => 1000;

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    ///     The event.
    /// </param>
    public void Handle(ForumPageInitEvent @event)
    {
        // start the stop watch on init...
        this.stopWatch.Start();
    }
}