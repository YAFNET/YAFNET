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
namespace YAF.Core.Tasks
{
    using System;

    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    /// <summary>
    ///     The forum delete task.
    /// </summary>
    public class CategoryDeleteTask : LongBackgroundTask, ICriticalBackgroundTask
    {
        #region Constants

        /// <summary>
        ///     The _task name.
        /// </summary>
        private const string _TaskName = "CategoryDeleteTask";

        #endregion

        #region Static Fields

        /// <summary>
        ///     The Blocking Task Names.
        /// </summary>
        private static readonly string[] BlockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets TaskName.
        /// </summary>
        public static string TaskName
        {
            get
            {
                return _TaskName;
            }
        }

        /// <summary>
        ///     Gets or sets CategoryId.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     Gets or sets Forum New Id.
        /// </summary>
        public int ForumNewId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates the Forum Delete Task
        /// </summary>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="failureMessage">
        /// The failure message - is empty if task is launched successfully.
        /// </param>
        /// <returns>
        /// Returns if Task was Successfull
        /// </returns>
        public static bool Start(int categoryId, out string failureMessage)
        {
            failureMessage = string.Empty;

            if (YafContext.Current.Get<ITaskModuleManager>() == null)
            {
                return false;
            }

            if (!YafContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
            {
                YafContext.Current.Get<ITaskModuleManager>().StartTask(TaskName, () => new CategoryDeleteTask { CategoryId = categoryId });
            }
            else
            {
                failureMessage =
                    "You can't delete category while some of the blocking {0} tasks are running.".FormatWith(BlockingTaskNames.ToDelimitedString(","));
                return false;
            }

            return true;
        }

        /// <summary>
        ///     The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                this.Logger.Info("Starting Category {0} delete task.", this.CategoryId);
                this.GetRepository<Category>().DeleteById(this.CategoryId);
                this.Logger.Info("Category (ID: {0}) Delete Task Complete.", this.CategoryId);
            }
            catch (Exception x)
            {
                this.Logger.Error(x, "Error In Category (ID: {0}) Delete Task".FormatWith(this.CategoryId), x);
            }
        }

        #endregion
    }
}