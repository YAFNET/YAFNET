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

namespace YAF.Core.ForumModules;

using System;

using YAF.Core.BaseModules;
using YAF.Types.Attributes;

/// <summary>
/// The user clean up task module.
/// </summary>
[Module("Clean Up PageUser Task Starting Module", "Tiny Gecko", 1)]
public class UserCleanUpTaskForumModule : BaseForumModule
{
    /// <summary>
    /// The init.
    /// </summary>
    public override void Init()
    {
        // hook the page init for mail sending...
        this.PageContext.AfterInit += this.Current_AfterInit;
    }

    /// <summary>
    /// Start Task after Initialization
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Current_AfterInit(object sender, EventArgs e)
    {
        this.Get<ITaskModuleManager>().StartTask(UserCleanUpTask.TaskName, () => new UserCleanUpTask());
    }
}