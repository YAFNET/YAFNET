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
namespace YAF.Core.Extensions;

using System;

/// <summary>
/// The i task module manager extensions.
/// </summary>
public static class ITaskModuleManagerExtensions
{
    /// <summary>
    /// The start.
    /// </summary>
    /// <param name="taskModuleManager">
    /// The task module manager.
    /// </param>
    /// <param name="createTask">
    /// The create task.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool Start<T>(this ITaskModuleManager taskModuleManager, Func<T> createTask)
        where T : IBackgroundTask
    {
        var taskName = typeof(T).ToString();

        return taskModuleManager.StartTask(taskName, () => createTask());
    }

    /// <summary>
    /// The start.
    /// </summary>
    /// <param name="taskModuleManager">
    /// The task module manager.
    /// </param>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool Start<T>(this ITaskModuleManager taskModuleManager, object data)
        where T : IBackgroundTask, new()
    {
        return Start(taskModuleManager, () => new T { Data = data });
    }
}