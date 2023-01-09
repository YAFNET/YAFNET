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

namespace YAF.Core.Controllers;

using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;

using YAF.Core.BasePages;
using YAF.Types.Constants;
using YAF.Types.Objects.Model;

/// <summary>
/// The IgnoredUser controller.
/// </summary>
[Authorize]
public class IgnoredUserController : ForumBaseController
{
    /// <summary>
    /// Removes the ignored user.
    /// </summary>
    /// <param name="m">The message id.</param>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [Route("RemoveIgnoredUser/{m:int}")]
    public IActionResult RemoveIgnoredUser(int m)
    {
        try
        {
            var messages = this.Get<ISessionService>().GetPageData<List<PagedMessage>>();

            var source = messages.FirstOrDefault(message => message.MessageID == m);

            this.Get<IUserIgnored>().RemoveIgnored(source.UserID);

            return this.RedirectToPage(ForumPages.Posts.GetPageName(), new { m, name = source.Topic });
        }
        catch (Exception)
        {
            return this.RedirectToPage(ForumPages.Posts.GetPageName(), new { m });
        }
    }

    /// <summary>
    /// Adds the ignored user.
    /// </summary>
    /// <param name="m">The message id.</param>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [Route("AddIgnoredUser/{m:int}")]
    public IActionResult AddIgnoredUser(int m)
    {
        try
        {
            var messages = this.Get<ISessionService>().GetPageData<List<PagedMessage>>();

            var source = messages.FirstOrDefault(message => message.MessageID == m);

            this.Get<IUserIgnored>().AddIgnored(source.UserID);

            return this.RedirectToPage(ForumPages.Posts.GetPageName(), new { m, name = source.Topic });
        }
        catch (Exception)
        {
            return this.RedirectToPage(ForumPages.Posts.GetPageName(), new { m });
        }
    }
}