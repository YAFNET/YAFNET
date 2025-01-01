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
namespace YAF.Types.Interfaces.Services;

using System.Collections.Generic;

/// <summary>
/// User Display Name interface.
/// </summary>
public interface IUserDisplayName
{
    /// <summary>
    /// Get the Display Name from a <paramref name="userId"/>
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    string GetNameById(int userId);

    /// <summary>
    /// Find user
    /// </summary>
    /// <param name="contains">The contains.</param>
    /// <returns>
    /// Returns the Found User
    /// </returns>
    
    IList<User> FindUserContainsName(string contains);

    /// <summary>
    /// Find User By (Display) Name
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The <see cref="User"/>.
    /// </returns>
    
    User FindUserByName(string name);
}