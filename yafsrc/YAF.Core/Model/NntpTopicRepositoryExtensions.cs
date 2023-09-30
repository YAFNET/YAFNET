/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
namespace YAF.Core.Model;

using System;

using YAF.Types.Models;

/// <summary>
///     The NntpTopic repository extensions.
/// </summary>
public static class NntpTopicRepositoryExtensions
{
    /// <summary>
    /// The save message.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="nntpForum">
    /// The NNTP Forum.
    /// </param>
    /// <param name="topicName">
    /// The topic name.
    /// </param>
    /// <param name="body">
    /// The body.
    /// </param>
    /// <param name="user">
    /// The user
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="ip">
    /// The IP Address.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    /// <param name="referenceMessageId">
    /// The reference message id.
    /// </param>
    public static void SaveMessage(
        this IRepository<NntpTopic> repository,
        NntpForum nntpForum,
        string topicName,
        string body,
        User user,
        string userName,
        string ip,
        DateTime posted,
        string referenceMessageId)
    {
        CodeContracts.VerifyNotNull(repository);

        int? replyTo = null;

        var externalMessage = BoardContext.Current.GetRepository<Message>()
            .GetSingle(m => m.ExternalMessageId == referenceMessageId);

        var forum = BoardContext.Current.GetRepository<Forum>().GetById(nntpForum.ForumID);

        Topic topic;

        if (externalMessage != null)
        {
            // -- referenced message exists
            replyTo = externalMessage.ID;
            topic = BoardContext.Current.GetRepository<Topic>().GetById(externalMessage.TopicID);
        }
        else
        {
            // --thread doesn't exists
            topic = new Topic
                        {
                            ForumID = nntpForum.ForumID,
                            UserID = user.ID,
                            UserName = userName,
                            UserDisplayName = userName,
                            Posted = posted,
                            TopicName = topicName,
                            Views = 0,
                            Priority = 0,
                            NumPosts = 0
                        };

            topic.ID = BoardContext.Current.GetRepository<Topic>().Insert(topic);

            repository.Insert(
                new NntpTopic { NntpForumID = nntpForum.ID, Thread = string.Empty, TopicID = topic.ID });
        }

        BoardContext.Current.GetRepository<Message>().SaveNew(
            forum,
            topic,
            user,
            body,
            userName,
            ip,
            posted,
            replyTo,
            new MessageFlags(17));
    }
}