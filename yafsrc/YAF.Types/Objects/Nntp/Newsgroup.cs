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
namespace YAF.Types.Objects.Nntp
{
    using System;

    /// <summary>
    /// The newsgroup.
    /// </summary>
    public class Newsgroup : IComparable
    {
        /// <summary>
        /// The group.
        /// </summary>
        protected string group;

        /// <summary>
        /// The high.
        /// </summary>
        protected int high;

        /// <summary>
        /// The low.
        /// </summary>
        protected int low;

        /// <summary>
        /// Initializes a new instance of the <see cref="Newsgroup"/> class.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <param name="low">
        /// The low.
        /// </param>
        /// <param name="high">
        /// The high.
        /// </param>
        public Newsgroup(string group, int low, int high)
        {
            this.group = group;
            this.low = low;
            this.high = high;
        }

        /// <summary>
        /// Gets or sets Group.
        /// </summary>
        public string Group
        {
            get => this.group;

            set => this.group = value;
        }

        /// <summary>
        /// Gets or sets Low.
        /// </summary>
        public int Low
        {
            get => this.low;

            set => this.low = value;
        }

        /// <summary>
        /// Gets or sets High.
        /// </summary>
        public int High
        {
            get => this.high;

            set => this.high = value;
        }

        #region IComparable Members

        /// <summary>
        /// The compare to.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <returns>
        /// The compare to.
        /// </returns>
        public int CompareTo(object r)
        {
            return string.Compare(this.Group, ((Newsgroup)r).Group, StringComparison.Ordinal);
        }

        #endregion
    }
}