/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using ServiceStack.DataAnnotations;

namespace YAF.Types.Models;

using YAF.Types.Objects.Model;

/// <summary>
/// A class which represents the Message table.
/// </summary>
[Serializable]
public class Message : IEntity, IHaveID
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    public Message()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public Message(PagedMessage row)
    {
        this.ID = row.MessageID;
        this.UserID = row.UserID;
        this.UserName = row.UserName;
        this.MessageText = row.Message;
        this.TopicID = row.TopicID;

        this.Posted = row.Posted;

        this.TopicName = row.Topic;

        this.Flags = row.Flags;

        this.Edited = row.Edited;
        this.EditReason = row.EditReason;

        try
        {
            this.Position = row.Position;
        }
        catch (Exception)
        {
            this.Position = 0;
        }

        try
        {
            this.IsModeratorChanged = row.IsModeratorChanged;
        }
        catch (Exception)
        {
            this.IsModeratorChanged = false;
        }

        try
        {
            this.DeleteReason = row.DeleteReason;
        }
        catch (Exception)
        {
            this.DeleteReason = string.Empty;
        }

        try
        {
            this.IP = row.IP;
        }
        catch (Exception)
        {
            this.IP = string.Empty;
        }

        try
        {
            this.ExternalMessageId = row.ExternalMessageId;
        }
        catch (Exception)
        {
            this.ExternalMessageId = string.Empty;
        }

        try
        {
            this.AnswerMessageId = row.AnswerMessageId;
        }
        catch (Exception)
        {
            this.AnswerMessageId = null;
        }

        try
        {
            this.Signature = row.Signature;
        }
        catch (Exception)
        {
            this.Signature = string.Empty;
        }
    }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [AutoIncrement]
    [Alias("MessageID")]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the topic.
    /// </summary>
    [Ignore]
    public string TopicName { get; set; }

    /// <summary>
    /// Gets or sets the topic.
    /// </summary>
    /// <value>The topic.</value>
    [Reference]
    public Topic Topic { get; set; }

    /// <summary>
    /// Gets or sets the topic id.
    /// </summary>
    [References(typeof(Topic))]
    [Required]
    [Index]

    public int TopicID { get; set; }

    /// <summary>
    /// Gets or sets the reply to.
    /// </summary>
    public int? ReplyTo { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    [Required]
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets the indent.
    /// </summary>
    [Required]
    public int Indent { get; set; }

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    /// <value>The user.</value>
    [Reference]
    public User User { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [References(typeof(User))]
    [Required]
    [Index]

    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [StringLength(255)]
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the posted.
    /// </summary>
    [Required]
    public DateTime Posted { get; set; }

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    [Alias("Message")]
    [CustomField(OrmLiteVariables.MaxTextUnicode)]
    public string MessageText { get; set; }

    /// <summary>
    /// Gets or sets the IP.
    /// </summary>
    [Required]
    [StringLength(39)]
    public string IP { get; set; }

    /// <summary>
    /// Gets or sets the edited.
    /// </summary>
    public DateTime? Edited { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    [Required]
    [Default(23)]
    [Index]
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the message flags.
    /// </summary>
    [Ignore]
    public MessageFlags MessageFlags
    {
        get => new(this.Flags);

        set => this.Flags = value.BitValue;
    }

    /// <summary>
    /// Gets or sets the edit reason.
    /// </summary>
    [StringLength(100)]
    public string EditReason { get; set; }

    /// <summary>
    /// Gets or sets the signature.
    /// </summary>
    [Ignore]
    public string Signature { get; set; }

    /// <summary>
    /// Gets or sets the is moderator changed.
    /// </summary>
    [Required]
    [Default(typeof(bool), "0")]
    public bool? IsModeratorChanged { get; set; }

    /// <summary>
    /// Gets or sets the delete reason.
    /// </summary>
    [StringLength(100)]
    public string DeleteReason { get; set; }

    /// <summary>
    /// Gets or sets the edited by.
    /// </summary>
    public int? EditedBy { get; set; }

    /// <summary>
    /// Gets or sets the external message id.
    /// </summary>
    [StringLength(255)]
    public string ExternalMessageId { get; set; }

    /// <summary>
    /// Gets or sets the user display name.
    /// </summary>
    [StringLength(255)]
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the answer message id.
    /// </summary>
    [Ignore]
    public int? AnswerMessageId { get; set; }
}