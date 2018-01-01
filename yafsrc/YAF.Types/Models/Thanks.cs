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
namespace YAF.Types.Models
{
    #region Using

    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     A class which represents the yaf_Thanks table in the Yaf Database.
    /// </summary>
    [Serializable]
    public partial class Thanks : IEntity, IHaveID
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Thanks" /> class.
        /// </summary>
        public Thanks()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Thanks id.
        /// </summary>
        [AutoIncrement]
        [Alias("ThanksID")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the thanks from user identifier.
        /// </summary>
        /// <value>
        /// The thanks from user identifier.
        /// </value>
        public int ThanksFromUserID { get; set; }

        /// <summary>
        /// Gets or sets the thanks to user identifier.
        /// </summary>
        /// <value>
        /// The thanks to user identifier.
        /// </value>
        public int ThanksToUserID { get; set; }

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public int MessageID { get; set; }

        /// <summary>
        /// Gets or sets the thanks date.
        /// </summary>
        /// <value>
        /// The thanks date.
        /// </value>
        public DateTime ThanksDate { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}