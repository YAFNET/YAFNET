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
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The Update Search Index task.
/// </summary>
public class UpdateSearchIndexTask : LongBackgroundTask
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "UpdateSearchIndexTask" /> class.
    /// </summary>
    public UpdateSearchIndexTask()
    {
        // set interval values...
        this.StartDelayMs = 30000;
    }

    /// <summary>
    /// Gets the name of the task.
    /// </summary>
    /// <value>
    /// The name of the task.
    /// </value>
    public override string TaskName => nameof(UpdateSearchIndexTask);

    /// <summary>
    /// Gets the task description.
    /// </summary>
    /// <value>
    /// The task description.
    /// </value>
    public override string TaskDescription => "Updates the search index.";

    /// <summary>
    /// The run once.
    /// </summary>
    public async override Task RunOnceAsync()
    {
        try
        {
            Thread.BeginCriticalRegion();

            if (BoardContext.Current == null)
            {
                return;
            }

            var forceUpdate = BoardContext.Current.BoardSettings.ForceUpdateSearchIndex;

            var lastSend = GetLastSend();

            if (!IsTimeToUpdateSearchIndex(lastSend))
            {
                return;
            }

            // reload all
            if (forceUpdate)
            {
                lastSend = DateTime.MinValue;
            }

            var forums = this.GetRepository<Forum>().ListAll(BoardContext.Current.PageBoardID);

            var topicTags = this.GetRepository<TopicTag>().ListAll();

            foreach (var messages in from forum in forums.Select(forum => forum.Item2)
                     where forum.LastPosted.HasValue && forum.LastPosted.Value > lastSend
                     select this.GetRepository<Message>().GetAllSearchMessagesByForum(forum.ID, topicTags))
            {
                await this.Get<ISearch>().AddSearchIndexAsync(messages);
            }

            this.Get<ILogger<UpdateSearchIndexTask>>().Info("search index updated");
        }
        catch (Exception x)
        {
            this.Logger.Error(x, $"Error In {this.TaskName} Task");
        }
        finally
        {
            Thread.EndCriticalRegion();
        }
    }

    private static DateTime GetLastSend()
    {
        var lastSend = DateTime.MinValue;

        var boardSettings = BoardContext.Current.BoardSettings;

        if (!boardSettings.LastSearchIndexUpdated.IsSet())
        {
            return lastSend;
        }

        try
        {
            lastSend = Convert.ToDateTime(boardSettings.LastSearchIndexUpdated, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            lastSend = DateTime.MinValue;
        }

        return lastSend;
    }

    /// <summary>
    /// The is time to update search index.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private static bool IsTimeToUpdateSearchIndex(DateTime lastSend)
    {
        var boardSettings = BoardContext.Current.BoardSettings;

        var sendEveryXHours = boardSettings.UpdateSearchIndexEveryXHours;

        if (boardSettings.ForceUpdateSearchIndex)
        {
            boardSettings.LastSearchIndexUpdated = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            boardSettings.ForceUpdateSearchIndex = false;

            BoardContext.Current.Get<BoardSettingsService>().SaveRegistry(boardSettings);

            return true;
        }

        var updateIndex = lastSend < DateTime.Now.AddHours(-sendEveryXHours)
                          && DateTime.Now < DateTime.Today.AddHours(6);

        if (!updateIndex)
        {
            return false;
        }

        boardSettings.LastSearchIndexUpdated = DateTime.Now.ToString(CultureInfo.InvariantCulture);

        BoardContext.Current.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        return true;
    }
}