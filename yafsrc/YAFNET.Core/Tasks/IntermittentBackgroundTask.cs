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

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The intermittent background task.
/// </summary>
public class IntermittentBackgroundTask : BaseBackgroundTask
{
    /// <summary>
    /// Gets or sets Start Delay.
    /// </summary>
    public long StartDelayMs { get; set; }

    /// <summary>
    /// Gets or sets Run Period.
    /// </summary>
    public long RunPeriodMs { get; set; }

    /// <summary>
    /// The run once.
    /// </summary>
    public override Task RunOnceAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// The run.
    /// </summary>
    public override void Run()
    {
        if (this.IsRunning)
        {
            return;
        }

        // we're running this thread now...
        this.IsRunning = true;

        this.ExecuteTask();
    }

    /// <summary>
    /// Executes the task.
    /// </summary>
    public virtual void ExecuteTask()
    {
        StartInBackground(async () => {
            // both values are milliseconds: wait before the first run, then between the runs
            await Task.Delay(TimeSpan.FromMilliseconds(Math.Max(0, this.StartDelayMs)));

            // a disposed task is no longer running and stops
            while (this.IsRunning)
            {
                try
                {
                    await this.RunOnceAsync();
                }
                catch (Exception ex)
                {
                    // a failed run must not stop the task for good
                    this.Logger?.Error(ex, $"Error In {this.TaskName} Task");
                }

                // no period (e.g. Timeout.Infinite) means the task runs only once
                if (this.RunPeriodMs <= 0)
                {
                    this.IsRunning = false;
                    return;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(this.RunPeriodMs));
            }
        });
    }

    /// <summary>
    /// Runs the work on the thread pool without the execution context of the caller: a task started by a
    /// request would see the HttpContext of that request (IHttpContextAccessor) and could create or replace
    /// the BoardContext of the request while the page uses it.
    /// </summary>
    /// <param name="work">The work.</param>
    /// <returns>The task of the work.</returns>
    protected static Task StartInBackground(Func<Task> work)
    {
        using (ExecutionContext.SuppressFlow())
        {
            return Task.Run(work);
        }
    }
}