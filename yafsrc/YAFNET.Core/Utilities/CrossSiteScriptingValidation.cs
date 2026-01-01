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

namespace YAF.Core.Utilities;

using Newtonsoft.Json;

/// <summary>
/// Imported from System.Web.CrossSiteScriptingValidation Class
/// </summary>
public static class CrossSiteScriptingValidation
{
    /// <summary>
    /// The starting chars
    /// </summary>
    private readonly static char[] StartingChars = ['<', '&'];

    /// <summary>
    /// Determines whether [is dangerous string] [the specified s].
    /// </summary>
    /// <param name="s">The s.</param>
    /// <param name="matchIndex">Index of the match.</param>
    /// <returns><c>true</c> if [is dangerous string] [the specified s]; otherwise, <c>false</c>.</returns>
    public static bool IsDangerousString(string s, out int matchIndex)
    {
        matchIndex = 0;

        for (var i = 0; ;)
        {
            // Look for the start of one of our patterns
            var n = s.IndexOfAny(StartingChars, i);

            // If not found, the string is safe
            if (n < 0)
            {
                return false;
            }

            // If it's the last char, it's safe
            if (n == s.Length - 1)
            {
                return false;
            }

            matchIndex = n;

            switch (s[n])
            {
                case '<':
                    // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
                    if (IsAtoZ(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?')
                    {
                        return true;
                    }

                    break;
                case '&':
                    // If the & is followed by a #, it's unsafe (e.g. S)
                    if (s[n + 1] == '#')
                    {
                        return true;
                    }

                    break;
            }

            // Continue searching
            i = n + 1;
        }
    }

    /// <summary>
    /// Determines whether [is ato z] [the specified c].
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns><c>true</c> if [is ato z] [the specified c]; otherwise, <c>false</c>.</returns>
    private static bool IsAtoZ(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    }

    /// <summary>
    /// Adds the headers.
    /// </summary>
    /// <param name="headers">The headers.</param>
    public static void AddHeaders(this IHeaderDictionary headers)
    {
        if (headers["P3P"].NullOrEmpty())
        {
            headers.Append("P3P", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
        }
    }

    /// <summary>
    /// Converts to json.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string ToJson(this object value)
    {
        return JsonConvert.SerializeObject(value);
    }
}