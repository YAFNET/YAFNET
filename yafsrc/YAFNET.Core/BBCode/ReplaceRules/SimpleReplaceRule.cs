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

/// <summary>
/// Not regular expression, just a simple replace
/// </summary>
public class SimpleReplaceRule : BaseReplaceRule
{
    /// <summary>
    ///   The find.
    /// </summary>
    private readonly string find;

    /// <summary>
    ///   The replace.
    /// </summary>
    private readonly string replace;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleReplaceRule"/> class.
    /// </summary>
    /// <param name="find">
    /// The find.
    /// </param>
    /// <param name="replace">
    /// The replace.
    /// </param>
    public SimpleReplaceRule(string find, string replace)
    {
        this.find = find;
        this.replace = replace;

        // lower the rank by default
        this.RuleRank = 100;
    }

    /// <summary>
    ///   Gets RuleDescription.
    /// </summary>
    public override string RuleDescription => $"Find = \"{this.find}\"";

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
        int index;

        do
        {
            index = text.FastIndexOf(this.find);

            if (index < 0)
            {
                continue;
            }

            // replace it...
            var replaceIndex = replacement.Add(this.replace);
            text = text[..index] + replacement.Get(replaceIndex) +
                   text[(index + this.find.Length)..];
        }
        while (index >= 0);
    }
}