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
/// The footer html helper.
/// </summary>
public static class IconHeaderHtmlHelper
{
    /// <summary>
    /// The footer helper.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="iconName">
    /// The icon Name.
    /// </param>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <param name="iconType">
    /// The icon Type.
    /// </param>
    /// <param name="iconSize">
    /// The icon Size.
    /// </param>
    /// <param name="param0">
    /// Parameter 0
    /// </param>
    /// <param name="param1">
    /// Parameter 1
    /// </param>
    /// <param name="param2">
    /// Parameter 2
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent IconHeader(
        this IHtmlHelper htmlHelper,
        string iconName,
        string page,
        string tag,
        string iconSize = "",
        string iconType = "text-secondary",
        string param0 = "",
        string param1 = "",
        string param2 = "")
    {
        var content = new HtmlContentBuilder();

        var icon = htmlHelper.Icon(iconName, iconType, "fas", iconSize);

        var header = BoardContext.Current.Get<ILocalization>().GetText(page, tag).FormatWith(param0, param1, param2);

        content.AppendHtml(icon);

        content.AppendHtml(header);

        return content;
    }
}