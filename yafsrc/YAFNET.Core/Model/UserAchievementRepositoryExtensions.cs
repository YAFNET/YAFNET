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

namespace YAF.Core.Model;

using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The UserAchievement repository extensions.
/// </summary>
public static class UserAchievementRepositoryExtensions
{
    /// <param name="repository">The repository.</param>
    extension(IRepository<UserAchievement> repository)
    {
        /// <summary>
        /// Counts the users that have unlocked the achievements.
        /// </summary>
        /// <param name="achievementIds">The achievement ids.</param>
        /// <returns>The number of the users per achievement (the achievements nobody has are missing).</returns>
        public Dictionary<int, int> CountUsers(List<int> achievementIds)
        {
            if (achievementIds.Count == 0)
            {
                return [];
            }

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<UserAchievement>();

                    expression.Where(u => Sql.In(u.AchievementID, achievementIds)).GroupBy(u => u.AchievementID)
                        .Select(u => new { u.AchievementID, Count = Sql.Count(u.UserID) });

                    return db.Connection.Dictionary<int, int>(expression);
                });
        }
    }
}
