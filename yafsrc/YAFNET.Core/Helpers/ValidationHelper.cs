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
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

/// <summary>
/// The validation helper.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Checks if string is a valid email address.
    /// </summary>
    /// <param name="email">
    /// The email string to check
    /// </param>
    /// <returns>
    /// Returns indicating whether the value is a valid email
    /// </returns>
    public static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(
            email,
            "^([0-9a-z]+[-._+&])*[0-9a-z]+@([-0-9a-z]+[.])+[a-z]{2,6}$",
            RegexOptions.IgnoreCase,TimeSpan.FromMilliseconds(100));
    }

    /// <summary>
    /// Checks if string is a valid email address (XMPP).
    /// </summary>
    /// <param name="xmpp">
    /// The XMPP string to check
    /// </param>
    /// <returns>
    /// Returns indicating whether the value is a valid XMPP
    /// </returns>
    public static bool IsValidXmpp(string xmpp)
    {
        return IsValidEmail(xmpp);
    }

    /// <summary>
    /// Checks if string is an valid Url.
    /// </summary>
    /// <param name="url">
    /// The url string to check
    /// </param>
    /// <returns>
    /// Returns indicating whether the value is a valid Url
    /// </returns>
    public static bool IsValidUrl([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        try
        {
            _ = new Uri(url, UriKind.Absolute);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if string is an valid integer
    /// </summary>
    /// <param name="input">
    /// The value to check.
    /// </param>
    /// <returns>
    /// Returns indicating whether the value is a valid Integer
    /// </returns>
    public static bool IsValidInt(string input)
    {
        return int.TryParse(input, out _);
    }

    /// <summary>
    /// Check if String is a Number
    /// </summary>
    /// <param name="valueToCheck">The value to check.</param>
    /// <returns>
    /// Returns indicating whether the value is a valid Number
    /// </returns>
    public static bool IsNumeric(string valueToCheck)
    {
        return double.TryParse(valueToCheck, NumberStyles.Any, null, out _);
    }

    /// <summary>
    /// Check if pattern is a valid Regex
    /// </summary>
    /// <param name="pattern">
    /// The pattern.
    /// </param>
    /// <returns>
    /// Returns indicating whether the value is valid Regex String.
    /// </returns>
    public static bool IsValidRegex(string pattern)
    {
        if (pattern.IsNotSet())
        {
            return false;
        }

        try
        {
            _ = Regex.Match(string.Empty, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }
}