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

namespace YAF.Core.Model;

using YAF.Types.Models;

/// <summary>
/// The Choice Repository Extensions
/// </summary>
public static class ChoiceRepositoryExtensions
{
    /// <summary>
    /// Add new Choice to the Poll
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="pollId">
    /// The poll id.
    /// </param>
    /// <param name="choice">
    /// The choice.
    /// </param>
    /// <param name="objectPath">
    /// The object path.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int AddChoice(
        this IRepository<Choice> repository,
        int pollId,
        string choice,
        string objectPath)
    {
        

        var entity = new Choice { PollID = pollId, ChoiceName = choice, Votes = 0, ObjectPath = objectPath };

        var newId = repository.Insert(entity);

        repository.FireNew(entity);

        return newId;
    }

    /// <summary>
    /// Update Choice
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="choiceId">
    /// The choice id.
    /// </param>
    /// <param name="choice">
    /// The choice.
    /// </param>
    /// <param name="objectPath">
    /// The object path.
    /// </param>
    public static void UpdateChoice(
        this IRepository<Choice> repository,
        int choiceId,
        string choice,
        string objectPath)
    {
        

        repository.UpdateOnly(
            () => new Choice { ChoiceName = choice, ObjectPath = objectPath },
            c => c.ID == choiceId);
    }

    /// <summary>
    /// Ads A Vote to the Choice
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="choiceId">
    /// The choice id.
    /// </param>
    public static void Vote(this IRepository<Choice> repository, int choiceId)
    {
        

        repository.UpdateAdd(() => new Choice { Votes = 1 }, a => a.ID == choiceId);
    }
}