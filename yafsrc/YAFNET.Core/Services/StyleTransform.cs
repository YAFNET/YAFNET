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

namespace YAF.Core.Services;

using System;

/// <summary>
/// Transforms the style.
/// </summary>
public class StyleTransform : IStyleTransform
{
    /// <summary>
    /// Decode Style by String
    /// </summary>
    /// <param name="style">The style string.</param>
    /// <returns>
    /// The decode style by string.
    /// </returns>
    public string Decode(string style)
    {
        if (style.IsNotSet())
        {
            return string.Empty;
        }

        var styleRow = style.Trim().Split('/');

        styleRow.Select(s => s.Split('!')).Where(x => x.Length > 1).ForEach(
            pair => style = GetColorOnly(pair[1]));

        return style;
    }

    /// <summary>
    /// Gets the color only
    /// </summary>
    /// <param name="styleString">
    /// The style string.
    /// </param>
    /// <returns>
    /// The get color only.
    /// </returns>
    private static string GetColorOnly(string styleString)
    {
        var styleArray = styleString.Split(';');

        return Array.Find(styleArray, t => t.ToLower().Contains("color"));
    }
}