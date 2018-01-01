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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;

    using YAF.Classes.Data;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Utils.Helpers.StringUtils;

    /// <summary>
    /// Does some user clean up tasks such as unsuspending users...
    /// </summary>
    public class UserCleanUpTask : IntermittentBackgroundTask
    {
        /// <summary>
        /// The _task name.
        /// </summary>
        private const string _taskName = "UserCleanUpTask";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCleanUpTask"/> class.
        /// </summary>
        public UserCleanUpTask()
        {
            // set interval values...
            RunPeriodMs = 3600000;
            StartDelayMs = 30000;
        }

        /// <summary>
        /// Gets TaskName.
        /// </summary>
        public static string TaskName
        {
            get
            {
                return _taskName;
            }
        }

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                // get all boards...
                List<int> boardIds = this.GetRepository<Board>().ListTyped().Select(x => x.ID).ToList();

                // go through each board...
                foreach (int boardId in boardIds)
                {
                    // get users for this board...
                    List<DataRow> users = LegacyDb.user_list(boardId, null, null).Rows.Cast<DataRow>().ToList();

                    // handle unsuspension...
                    var suspendedUsers = from u in users
                                         where (u["Suspended"] != DBNull.Value && (DateTime)u["Suspended"] < DateTime.UtcNow)
                                         select u;

                    // unsuspend these users...
                    foreach (var user in suspendedUsers)
                    {
                        this.GetRepository<User>().Suspend(user["UserId"].ToType<int>(), null);

                        // sleep for a quarter of a second so we don't pound the server...
                        Thread.Sleep(250);
                    }

                    // sleep for a second...
                    Thread.Sleep(1000);
                }
            }
            catch (Exception x)
            {
                this.Logger.Error(x, "Error In {0} Task".FormatWith(TaskName));
            }
        }
    }
}