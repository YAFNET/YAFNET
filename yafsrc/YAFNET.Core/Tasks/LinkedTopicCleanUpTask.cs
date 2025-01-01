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
/// Does some Linked topic cleaning...
/// </summary>
public class LinkedTopicCleanUpTask : IntermittentBackgroundTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedTopicCleanUpTask"/> class.
    /// </summary>
    public LinkedTopicCleanUpTask()
    {
        // set interval values...
        this.RunPeriodMs = 3600000;
        this.StartDelayMs = 30000;
    }

    /// <summary>
    /// Gets TaskName.
    /// </summary>
    public static string TaskName => "LinkedTopicCleanUpTask";

    /// <summary>
    /// The run once.
    /// </summary>
    public override Task RunOnceAsync()
    {
        try
        {
            var linkDate = DateTime.UtcNow;

            var topics = this.GetRepository<Topic>().Get(
                x => x.TopicMovedID != null && x.LinkDate != null && x.LinkDate < linkDate);

            topics.ForEach(t => this.GetRepository<Topic>().Delete(t.ForumID, t.ID, true));
        }
        catch (Exception x)
        {
            this.Logger.Error(x, $"Error In {TaskName} Task");
        }

        return Task.CompletedTask;
    }
}