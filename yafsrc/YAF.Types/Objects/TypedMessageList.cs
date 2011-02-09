/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Types.Objects
{
  #region Using

  using System;
  using System.Data;

  using YAF.Types.Flags;

  #endregion

  /// <summary>
  /// The typed message list.
  /// </summary>
  [Serializable]
  public class TypedMessageList
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedMessageList"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedMessageList([NotNull] DataRow row)
    {
      this.MessageID = row.Field<int?>("MessageID");
      this.UserID = row.Field<int?>("UserID");
      this.UserName = row.Field<string>("UserName");
      this.Message = row.Field<string>("Message");
      this.TopicID = row.Field<int?>("TopicID");
      this.ForumID = row.Field<int?>("ForumID");
      this.Topic = row.Field<string>("Topic");
      this.Priority = row.Field<short?>("Priority");
      this.Flags = new MessageFlags(row.Field<int?>("Flags") ?? 0);
      this.TopicOwnerID = row.Field<int?>("TopicOwnerID");
      this.Edited = row.Field<DateTime?>("Edited");
      this.TopicFlags = new TopicFlags(row.Field<int?>("TopicFlags") ?? 0);
      this.ForumFlags = new ForumFlags(row.Field<int?>("ForumFlags") ?? 0);
      this.EditReason = row.Field<string>("EditReason");
      this.Position = row.Field<int?>("Position");
      this.IsModeratorChanged = row.Field<bool?>("IsModeratorChanged");
      this.DeleteReason = row.Field<string>("DeleteReason");
      this.BlogPostID = row.Field<string>("BlogPostID");
      this.PollID = row.Field<int?>("PollID");
      this.IP = row.Field<string>("IP");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedMessageList"/> class.
    /// </summary>
    /// <param name="messageid">
    /// The messageid.
    /// </param>
    /// <param name="userid">
    /// The userid.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="topicid">
    /// The topicid.
    /// </param>
    /// <param name="forumid">
    /// The forumid.
    /// </param>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <param name="priority">
    /// The priority.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <param name="topicownerid">
    /// The topicownerid.
    /// </param>
    /// <param name="edited">
    /// The edited.
    /// </param>
    /// <param name="topicflags">
    /// The topicflags.
    /// </param>
    /// <param name="forumflags">
    /// The forumflags.
    /// </param>
    /// <param name="editreason">
    /// The editreason.
    /// </param>
    /// <param name="position">
    /// The position.
    /// </param>
    /// <param name="ismoderatorchanged">
    /// The ismoderatorchanged.
    /// </param>
    /// <param name="deletereason">
    /// The deletereason.
    /// </param>
    /// <param name="blogpostid">
    /// The blogpostid.
    /// </param>
    /// <param name="pollid">
    /// The pollid.
    /// </param>
    /// <param name="ip">
    /// The ip.
    /// </param>
    public TypedMessageList(
      int? messageid, 
      int? userid,  string username, string message, 
      int? topicid, 
      int? forumid, string topic, 
      short? priority, MessageFlags flags, 
      int? topicownerid, 
      DateTime? edited, 
      TopicFlags topicflags, 
      ForumFlags forumflags, string editreason, 
      int? position, 
      bool? ismoderatorchanged, string deletereason, string blogpostid, 
      int? pollid, string ip)
    {
      this.MessageID = messageid;
      this.UserID = userid;
      this.UserName = username;
      this.Message = message;
      this.TopicID = topicid;
      this.ForumID = forumid;
      this.Topic = topic;
      this.Priority = priority;
      this.Flags = flags;
      this.TopicOwnerID = topicownerid;
      this.Edited = edited;
      this.TopicFlags = topicflags;
      this.ForumFlags = forumflags;
      this.EditReason = editreason;
      this.Position = position;
      this.IsModeratorChanged = ismoderatorchanged;
      this.DeleteReason = deletereason;
      this.BlogPostID = blogpostid;
      this.PollID = pollid;
      this.IP = ip;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets BlogPostID.
    /// </summary>
    public string BlogPostID { get; set; }

    /// <summary>
    /// Gets or sets DeleteReason.
    /// </summary>
    public string DeleteReason { get; set; }

    /// <summary>
    /// Gets or sets EditReason.
    /// </summary>
    public string EditReason { get; set; }

    /// <summary>
    /// Gets or sets Edited.
    /// </summary>
    public DateTime? Edited { get; set; }

    /// <summary>
    /// Gets or sets Flags.
    /// </summary>
    public MessageFlags Flags { get; set; }

    /// <summary>
    /// Gets or sets ForumFlags.
    /// </summary>
    public ForumFlags ForumFlags { get; set; }

    /// <summary>
    /// Gets or sets ForumID.
    /// </summary>
    public int? ForumID { get; set; }

    /// <summary>
    /// Gets or sets IP.
    /// </summary>
    public string IP { get; set; }

    /// <summary>
    /// Gets or sets IsModeratorChanged.
    /// </summary>
    public bool? IsModeratorChanged { get; set; }

    /// <summary>
    /// Gets or sets Message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets MessageID.
    /// </summary>
    public int? MessageID { get; set; }

    /// <summary>
    /// Gets or sets PollID.
    /// </summary>
    public int? PollID { get; set; }

    /// <summary>
    /// Gets or sets Position.
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// Gets or sets Priority.
    /// </summary>
    public short? Priority { get; set; }

    /// <summary>
    /// Gets or sets Topic.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Gets or sets TopicFlags.
    /// </summary>
    public TopicFlags TopicFlags { get; set; }

    /// <summary>
    /// Gets or sets TopicID.
    /// </summary>
    public int? TopicID { get; set; }

    /// <summary>
    /// Gets or sets TopicOwnerID.
    /// </summary>
    public int? TopicOwnerID { get; set; }

    /// <summary>
    /// Gets or sets UserID.
    /// </summary>
    public int? UserID { get; set; }

    /// <summary>
    /// Gets or sets UserName.
    /// </summary>
    public string UserName { get; set; }

    #endregion
  }
}