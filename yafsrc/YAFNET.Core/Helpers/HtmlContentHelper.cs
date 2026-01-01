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

namespace YAF.Core.Helpers;

using System.IO;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.WebEncoders.Testing;

/// <summary>
/// Class HtmlContentHelper.
/// </summary>
public static class HtmlContentHelper
{
    /// <summary>
    /// Render Control as String
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="encoder">The encoder.</param>
    /// <returns>System.String.</returns>
    public static string RenderToString(this IHtmlContent content, HtmlEncoder encoder = null)
    {
        encoder ??= new HtmlTestEncoder();

        using var writer = new StringWriter();
        content.WriteTo(writer, encoder);
        return writer.ToString().Replace("HtmlEncode[[", "").Replace("]]", "");
    }
}