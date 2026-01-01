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

namespace YAF.Core.BBCode.ReplaceRules;

using System.Text.RegularExpressions;

/// <summary>
/// For the font size with replace
/// </summary>
public class FontSizeRegexReplaceRule : VariableRegexReplaceRule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FontSizeRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The Search Regex
    /// </param>
    /// <param name="regExReplace">
    /// The Replace Regex.
    /// </param>
    /// <param name="regExOptions">
    /// The Regex options.
    /// </param>
    public FontSizeRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
        : base(regExSearch, regExReplace, regExOptions, ["size"], ["5"])
    {
        this.RuleRank = 25;
    }

    /// <summary>
    /// Override to change default variable handling...
    /// </summary>
    /// <param name="variableName">The variable name.</param>
    /// <param name="variableValue">The variable value.</param>
    /// <param name="handlingValue">The handling value.</param>
    /// <returns>
    /// The manage variable value.
    /// </returns>
    override protected string ManageVariableValue(string variableName, string variableValue, string handlingValue)
    {
        return variableName == "size" ? GetFontSize(variableValue) : variableValue;
    }

    /// <summary>
    /// Gets the size of the font.
    /// </summary>
    /// <param name="inputStr">The input string.</param>
    /// <returns>Returns the Font size</returns>
    private static string GetFontSize(string inputStr)
    {
        int[] sizes = [50, 70, 80, 90, 100, 120, 140, 160, 180];

        // try to parse the input string...
        int.TryParse(inputStr, out var size);

        switch (size)
        {
            case > 9:
                return $"{size}%";
            case 9:
                return $"{size}px";
            case < 1:
                size = 1;
                break;
        }

        if (size > sizes.Length)
        {
            size = 5;
        }

        return $"{sizes[size - 1]}%";
    }
}