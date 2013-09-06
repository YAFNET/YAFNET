/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Types.Models
{
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces.Data;

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

        #region Properties

        [AutoIncrement]
        [Alias("MessageID")]
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