/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Class to Generate The Reputation Bar
    /// </summary>
    public class Reputation : IReputation, IHaveServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reputation"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public Reputation([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        /// <summary>
        /// Checks if allow reputation voting.
        /// </summary>
        /// <param name="voteDateToCheck">The last vote date to check.</param>
        /// <returns>
        /// Returns if the Users is allowed to Vote
        /// </returns>
        public bool CheckIfAllowReputationVoting(object voteDateToCheck)
        {
            if (voteDateToCheck.IsNullOrEmptyDBField())
            {
                return true;
            }

            var reputationVoteDate = voteDateToCheck.ToType<System.DateTime>();

            return reputationVoteDate < System.DateTime.UtcNow.AddHours(-24);
        }

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
        public string GenerateReputationBar([NotNull] int points, [NotNull] int userId)
        {
            var formatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };

            var percentage = this.Get<IReputation>().ConvertPointsToPercentage(points);

            return $@"<div class=""progress"">
                      <div class=""progress-bar progress-bar-striped{this.Get<IReputation>().GetReputationBarColor(percentage)}"" 
                           role=""progressbar""
                           style=""width:{percentage.ToString(formatInfo)}%;"" 
                           aria-valuenow=""{percentage.ToString(formatInfo)}"" 
                           aria-valuemax=""100"">
                      {percentage.ToString(formatInfo)}% ({GetReputationBarText(percentage)})
                      </div>
                  </div>";
        }

        /// <summary>
        /// Gets the reputation bar text.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <returns>Returns the Text for the Current Value</returns>
        [NotNull]
        public string GetReputationBarText([NotNull] float percentage)
        {
            var lookup = new Dictionary<int, string>
                             {
                                 { 0, "HATED" },
                                 { 20, "HOSTILE" },
                                 { 30, "HOSTILE" },
                                 { 40, "HOSTILE" },
                                 { 50, "UNFRIENDLY" },
                                 { 60, "NEUTRAL" },
                                 { 80, "FRIENDLY" },
                                 { 90, "HONORED" },
                                 { int.MaxValue, "EXALTED" }
                             };

            var pageName = "HATED";

            if (percentage > 1)
            {
                pageName =
                    lookup.OrderBy(s => s.Key).Where(x => percentage < x.Key).Select(x => x.Value).FirstOrDefault();
            }

            return BoardContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", pageName);
        }

        /// <summary>
        /// Gets the reputation bar color.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <returns>Returns the Color for the Current Value</returns>
        [NotNull]
        public string GetReputationBarColor([NotNull] float percentage)
        {
            var lookup = new Dictionary<int, string>
                             {
                                 { 0, " bg-danger" },
                                 { 20, " bg-warning" },
                                 { 30, " bg-warning" },
                                 { 40, " bg-warning" },
                                 { 50, " bg-info" },
                                 { 60, " bg-info" },
                                 { 80, " bg-success" },
                                 { 90, " bg-success" },
                                 { int.MaxValue, " bg-success" }
                             };

            var color = "bg-danger";

            if (percentage > 1)
            {
                color =
                    lookup.OrderBy(s => s.Key).Where(x => percentage < x.Key).Select(x => x.Value).FirstOrDefault();
            }

            return color;
        }

        /// <summary>
        /// Converts the points to percentage.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>Returns the Percentage Value</returns>
        [NotNull]
        public float ConvertPointsToPercentage([NotNull] int points)
        {
            var percentage = points;

            var minValue = BoardContext.Current.Get<BoardSettings>().ReputationMaxNegative;

            var maxValue = BoardContext.Current.Get<BoardSettings>().ReputationMaxPositive;

            if (!BoardContext.Current.Get<BoardSettings>().ReputationAllowNegative)
            {
                minValue = 0;
            }

            var testValue = minValue + maxValue;

            if (percentage.Equals(0) && this.Get<BoardSettings>().ReputationAllowNegative)
            {
                return 50;
            }

            if (percentage >= maxValue)
            {
                return 100;
            }

            if (percentage <= minValue)
            {
                return 0;
            }

            var returnValue = 100 / (float)testValue * percentage + 50;

            return returnValue > 100 ? 100 : returnValue;
        }
    }
}
