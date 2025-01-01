﻿/* Yet Another Forum.NET
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
namespace YAF.Core.Tasks;

using System.Security.Principal;
using System.Threading;

/// <summary>
/// The intermittent background task.
/// </summary>
public class IntermittentBackgroundTask : BaseBackgroundTask
{
    /// <summary>
    /// The primary thread identity
    /// </summary>
    private WindowsIdentity primaryThreadIdentity;

    /// <summary>
    /// Gets or sets the intermittent timer.
    /// </summary>
    public Timer intermittentTimer { get; set; }

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
    public override void RunOnce()
    {
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

        // keep the context...
        this.primaryThreadIdentity = WindowsIdentity.GetCurrent();

        // we're running this thread now...
        this.IsRunning = true;

        this.Logger.Debug($"Starting Background Task {this.GetType().Name} Now");

        // create the timer...);
        this.intermittentTimer = new Timer(this.TimerCallback, null, this.StartDelayMs, this.RunPeriodMs);
    }

    /// <summary>
    /// The timer callback.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    protected virtual void TimerCallback(object sender)
    {
        if (!Monitor.TryEnter(this))
        {
            return;
        }

        WindowsImpersonationContext impersonationContext = null;

        if (this.primaryThreadIdentity != null)
        {
            impersonationContext = this.primaryThreadIdentity.Impersonate();
        }

        try
        {
            this.RunOnce();
        }
        finally
        {
            Monitor.Exit(this);

            impersonationContext?.Undo();
        }
    }
}