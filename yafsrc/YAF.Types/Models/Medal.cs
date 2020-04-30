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

    using ServiceStack.DataAnnotations;

    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     A class which represents the Medal table.
    /// </summary>
    [Serializable]
    public class Medal : IEntity, IHaveBoardID, IHaveID
    {
        #region Public Properties

        [Required]
        public int BoardID { get; set; }

        [Alias("MedalID")]
        [AutoIncrement]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Message { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        [StringLength(250)]
        public string MedalURL { get; set; }

        [StringLength(250)]
        public string RibbonURL { get; set; }

        [Required]
        [StringLength(250)]
        public string SmallMedalURL { get; set; }

        [StringLength(250)]
        public string SmallRibbonURL { get; set; }

        [Required]
        public short SmallMedalWidth { get; set; }

        [Required]
        public short SmallMedalHeight { get; set; }

        public short? SmallRibbonWidth { get; set; }

        public short? SmallRibbonHeight { get; set; }

        [Required]
        [Default(255)]
        public byte SortOrder { get; set; }

        [Required]
        [Default(0)]
        public int Flags { get; set; }

        [Ignore]
        public MedalFlags MedalFlags
        {
            get => new MedalFlags(this.Flags);

            set => this.Flags = value.BitValue;
        }

        #endregion
    }
}