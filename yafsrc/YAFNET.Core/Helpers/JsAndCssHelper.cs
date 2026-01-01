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

using System;

using YAF.Core.Utilities.MinifyUtils;

/// <summary>
/// The JS and CSS helper.
/// </summary>
public static class JsAndCssHelper
{
    /// <summary>
    /// Compresses JavaScript
    /// </summary>
    /// <param name="javaScript">
    /// The Uncompressed Input JS
    /// </param>
    /// <returns>
    /// The compressed javascript.
    /// </returns>
    public static string CompressJavaScript(string javaScript)
    {
        try
        {
            return JsMinify.Minify(javaScript);
        }
        catch (Exception)
        {
            return javaScript;
        }
    }

    /// <summary>
    /// Compresses CSS
    /// </summary>
    /// <param name="css">
    /// The Uncompressed Input CSS
    /// </param>
    /// <returns>
    /// The compressed CSS output.
    /// </returns>
    public static string CompressCss(string css)
    {
        try
        {
            return JsMinify.Minify(css);
        }
        catch (Exception)
        {
            return css;
        }
    }
}