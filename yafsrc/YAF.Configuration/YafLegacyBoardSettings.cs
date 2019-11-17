/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Configuration
{
    /// <summary>
    /// The YAF legacy board settings.
    /// </summary>
    public class YafLegacyBoardSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YafLegacyBoardSettings"/> class.
        /// </summary>
        public YafLegacyBoardSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YafLegacyBoardSettings" /> class.
        /// </summary>
        /// <param name="boardName">The board name.</param>
        /// <param name="allowThreaded">The allow threaded.</param>
        /// <param name="membershipAppName">The membership app name.</param>
        /// <param name="rolesAppName">The roles app name.</param>
        public YafLegacyBoardSettings(string boardName, bool allowThreaded, string membershipAppName, string rolesAppName)
            : this()
        {
            this.BoardName = boardName;
            this.AllowThreaded = allowThreaded;
            this.MembershipAppName = membershipAppName;
            this.RolesAppName = rolesAppName;
        }

        /// <summary>
        /// Gets or sets BoardName.
        /// </summary>
        public string BoardName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowThreaded.
        /// </summary>
        public bool AllowThreaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets MembershipAppName.
        /// </summary>
        public string MembershipAppName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets RolesAppName.
        /// </summary>
        public string RolesAppName
        {
            get;
            set;
        }
    }
}