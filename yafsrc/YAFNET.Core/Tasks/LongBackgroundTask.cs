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

namespace YAF.Core.Tasks;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The long background task.
/// </summary>
public class LongBackgroundTask : IntermittentBackgroundTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LongBackgroundTask"/> class.
    /// </summary>
    public LongBackgroundTask()
    {
        this.StartDelayMs = 50;
        this.RunPeriodMs = Timeout.Infinite;
    }

    /// <summary>
    /// Executes the task.
    /// </summary>
    public override void ExecuteTask()
    {
        // Run this item once in the background, without blocking the calling (request) thread,
        // but only report IsRunning as false once the work has actually finished -- otherwise
        // callers relying on IsRunning to avoid overlapping runs (e.g. ForumDeleteTask) would see
        // the task as "done" while it is still executing.
        _ = Task.Run(
            async () =>
                {
                    try
                    {
                        await this.RunOnceAsync();
                    }
                    finally
                    {
                        this.IsRunning = false;
                    }
                });
    }
}