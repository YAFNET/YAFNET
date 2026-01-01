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

namespace YAF.Core.Extensions;

using System;

/// <summary>
/// The DateTime extensions.
/// </summary>
public static class IDateTimeServiceExtensions
{
    /// <param name="dateTimeInstance">
    /// The datetime.
    /// </param>
    extension(IDateTimeService dateTimeInstance)
    {
        /// <summary>
        /// Format objectDateTime according to the format enum. "[error]" if the value is invalid.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="objectDateTime">
        /// The object date time.
        /// </param>
        /// <returns>
        /// Formatted datetime or "[error]" if invalid.
        /// </returns>
        public string Format(DateTimeFormat format,
            object objectDateTime)
        {
            ArgumentNullException.ThrowIfNull(dateTimeInstance);
            ArgumentNullException.ThrowIfNull(objectDateTime);

            try
            {
                var dateTime = Convert.ToDateTime(objectDateTime);

                return format switch
                {
                    DateTimeFormat.BothDateShort => dateTimeInstance.FormatDateTimeShort(dateTime),
                    DateTimeFormat.BothTopic => dateTimeInstance.FormatDateTimeTopic(dateTime),
                    DateTimeFormat.DateLong => dateTimeInstance.FormatDateLong(dateTime),
                    DateTimeFormat.DateShort => dateTimeInstance.FormatDateShort(dateTime),
                    DateTimeFormat.Time => dateTimeInstance.FormatTime(dateTime),
                    DateTimeFormat.Both => dateTimeInstance.FormatDateTime(dateTime),
                    _ => dateTimeInstance.FormatDateTime(dateTime)
                };
            }
            catch
            {
                // failed convert...
                return "[error]";
            }
        }

        /// <summary>
        /// The format date time topic.
        /// </summary>
        /// <param name="objectDateTime">
        /// The object date time.
        /// </param>
        /// <returns>
        /// The format date time topic.
        /// </returns>
        public string FormatDateTimeTopic(object objectDateTime)
        {
            ArgumentNullException.ThrowIfNull(dateTimeInstance);
            ArgumentNullException.ThrowIfNull(objectDateTime);

            try
            {
                var dateTime = Convert.ToDateTime(objectDateTime, CultureInfo.InvariantCulture);
                return dateTimeInstance.FormatDateTimeTopic(dateTime);
            }
            catch
            {
                // failed convert...
                return "[error]";
            }
        }
    }
}