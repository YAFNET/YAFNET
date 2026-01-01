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

using System.Collections.Generic;
using System.Linq;

namespace YAF.Types.Objects;

using System.Globalization;

using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// The Search Message
/// </summary>
public class SearchMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchMessage"/> class.
    /// </summary>
    /// <param name="tupleSearchItem">
    ///     The tuple Search Item.
    /// </param>
    /// <param name="topicTags"></param>
    public SearchMessage(Tuple<Forum, Topic, Message, User> tupleSearchItem, List<Tag> topicTags)
    {
        this.MessageId = tupleSearchItem.Item3.ID;
        this.Message = tupleSearchItem.Item3.MessageText;
        this.Flags = tupleSearchItem.Item3.Flags;
        this.Posted = tupleSearchItem.Item3.Posted.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        this.UserName = tupleSearchItem.Item3.UserName.IsSet()
                            ? tupleSearchItem.Item3.UserName
                            : tupleSearchItem.Item4.Name;
        this.UserDisplayName = tupleSearchItem.Item3.UserDisplayName.IsSet() ? tupleSearchItem.Item3.UserDisplayName : tupleSearchItem.Item4.DisplayName;
        this.UserStyle = tupleSearchItem.Item4.UserStyle;
        this.UserId = tupleSearchItem.Item3.UserID;
        this.Suspended = tupleSearchItem.Item4.Suspended;

        this.TopicId = tupleSearchItem.Item2.ID;
        this.Topic = tupleSearchItem.Item2.TopicName;
        this.ForumId = tupleSearchItem.Item1.ID;
        this.ForumName = tupleSearchItem.Item1.Name;
        this.Description = tupleSearchItem.Item2.Description;

        if (topicTags.Exists(x => x.ID == tupleSearchItem.Item2.ID))
        {
            this.TopicTags = topicTags.First(x => x.ID == tupleSearchItem.Item2.ID).TagName;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchMessage"/> class.
    /// </summary>
    public SearchMessage()
    {
    }

    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    /// <value>
    /// The message identifier.
    /// </value>
    public int MessageId { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>
    /// The flags.
    /// </value>
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the posted.
    /// </summary>
    /// <value>
    /// The posted.
    /// </value>
    public string Posted { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    /// <value>
    /// The display name of the user.
    /// </value>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the user style.
    /// </summary>
    /// <value>
    /// The user style.
    /// </value>
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets the user suspended.
    /// </summary>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the topic identifier.
    /// </summary>
    /// <value>
    /// The topic identifier.
    /// </value>
    public int? TopicId { get; set; }

    /// <summary>
    /// Gets or sets the topic.
    /// </summary>
    /// <value>
    /// The topic.
    /// </value>
    public string Topic { get; set; }

    /// <summary>
    /// Gets or sets the topic tags.
    /// </summary>
    public string TopicTags { get; set; }

    /// <summary>
    /// Gets or sets the forum identifier.
    /// </summary>
    /// <value>
    /// The forum identifier.
    /// </value>
    public int? ForumId { get; set; }

    /// <summary>
    /// Gets or sets the name of the forum.
    /// </summary>
    /// <value>
    /// The name of the forum.
    /// </value>
    public string ForumName { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the topic URL.
    /// </summary>
    /// <value>
    /// The topic URL.
    /// </value>
    public string TopicUrl { get; set; }

    /// <summary>
    /// Gets or sets the message URL.
    /// </summary>
    /// <value>
    /// The message URL.
    /// </value>
    public string MessageUrl { get; set; }

    /// <summary>
    /// Gets or sets the forum URL.
    /// </summary>
    /// <value>
    /// The forum URL.
    /// </value>
    public string ForumUrl { get; set; }
}