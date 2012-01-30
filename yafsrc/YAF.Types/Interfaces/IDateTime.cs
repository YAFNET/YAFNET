/* Yet Another Forum.NET
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
namespace YAF.Types.Interfaces
{
  using System;

  public interface IDateTime
  {
    /// <summary>
    ///   Gets the time zone offset 
    ///   for the current user.
    /// </summary>
    TimeSpan TimeOffset { get; }

    /// <summary>
    /// Formats a datetime value into 7. february 2003
    /// </summary>
    /// <param name="dateTime">
    /// The date to be formatted
    /// </param>
    /// <returns>
    /// The format date long.
    /// </returns>
    string FormatDateLong(DateTime dateTime);

    /// <summary>
    /// Formats a datetime value into 07.03.2003
    /// </summary>
    /// <param name="dateTime">
    /// The date Time.
    /// </param>
    /// <returns>
    /// Short formatted date.
    /// </returns>
    string FormatDateShort([NotNull] DateTime dateTime);

    /// <summary>
    /// Formats a datetime value into 07.03.2003 22:32:34
    /// </summary>
    /// <param name="dateTime">
    /// The date Time.
    /// </param>
    /// <returns>
    /// Formatted  <see cref="string"/> of the formatted <see cref="DateTime"/> Object.
    /// </returns>
    string FormatDateTime([NotNull] DateTime dateTime);

    /// <summary>
    /// This formats a DateTime into a short string
    /// </summary>
    /// <param name="dateTime">
    /// The date Time.
    /// </param>
    /// <returns>
    /// The formatted string created from the DateTime object.
    /// </returns>
    string FormatDateTimeShort([NotNull] DateTime dateTime);

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
    string FormatDateTimeTopic([NotNull] DateTime dateTime);

    /// <summary>
    /// Formats a datetime value into 22:32:34
    /// </summary>
    /// <param name="dateTime">
    /// The date to be formatted
    /// </param>
    /// <returns>
    /// The format time.
    /// </returns>
    string FormatTime(DateTime dateTime);
  }
}