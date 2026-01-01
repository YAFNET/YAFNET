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

namespace YAF.Core.Services;

using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The default user display name.
/// </summary>
public class DefaultUserDisplayName : IUserDisplayName, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultUserDisplayName"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    public DefaultUserDisplayName(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets the ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Find user
    /// </summary>
    /// <param name="contains">The contains.</param>
    /// <returns>
    /// Returns the Found PageUser
    /// </returns>
    public IList<User> FindUserContainsName(string contains)
    {
        return this.Get<BoardSettings>().EnableDisplayName
                   ? this.GetRepository<User>().Get(
                       u => u.DisplayName.Contains(contains) &&
                            u.BoardID == this.Get<BoardSettings>().BoardId)
                   : this.GetRepository<User>().Get(
                       u => u.Name.Contains(contains) && u.BoardID == this.Get<BoardSettings>().BoardId);
    }

    /// <summary>
    /// Find PageUser By (Display) Name
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The <see cref="User"/>.
    /// </returns>
    public User FindUserByName(string name)
    {
        return this.Get<BoardSettings>().EnableDisplayName
                   ? this.GetRepository<User>().GetSingle(
                       u => u.DisplayName == name &&
                            u.BoardID == this.Get<BoardSettings>().BoardId)
                   : this.GetRepository<User>().GetSingle(
                       u => u.Name == name && u.BoardID == this.Get<BoardSettings>().BoardId);
    }

    /// <summary>
    /// Get the Display Name from a <paramref name="userId"/>
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetNameById(int userId)
    {
        var user = this.GetRepository<User>().GetById(userId);

        return user?.DisplayOrUserName();
    }
}