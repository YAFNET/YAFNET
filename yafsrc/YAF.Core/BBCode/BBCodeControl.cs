/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Core.BBCode
{
    #region Using

    using System.Collections.Generic;

    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The YAF BBCode control.
    /// </summary>
    public class BBCodeControl : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets CurrentMessageFlags.
        /// </summary>
        public MessageFlags CurrentMessageFlags { get; set; }

        /// <summary>
        ///   Gets or sets MessageID.
        /// </summary>
        public int? MessageID { get; set; }

        /// <summary>
        ///   Gets or sets DisplayUserID.
        /// </summary>
        public int? DisplayUserID { get; set; }

        /// <summary>
        ///   Gets or sets Parameters.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the localized string.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="defaultString">
        /// The default string.
        /// </param>
        /// <returns>
        /// Returns the localized string.
        /// </returns>
        protected string LocalizedString([NotNull] string tag, [NotNull] string defaultString)
        {
            return this.Get<ILocalization>().GetTextExists("BBCODEMODULE", tag)
                       ? this.GetText("BBCODEMODULE", tag)
                       : defaultString;
        }

        #endregion
    }
}