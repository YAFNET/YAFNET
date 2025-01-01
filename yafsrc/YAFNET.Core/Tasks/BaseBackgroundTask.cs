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

namespace YAF.Core.Tasks;

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Types.Attributes;

/// <summary>
/// The base background task.
/// </summary>
public abstract class BaseBackgroundTask : IBackgroundTask, IHaveServiceLocator
{
    /// <summary>
    /// The board id.
    /// </summary>
    protected int BoardId = BoardContext.Current.Get<ControlSettings>().BoardID;

    /// <summary>
    /// The is running.
    /// </summary>
    protected bool isRunning;

    /// <summary>
    /// The lock object.
    /// </summary>
    readonly protected object LockObject = new ();

    /// <summary>
    /// The started.
    /// </summary>
    protected DateTime started;

    /// <summary>
    /// Gets or sets the ServiceLocator.
    /// </summary>
    [Inject]
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<BaseBackgroundTask> Logger { get; set; }

    /// <summary>
    /// Gets or sets BoardID.
    /// </summary>
    public virtual object Data
    {
        protected get => this.BoardId;

        set => this.BoardId = (int)value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsRunning.
    /// </summary>
    public virtual bool IsRunning
    {
        get
        {
            lock (this.LockObject)
            {
                return this.isRunning;
            }
        }

        protected set
        {
            lock (this.LockObject)
            {
                if (!this.isRunning && value)
                {
                    this.started = DateTime.UtcNow;
                }

                this.isRunning = value;
            }
        }
    }

    /// <summary>
    /// Gets Started.
    /// </summary>
    public virtual DateTime Started => this.started;

    /// <summary>
    /// The run.
    /// </summary>
    public virtual void Run()
    {
        this.IsRunning = true;

        this.RunOnceAsync();

        this.IsRunning = false;
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public virtual void Dispose()
    {
        this.IsRunning = false;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public abstract Task RunOnceAsync();
}