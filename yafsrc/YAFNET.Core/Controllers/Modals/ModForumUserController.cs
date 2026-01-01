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

namespace YAF.Core.Controllers.Modals;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Core.Model;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Medal controller.
/// </summary>
[EnableRateLimiting("fixed")]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization]
public class ModForumUserController : ForumBaseController
{
    /// <summary>
    /// Add or Edit the User Medal
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("EditModForumUser")]
    public IActionResult EditModForumUser([FromBody] ModForumUserModal model)
    {
        if (model.UserID == 0)
        {
            // no username, nor userID specified
            return this.Ok(
                new MessageModalNotification(this.GetText("MSG_VALID_USER"), MessageTypes.warning));
        }

        // save permission
        this.GetRepository<UserForum>().Save(
            model.UserID,
            model.ForumId,
            model.AccessMaskID);

        return this.Ok();
    }
}