/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Core.Controllers.Modals;

using System;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Core.Model;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Nntp Controller
/// Implements the <see cref="ForumBaseController" />
/// </summary>
/// <seealso cref="ForumBaseController" />
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization]
public class NntpController : ForumBaseController
{
    /// <summary>
    /// Edit Server
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("EditServer")]
    public IActionResult EditServer([FromBody] NntpServerEditModal model)
    {
        this.GetRepository<NntpServer>().Save(
            model.Id,
            this.PageBoardContext.PageBoardID,
            model.Name,
            model.Address,
            model.Port == 0 ? null : model.Port,
            model.UserName,
            model.UserPass);

        return this.Ok();
    }
    
    /// <summary>
    /// Edit Forum
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("EditForum")]
    public IActionResult EditForum([FromBody] NntpForumEditModal model)
    {
        if (model.Id is 0)
        {
            model.Id = null;
        }
        
        if (model.ForumID <= 0)
        {
            return this.Ok(
                new MessageModalNotification(
                    this.GetText("ADMIN_EDITNNTPFORUM", "MSG_SELECT_FORUM"),
                    MessageTypes.warning));        }

        this.GetRepository<NntpForum>().Save(
            model.Id,
            model.NntpServerID,
            model.GroupName,
            model.ForumID,
            model.Active,
            model.DateCutOff == DateTime.MinValue ? null : model.DateCutOff);

        return this.Ok();
    }
}