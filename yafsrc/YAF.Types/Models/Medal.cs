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
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     A class which represents the Medal table.
    /// </summary>
    [Serializable]
    public partial class Medal : IEntity, IHaveBoardID, IHaveID
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Medal"/> class.
        /// </summary>
        public Medal()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the medal id.
        /// </summary>
        [AutoIncrement]
        [Alias("MedalID")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the board id.
        /// </summary>
        public int BoardID { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the medal url.
        /// </summary>
        public string MedalURL { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ribbon url.
        /// </summary>
        public string RibbonURL { get; set; }

        /// <summary>
        /// Gets or sets the small medal height.
        /// </summary>
        public short SmallMedalHeight { get; set; }

        /// <summary>
        /// Gets or sets the small medal url.
        /// </summary>
        public string SmallMedalURL { get; set; }

        /// <summary>
        /// Gets or sets the small medal width.
        /// </summary>
        public short SmallMedalWidth { get; set; }

        /// <summary>
        /// Gets or sets the small ribbon height.
        /// </summary>
        public short? SmallRibbonHeight { get; set; }

        /// <summary>
        /// Gets or sets the small ribbon url.
        /// </summary>
        public string SmallRibbonURL { get; set; }

        /// <summary>
        /// Gets or sets the small ribbon width.
        /// </summary>
        public short? SmallRibbonWidth { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public byte SortOrder { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}