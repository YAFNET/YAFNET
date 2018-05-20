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
    using System.Data;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Flags;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Objects;

    /// <summary>
    /// A class which represents the Message table.
    /// </summary>
    [Serializable]
    [Table(Name = "Message")]
    public partial class Message : IEntity, IHaveID
    {
        partial void OnCreated();

        public Message()
        {
            this.OnCreated();
        }

        public Message([NotNull] DataRow row)
        {
            this.ID = row.Field<int?>("MessageID") ?? 0;
            this.UserID = row.Field<int?>("UserID") ?? 0;
            this.UserName = row.Field<string>("UserName");
            this.MessageText = row.Field<string>("Message");
            this.TopicID = row.Field<int?>("TopicID") ?? 0;
            this.Topic = row.Field<string>("Topic");
            this.Flags = row.Field<int?>("Flags") ?? 0;
            this.Edited = row.Field<DateTime?>("Edited");
            this.EditReason = row.Field<string>("EditReason");
            this.Position = row.Field<int?>("Position") ?? 0;
            this.IsModeratorChanged = row.Field<bool?>("IsModeratorChanged");
            this.DeleteReason = row.Field<string>("DeleteReason");
            this.BlogPostID = row.Field<string>("BlogPostID");
            this.IP = row.Field<string>("IP");
            this.ExternalMessageId = row.Field<string>("ExternalMessageId");
            this.ReferenceMessageId = row.Field<string>("ReferenceMessageId");
            this.HasAttachments = row.Field<bool?>("HasAttachments");
        }

        #region Properties

        [AutoIncrement]
        [AliasAttribute("MessageID")]
        public int ID { get; set; }

        public int TopicID { get; set; }

        public string Topic { get; set; }

        public int? ReplyTo { get; set; }

        public int Position { get; set; }

        public int Indent { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public DateTime Posted { get; set; }

        public bool? HasAttachments { get; set; }

        [Alias("Message")]
        public string MessageText { get; set; }

        public string IP { get; set; }

        public DateTime? Edited { get; set; }

        public int Flags { get; set; }

        public bool? IsDeleted { get; set; }

        public bool? IsApproved { get; set; }

        public string BlogPostID { get; set; }

        public string EditReason { get; set; }

        public string Signature { get; set; }

        public bool? IsModeratorChanged { get; set; }

        public string DeleteReason { get; set; }

        public int? EditedBy { get; set; }

        public string ExternalMessageId { get; set; }

        public string ReferenceMessageId { get; set; }

        public string UserDisplayName { get; set; }

        #endregion
    }
}