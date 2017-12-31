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
    #region Using

    using System;

    using YAF.Classes.Data;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Run when we want to do migration of users in the background...
    /// </summary>
    public class BoardCreateTask : LongBackgroundTask, ICriticalBackgroundTask
    {
        private static long _boardOut;

        /// <summary>
        /// The Blocking Task Names.
        /// </summary>
        private static readonly string[] _blockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;

        /// <summary>
        /// The _task name.
        /// </summary>
        private const string _TaskName = "BoardCreateTask";

        /// <summary>
        ///  Gets TaskName.
        /// </summary>
        public static string TaskName
        {
            get
            {
                return _TaskName;
            }
        }

        /// <summary>
        /// Gets Blocking Task Names.
        /// </summary>
        public static string[] BlockingTaskNames
        {
            get
            {
                return _blockingTaskNames;
            }
        }

        /// <summary>
        /// Gets or sets ForumOut.
        /// </summary>
        public static long BoardOut
        {
            get
            {
                return _boardOut;
            }

            set
            {
                _boardOut = value;
            }
        }

        /// <summary>
        /// Gets or sets AdminUserName.
        /// </summary>
        public object AdminUserName { get; set; }

        /// <summary>
        /// Gets or sets AdminUserEmail.
        /// </summary>
        public object AdminUserEmail { get; set; }

        /// <summary>
        /// Gets or sets AdminUserProviderUserKey.
        /// </summary>
        public object AdminUserProviderUserKey { get; set; }

        /// <summary>
        /// Gets or sets Board Name.
        /// </summary>
        public object BoardName { get; set; }

        /// <summary>
        /// Gets or sets Culture.
        /// </summary>
        public object Culture { get; set; }

        /// <summary>
        /// Gets or sets LanguageFileName.
        /// </summary>
        public object LanguageFileName { get; set; }

        /// <summary>
        /// Gets or sets BoardMembershipAppName.
        /// </summary>
        public object BoardMembershipAppName { get; set; }

        /// <summary>
        /// Gets or sets BoardRolesAppName.
        /// </summary>
        public object BoardRolesAppName { get; set; }

        /// <summary>
        /// Gets or sets RolePrefix.
        /// </summary>
        public object RolePrefix { get; set; }

        /// <summary>
        /// Starts the Board Create Task
        /// </summary>
        /// <param name="adminUserName">Name of the admin user.</param>
        /// <param name="admiUserEmail">The admi user email.</param>
        /// <param name="adminUserProviderUserkey">The admin user provider userkey.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="languageFileName">Name of the language file.</param>
        /// <param name="boardMembershipAppName">Name of the board membership app.</param>
        /// <param name="boardRolesAppName">Name of the board roles app.</param>
        /// <param name="rolePrefix">The role prefix.</param>
        /// <param name="failureMessage">The failure message.</param>
        /// <returns>
        /// Returns the new Board ID
        /// </returns>
        public static long Start(
            object adminUserName,
            object admiUserEmail,
            object adminUserProviderUserkey,
            object boardName,
            object culture,
            object languageFileName,
            object boardMembershipAppName,
            object boardRolesAppName,
            object rolePrefix,
            out string failureMessage)
        {
            failureMessage = string.Empty;
            if (YafContext.Current.Get<ITaskModuleManager>() == null)
            {
                return 0;
            }

            if (!YafContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
            {
                YafContext.Current.Get<ITaskModuleManager>().StartTask(
                    TaskName,
                    () =>
                    new BoardCreateTask
                        {
                            AdminUserName = adminUserName,
                            AdminUserEmail = admiUserEmail,
                            AdminUserProviderUserKey = adminUserProviderUserkey,
                            BoardName = boardName,
                            Culture = culture,
                            LanguageFileName = languageFileName,
                            BoardMembershipAppName = boardMembershipAppName,
                            BoardRolesAppName = boardRolesAppName,
                            RolePrefix = rolePrefix
                        });
            }
            else
            {
                failureMessage =
                    "You can't delete forum while blocking {0} tasks are running.".FormatWith(
                        BlockingTaskNames.ToDelimitedString(","));
                _boardOut = -1;
            }

            return BoardOut;
        }

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                this.Logger.Info("Starting  Board Add Task for board {0}.", _boardOut);

                // Create Board
                _boardOut = this.GetRepository<Board>().Create(
                    this.BoardName.ToString(),
                    this.Culture.ToString(),
                    this.LanguageFileName.ToString(),
                    this.BoardMembershipAppName.ToString(),
                    this.BoardRolesAppName.ToString(),
                    this.AdminUserName.ToString(),
                    this.AdminUserEmail.ToString(),
                    this.AdminUserProviderUserKey.ToString(),
                    false,
                    this.RolePrefix.ToString());

                this.Logger.Info("Board Add Task for board {0} completed.", _boardOut);
            }
            catch (Exception)
            {
                _boardOut = -1;
            }
        }
    }
}