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

namespace YAF.Core.Controllers.Modals;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Types.Attributes;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// Profile Definition Controller
/// Implements the <see cref="ForumBaseController" />
/// </summary>
/// <seealso cref="ForumBaseController" />
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization]
public class ProfileDefinitionController : ForumBaseController
{
    /// <summary>
    /// Edit Server
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("Edit")]
    public IActionResult Edit([FromBody] EditProfileDefinitionModal model)
    {
        if (model.Id is 0)
        {
            model.Id = null;
        }

        this.GetRepository<ProfileDefinition>().Upsert(
            new ProfileDefinition
                {
                    BoardID = this.PageBoardContext.PageBoardID,
                    ID = model.Id ?? 0,
                    Name = model.Name,
                    DataType = model.DataType,
                    DefaultValue = model.DefaultValue,
                    Length = model.Length,
                    Required = model.Required,
                    ShowInUserInfo = model.ShowInUserInfo,
                    ShowOnRegisterPage = model.ShowOnRegisterPage
                });

        return this.Ok();
    }
}