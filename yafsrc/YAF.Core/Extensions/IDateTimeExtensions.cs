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

namespace YAF.Core
{
  #region Using

  using System;

  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The yaf date time extensions.
  /// </summary>
  public static class IDateTimeExtensions
  {
    #region Public Methods

    /// <summary>
    /// Format objectDateTime according to the format enum. "[error]" if the value is invalid.
    /// </summary>
    /// <param name="dateTimeInstance">
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
    public static string Format([NotNull] this IDateTime dateTimeInstance, DateTimeFormat format, [NotNull] object objectDateTime)
    {
      CodeContracts.VerifyNotNull(dateTimeInstance, "dateTimeInstance");
      CodeContracts.VerifyNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);

        switch (format)
        {
          case DateTimeFormat.BothDateShort:
            return dateTimeInstance.FormatDateTimeShort(dateTime);

          case DateTimeFormat.BothTopic:
            return dateTimeInstance.FormatDateTimeTopic(dateTime);

          case DateTimeFormat.DateLong:
            return dateTimeInstance.FormatDateLong(dateTime);

          case DateTimeFormat.DateShort:
            return dateTimeInstance.FormatDateShort(dateTime);

          case DateTimeFormat.Time:
            return dateTimeInstance.FormatTime(dateTime);
          
          case DateTimeFormat.Both:
          default:
            return dateTimeInstance.FormatDateTime(dateTime);

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
    /// <param name="dateTimeInstance">
    /// </param>
    /// <param name="objectDateTime">
    /// </param>
    /// <returns>
    /// The format date long.
    /// </returns>
    public static string FormatDateLong([NotNull] this IDateTime dateTimeInstance, [NotNull] object objectDateTime)
    {
      CodeContracts.VerifyNotNull(dateTimeInstance, "dateTimeInstance");
      CodeContracts.VerifyNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return dateTimeInstance.FormatDateLong(dateTime);
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
    /// <param name="dateTimeInstance">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format date short.
    /// </returns>
    public static string FormatDateShort([NotNull] this IDateTime dateTimeInstance, [NotNull] object objectDateTime)
    {
      CodeContracts.VerifyNotNull(dateTimeInstance, "dateTimeInstance");
      CodeContracts.VerifyNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return dateTimeInstance.FormatDateShort(dateTime);
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
    /// <param name="dateTimeInstance">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format date time.
    /// </returns>
    public static string FormatDateTime([NotNull] this IDateTime dateTimeInstance, [NotNull] object objectDateTime)
    {
      CodeContracts.VerifyNotNull(dateTimeInstance, "dateTimeInstance");
      CodeContracts.VerifyNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return dateTimeInstance.FormatDateTime(dateTime);
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
    /// <param name="dateTimeInstance">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format date time short.
    /// </returns>
    public static string FormatDateTimeShort([NotNull] this IDateTime dateTimeInstance, [NotNull] object objectDateTime)
    {
      CodeContracts.VerifyNotNull(dateTimeInstance, "dateTimeInstance");
      CodeContracts.VerifyNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return dateTimeInstance.FormatDateTimeShort(dateTime);
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
    /// <param name="dateTimeInstance">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format date time topic.
    /// </returns>
    public static string FormatDateTimeTopic([NotNull] this IDateTime dateTimeInstance, [NotNull] object objectDateTime)
    {
      CodeContracts.VerifyNotNull(dateTimeInstance, "dateTimeInstance");
      CodeContracts.VerifyNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return dateTimeInstance.FormatDateTimeTopic(dateTime);
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
    /// <param name="dateTimeInstance">
    /// The yaf date time.
    /// </param>
    /// <param name="objectDateTime">
    /// The object date time.
    /// </param>
    /// <returns>
    /// The format time.
    /// </returns>
    public static string FormatTime([NotNull] this IDateTime dateTimeInstance, [NotNull] object objectDateTime)
    {
      CodeContracts.VerifyNotNull(dateTimeInstance, "dateTimeInstance");
      CodeContracts.VerifyNotNull(objectDateTime, "objectDateTime");

      try
      {
        DateTime dateTime = Convert.ToDateTime(objectDateTime);
        return dateTimeInstance.FormatDateTime(dateTime);
      }
      catch
      {
        // failed convert...
        return "[error]";
      }
    }

    /// <summary>
    /// The minimal date time suitable for database.
    /// </summary>
    /// <param name="dateTimeInstance">
    /// The yaf date time.
    /// </param>
    /// <returns>
    /// Returnes the minimal date time suitable for database.
    /// </returns>
    public static DateTime SqlDbMinTime([NotNull] this DateTime dateTimeInstance)
    {
        return DateTime.MinValue.AddYears(1902);
    }


    #endregion
  }
}