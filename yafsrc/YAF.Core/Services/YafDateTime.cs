/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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
    #region Using

    using System;

    using FarsiLibrary;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The yaf date time.
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
                if (YafContext.Current.Page != null)
                {
                    int min = YafContext.Current.TimeZoneUser;
                    int hrs = min / 60;

                    return new TimeSpan(
                        hrs, (min % 60) + YafContext.Current.Get<YafBoardSettings>().ServerTimeCorrection, 0);
                }

                return new TimeSpan(0, YafContext.Current.Get<YafBoardSettings>().ServerTimeCorrection, 0);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Formats a datetime value into 7. february 2003
        /// </summary>
        /// <param name="dateTime">
        /// The date to be formatted
        /// </param>
        /// <returns>
        /// The format date long.
        /// </returns>
        public string FormatDateLong(DateTime dateTime)
        {
            string strDateFormat;
            dateTime = this.AccountForDST(dateTime + this.TimeOffset);

            try
            {
                strDateFormat =
                    YafContext.Current.Get<ILocalization>().FormatDateTime(
                        YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_LONG"), dateTime);
            }
            catch (Exception)
            {
                strDateFormat = dateTime.ToString("D");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("D")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(strDateFormat, this.timeZoneName)
                             : strDateFormat;
        }

        /// <summary>
        /// Formats a datetime value into 07.03.2003
        /// </summary>
        /// <param name="dateTime">
        /// The date Time.
        /// </param>
        /// <returns>
        /// Short formatted date.
        /// </returns>
        public string FormatDateShort([NotNull] DateTime dateTime)
        {
            string strDateFormat;
            dateTime = this.AccountForDST(dateTime + this.TimeOffset);

            try
            {
                strDateFormat =
                    YafContext.Current.Get<ILocalization>().FormatDateTime(
                        YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_SHORT"), dateTime);
            }
            catch (Exception)
            {
                strDateFormat = dateTime.ToString("d");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("d")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(strDateFormat, this.timeZoneName)
                             : strDateFormat;
        }

        /// <summary>
        /// Formats a datetime value into 07.03.2003 22:32:34
        /// </summary>
        /// <param name="dateTime">
        /// The date Time.
        /// </param>
        /// <returns>
        /// Formatted  <see cref="string"/> of the formatted <see cref="DateTime"/> Object.
        /// </returns>
        public string FormatDateTime([NotNull] DateTime dateTime)
        {
            dateTime = this.AccountForDST(dateTime + this.TimeOffset);

            string strDateFormat;

            try
            {
                strDateFormat =
                    YafContext.Current.Get<ILocalization>().FormatDateTime(
                        YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_LONG"), dateTime);
            }
            catch (Exception)
            {
                strDateFormat = dateTime.ToString("F");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString()
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(strDateFormat, this.timeZoneName)
                             : strDateFormat;
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
            string strDateFormat;

            dateTime = this.AccountForDST(dateTime + this.TimeOffset);

            try
            {
                strDateFormat =
                    YafContext.Current.Get<ILocalization>().FormatDateTime(
                        YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_SHORT"), dateTime);
            }
            catch (Exception)
            {
                strDateFormat = dateTime.ToString("G");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("G")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(strDateFormat, this.timeZoneName)
                             : strDateFormat;
        }

        /// <summary>
        /// Formats a datatime value into 07.03.2003 00:00:00 except if 
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
            dateTime = this.AccountForDST(dateTime + this.TimeOffset);
            DateTime nowDateTime = this.AccountForDST(DateTime.UtcNow + this.TimeOffset);

            string strDateFormat;
            try
            {
                if (dateTime.Date == nowDateTime.Date)
                {
                    // today
                    strDateFormat =
                        YafContext.Current.Get<ILocalization>().FormatString(
                            YafContext.Current.Get<ILocalization>().GetText("TodayAt"), dateTime);
                }
                else if (dateTime.Date == nowDateTime.AddDays(-1).Date)
                {
                    // yesterday
                    strDateFormat =
                        YafContext.Current.Get<ILocalization>().FormatString(
                            YafContext.Current.Get<ILocalization>().GetText("YesterdayAt"), dateTime);
                }
                else
                {
                    strDateFormat =
                        YafContext.Current.Get<ILocalization>().FormatDateTime(
                            YafContext.Current.Get<ILocalization>().GetText("FORMAT_DATE_TIME_SHORT"), dateTime);
                }
            }
            catch (Exception)
            {
                strDateFormat = dateTime.ToString("G");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("G")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(strDateFormat, this.timeZoneName)
                             : strDateFormat;
        }

        /// <summary>
        /// Formats a datetime value into 22:32:34
        /// </summary>
        /// <param name="dateTime">
        /// The date to be formatted
        /// </param>
        /// <returns>
        /// The format time.
        /// </returns>
        public string FormatTime(DateTime dateTime)
        {
            string strDateFormat;

            dateTime = this.AccountForDST(dateTime + this.TimeOffset);

            try
            {
                strDateFormat =
                    YafContext.Current.Get<ILocalization>().FormatDateTime(
                        YafContext.Current.Get<ILocalization>().GetText("FORMAT_TIME"), dateTime);
            }
            catch (Exception)
            {
                strDateFormat = dateTime.ToString("T");
            }

            return YafContext.Current.Get<YafBoardSettings>().UseFarsiCalender
                       ? PersianDateConverter.ToPersianDate(dateTime).ToString("T")
                       : YafContext.Current.IsGuest
                             ? "{0}{1}".FormatWith(strDateFormat, this.timeZoneName)
                             : strDateFormat;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines if the given date falls during DST and updates accordingly if the user chosen to allow for DST.
        /// </summary>
        /// <param name="dtCurrent">
        /// The date to be checked
        /// </param>
        /// <returns>
        /// DateTime object account for DST (if required).
        /// </returns>
        private DateTime AccountForDST(DateTime dtCurrent)
        {
            if (YafContext.Current.DSTUser)
            {
                if (TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").IsDaylightSavingTime(dtCurrent))
                {
                    return dtCurrent.AddHours(1);
                }
            }

            return dtCurrent;
        }

        #endregion
    }
}