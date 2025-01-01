/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
/// The user link html helper.
/// </summary>
public static class UserLabelHtmlHelper
{
    /// <summary>
    /// The user link.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent UserLabel(this IHtmlHelper htmlHelper, User user)
    {
        var content = new HtmlContentBuilder();
        var context = BoardContext.Current;

        var span = new TagBuilder(HtmlTag.Span);

        if (user.UserStyle.IsSet() && context.BoardSettings.UseStyledNicks)
        {
            var styleFormatted = context.Get<IStyleTransform>().Decode(user.UserStyle);

            span.MergeAttribute(HtmlAttribute.Style, HttpUtility.HtmlEncode(styleFormatted));
        }

        span.InnerHtml.AppendHtml(HttpUtility.HtmlEncode(user.DisplayOrUserName()));

        return content.AppendHtml(span);
    }

    /// <summary>
    /// The user link.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="userName"></param>
    /// <param name="userStyle"></param>
    /// <param name="crawlerName"></param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent UserLabel(this IHtmlHelper htmlHelper, string userName, string userStyle, string crawlerName = null)
    {
        var content = new HtmlContentBuilder();
        var context = BoardContext.Current;

        var span = new TagBuilder(HtmlTag.Span);

        if (userStyle.IsSet() && context.BoardSettings.UseStyledNicks)
        {
            var styleFormatted = context.Get<IStyleTransform>().Decode(userStyle);

            span.MergeAttribute(HtmlAttribute.Style, HttpUtility.HtmlEncode(styleFormatted));
        }

        span.InnerHtml.AppendHtml(HttpUtility.HtmlEncode(crawlerName.IsNotSet() ? userName : crawlerName));

        return content.AppendHtml(span);
    }
}