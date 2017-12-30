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

    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the Group table.
    /// </summary>
    [Serializable]
    [Table(Name = "Group")]
    public partial class Group : IEntity, IHaveID, IHaveBoardID
    {
        partial void OnCreated();

        public Group()
        {
            this.OnCreated();
        }

        #region Properties

        [AutoIncrement]
        [Alias("GroupID")]
        public int ID { get; set; }

        public int BoardID { get; set; }

        public string Name { get; set; }

        [Ignore]
        public GroupFlags GroupFlags
        {
            get
            {
                return new GroupFlags(this.Flags);
            }

            set
            {
                this.Flags = value.BitValue;
            }
        }

        public int Flags { get; set; }

        public int PMLimit { get; set; }

        public string Style { get; set; }

        public short SortOrder { get; set; }

        public string Description { get; set; }

        public int UsrSigChars { get; set; }

        public string UsrSigBBCodes { get; set; }

        public string UsrSigHTMLTags { get; set; }

        public int UsrAlbums { get; set; }

        public int UsrAlbumImages { get; set; }

        public bool? IsHidden { get; set; }

        public bool? IsUserGroup { get; set; }


        #endregion
    }
}