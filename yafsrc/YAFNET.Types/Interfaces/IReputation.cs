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

namespace YAF.Types.Interfaces;

/// <summary>
/// Reputation Interface
/// </summary>
public interface IReputation
{
    /// <summary>
    /// Checks if allow reputation voting.
    /// </summary>
    /// <param name="voteDateToCheck">The last vote date to check.</param>
    /// <returns>
    /// Returns if the Users is allowed to Vote
    /// </returns>
    bool CheckIfAllowReputationVoting(object voteDateToCheck);

    /// <summary>
    /// Generate The Reputation Bar for the user
    /// </summary>
    /// <param name="points">
    /// The points.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// Returns the Html String
    /// </returns>
    string GenerateReputationBar(int points, int userId);

    /// <summary>
    /// Gets the reputation bar text.
    /// </summary>
    /// <param name="percentage">The percentage.</param>
    /// <returns>Returns the Text for the Current Value</returns>
    string GetReputationBarText(float percentage);

    /// <summary>
    /// Gets the reputation bar color.
    /// </summary>
    /// <param name="percentage">The percentage.</param>
    /// <returns>Returns the Color for the Current Value</returns>
    string GetReputationBarColor(float percentage);

    /// <summary>
    /// Converts the points to percentage.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <returns>Returns the Percentage Value</returns>
    float ConvertPointsToPercentage(int points);
}