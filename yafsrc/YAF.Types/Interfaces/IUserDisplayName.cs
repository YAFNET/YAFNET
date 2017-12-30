/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Interfaces
{
  using System.Collections.Generic;

  /// <summary>
  /// User Display Name interface.
  /// </summary>
  public interface IUserDisplayName
  {
    /// <summary>
    /// Get the Display Name from a <paramref name="userId"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    string GetName(int userId);

    /// <summary>
    /// Get the <paramref name="userId"/> from the user name.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// </returns>
    int? GetId(string name);

    /// <summary>
    /// Find a displayName value.
    /// </summary>
    /// <param name="contains">
    /// The contains.
    /// </param>
    /// <returns>
    /// </returns>
    IDictionary<int, string> Find(string contains);

    /// <summary>
    /// Clears a user value (if there is one) for <paramref name="userId"/> from the cache
    /// </summary>
    /// <param name="userId"></param>
    void Clear(int userId);

    /// <summary>
    /// Clears the display name cache (if there is one)
    /// </summary>
    void Clear();
  }
}