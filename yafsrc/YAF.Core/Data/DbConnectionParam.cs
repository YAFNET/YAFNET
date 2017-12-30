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

namespace YAF.Core.Data
{
    using YAF.Types;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     The db connection param.
    /// </summary>
    public struct DbConnectionParam : IDbConnectionParam
    {
        #region Fields

        private int _id;

        private string _name;

        private string _value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbConnectionParam" /> class.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="defaultValue">
        ///     The value.
        /// </param>
        /// <param name="visible">
        ///     The visible.
        /// </param>
        public DbConnectionParam(int id, string name, [NotNull] string defaultValue = null)
        {
            this._id = id;
            this._name = name;
            this._value = defaultValue ?? string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ID.
        /// </summary>
        public int ID
        {
            get
            {
                return this._id;
            }

            set
            {
                this._id = value;
            }
        }

        /// <summary>
        ///     Gets or sets Label.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        ///     Gets or sets DefaultValue.
        /// </summary>
        public string Value
        {
            get
            {
                return this._value;
            }
        }

        #endregion
    }
}