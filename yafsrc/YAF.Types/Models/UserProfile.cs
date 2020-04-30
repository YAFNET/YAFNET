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

    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the yaf_UserProfile table.
    /// </summary>
    [Serializable]
    [UniqueConstraint(nameof(ID), nameof(ApplicationName))]
    [Table(Name = "UserProfile")]
    public class UserProfile : IEntity, IHaveID
    {
        #region Properties


        [Alias("UserID")]
        [References(typeof(User))]
        [Required]
        public int ID { get; set; }
        [Required]
        public DateTime LastUpdatedDate { get; set; }
        public DateTime? LastActivity { get; set; }
        [Required]
        [StringLength(255)]
        public string ApplicationName { get; set; }
        [Required]
        public bool IsAnonymous { get; set; }
        [Required]
        [StringLength(255)]
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Country { get; set; }
        public string BlogServicePassword { get; set; }
        public string ICQ { get; set; }
        public string BlogServiceUsername { get; set; }
        public string Skype { get; set; }
        public string Twitter { get; set; }
        public DateTime? Birthday { get; set; }
        public string XMPP { get; set; }
        public string Interests { get; set; }
        public string Blog { get; set; }
        public int? Gender { get; set; }
        public string BlogServiceUrl { get; set; }
        public string Occupation { get; set; }
        public string Homepage { get; set; }
        public string TwitterId { get; set; }
        public string City { get; set; }
        public string Facebook { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string FacebookId { get; set; }
        public string Google { get; set; }
        public string GoogleId { get; set; }
        public DateTime? LastSyncedWithDNN { get; set; }

        #endregion
    }
}