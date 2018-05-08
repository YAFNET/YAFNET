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

namespace YAF.Utils.Helpers
{
    using System;

    using YAF.Types.Extensions;

    /// <summary>
    /// DateTime Helper
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// the sql compatible DateTime Min Value
        /// </summary>
        /// <returns>
        /// Returns the sql compatible DateTime Min Value
        /// </returns>
        public static DateTime SqlDbMinTime()
        {
            return DateTime.MinValue.AddYears(1902);
        }

        /// <summary>
        /// Gets the time zone by id
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Returns the Time Zone Info</returns>
        public static TimeZoneInfo GetTimeZoneInfo(string input)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(input);
            }
            catch (Exception)
            {
                return TimeZoneInfo.Local;
            }
        }

        /// <summary>
        /// Gets the time zone offset.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// Returns the Offset
        /// </returns>
        public static int GetTimeZoneOffset(string input)
        {
            TimeZoneInfo timeZone;

            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(input);
            }
            catch (Exception)
            {
                timeZone = TimeZoneInfo.Local;
            }

            return GetTimeZoneOffset(timeZone);
        }

        /// <summary>
        /// Gets the time zone offset.
        /// </summary>
        /// <param name="timeZoneInfo">The time zone information.</param>
        /// <returns>Returns the Offset</returns>
        public static int GetTimeZoneOffset(TimeZoneInfo timeZoneInfo)
        {
            var utcOffSet = timeZoneInfo.BaseUtcOffset;
            var timeZone = utcOffSet < TimeSpan.Zero
                               ? "-{0}".FormatWith(utcOffSet.ToString("hh"))
                               : utcOffSet.ToString("hh");

            return (timeZone.ToType<decimal>() * 60).ToType<int>();
        }
    }
}