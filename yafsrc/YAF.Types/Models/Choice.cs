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
namespace YAF.Types.Models
{
    using System;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     A class which represents the yaf_Choice table.
    /// </summary>
    [Serializable]
    public partial class Choice : IEntity, IHaveID
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Choice"/> class.
        /// </summary>
        public Choice()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        [AutoIncrement]
        [Alias("ChoiceID")]
        public int ID { get; set; }

        [References(typeof(Poll))]
        [Required]
        public int PollID { get; set; }

        [Alias("Choice")]
        [Required]
        [StringLength(50)]
        public string ChoiceName { get; set; }

        [Required]
        public int Votes { get; set; }

        [StringLength(255)]
        public string ObjectPath { get; set; }

        [StringLength(50)]
        public string MimeType { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}