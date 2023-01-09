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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Types.Models;

using YAF.Core.BasePages;

/// <summary>
/// The CustomBBCodes controller.
/// </summary>
[Route("api/[controller]")]
public class CustomBBCodes : ForumBaseController
{
    /// <summary>
    /// Gets the forum user info as JSON string for the hover cards
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetList")]
    public Task<ActionResult<List<string>>> GetList()
    {
        try
        {
            // Check if user has access
            if (BoardContext.Current == null)
            {
                return Task.FromResult<ActionResult<List<string>>>(this.NotFound());
            }

            var customBbCode = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.CustomBBCode,
                () => this.GetRepository<BBCode>().GetByBoardId());

            var list = customBbCode.Where(e => e.Name != "ALBUMIMG" && e.Name != "ATTACH").Select(e => e.Name).ToList();

            return Task.FromResult<ActionResult<List<string>>>(this.Ok(list));
        }
        catch (Exception x)
        {
            this.Get<ILogger<CustomBBCodes>>().Log(BoardContext.Current != null ? BoardContext.Current.PageUserID : null, this, x, EventLogTypes.Information);

            return Task.FromResult<ActionResult<List<string>>>(this.NotFound()); 
        }
    }
}