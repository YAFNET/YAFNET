/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.Classes.Core
{
  #region Using

  using System;

  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// The yaf date time extensions.
  /// </summary>
  public static class YafDateTimeExtensions
  {
    #region Public Methods

    /// <summary>
    /// Format objectDateTime according to the format enum. "[error]" if the value is invalid.
    /// </summary>
    /// <param name="yafDateTime">
    /// The yaf date time.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// Formatted datetime or "[error]" if invalid.
    /// </returns>
    public static string Format([NotNull] this YafDateTime yafDateTime, DateTimeFormat format, [NotNull] object objectDateTime)
    {
      CodeContracts.ArgumentNotNull(yafDateTime, "yafDateTime");
      CodeContracts.ArgumentNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);

        switch (format)
        {
          case DateTimeFormat.BothDateShort:
            return yafDateTime.FormatDateTimeShort(dateTime);

          case DateTimeFormat.BothTopic:
            return yafDateTime.FormatDateTimeTopic(dateTime);

          case DateTimeFormat.DateLong:
            return yafDateTime.FormatDateLong(dateTime);

          case DateTimeFormat.DateShort:
            return yafDateTime.FormatDateShort(dateTime);

          case DateTimeFormat.Time:
            return yafDateTime.FormatTime(dateTime);
          
          case DateTimeFormat.Both:
          default:
            return yafDateTime.FormatDateTime(dateTime);

        }
      }
      catch
      {
      }

      // failed convert...
      return "[error]";
    }

    /// <summary>
    /// Format a date long -- using an object conversion.
    /// </summary>
    /// <param name="yafDateTime">
    /// </param>
    /// <param name="objectDateTime">
    /// </param>
    /// <returns>
    /// The format date long.
    /// </returns>
    public static string FormatDateLong([NotNull] this YafDateTime yafDateTime, [NotNull] object objectDateTime)
    {
      CodeContracts.ArgumentNotNull(yafDateTime, "yafDateTime");
      CodeContracts.ArgumentNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return yafDateTime.FormatDateLong(dateTime);
      }
      catch
      {
        // failed convert...
        return "[error]";
      }
    }

    /// <summary>
    /// The format date short.
    /// </summary>
    /// <param name="yafDateTime">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format date short.
    /// </returns>
    public static string FormatDateShort([NotNull] this YafDateTime yafDateTime, [NotNull] object objectDateTime)
    {
      CodeContracts.ArgumentNotNull(yafDateTime, "yafDateTime");
      CodeContracts.ArgumentNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return yafDateTime.FormatDateShort(dateTime);
      }
      catch
      {
        // failed convert...
        return "[error]";
      }
    }

    /// <summary>
    /// The format date time.
    /// </summary>
    /// <param name="yafDateTime">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format date time.
    /// </returns>
    public static string FormatDateTime([NotNull] this YafDateTime yafDateTime, [NotNull] object objectDateTime)
    {
      CodeContracts.ArgumentNotNull(yafDateTime, "yafDateTime");
      CodeContracts.ArgumentNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return yafDateTime.FormatDateTime(dateTime);
      }
      catch
      {
        // failed convert...
        return "[error]";
      }
    }

    /// <summary>
    /// The format date time short.
    /// </summary>
    /// <param name="yafDateTime">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format date time short.
    /// </returns>
    public static string FormatDateTimeShort([NotNull] this YafDateTime yafDateTime, [NotNull] object objectDateTime)
    {
      CodeContracts.ArgumentNotNull(yafDateTime, "yafDateTime");
      CodeContracts.ArgumentNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return yafDateTime.FormatDateTimeShort(dateTime);
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
    /// <param name="yafDateTime">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format date time topic.
    /// </returns>
    public static string FormatDateTimeTopic([NotNull] this YafDateTime yafDateTime, [NotNull] object objectDateTime)
    {
      CodeContracts.ArgumentNotNull(yafDateTime, "yafDateTime");
      CodeContracts.ArgumentNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return yafDateTime.FormatDateTimeTopic(dateTime);
      }
      catch
      {
        // failed convert...
        return "[error]";
      }
    }

    /// <summary>
    /// The format time.
    /// </summary>
    /// <param name="yafDateTime">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format time.
    /// </returns>
    public static string FormatTime([NotNull] this YafDateTime yafDateTime, [NotNull] object objectDateTime)
    {
      CodeContracts.ArgumentNotNull(yafDateTime, "yafDateTime");
      CodeContracts.ArgumentNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return yafDateTime.FormatDateTime(dateTime);
      }
      catch
      {
        // failed convert...
        return "[error]";
      }
    }

    #endregion
  }
}