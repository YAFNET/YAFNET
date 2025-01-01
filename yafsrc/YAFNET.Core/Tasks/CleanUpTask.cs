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

using System.Threading.Tasks;

/// <summary>
/// Automatically cleans up the tasks if they are no longer running...
/// </summary>
public class CleanUpTask : IntermittentBackgroundTask, ICriticalBackgroundTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CleanUpTask"/> class.
    /// </summary>
    public CleanUpTask()
    {
        // set interval values...
        this.RunPeriodMs = 500;
        this.StartDelayMs = 500;
    }

    /// <summary>
    /// Gets or sets TaskManager.
    /// </summary>
    public ITaskModuleManager TaskManager { get; set; }

    /// <summary>
    /// The run once.
    /// </summary>
    public override Task RunOnceAsync()
    {
        // look for tasks to clean up...
        if (this.TaskManager == null)
        {
            return Task.CompletedTask;
        }

        // make collection local...
        var taskListKeys = this.TaskManager.TaskManagerInstances;

        taskListKeys.ForEach(
            instanceName =>
                {
                    if (this.TaskManager.TryGetTask(instanceName, out var task))
                    {
                        if (task.IsRunning)
                        {
                            return;
                        }

                        this.TaskManager.TryRemoveTask(instanceName);
                        task.Dispose();
                    }
                    else
                    {
                        this.TaskManager.TryRemoveTask(instanceName);
                    }
                });

        return Task.CompletedTask;
    }
}