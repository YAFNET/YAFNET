/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Types.Objects
{
    using YAF.Types;

    /// <summary>
    /// The internal pop menu item.
    /// </summary>
    public class InternalPopMenuItem
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalPopMenuItem"/> class.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="postBackArgument">
        /// The post-back argument.
        /// </param>
        /// <param name="clientScript">
        /// The client script.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        public InternalPopMenuItem(
            [NotNull] string description,
            [NotNull] string postBackArgument,
            [NotNull] string clientScript,
            [NotNull] string icon)
        {
            this.Description = description;
            this.PostBackArgument = postBackArgument;
            this.ClientScript = clientScript;
            this.Icon = icon;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///   Gets or sets ClientScript.
        /// </summary>
        public string ClientScript { get; set; }

        /// <summary>
        ///   Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///   Gets or sets PostBackArgument.
        /// </summary>
        public string PostBackArgument { get; set; }

        #endregion
    }
}