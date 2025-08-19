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

using System.Threading.Tasks;

namespace YAF.Core.Model;

using System;
using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The Topic Tag repository extensions.
/// </summary>
public static class TopicTagRepositoryExtensions
{
    /// <summary>
    /// Adds Topic Tag
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="tagId">
    /// The tag Id.
    /// </param>
    /// <param name="topicId">
    /// The topic ID.
    /// </param>
    public static void Add(this IRepository<TopicTag> repository, int tagId, int topicId)
    {
        repository.Insert(new TopicTag { TagID = tagId, TopicID = topicId });
    }

    /// <summary>
    /// Add New or existing tags to a topic
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="tagsString">
    /// The tags as delimited string.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    public async static Task AddTagsToTopicAsync(this IRepository<TopicTag> repository, string tagsString, int topicId)
    {
        if (tagsString.IsNotSet())
        {
            return;
        }

        var tags = tagsString.Split(',');

        var boardTags = await BoardContext.Current.GetRepository<Tag>().GetByBoardIdAsync();

        tags.ForEach(
            tag =>
                {
                    tag = HtmlTagHelper.StripHtml(tag);

                    if (tag.IsNotSet())
                    {
                        return;
                    }

                    // Filter common words
                    var commonWords = BoardContext.Current.Get<ILocalization>().GetText("COMMON", "COMMON_WORDS")
                        .StringToList(',');

                    if (commonWords.Contains(tag))
                    {
                        return;
                    }

                    var existTag = boardTags.FirstOrDefault(t => t.TagName == tag);

                    if (existTag != null)
                    {
                        // add to topic
                        repository.Add(existTag.ID, topicId);
                    }
                    else
                    {
                        // save new Tag
                        var newTagId = BoardContext.Current.GetRepository<Tag>().Add(tag);

                        // add to topic
                        repository.Add(newTagId, topicId);
                    }
                });
    }

    /// <summary>
    /// List all Topic Tags
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    public static List<Tuple<TopicTag, Tag>> List(this IRepository<TopicTag> repository, int topicId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<TopicTag>();

        expression.Join<Tag>((topicTag, tag) => tag.ID == topicTag.TagID)
            .Where<TopicTag>(t => t.TopicID == topicId);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<TopicTag, Tag>(expression));
    }

    /// <summary>
    /// Lists all tags By topic.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <returns>List with all tags as delimited string.</returns>
    public static List<Tag> ListAll(
        this IRepository<TopicTag> repository)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<TopicTag>();

        expression.Join<Tag>((topicTag, tag) => tag.ID == topicTag.TagID);

        expression.Select<TopicTag, Tag>(
            (topicTag, tag) => new {
                topicTag.TopicID,
                tag.TagName
            });

        var tags = repository.DbAccess.Execute(
            db => db.Connection.Select<(int TopicID, string TagName)>(expression)).GroupBy(x => x.TopicID);

        var topicTags = tags.ToList();

        return [.. topicTags.Select(topicTag => new Tag
            { ID = topicTag.Key, TagName = topicTag.Select(t => t.TagName).ToDelimitedString(",") })];
    }

    /// <summary>
    /// List all Topic Tags
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    public static string ListAsDelimitedString(this IRepository<TopicTag> repository, int topicId)
    {
        return repository.List(topicId).Select(t => t.Item2.TagName).ToDelimitedString(",");
    }
}