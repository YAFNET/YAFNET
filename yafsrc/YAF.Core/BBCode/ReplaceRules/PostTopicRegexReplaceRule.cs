/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.BBCode.ReplaceRules
{
    using System.Text.RegularExpressions;

    using YAF.Types.Constants;
    using YAF.Utils;

    /// <summary>
    /// For the font size with replace
    /// </summary>
    public class PostTopicRegexReplaceRule : VariableRegexReplaceRule
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PostTopicRegexReplaceRule"/> class.
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
        public PostTopicRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
            : base(regExSearch, regExReplace, regExOptions, new[] { "post", "topic" })
        {
            this.RuleRank = 200;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The manage variable value.
        /// </summary>
        /// <param name="variableName">
        /// The variable name.
        /// </param>
        /// <param name="variableValue">
        /// The variable value.
        /// </param>
        /// <param name="handlingValue">
        /// The handling value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string ManageVariableValue(string variableName, string variableValue, string handlingValue)
        {
            if (variableName != "post" && variableName != "topic")
            {
                return variableValue;
            }

            if (!int.TryParse(variableValue, out var id))
            {
                return variableValue;
            }

            return variableName switch
                {
                    "post" => BuildLink.GetLink(ForumPages.Posts, "m={0}#post{0}", id),
                    "topic" => BuildLink.GetLink(ForumPages.Posts, "t={0}", id),
                    _ => variableValue
                };
        }

        #endregion
    }
}