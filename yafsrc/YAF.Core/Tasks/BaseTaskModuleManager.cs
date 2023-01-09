/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
using System.Collections.Concurrent;
using System.Collections.Generic;

using YAF.Types.Attributes;

/// <summary>
///     The base task module manager.
/// </summary>
public abstract class BaseTaskModuleManager : ITaskModuleManager
{
    /// <summary>
    /// The task manager.
    /// </summary>
    protected static ConcurrentDictionary<string, IBackgroundTask> taskManager = new();

    /// <summary>
    ///     Gets TaskCount.
    /// </summary>
    public virtual int TaskCount => taskManager.Count;

    /// <summary>
    ///     All the names of tasks running.
    /// </summary>
    [NotNull]
    public virtual IList<string> TaskManagerInstances => taskManager.Keys.ToList();

    /// <summary>
    ///     Gets TaskManagerSnapshot.
    /// </summary>
    [NotNull]
    public virtual IDictionary<string, IBackgroundTask> TaskManagerSnapshot
    {
        get
        {
            return taskManager.ToDictionary(k => k.Key, v => v.Value);
        }
    }

    /// <summary>
    /// Check if Tasks are Running.
    /// </summary>
    /// <param name="instanceName">
    /// The instance Names.
    /// </param>
    /// <returns>
    /// The tasks are running.
    /// </returns>
    public virtual bool AreTasksRunning([NotNull] string[] instanceName)
    {
        var isRunning = false;
            
        foreach (var s in instanceName)
        {
            isRunning = this.TryGetTask(s, out var task) && task.IsRunning;
            if (isRunning)
            {
                break;
            }
        }

        return isRunning;
    }

    /// <summary>
    ///     Check if a Task is Running.
    /// </summary>
    /// <param name="instanceName">
    /// The instance Name.
    /// </param>
    /// <returns>
    ///     The is task running.
    /// </returns>
    public virtual bool IsTaskRunning([NotNull] string instanceName)
    {
        return this.TryGetTask(instanceName, out var task) && task.IsRunning;
    }

    /// <summary>
    /// Start a non-running task -- will set the <see cref="HttpApplication"/> instance.
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
    public abstract bool StartTask([NotNull] string instanceName, [NotNull] Func<IBackgroundTask> startTask);

    /// <summary>
    /// Check if a task exists in the task manager. May not be running.
    /// </summary>
    /// <param name="instanceName">
    /// The instance Name.
    /// </param>
    /// <returns>
    /// The task exists.
    /// </returns>
    public virtual bool TaskExists([NotNull] string instanceName)
    {
        return taskManager.ContainsKey(instanceName);
    }

    /// <summary>
    /// Attempt to get the instance of the task.
    /// </summary>
    /// <param name="instanceName">
    /// The instance Name.
    /// </param>
    /// <param name="task">
    /// The task.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public virtual bool TryGetTask([NotNull] string instanceName, out IBackgroundTask task) => taskManager.TryGetValue(instanceName, out task);

    /// <summary>
    /// The try remove task.
    /// </summary>
    /// <param name="instanceName">
    /// The instance name.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public virtual bool TryRemoveTask([NotNull] string instanceName)
    {
        return taskManager.TryRemove(instanceName, out _);
    }
}