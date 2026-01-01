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

namespace YAF.Core.Tasks;

using System;
using System.Threading.Tasks;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// Run when we want to do migration of users in the background...
/// </summary>
public class PruneTopicTask : LongBackgroundTask
{
    /// <summary>
    /// Gets the name of the task.
    /// </summary>
    /// <value>
    /// The name of the task.
    /// </value>
    public override string TaskName => nameof(PruneTopicTask);

    /// <summary>
    /// Gets the task description.
    /// </summary>
    /// <value>
    /// The task description.
    /// </value>
    public override string TaskDescription => "Delete old topic(s).";

    /// <summary>
    /// Gets or sets ForumId.
    /// </summary>
    public int ForumId { get; set; }

    /// <summary>
    /// Gets or sets Days.
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether PermDelete.
    /// </summary>
    public bool PermDelete { get; set; }

    /// <summary>
    /// Start Task
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="days">
    /// The days.
    /// </param>
    /// <param name="permDelete">
    /// The perm delete.
    /// </param>
    /// <returns>
    /// The start.
    /// </returns>
    public static bool Start(int boardId, int forumId, int days, bool permDelete)
    {
        if (BoardContext.Current.Get<ITaskModuleManager>() == null)
        {
            return false;
        }

        BoardContext.Current.Get<ITaskModuleManager>().StartTask(
            nameof(PruneTopicTask),
            () => new PruneTopicTask { Data = boardId, ForumId = forumId, Days = days, PermDelete = permDelete });

        return true;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public override Task RunOnceAsync()
    {
        try
        {
            this.Logger.Info(
                $"Starting Prune Task for ForumID {this.ForumId}, {this.Days} Days, Perm Delete {this.PermDelete}.");

            var count = this.GetRepository<Topic>().Prune((int)this.Data, this.ForumId, this.Days);

            this.Logger.Info($"Prune Task Complete. Pruned {count} topics.");
        }
        catch (Exception x)
        {
            this.Logger.Error(x, $"Error In Prune Topic Task: {x.Message}");
        }

        return Task.CompletedTask;
    }
}