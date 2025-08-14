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

using YAF.Core.Model;

namespace YAF.Core.Services;

using System;

using YAF.Types.Models;

/// <summary>
/// Class to get the User Medals
/// </summary>
public class UserMedalService : IUserMedalService, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserMedalService"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public UserMedalService(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets all the user medals.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>System.String.</returns>
    public string GetUserMedals(int userId)
    {
        var key = string.Format(Constants.Cache.UserMedals, userId);

        // get the medals cached...
        var userMedals = this.Get<IDataCache>().GetOrSet(
            key,
            () => this.GetRepository<Medal>().ListUserMedals(userId),
            TimeSpan.FromMinutes(10));

        if (userMedals.Count == 0)
        {
            return string.Empty;
        }

        var ribbonBar = new StringBuilder();
        var medals = new StringBuilder();

        userMedals.ForEach(
            medal =>
            {
                var flags = new MedalFlags(medal.Flags);

                // skip hidden medals
                if (flags.AllowHiding && medal.Hide)
                {
                    return;
                }

                var title = $"{medal.Name}{(flags.ShowMessage ? $": {medal.Message}" : string.Empty)}";

                ribbonBar.AppendFormat(
                    "<li class=\"list-inline-item\"><img src=\"/{2}/{0}\" alt=\"{1}\" title=\"{1}\" data-bs-toggle=\"tooltip\"></li>",
                    medal.MedalURL,
                    title,
                    this.Get<BoardFolders>().Medals);
            });

       return $"<ul class=\"list-inline\">{ribbonBar}{medals}</ul>";
    }
}