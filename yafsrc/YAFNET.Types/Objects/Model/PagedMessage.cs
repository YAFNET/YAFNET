/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Types.Objects.Model;

/// <summary>
/// The paged post (message).
/// </summary>
[Serializable]
public class PagedMessage
{
    /// <summary>
    /// Gets or sets the topic id.
    /// </summary>
    public int TopicID { get; set; }

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the styles.
    /// </summary>
    public string Styles { get; set; }

    /// <summary>
    /// Gets or sets the poll id.
    /// </summary>
    public int? PollID { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public int TopicOwnerID { get; set; }

    /// <summary>
    /// Gets or sets the topic flags.
    /// </summary>
    public int TopicFlags { get; set; }

    /// <summary>
    /// Gets or sets the forum name.
    /// </summary>
    public string ForumName { get; set; }

    /// <summary>
    /// Gets or sets the forum flags.
    /// </summary>
    public int ForumFlags { get; set; }

    /// <summary>
    /// Gets or sets the message id.
    /// </summary>
    public int MessageID { get; set; }

    /// <summary>
    /// Gets or sets the posted.
    /// </summary>
    public DateTime Posted { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the IP Address.
    /// </summary>
    public string IP { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets the edit reason.
    /// </summary>
    public string EditReason { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is moderator changed.
    /// </summary>
    public bool IsModeratorChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the delete reason.
    /// </summary>
    public string DeleteReason { get; set; }

    /// <summary>
    /// Gets or sets the external message id.
    /// </summary>
    public string ExternalMessageId { get; set; }

    /// <summary>
    /// Gets or sets the answer message id.
    /// </summary>
    public int? AnswerMessageId { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the block flags.
    /// </summary>
    public int BlockFlags { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the joined.
    /// </summary>
    public DateTime Joined { get; set; }

    /// <summary>
    /// Gets or sets the avatar.
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the signature.
    /// </summary>
    public string Signature { get; set; }

    /// <summary>
    /// Gets or sets the posts.
    /// </summary>
    public int Posts { get; set; }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Gets or sets the reputation vote date.
    /// </summary>
    public DateTime ReputationVoteDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is guest.
    /// </summary>
    public bool IsGuest { get; set; }

    /// <summary>
    /// Gets or sets the views.
    /// </summary>
    public int Views { get; set; }

    /// <summary>
    /// Gets or sets the forum id.
    /// </summary>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets the rank name.
    /// </summary>
    public string RankName { get; set; }

    /// <summary>
    /// Gets or sets the rank style.
    /// </summary>
    public string RankStyle { get; set; }

    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets the edited.
    /// </summary>
    public DateTime Edited { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether has avatar image.
    /// </summary>
    public bool HasAvatarImage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is thanked by user.
    /// </summary>
    public bool IsThankedByUser { get; set; }

    /// <summary>
    /// Gets or sets the thanks number.
    /// </summary>
    public int ThanksNumber { get; set; }

    /// <summary>
    /// Gets or sets the total rows.
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// Gets or sets the page index.
    /// </summary>
    public int PageIndex { get; set; }
}