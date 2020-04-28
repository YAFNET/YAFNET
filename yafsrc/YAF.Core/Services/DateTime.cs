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
    #region Using

    using System;

    using FarsiLibrary.Utils;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The YAF DateTime.
    /// </summary>
    public class DateTime : IDateTime, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        ///   Time zone suffix for Guests
        /// </summary>
        private readonly string timeZoneName = BoardContext.Current.Get<ILocalization>().GetText("TIMEZONES", "NAME_UTC");

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTime"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public DateTime(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        ///   Gets the time zone offset 
        ///   for the current user.
        /// </summary>
        public TimeSpan TimeOffset
        {
            get
            {
                if (BoardContext.Current.Page == null)
                {
                    return new TimeSpan(0, this.Get<BoardSettings>().ServerTimeCorrection, 0);
                }

                var min = BoardContext.Current.TimeZoneUserOffSet;
                var hrs = min / 60;

                return new TimeSpan(
                    hrs,
                    min % 60 + this.Get<BoardSettings>().ServerTimeCorrection,
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
        public string FormatDateLong(System.DateTime dateTime)
        {
            string dateFormat;
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, BoardContext.Current.TimeZoneInfoUser);

            try
            {
                dateFormat =
                    this.Get<ILocalization>()
                        .FormatDateTime(this.Get<ILocalization>().GetText("FORMAT_DATE_LONG"), dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("D");
            }

            return BoardContext.Current.Get<BoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("D")
                       : BoardContext.Current.IsGuest
                             ? $"{dateFormat}{this.timeZoneName}"
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
        public string FormatDateShort([NotNull] System.DateTime dateTime)
        {
            string dateFormat;
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, BoardContext.Current.TimeZoneInfoUser);

            try
            {
                dateFormat =
                    BoardContext.Current.Get<ILocalization>()
                        .FormatDateTime(BoardContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_SHORT"), dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("d");
            }

            return BoardContext.Current.Get<BoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("d")
                       : BoardContext.Current.IsGuest
                             ? $"{dateFormat}{this.timeZoneName}"
                             : dateFormat;
        }

        /// <summary>
        /// Formats a DateTime value into 07.03.2003 22:32:34
        /// </summary>
        /// <param name="dateTime">
        /// The date Time.
        /// </param>
        /// <returns>
        /// Formatted  <see cref="string"/> of the formatted <see cref="System.DateTime"/> Object.
        /// </returns>
        public string FormatDateTime([NotNull] System.DateTime dateTime)
        {
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, BoardContext.Current.TimeZoneInfoUser);

            string dateFormat;

            try
            {
                dateFormat =
                    BoardContext.Current.Get<ILocalization>()
                        .FormatDateTime(
                            BoardContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_LONG"),
                            dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("F");
            }

            return BoardContext.Current.Get<BoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString()
                       : BoardContext.Current.IsGuest
                             ? $"{dateFormat}{this.timeZoneName}"
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
        public string FormatDateTimeShort([NotNull] System.DateTime dateTime)
        {
            string dateFormat;

            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, BoardContext.Current.TimeZoneInfoUser);

            try
            {
                dateFormat =
                    BoardContext.Current.Get<ILocalization>()
                        .FormatDateTime(
                            BoardContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_SHORT"),
                            dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("G");
            }

            return BoardContext.Current.Get<BoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("G")
                       : BoardContext.Current.IsGuest
                             ? $"{dateFormat}{this.timeZoneName}"
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
        public string FormatDateTimeTopic([NotNull] System.DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local)
            {
                dateTime = System.DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            }

            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, BoardContext.Current.TimeZoneInfoUser);
            var nowDateTime = TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.UtcNow, BoardContext.Current.TimeZoneInfoUser);

            string dateFormat;
            try
            {
                if (dateTime.Date == nowDateTime.Date)
                {
                    // today
                    dateFormat =
                        BoardContext.Current.Get<ILocalization>()
                            .FormatString(BoardContext.Current.Get<ILocalization>().GetText("TodayAt"), dateTime);
                }
                else if (dateTime.Date == nowDateTime.AddDays(-1).Date)
                {
                    // yesterday
                    dateFormat =
                        BoardContext.Current.Get<ILocalization>()
                            .FormatString(BoardContext.Current.Get<ILocalization>().GetText("YesterdayAt"), dateTime);
                }
                else
                {
                    dateFormat =
                        BoardContext.Current.Get<ILocalization>()
                            .FormatDateTime(
                                BoardContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_SHORT"),
                                dateTime);
                }
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("G");
            }

            return BoardContext.Current.Get<BoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("G")
                       : BoardContext.Current.IsGuest
                             ? $"{dateFormat}{this.timeZoneName}"
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
        public string FormatTime(System.DateTime dateTime)
        {
            string dateFormat;

            dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, BoardContext.Current.TimeZoneInfoUser);

            try
            {
                dateFormat =
                    BoardContext.Current.Get<ILocalization>()
                        .FormatDateTime(BoardContext.Current.Get<ILocalization>().GetText("FORMAT_TIME"), dateTime);
            }
            catch (Exception)
            {
                dateFormat = dateTime.ToString("T");
            }

            return BoardContext.Current.Get<BoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("T")
                       : BoardContext.Current.IsGuest
                             ? $"{dateFormat}{this.timeZoneName}"
                             : dateFormat;
        }

        /// <summary>
        /// Gets the user DateTime.
        /// </summary>
        /// <param name="dateTime">The Date Time.</param>
        /// <returns>Returns the user Date Time</returns>
        public System.DateTime GetUserDateTime(System.DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, BoardContext.Current.TimeZoneInfoUser);
        }

        /// <summary>
        /// Gets the user DateTime.
        /// </summary>
        /// <param name="dateTime">The Date Time.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns>
        /// Returns the user Date Time
        /// </returns>
        public System.DateTime GetUserDateTime(System.DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }

        #endregion
    }
}