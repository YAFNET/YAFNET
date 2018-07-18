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

namespace YAF.Core.Services
{
    #region Using

    using System;

    using FarsiLibrary.Utils;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The YAF DateTime.
    /// </summary>
    public class YafDateTime : IDateTime
    {
        #region Constants and Fields

        /// <summary>
        ///   Time zone suffix for Guests
        /// </summary>
        private readonly string timeZoneName = YafContext.Current.Get<ILocalization>().GetText("TIMEZONES", "NAME_UTC");

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the time zone offset 
        ///   for the current user.
        /// </summary>
        public TimeSpan TimeOffset
        {
            get
            {
                if (YafContext.Current.Page == null)
                {
                    return new TimeSpan(0, YafContext.Current.Get<YafBoardSettings>().ServerTimeCorrection, 0);
                }

                var min = YafContext.Current.TimeZoneUserOffSet;
                var hrs = min / 60;

                return new TimeSpan(
                    hrs,
                    (min % 60) + YafContext.Current.Get<YafBoardSettings>().ServerTimeCorrection,
                    0);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Formats a DateTime value into 7. february 2003
        /// </summary>
        /// <param name="dateTime">
        /// The date to be formatted
        /// </param>
        /// <returns>
        /// The format date long.
        /// </returns>
        public string FormatDateLong(DateTime dateTime)
        {
            string dateFormat;
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, YafContext.Current.TimeZoneInfoUser);

            try
            {
                dateFormat =
                    YafContext.Current.Get<ILocalization>()
                        .FormatDateTime(YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_LONG"), dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("D");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("D")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(dateFormat, this.timeZoneName)
                             : dateFormat;
        }

        /// <summary>
        /// Formats a DateTime value into 07.03.2003
        /// </summary>
        /// <param name="dateTime">
        /// The date Time.
        /// </param>
        /// <returns>
        /// Short formatted date.
        /// </returns>
        public string FormatDateShort([NotNull] DateTime dateTime)
        {
            string dateFormat;
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, YafContext.Current.TimeZoneInfoUser);

            try
            {
                dateFormat =
                    YafContext.Current.Get<ILocalization>()
                        .FormatDateTime(YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_SHORT"), dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("d");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("d")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(dateFormat, this.timeZoneName)
                             : dateFormat;
        }

        /// <summary>
        /// Formats a DateTime value into 07.03.2003 22:32:34
        /// </summary>
        /// <param name="dateTime">
        /// The date Time.
        /// </param>
        /// <returns>
        /// Formatted  <see cref="string"/> of the formatted <see cref="DateTime"/> Object.
        /// </returns>
        public string FormatDateTime([NotNull] DateTime dateTime)
        {
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, YafContext.Current.TimeZoneInfoUser);

            string dateFormat;

            try
            {
                dateFormat =
                    YafContext.Current.Get<ILocalization>()
                        .FormatDateTime(
                            YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_LONG"),
                            dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("F");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString()
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(dateFormat, this.timeZoneName)
                             : dateFormat;
        }

        /// <summary>
        /// This formats a DateTime into a short string
        /// </summary>
        /// <param name="dateTime">
        /// The date Time.
        /// </param>
        /// <returns>
        /// The formatted string created from the DateTime object.
        /// </returns>
        public string FormatDateTimeShort([NotNull] DateTime dateTime)
        {
            string dateFormat;

            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, YafContext.Current.TimeZoneInfoUser);

            try
            {
                dateFormat =
                    YafContext.Current.Get<ILocalization>()
                        .FormatDateTime(
                            YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_SHORT"),
                            dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("G");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("G")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(dateFormat, this.timeZoneName)
                             : dateFormat;
        }

        /// <summary>
        /// Formats a DateTime value into 07.03.2003 00:00:00 except if 
        ///   the date is yesterday or today -- in which case it says that.
        /// </summary>
        /// <param name="dateTime">
        /// The date Time.
        /// </param>
        /// <returns>
        /// Formatted string of DateTime object
        /// </returns>
        public string FormatDateTimeTopic([NotNull] DateTime dateTime)
        {
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, YafContext.Current.TimeZoneInfoUser);
            var nowDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, YafContext.Current.TimeZoneInfoUser);

            string dateFormat;
            try
            {
                if (dateTime.Date == nowDateTime.Date)
                {
                    // today
                    dateFormat =
                        YafContext.Current.Get<ILocalization>()
                            .FormatString(YafContext.Current.Get<ILocalization>().GetText("TodayAt"), dateTime);
                }
                else if (dateTime.Date == nowDateTime.AddDays(-1).Date)
                {
                    // yesterday
                    dateFormat =
                        YafContext.Current.Get<ILocalization>()
                            .FormatString(YafContext.Current.Get<ILocalization>().GetText("YesterdayAt"), dateTime);
                }
                else
                {
                    dateFormat =
                        YafContext.Current.Get<ILocalization>()
                            .FormatDateTime(
                                YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_SHORT"),
                                dateTime);
                }
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("G");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("G")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(dateFormat, this.timeZoneName)
                             : dateFormat;
        }

        /// <summary>
        /// Formats a DateTime value into 22:32:34
        /// </summary>
        /// <param name="dateTime">
        /// The date to be formatted
        /// </param>
        /// <returns>
        /// The format time.
        /// </returns>
        public string FormatTime(DateTime dateTime)
        {
            string dateFormat;

            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, YafContext.Current.TimeZoneInfoUser);

            try
            {
                dateFormat =
                    YafContext.Current.Get<ILocalization>()
                        .FormatDateTime(YafContext.Current.Get<ILocalization>().GetText("FORMAT_TIME"), dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("T");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("T")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(dateFormat, this.timeZoneName)
                             : dateFormat;
        }

        /// <summary>
        /// Gets the user DateTime.
        /// </summary>
        /// <param name="dateTime">The Date Time.</param>
        /// <returns>Returns the user Date Time</returns>
        public DateTime GetUserDateTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, YafContext.Current.TimeZoneInfoUser);
        }

        /// <summary>
        /// Gets the user DateTime.
        /// </summary>
        /// <param name="dateTime">The Date Time.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns>
        /// Returns the user Date Time
        /// </returns>
        public DateTime GetUserDateTime(DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }

        #endregion
    }
}