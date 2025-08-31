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


using System.Collections.Generic;

namespace YAF.Core.Controllers;

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Stats controller.
/// </summary>
[Route("api/[controller]")]
public class Stats : ForumBaseController
{
    /// <summary>
    /// Gets the user stats (Countries and Browsers)
    /// </summary>
    /// <returns>List with user stats.</returns>
    [CamelCaseOutput]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserStats))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetUserStats")]
    public async Task<ActionResult<UserStats>> GetUserStats()
    {
        try
        {
            // Check if user has access
            if (BoardContext.Current == null)
            {
                return this.NotFound();
            }

            if (!BoardContext.Current.IsAdmin)
            {
                return this.NotFound();
            }

            var activeUsers = await this.GetRepository<Active>().GetAsync(a => a.BoardID == this.Get<BoardSettings>().BoardId);

            var browsers = activeUsers
                .GroupBy(u => new { u.Browser })
                .Select(g => new StatsData { Label = g.Key.Browser, Data = g.Count() });

            var platforms = activeUsers
                .GroupBy(u => new { u.Platform })
                .Select(g => new StatsData { Label = g.Key.Platform, Data = g.Count() });

            IEnumerable<StatsData> countries = [];

            if(this.Get<IGeoIpCountryService>().DatabaseExists())
            {
                countries = activeUsers
                    .GroupBy(u => new { u.Country }).Where(g => g.Key.Country.IsSet())
                    .Select(g => new StatsData
                    {
                        Label = g.Key.Country.Equals("--") ? g.Key.Country : this.GetText(g.Key.Country),
                        Data = g.Count()
                    });
            }

            var users = await this.Get<IDataCache>().GetOrSetAsync(
                Constants.Cache.RegisteredUsersByMonth,
                () => this.GetRepository<User>().GetRegisteredUsersByMonthAsync(this.Get<BoardSettings>().BoardId));

            var registrations = users
                .GroupBy(u => new { u.Joined.Date.Year, u.Joined.Date.Month })
                .Select(g => new StatsData { Label = $"{this.GetText("CALENDER", g.Key.Month.ToString())[..3]} {g.Key.Year}", Data = g.Count() });

            var results = new UserStats
            {
                Browsers = browsers,
                Platforms = platforms,
                Countries = countries,
                Registrations = registrations
            };

            return this.Ok(results);
        }
        catch (Exception x)
        {
            this.Get<ILogger<Stats>>().Log(BoardContext.Current?.PageUserID, this, x, EventLogTypes.Information);

            return this.NotFound();
        }
    }
}