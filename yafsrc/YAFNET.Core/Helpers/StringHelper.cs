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

using System.Web;

/// <summary>
/// Class StringHelper.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Gets the abbreviation.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>System.String.</returns>
    public static string GetAbbreviation(this string data)
    {
        var trimmedData = data.Unidecode().Trim();

        if (trimmedData.Contains(' '))
        {
            var splitted = trimmedData.Split(' ');
            return $"{splitted[0].ToUpper()[0]}{splitted[1].ToUpper()[0]}";
        }

        var pascalCase = trimmedData;

        pascalCase = trimmedData.ToUpper()[0] + pascalCase[1..];

        var upperCaseOnly = string.Concat(pascalCase.Where(char.IsUpper));

        if (upperCaseOnly.Length is > 1 and <= 3)
        {
            return upperCaseOnly.ToUpper();
        }

        return trimmedData.Length <= 3 ? trimmedData.ToUpper() : trimmedData[..3].ToUpper();
    }

    /// <summary>
    /// Determines whether the text is Html Encoded or not
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>Returns if text is Html Encoded or not</returns>
    public static bool IsHtmlEncoded(string text)
    {
        if (text.Contains('<'))
        {
            return false;
        }

        if (text.Contains('>'))
        {
            return false;
        }

        if (text.Contains('\"'))
        {
            return false;
        }

        if (text.Contains('\''))
        {
            return false;
        }

        // if decoded string == original string, it is already encoded
        return HttpUtility.HtmlDecode(text) != text;
    }
}