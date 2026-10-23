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

namespace YAF.Types.Interfaces.Services;

using YAF.Types.Objects;

/// <summary>
/// The Reactions interface.
/// </summary>
public interface IReactions
{
    /// <summary>
    /// Creates the reaction info of the message for the current user.
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <returns>
    /// Returns the Reaction Info
    /// </returns>
    ReactionInfo GetReactionInfo(int messageId);

    /// <summary>
    /// Creates the HTML of the reactions box of the message: the given reactions
    ///   with their counts, and the reaction picker if the current user can react.
    /// </summary>
    /// <param name="messageId">
    /// The Message Id.
    /// </param>
    /// <param name="canReact">
    /// If the current user can react to the message.
    /// </param>
    /// <returns>
    /// The reactions HTML.
    /// </returns>
    string ReactionsHtml(int messageId, bool canReact);
}
