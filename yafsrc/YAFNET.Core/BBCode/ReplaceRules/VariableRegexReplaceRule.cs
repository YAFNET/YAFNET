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
using System.Web;

/// <summary>
/// For complex regex with variable/default and truncate support
/// </summary>
public class VariableRegexReplaceRule : SimpleRegexReplaceRule
{
    /// <summary>
    ///   The variable defaults.
    /// </summary>
    readonly protected string[] VariableDefaults;

    /// <summary>
    ///   The variables.
    /// </summary>
    readonly protected string[] Variables;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The Search Regex.
    /// </param>
    /// <param name="regExReplace">
    /// The Replace Regex.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="defaults">
    /// The defaults.
    /// </param>
    public VariableRegexReplaceRule(Regex regExSearch, string regExReplace, string[] variables, string[] defaults)
        : base(regExSearch, regExReplace)
    {
        this.Variables = variables;
        this.VariableDefaults = defaults;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The Search Regex.
    /// </param>
    /// <param name="regExReplace">
    /// The Replace Regex.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    public VariableRegexReplaceRule(Regex regExSearch, string regExReplace, string[] variables)
        : base(regExSearch, regExReplace)
    {
        this.Variables = variables;
        this.VariableDefaults = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The Search Regex.
    /// </param>
    /// <param name="regExReplace">
    /// The Replace Regex.
    /// </param>
    /// <param name="regExOptions">
    /// The Regex options.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="defaults">
    /// The defaults.
    /// </param>
    public VariableRegexReplaceRule(
        string regExSearch,
        string regExReplace,
        RegexOptions regExOptions,
        string[] variables,
        string[] defaults)
        : base(regExSearch, regExReplace, regExOptions)
    {
        this.Variables = variables;
        this.VariableDefaults = defaults;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The Search Regex.
    /// </param>
    /// <param name="regExReplace">
    /// The Replace Regex.
    /// </param>
    /// <param name="regExOptions">
    /// The Regex options.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    public VariableRegexReplaceRule(
        string regExSearch,
        string regExReplace,
        RegexOptions regExOptions,
        string[] variables)
        : base(regExSearch, regExReplace, regExOptions)
    {
        this.Variables = variables;
        this.VariableDefaults = null;
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, IReplaceBlocks replacement)
    {
        var sb = new StringBuilder(text);

        var m = this.RegExSearch.Match(text);
        while (m.Success)
        {
            var innerReplace = new StringBuilder(this.RegExReplace);
            var i = 0;

            this.Variables.ForEach(
                tVar =>
                    {
                        var varName = tVar;
                        var handlingValue = string.Empty;

                        if (varName.Contains(":"))
                        {
                            // has handling section
                            var tmpSplit = varName.Split(':');
                            varName = tmpSplit[0];
                            handlingValue = tmpSplit[1];
                        }

                        var tValue = m.Groups[varName].Value;

                        if (this.VariableDefaults != null && tValue.Length == 0)
                        {
                            // use default instead
                            tValue = this.VariableDefaults[i];
                        }

                        innerReplace.Replace(
                            $"${{{varName}}}",
                            this.ManageVariableValue(varName, tValue, handlingValue));
                        i++;
                    });

            innerReplace.Replace("${inner}", m.Groups["inner"].Value);

            innerReplace.Replace("${innertrunc}", m.Groups["inner"].Value);

            // pulls the html's into the replacement collection before it's inserted back into the main text
            replacement.ReplaceHtmlFromText(ref innerReplace);

            // remove old bbcode...
            sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

            // insert replaced value(s)
            sb.Insert(m.Groups[0].Index, innerReplace.ToString());

            m = this.RegExSearch.Match(sb.ToString());
        }

        text = sb.ToString();
    }

    /// <summary>
    /// Override to change default variable handling...
    /// </summary>
    /// <param name="variableName">
    /// The variable Name.
    /// </param>
    /// <param name="variableValue">
    /// The variable Value.
    /// </param>
    /// <param name="handlingValue">
    /// variable transformation desired
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    protected virtual string ManageVariableValue(string variableName, string variableValue, string handlingValue)
    {
        if (!handlingValue.IsSet())
        {
            return variableValue;
        }

        variableValue = handlingValue.ToLower() switch
            {
                "decode" => HttpUtility.HtmlDecode(variableValue),
                "encode" => HttpUtility.HtmlEncode(variableValue),
                _ => variableValue
            };

        return variableValue;
    }
}