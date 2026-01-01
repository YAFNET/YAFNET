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

namespace YAF.Types.Objects.Model;

/// <summary>
/// Class MessageHistoryTopic.
/// </summary>
public class MessageHistoryTopic
{
    /// <summary>
    /// Gets or sets the edit reason.
    /// </summary>
    /// <value>The edit reason.</value>
    public string EditReason { get; set; }

    /// <summary>
    /// Gets or sets the edited.
    /// </summary>
    /// <value>The edited.</value>
    public DateTime Edited { get; set; }

    /// <summary>
    /// Gets or sets the edited by.
    /// </summary>
    /// <value>The edited by.</value>
    public int EditedBy { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>The flags.</value>
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the ip.
    /// </summary>
    /// <value>The ip.</value>
    public string IP { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is moderator changed.
    /// </summary>
    /// <value><c>null</c> if [is moderator changed] contains no value, <c>true</c> if [is moderator changed]; otherwise, <c>false</c>.</value>
    public bool? IsModeratorChanged { get; set; }

    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    /// <value>The message identifier.</value>
    public int MessageID { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>The display name.</value>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the user style.
    /// </summary>
    /// <value>The user style.</value>
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    /// <value>The suspended.</value>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the forum identifier.
    /// </summary>
    /// <value>The forum identifier.</value>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets the topic identifier.
    /// </summary>
    /// <value>The topic identifier.</value>
    public int TopicID { get; set; }

    /// <summary>
    /// Gets or sets the topic.
    /// </summary>
    /// <value>The topic.</value>
    public string Topic { get; set; }

    /// <summary>
    /// Gets or sets the posted.
    /// </summary>
    /// <value>The posted.</value>
    public DateTime Posted { get; set; }

    /// <summary>
    /// Gets or sets the message ip.
    /// </summary>
    /// <value>The message ip.</value>
    public string MessageIP { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="MessageHistoryTopic"/> is selected.
    /// </summary>
    /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
    public bool Selected { get; set; }
}