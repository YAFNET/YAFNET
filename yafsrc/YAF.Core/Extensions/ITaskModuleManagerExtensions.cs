/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core
{
  #region Using

  using System;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The i task module manager extensions.
  /// </summary>
  public static class ITaskModuleManagerExtensions
  {
    #region Public Methods

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
    /// The start.
    /// </returns>
    public static bool Start<T>([NotNull] this ITaskModuleManager taskModuleManager, [NotNull] Func<T> createTask) where T : IBackgroundTask
    {
      CodeContracts.VerifyNotNull(taskModuleManager, "taskModuleManager");
      CodeContracts.VerifyNotNull(createTask, "createTask");

      string taskName = typeof(T).ToString();

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
    /// The start.
    /// </returns>
    public static bool Start<T>([NotNull] this ITaskModuleManager taskModuleManager, [CanBeNull] object data) where T : IBackgroundTask, new()
    {
      CodeContracts.VerifyNotNull(taskModuleManager, "taskModuleManager");

      return Start(taskModuleManager, () => new T { Data = data });
    }

    #endregion
  }
}