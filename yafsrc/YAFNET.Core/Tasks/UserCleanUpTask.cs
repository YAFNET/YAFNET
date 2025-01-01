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

using System;
using System.Threading.Tasks;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// Does some user clean up tasks such as un-suspending users...
/// </summary>
public class UserCleanUpTask : IntermittentBackgroundTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCleanUpTask"/> class.
    /// </summary>
    public UserCleanUpTask()
    {
        // set interval values...
        this.RunPeriodMs = 3600000;
        this.StartDelayMs = 30000;
    }

    /// <summary>
    /// Gets TaskName.
    /// </summary>
    public static string TaskName => "UserCleanUpTask";

    /// <summary>
    /// The run once.
    /// </summary>
    public async override Task RunOnceAsync()
    {
        try
        {
            // get all boards...
            var boardIds = this.GetRepository<Board>().GetAll().Select(x => x.ID);

            // go through each board...
            foreach (var i in boardIds)
            {
                await Task.Run(() => this.CleanUpUsersAsync(i));
            }
        }
        catch (Exception x)
        {
            this.Logger.Error(x, $"Error In {TaskName} Task");
        }
    }

    private Task CleanUpUsersAsync(int id)
    {
        // Check for users ...
        var users = this.GetRepository<User>().Get(
                        u => u.BoardID == id && u.Suspended.HasValue && u.Suspended < DateTime.UtcNow);

        // un-suspend these users...
        users.ForEach(user => this.GetRepository<User>().Suspend(user.ID));

        return Task.CompletedTask;
    }
}