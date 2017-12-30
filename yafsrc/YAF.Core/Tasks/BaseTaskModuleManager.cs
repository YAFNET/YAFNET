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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using YAF.Core.Tasks;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The base task module manager.
    /// </summary>
    public abstract class BaseTaskModuleManager : ITaskModuleManager
    {
        #region Static Fields

        protected static ConcurrentDictionary<string, IBackgroundTask> _taskManager = new ConcurrentDictionary<string, IBackgroundTask>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets TaskCount.
        /// </summary>
        public virtual int TaskCount
        {
            get
            {
                return _taskManager.Count;
            }
        }

        /// <summary>
        ///     All the names of tasks running.
        /// </summary>
        [NotNull]
        public virtual IList<string> TaskManagerInstances
        {
            get
            {
                return _taskManager.Keys.ToList();
            }
        }

        /// <summary>
        ///     Gets TaskManagerSnapshot.
        /// </summary>
        [NotNull]
        public virtual IDictionary<string, IBackgroundTask> TaskManagerSnapshot
        {
            get
            {
                return _taskManager.ToDictionary(k => k.Key, v => v.Value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Check if Tasks are Running.
        /// </summary>
        /// <param name="instanceNames">
        /// </param>
        /// <returns>
        ///     The tasks are running.
        /// </returns>
        public virtual bool AreTasksRunning([NotNull] string[] instanceNames)
        {
            bool isRunning = false;
            foreach (var s in instanceNames)
            {
                IBackgroundTask task;
                isRunning = this.TryGetTask(s, out task) && task.IsRunning;
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
        /// </param>
        /// <returns>
        ///     The is task running.
        /// </returns>
        public virtual bool IsTaskRunning([NotNull] string instanceName)
        {
            IBackgroundTask task;

            return this.TryGetTask(instanceName, out task) && task.IsRunning;
        }

        /// <summary>
        ///     Start a non-running task -- will set the <see cref="HttpApplication" /> instance.
        /// </summary>
        /// <param name="instanceName">
        ///     Unique name of this task
        /// </param>
        /// <param name="start">
        ///     Task to run
        /// </param>
        public abstract bool StartTask([NotNull] string instanceName, [NotNull] Func<IBackgroundTask> start);

        /// <summary>
        ///     Stops a task from running if it's not critical
        /// </summary>
        /// <param name="instanceName">
        /// </param>
        public virtual void StopTask([NotNull] string instanceName)
        {
            IBackgroundTask task;

            if (this.TryGetTask(instanceName, out task))
            {
                if (task != null && task.IsRunning && !(task is ICriticalBackgroundTask))
                {
                    if (this.TryRemoveTask(instanceName))
                    {
                        task.Dispose();
                    }
                }
            }
        }

        /// <summary>
        ///     Check if a task exists in the task manager. May not be running.
        /// </summary>
        /// <param name="instanceName">
        /// </param>
        /// <returns>
        ///     The task exists.
        /// </returns>
        public virtual bool TaskExists([NotNull] string instanceName)
        {
            return _taskManager.ContainsKey(instanceName);
        }

        /// <summary>
        ///     Attempt to get the instance of the task.
        /// </summary>
        /// <param name="instanceName">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual bool TryGetTask([NotNull] string instanceName, out IBackgroundTask task)
        {
            return _taskManager.TryGetValue(instanceName, out task);
        }

        /// <summary>
        ///     The try remove task.
        /// </summary>
        /// <param name="instanceName">
        ///     The instance name.
        /// </param>
        /// <returns>
        ///     The try remove task.
        /// </returns>
        public virtual bool TryRemoveTask([NotNull] string instanceName)
        {
            IBackgroundTask task;

            return _taskManager.TryRemove(instanceName, out task);
        }

        #endregion
    }
}