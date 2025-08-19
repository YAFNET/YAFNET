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

namespace YAF.Core.Events;

using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using YAF.Types.Attributes;

/// <summary>
/// Initializes the Application task manager.
/// </summary>
[ExportService(ServiceLifetimeScope.Singleton)]
public class AppInitTaskManager : BaseTaskModuleManager, IHandleEvent<HttpContextInitEvent>, IHaveServiceLocator
{
    /// <summary>
    ///   The app instance.
    /// </summary>
    private HttpContext appInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInitTaskManager"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public AppInitTaskManager(IServiceLocator serviceLocator, ILogger<AppInitTaskManager> logger)
    {
        this.ServiceLocator = serviceLocator;
        this.Logger = logger;
    }

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 5;

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Start a non-running task -- will set the <see cref="HttpContext"/> instance.
    /// </summary>
    /// <param name="instanceName">
    /// Unique name of this task
    /// </param>
    /// <param name="startTask">
    /// Task to run
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public override bool StartTask(string instanceName, Func<IBackgroundTask> startTask)
    {
        ArgumentNullException.ThrowIfNull(instanceName);
        ArgumentNullException.ThrowIfNull(startTask);

        if (this.appInstance == null)
        {
            return false;
        }

        // add and start this module...
        if (this.TaskExists(instanceName))
        {
            return false;
        }

        var injectServices = this.Get<IInjectServices>();

        TaskManager.AddOrUpdate(
            instanceName,
            _ =>
                {
                    var task = startTask();
                    injectServices.Inject(task);
                    task.Run();
                    return task;
                },
            (_, task) =>
                {
                    task?.Dispose();

                    var newTask = startTask();
                    injectServices.Inject(newTask);
                    newTask.Run();
                    return task;
                });

        return true;
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    ///     The event.
    /// </param>
    public void Handle(HttpContextInitEvent @event)
    {
        this.appInstance = @event.HttpContext;

        // create intermittent cleanup task...
        this.StartTask("CleanUpTask", () => new CleanUpTask { TaskManager = this });

        this.Get<IEnumerable<IStartTasks>>().ForEach(
            instance =>
                {
                    try
                    {
                        instance.Start(this);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex, $"Failed to start: {instance.GetType().Name}");
                    }
                });
    }
}