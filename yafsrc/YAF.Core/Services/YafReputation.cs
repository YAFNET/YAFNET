/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Core.Services
{
    using System;
    using System.Globalization;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Class to Generate The Reputation Bar
    /// </summary>
    public class YafReputation
    {
        /// <summary>
        /// Checks if allow reputation voting.
        /// </summary>
        /// <param name="voteDateToCheck">The last vote date to check.</param>
        /// <returns>
        /// Returns if the Users is allowed to Vote
        /// </returns>
        public static bool CheckIfAllowReputationVoting(object voteDateToCheck)
        {
            if (voteDateToCheck.IsNullOrEmptyDBField())
            {
                return true;
            }

            var reputationVoteDate = voteDateToCheck.ToType<DateTime>();

            return reputationVoteDate < DateTime.UtcNow.AddHours(-24);
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
        public static string GenerateReputationBar([NotNull]int points, [NotNull]int userId)
        {
            var formatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };

            float percentage = ConvertPointsToPercentage(points);

                var pointsSign = string.Empty;

                if (points > 0)
                {
                    pointsSign = "+";
                }
                else if (points < 0)
                {
                    pointsSign = "-";
                }

            return
                @"<div class=""ReputationBar ReputationUser_{2}"" data-percent=""{0}"" data-text=""{1}"" title=""{3}{4}""></div>".FormatWith(
                        percentage.ToString(formatInfo), GetReputationBarText(percentage), userId, pointsSign, points);
        }

        /// <summary>
        /// Gets the reputation bar text.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <returns>Returns the Text for the Current Value</returns>
        [NotNull]
        public static string GetReputationBarText([NotNull]float percentage)
        {
            string text;

            if (percentage.Equals(0))
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "HATED");
            }
            else if (percentage < 20)
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "HOSTILE");
            }
            else if (percentage < 30)
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "HOSTILE");
            }
            else if (percentage < 40)
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "HOSTILE");
            }
            else if (percentage < 50)
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "UNFRIENDLY");
            }
            else if (percentage < 60)
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "NEUTRAL");
            }
            else if (percentage < 80)
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "FRIENDLY");
            }
            else if (percentage < 90)
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "HONORED");
            }
            else
            {
                text = YafContext.Current.Get<ILocalization>().GetText("REPUTATION_VALUES", "EXALTED");
            }

            return text;
        }

        /// <summary>
        /// Converts the points to percentage.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>Returns the Percentage Value</returns>
        [NotNull]
        public static float ConvertPointsToPercentage([NotNull]int points)
        {
            int percantage = points;

            int minValue = YafContext.Current.Get<YafBoardSettings>().ReputationMaxNegative;

            int maxValue = YafContext.Current.Get<YafBoardSettings>().ReputationMaxPositive;

            if (!YafContext.Current.Get<YafBoardSettings>().ReputationAllowNegative)
            {
                minValue = 0;
            }

            int testValue = minValue + maxValue;

            if (percantage.Equals(0) && YafContext.Current.Get<YafBoardSettings>().ReputationAllowNegative)
            {
                return 50;
            }

            if (percantage >= maxValue)
            {
                return 100;
            }

            if (percantage <= minValue)
            {
                return 0;
            }

            //// ((100 / (float)(maxValue * 2)) * percantage) + 50;

            var returnValue = ((100 / (float)testValue) * percantage) + 50;

            return returnValue > 100 ? 100 : returnValue;
        }
    }
}
