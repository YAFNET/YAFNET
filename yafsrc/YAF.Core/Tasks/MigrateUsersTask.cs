/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Tasks
{
    #region Using

    using System;

    using YAF.Core.Context;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Run when we want to do migration of users in the background...
    /// </summary>
    public class MigrateUsersTask : LongBackgroundTask
    {
        #region Constants and Fields

        #endregion

        #region Properties

        /// <summary>
        ///   Gets TaskName.
        /// </summary>
        [NotNull]
        public static string TaskName { get; } = "MigrateUsersTask";

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the Migrate User Task
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <returns>
        /// Returns if the task was started or not
        /// </returns>
        public static bool Start(int boardId)
        {
            if (BoardContext.Current.Get<ITaskModuleManager>() == null)
            {
                return false;
            }

            BoardContext.Current.Get<ITaskModuleManager>()
                .StartTask(TaskName, () => new MigrateUsersTask { Data = boardId });

            return true;
        }

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                // attempt to run the migration code...
                RoleMembershipHelper.SyncUsers((int)this.Data);
            }
            catch (Exception x)
            {
                this.Logger.Error(x, $"Error In {TaskName} Task");
            }
        }

        #endregion
    }
}