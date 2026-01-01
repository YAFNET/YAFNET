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

using System;
using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The Forum Repository Extensions
/// </summary>
public static class ProfileCustomRepositoryExtensions
{
    /// <summary>
    /// Gets all Custom Profiles by User
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    public static List<Tuple<ProfileCustom, ProfileDefinition>> ListByUser(
        this IRepository<ProfileCustom> repository,
        int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<ProfileCustom>();

        expression.Join<ProfileDefinition>((c, d) => d.ID == c.ProfileDefinitionID)
            .Where<ProfileCustom>(
                c => c.UserID == userId);

        return repository.DbAccess.Execute(
            db => db.Connection.SelectMulti<ProfileCustom, ProfileDefinition>(expression));
    }
}