/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.BBCode
{
    #region Using

    using System;
    using System.Collections.Generic;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     Provides a way to handle layers of replacements rules
    /// </summary>
    public class ProcessReplaceRules : ICloneable, IProcessReplaceRules
    {
        #region Fields

        /// <summary>
        ///     The _rules list.
        /// </summary>
        private readonly List<IReplaceRule> _rulesList;

        /// <summary>
        ///     The _rules lock.
        /// </summary>
        private readonly object _rulesLock = new object();

        /// <summary>
        ///     The _need sort.
        /// </summary>
        private bool _needSort;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessReplaceRules" /> class.
        /// </summary>
        public ProcessReplaceRules()
        {
            this._rulesList = new List<IReplaceRule>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether any rules have been added.
        /// </summary>
        public bool HasRules
        {
            get
            {
                lock (this._rulesLock)
                {
                    return this._rulesList.Count > 0;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add rule.
        /// </summary>
        /// <param name="newRule">
        ///     The new rule.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public void AddRule(IReplaceRule newRule)
        {
            CodeContracts.VerifyNotNull(newRule, "newRule");

            lock (this._rulesLock)
            {
                this._rulesList.Add(newRule);
                this._needSort = true;
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
            var ruleArray = new IReplaceRule[this._rulesList.Count];
            this._rulesList.CopyTo(ruleArray);
            copyReplaceRules._rulesList.InsertRange(0, ruleArray);
            copyReplaceRules._needSort = this._needSort;

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
            if (this._needSort)
            {
                lock (this._rulesLock)
                {
                    this._rulesList.Sort();
                    this._needSort = false;
                }
            }

            // make the replacementCollection for this instance...
            var mainCollection = new ReplaceBlocksCollection();

            // get as local list...
            var localRulesList = new List<IReplaceRule>();

            lock (this._rulesLock)
            {
                localRulesList.AddRange(this._rulesList);
            }

            // apply all rules...
            foreach (IReplaceRule rule in localRulesList)
            {
                rule.Replace(ref text, mainCollection);
            }

            // reconstruct the html
            mainCollection.Reconstruct(ref text);
        }

        #endregion
    }
}