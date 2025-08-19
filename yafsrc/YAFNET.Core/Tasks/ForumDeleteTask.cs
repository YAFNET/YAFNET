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
/// The forum delete task.
/// </summary>
public class ForumDeleteTask : LongBackgroundTask, ICriticalBackgroundTask
{
    /// <summary>
    /// The Blocking Task Names.
    /// </summary>
    private readonly static string[] BlockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;

    /// <summary>
    /// Gets TaskName.
    /// </summary>
    public static string TaskName => "ForumDeleteTask";

    /// <summary>
    /// Gets or sets ForumId.
    /// </summary>
    public int ForumId { get; set; }

    /// <summary>
    /// Gets or sets Forum New Id.
    /// </summary>
    public int ForumNewId { get; set; }

    /// <summary>
    /// Creates the Forum Delete Task
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="failureMessage">
    /// The failure message - is empty if task is launched successfully.
    /// </param>
    /// <returns>
    /// Returns if Task was Successful
    /// </returns>
    public static bool Start(int boardId, int forumId, out string failureMessage)
    {
        failureMessage = string.Empty;
        if (BoardContext.Current.Get<ITaskModuleManager>() == null)
        {
            return false;
        }

        if (!BoardContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
        {
            BoardContext.Current.Get<ITaskModuleManager>().StartTask(
                TaskName,
                () => new ForumDeleteTask { Data = boardId, ForumId = forumId, ForumNewId = -1 });
        }
        else
        {
            failureMessage =
                $"You can't delete forum while blocking {BlockingTaskNames.ToDelimitedString(",")} tasks are running.";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates the Forum Delete Task and moves the Messages to a new Forum
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="forumOldId">
    /// The forum Old Id.
    /// </param>
    /// <param name="forumNewId">
    /// The Forum New Id.
    /// </param>
    /// <param name="failureMessage">
    /// The failure message - is empty if task is launched successfully.
    /// </param>
    /// <returns>
    /// Returns if Task was Successful
    /// </returns>
    public static bool Start(int boardId, int forumOldId, int forumNewId, out string failureMessage)
    {
        failureMessage = string.Empty;
        if (BoardContext.Current.Get<ITaskModuleManager>() == null)
        {
            return false;
        }

        BoardContext.Current.Get<ITaskModuleManager>().StartTask(
            TaskName,
            () => new ForumDeleteTask { Data = boardId, ForumId = forumOldId, ForumNewId = forumNewId });

        return true;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public async override Task RunOnceAsync()
    {
        try
        {
            if (this.ForumNewId.Equals(-1))
            {
                await this.GetRepository<Forum>().DeleteAsync(this.ForumId);
                this.Logger.Info($"Forum (ID: {this.ForumId}) Delete Task Complete.");
            }
            else
            {
                await this.GetRepository<Forum>().MoveAsync(this.ForumId, this.ForumNewId);

                this.Logger.Info(
                    $"Forum (ID: {this.ForumId}) Delete Task Complete, and Topics has been moved to Forum (ID: {this.ForumNewId})");
            }
        }
        catch (Exception x)
        {
            this.Logger.Error(x, $"Error In (ID: {this.ForumId}) Delete Task");
        }
    }
}