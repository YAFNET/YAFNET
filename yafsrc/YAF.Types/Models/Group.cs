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
namespace YAF.Types.Models
{
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the Group table.
    /// </summary>
    [Serializable]
    [Table(Name = "Group")]

    [UniqueConstraint(nameof(BoardID), nameof(Name))]
    public class Group : IEntity, IHaveID, IHaveBoardID
    {
        #region Properties

        [AutoIncrement]
        [Alias("GroupID")]
        public int ID { get; set; }

        [References(typeof(Board))]
        [Required]
        public int BoardID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Ignore]
        public GroupFlags GroupFlags
        {
            get => new GroupFlags(this.Flags);

            set => this.Flags = value.BitValue;
        }

        [Required]
        [Default(0)]
        public int Flags { get; set; }

        [Required]
        [Default(30)]
        public int PMLimit { get; set; }

        [Index]
        [StringLength(255)]
        public string Style { get; set; }

        [Required]
        [Index]
        [Default(0)]
        public short SortOrder { get; set; }

        [StringLength(128)]
        public string Description { get; set; }

        [Required]
        [Default(0)]
        public int UsrSigChars { get; set; }

        [StringLength(255)]
        public string UsrSigBBCodes { get; set; }

        [StringLength(255)]
        public string UsrSigHTMLTags { get; set; }

        [Required]
        [Default(0)]
        public int UsrAlbums { get; set; }

        [Required]
        [Default(0)]
        public int UsrAlbumImages { get; set; }

        [Compute]
        public bool? IsHidden { get; set; }

        [Compute]
        public bool? IsUserGroup { get; set; }


        #endregion
    }
}