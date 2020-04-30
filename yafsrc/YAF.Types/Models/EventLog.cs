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
    using ServiceStack.OrmLite;

    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the EventLog table.
    /// </summary>
    [Serializable]
    public class EventLog : IEntity, IHaveID
    {
        #region Properties

        [AutoIncrement]
        [Alias("EventLogID")]
        public int ID { get; set; }
        [Default(OrmLiteVariables.SystemUtc)]
        public DateTime EventTime { get; set; }

        [References(typeof(User))]
        public int? UserID { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Source { get; set; }

        public string Description { get; set; }

        [Required]
        [Default(0)]
        public int Type { get; set; }

        [Ignore]
        public EventLogTypes EventType
        {
            get => this.Type.ToType<EventLogTypes>();

            set => this.Type = (int)value;
        }

        #endregion
    }
}