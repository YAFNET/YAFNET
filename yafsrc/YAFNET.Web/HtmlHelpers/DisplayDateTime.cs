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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.HtmlHelpers;

/// <summary>
/// The display date time html helper.
/// </summary>
public static class DisplayDateTimeHtmlHelper
{
    /// <summary>
    /// The display date time.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="dateTime">
    /// The date time.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent DisplayDateTime(
        this IHtmlHelper htmlHelper,
        DateTimeFormat format,
        object dateTime)
    {
        var content = new HtmlContentBuilder();

        var context = BoardContext.Current;

        var formattedDatetime = context.Get<IDateTimeService>().Format(format, dateTime);

        var timeTag = new TagBuilder(HtmlTag.Abbr);

        timeTag.AddCssClass("timeago");

        timeTag.MergeAttribute(HtmlAttribute.Title, formattedDatetime);
        timeTag.MergeAttribute("data-bs-toggle", "tooltip");
        timeTag.MergeAttribute("data-bs-html", "true");

        timeTag.InnerHtml.AppendHtml(
            context.BoardSettings.ShowRelativeTime
                ? AsDateTime(dateTime).ToRelativeTime()
                : formattedDatetime);

        return content.AppendHtml(timeTag);
    }

    /// <summary>
    /// Gets AsDateTime.
    /// </summary>
    /// <param name="dateTime">
    /// The date Time.
    /// </param>
    /// <returns>
    /// The <see cref="DateTime"/>.
    /// </returns>
    private static DateTime AsDateTime(object dateTime)
    {
        if (dateTime is null)
        {
            return DateTimeHelper.SqlDbMinTime();
        }

        try
        {
            return Convert.ToDateTime(dateTime, CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException)
        {
            // not usable...
        }

        return DateTimeHelper.SqlDbMinTime();
    }
}