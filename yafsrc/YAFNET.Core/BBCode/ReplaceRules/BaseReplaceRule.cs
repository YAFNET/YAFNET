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

namespace YAF.Core.BBCode.ReplaceRules;

using System;

/// <summary>
/// Base class for all replacement rules.
///   Provides compare functionality based on the rule rank.
///   Override replace to handle replacement differently.
/// </summary>
public abstract class BaseReplaceRule : IComparable, IReplaceRule
{
    /// <summary>
    ///   The rule rank.
    /// </summary>
    public int RuleRank { get; set; } = 50;

    /// <summary>
    ///   Gets RuleDescription.
    /// </summary>
    public virtual string RuleDescription => string.Empty;

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public abstract void Replace(ref string text, IReplaceBlocks replacement);

    /// <summary>
    /// The compare to.
    /// </summary>
    /// <param name="obj">
    /// The object to compare to.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public int CompareTo(object obj)
    {
        if (obj is not BaseReplaceRule otherRule)
        {
            throw new ArgumentException("Object is not of type BaseReplaceRule.");
        }

        if (this.RuleRank > otherRule.RuleRank)
        {
            return 1;
        }

        if (this.RuleRank < otherRule.RuleRank)
        {
            return -1;
        }

        return 0;
    }
}