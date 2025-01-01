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

namespace YAF.Core.Extensions;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// The i replace blocks extensions.
/// </summary>
public static class IReplaceBlocksExtensions
{
    /// <summary>
    /// The _options.
    /// </summary>
    private readonly static RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    /// <summary>
    /// The _reg ex html.
    /// </summary>
    private readonly static Regex _regExHtml = new(
        """</?\w+((\s+\w+(\s*=\s*(?:".*?"|'.*?'|[^'">\s]+))?)+\s*|\s*)/?>""",
        _options | RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Pull replacement blocks from the text
    /// </summary>
    /// <param name="replaceBlocks">
    /// The replace Blocks.
    /// </param>
    /// <param name="strText">
    /// The str Text.
    /// </param>
    public static void ReplaceHtmlFromText(this IReplaceBlocks replaceBlocks, ref string strText)
    {
        var sb = new StringBuilder(strText);

        ReplaceHtmlFromText(replaceBlocks, ref sb);

        strText = sb.ToString();
    }

    /// <summary>
    /// The get replacements from text.
    /// </summary>
    /// <param name="replaceBlocks">
    /// The replace Blocks.
    /// </param>
    /// <param name="sb">
    /// The sb.
    /// </param>
    public static void ReplaceHtmlFromText(this IReplaceBlocks replaceBlocks, ref StringBuilder sb)
    {
        var m = _regExHtml.Match(sb.ToString());

        while (m.Success)
        {
            // add it to the list...
            var index = replaceBlocks.Add(m.Groups[0].Value);

            // replacement lookup code
            var replace = replaceBlocks.Get(index);

            // remove the replaced item...
            sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

            // insert the replaced value back in...
            sb.Insert(m.Groups[0].Index, replace);

            m = _regExHtml.Match(sb.ToString());
        }
    }
}