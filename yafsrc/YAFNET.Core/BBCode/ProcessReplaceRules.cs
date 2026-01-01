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

using System.Threading;

namespace YAF.Core.BBCode;

using System;
using System.Collections.Generic;

/// <summary>
///     Provides a way to handle layers of replacements rules
/// </summary>
public class ProcessReplaceRules : ICloneable, IProcessReplaceRules
{
    /// <summary>
    ///     The rules list.
    /// </summary>
    private readonly List<IReplaceRule> rulesList;

    /// <summary>
    ///     The rules lock.
    /// </summary>
    private readonly Lock rulesLock = new ();

    /// <summary>
    ///     The need sort.
    /// </summary>
    private bool needSort;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProcessReplaceRules" /> class.
    /// </summary>
    public ProcessReplaceRules()
    {
        this.rulesList = [];
    }

    /// <summary>
    ///     Gets a value indicating whether any rules have been added.
    /// </summary>
    public bool HasRules
    {
        get
        {
            lock (this.rulesLock)
            {
                return this.rulesList.Count > 0;
            }
        }
    }

    /// <summary>
    ///     Add New Rule
    /// </summary>
    /// <param name="newRule">
    ///     The new rule.
    /// </param>
    public void AddRule(IReplaceRule newRule)
    {
        ArgumentNullException.ThrowIfNull(newRule);

        lock (this.rulesLock)
        {
            this.rulesList.Add(newRule);
            this.needSort = true;
        }
    }

    /// <summary>
    ///     This clone method is a Deep Clone -- including all data.
    /// </summary>
    /// <returns>
    ///     The clone.
    /// </returns>
    public object Clone()
    {
        var copyReplaceRules = new ProcessReplaceRules();

        // move the rules over...
        var ruleArray = new IReplaceRule[this.rulesList.Count];
        this.rulesList.CopyTo(ruleArray);
        copyReplaceRules.rulesList.InsertRange(0, ruleArray);
        copyReplaceRules.needSort = this.needSort;

        return copyReplaceRules;
    }

    /// <summary>
    ///     The process.
    /// </summary>
    /// <param name="text">
    ///     The text.
    /// </param>
    public void Process(ref string text)
    {
        if (text.IsNotSet())
        {
            return;
        }

        // sort the rules according to rank...
        if (this.needSort)
        {
            lock (this.rulesLock)
            {
                this.rulesList.Sort();
                this.needSort = false;
            }
        }

        // make the replacementCollection for this instance...
        var mainCollection = new ReplaceBlocksCollection();

        // get as local list...
        var localRulesList = new List<IReplaceRule>();

        lock (this.rulesLock)
        {
            localRulesList.AddRange(this.rulesList);
        }

        // apply all rules...
        foreach (var rule in localRulesList)
        {
            rule.Replace(ref text, mainCollection);
        }

        // reconstruct the html
        mainCollection.Reconstruct(ref text);
    }
}