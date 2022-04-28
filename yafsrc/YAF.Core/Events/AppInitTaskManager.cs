/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

#region Using

using System;
using System.Collections.Generic;

using YAF.Core.Tasks;
using YAF.Types.Attributes;

#endregion

/// <summary>
/// Initializes the Application task manager.
/// </summary>
[ExportService(ServiceLifetimeScope.Singleton)]
public class AppInitTaskManager : BaseTaskModuleManager, IHandleEvent<HttpApplicationInitEvent>, IHaveServiceLocator
{
    #region Constants and Fields

    /// <summary>
    ///   The app instance.
    /// </summary>
    private HttpApplication appInstance;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInitTaskManager"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public AppInitTaskManager([NotNull] IServiceLocator serviceLocator, [NotNull] ILoggerService logger)
    {
        this.ServiceLocator = serviceLocator;
        this.Logger = logger;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILoggerService Logger { get; set; }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 5;

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start a non-running task -- will set the <see cref="HttpApplication"/> instance.
    /// </summary>
    /// <param name="instanceName">
    /// Unique name of this task
    /// </param>
    /// <param name="start">
    /// Task to run
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public override bool StartTask([NotNull] string instanceName, [NotNull] Func<IBackgroundTask> start)
    {
        CodeContracts.VerifyNotNull(instanceName);
        CodeContracts.VerifyNotNull(start);

        if (this.appInstance is null)
        {
            return false;
        }

        // add and start this module...
        if (this.TaskExists(instanceName))
        {
            return false;
        }

        this.Logger.Debug($"Starting Task {instanceName}...");

        var injectServices = this.Get<IInjectServices>();

        taskManager.AddOrUpdate(
            instanceName,
            s =>
                {
                    var task = start();
                    injectServices.Inject(task);
                    task.Run();
                    return task;
                },
            (s, task) =>
                {
                    task?.Dispose();

                    var newTask = start();
                    injectServices.Inject(newTask);
                    newTask.Run();
                    return task;
                });

        return true;
    }

    #endregion

    #region Implemented Interfaces

    #region IHandleEvent<HttpApplicationInitEvent>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle([NotNull] HttpApplicationInitEvent @event)
    {
        this.appInstance = @event.HttpApplication;

        // wire up provider so that the task module can be found...
        this.Get<CurrentTaskModuleProvider>().Instance = this;

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

    #endregion

    #endregion
}