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

/// <summary>
/// The YAF DateTime Interface
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    ///   Gets the time zone offset
    ///   for the current user.
    /// </summary>
    TimeSpan TimeOffset { get; }

    /// <summary>
    /// Formats a DateTime value into 7. february 2003
    /// </summary>
    /// <param name="dateTime">
    /// The date to be formatted
    /// </param>
    /// <returns>
    /// The format date long.
    /// </returns>
    string FormatDateLong(DateTime dateTime);

    /// <summary>
    /// Formats a DateTime value into 07.03.2003
    /// </summary>
    /// <param name="dateTime">
    /// The date Time.
    /// </param>
    /// <returns>
    /// Short formatted date.
    /// </returns>
    string FormatDateShort(DateTime dateTime);

    /// <summary>
    /// Formats a DateTime value into 07.03.2003 22:32:34
    /// </summary>
    /// <param name="dateTime">
    /// The date Time.
    /// </param>
    /// <returns>
    /// Formatted  <see cref="string"/> of the formatted <see cref="DateTime"/> Object.
    /// </returns>
    string FormatDateTime(DateTime dateTime);

    /// <summary>
    /// This formats a DateTime into a short string
    /// </summary>
    /// <param name="dateTime">
    /// The date Time.
    /// </param>
    /// <returns>
    /// The formatted string created from the DateTime object.
    /// </returns>
    string FormatDateTimeShort(DateTime dateTime);

    /// <summary>
    /// Formats a DateTime value into 07.03.2003 00:00:00 except if
    ///   the date is yesterday or today -- in which case it says that.
    /// </summary>
    /// <param name="dateTime">
    /// The Date Time.
    /// </param>
    /// <returns>
    /// Formatted string of Date Time object
    /// </returns>
    string FormatDateTimeTopic(DateTime dateTime);

    /// <summary>
    /// Formats a DateTime value into 22:32:34
    /// </summary>
    /// <param name="dateTime">
    /// The date to be formatted
    /// </param>
    /// <returns>
    /// The format time.
    /// </returns>
    string FormatTime(DateTime dateTime);

    /// <summary>
    /// Gets the user DateTime.
    /// </summary>
    /// <param name="dateTime">The Date Time.</param>
    /// <returns>Returns the user Date Time</returns>
    DateTime GetUserDateTime(DateTime dateTime);

    /// <summary>
    /// Gets the user DateTime.
    /// </summary>
    /// <param name="dateTime">The Date Time.</param>
    /// <param name="timeZone">The time zone.</param>
    /// <returns>
    /// Returns the user Date Time
    /// </returns>
    DateTime GetUserDateTime(DateTime dateTime, TimeZoneInfo timeZone);
}