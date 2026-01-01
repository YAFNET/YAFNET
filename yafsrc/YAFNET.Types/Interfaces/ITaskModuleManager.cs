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

namespace YAF.Types.Interfaces;

using System.Collections.Generic;

/// <summary>
/// The task module manager Interface.
/// </summary>
public interface ITaskModuleManager
{
    /// <summary>
    ///   Gets TaskCount.
    /// </summary>
    int TaskCount { get; }

    /// <summary>
    ///   All the names of tasks running.
    /// </summary>
    IList<string> TaskManagerInstances { get; }

    /// <summary>
    ///   Gets TaskManagerSnapshot.
    /// </summary>
    IDictionary<string, IBackgroundTask> TaskManagerSnapshot { get; }

    /// <summary>
    /// Check if a Task is Running.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// The is task running.
    /// </returns>
    bool IsTaskRunning(string instanceName);

    /// <summary>
    /// Check if Tasks are Running.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// The are tasks running.
    /// </returns>
    bool AreTasksRunning(string[] instanceName);

    /// <summary>
    /// Start a non-running task
    /// </summary>
    /// <param name="instanceName">
    /// Unique name of this task
    /// </param>
    /// <param name="startTask">
    /// Task to run
    /// </param>
    bool StartTask(string instanceName, Func<IBackgroundTask> startTask);

    /// <summary>
    /// Tries the get task.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="task">The task.</param>
    bool TryGetTask(string instanceName, out IBackgroundTask task);

    /// <summary>
    /// The try remove task.
    /// </summary>
    /// <param name="instanceName">
    /// The instance name.
    /// </param>
    /// <returns>
    /// The try remove task.
    /// </returns>
    bool TryRemoveTask(string instanceName);
}