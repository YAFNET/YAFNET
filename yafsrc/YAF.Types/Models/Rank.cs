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

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     A class which represents the YAF_Rank table in the YAF Database.
    /// </summary>
    [Serializable]
    public partial class Rank : IEntity, IHaveID, IHaveBoardID
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Rank" /> class.
        /// </summary>
        public Rank()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Rank ID.
        /// </summary>
        [AutoIncrement]
        [Alias("RankID")]
        public int ID { get; set; }

        /// <summary>
        ///     Gets or sets the board id.
        /// </summary>
        public int BoardID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the minimum posts.
        /// </summary>
        /// <value>
        /// The minimum posts.
        /// </value>
        public int MinPosts { get; set; }

        /// <summary>
        /// Gets or sets the rank image.
        /// </summary>
        /// <value>
        /// The rank image.
        /// </value>
        public string RankImage { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the pm limit.
        /// </summary>
        /// <value>
        /// The pm limit.
        /// </value>
        public int PMLimit { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the usr sig chars.
        /// </summary>
        /// <value>
        /// The usr sig chars.
        /// </value>
        public string UsrSigChars { get; set; }

        /// <summary>
        /// Gets or sets the usr sig bb codes.
        /// </summary>
        /// <value>
        /// The usr sig bb codes.
        /// </value>
        public string UsrSigBBCodes { get; set; }

        /// <summary>
        /// Gets or sets the usr sig HTML tags.
        /// </summary>
        /// <value>
        /// The usr sig HTML tags.
        /// </value>
        public string UsrSigHTMLTags { get; set; }

        /// <summary>
        /// Gets or sets the usr albums.
        /// </summary>
        /// <value>
        /// The usr albums.
        /// </value>
        public int UsrAlbums { get; set; }

        /// <summary>
        /// Gets or sets the usr album images.
        /// </summary>
        /// <value>
        /// The usr album images.
        /// </value>
        public int UsrAlbumImages { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}