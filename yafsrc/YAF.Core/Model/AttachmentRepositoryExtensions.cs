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

namespace YAF.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using YAF.Core.Helpers;
using YAF.Types.Models;

/// <summary>
///     The attachment repository extensions.
/// </summary>
public static class AttachmentRepositoryExtensions
{
    /// <summary>
    /// The attachment of a post: [attach]id[/attach] (like the bb code, any case).
    /// </summary>
    private static readonly Regex AttachTag = new(
        @"\[attach\]\s*(?<id>\d{1,9})\s*\[/attach\]",
        RegexOptions.IgnoreCase,
        RegexHelper.Timeout);

    /// <summary>
    /// Gets the posts that show the attachments: their text contains "[attach]id[/attach]" (not deleted posts of
    /// not deleted topics). The usages were the hits of the search index for "]id[", the index finds the number
    /// in any post and only after it was updated.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns>The posts (message id, topic id and topic name) by the attachment id.</returns>
    public static Dictionary<int, List<(int MessageId, int TopicId, string TopicName)>> GetUsages(
        this IRepository<Attachment> repository,
        IList<int> attachmentIds)
    {
        var usages = attachmentIds.Distinct().ToDictionary(id => id, _ => new List<(int, int, string)>());

        if (usages.Count == 0)
        {
            return usages;
        }

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        expression.Join<Topic>((m, t) => t.ID == m.TopicID)
            .Where<Message, Topic>((m, t) => (m.Flags & 8) != 8 && (t.Flags & 8) != 8);

        // the posts with one of the numbers after "attach]" (without "[", it is a wildcard of a like on SQL Server,
        // in lower case: the tag can be upper case), the tags are checked below
        var patterns = usages.Keys.Select(id => $"attach]{id}").ToList();

        expression.And(
            string.Join(
                " OR ",
                patterns.Select(
                    (_, index) => $"LOWER({expression.Column<Message>(m => m.MessageText, true)}) LIKE {{{index}}}")),
            patterns.Select(pattern => (object)$"%{pattern}%").ToArray());

        var messages = repository.DbAccess.Execute(db => db.Connection.SelectMulti<Message, Topic>(expression));

        foreach (var (message, topic) in messages)
        {
            var ids = AttachTag.Matches(message.MessageText).Cast<Match>()
                .Select(match => int.Parse(match.Groups["id"].Value))
                .Distinct();

            foreach (var id in ids)
            {
                if (usages.TryGetValue(id, out var list))
                {
                    list.Add((message.ID, topic.ID, topic.TopicName));
                }
            }
        }

        return usages;
    }

    /// <summary>
    /// Gets All User Attachments by Board ID as paged Result
    /// </summary>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="count">
    /// The count.
    /// </param>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="pageIndex">
    /// Index of the page.
    /// </param>
    /// <param name="pageSize">
    /// Size of the page.
    /// </param>
    /// <returns>
    /// The <see cref="List"/>.
    /// </returns>
    public static List<Tuple<User, Attachment>> GetByBoardPaged<T>(
        this IRepository<T> repository,
        out int count,
        int boardId,
        int? pageIndex = 0,
        int? pageSize = 10000000)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

        expression.Join<Attachment>((user, attach) => attach.UserID == user.ID);

        expression.Where(u => u.BoardID == boardId);

        count = repository.DbAccess.Execute(db => db.Connection.Count(expression)).ToType<int>();

        expression.OrderByDescending<Attachment>(item => item.ID).Page(pageIndex + 1, pageSize);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<User, Attachment>(expression));
    }

    /// <summary>
    /// Gets all Messages with Attachments
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <returns>
    /// The <see cref="List"/>.
    /// </returns>
    public static List<Message> GetMessageAttachments(this IRepository<Attachment> repository)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        expression.Join<Attachment>((m, a) => a.MessageID == m.ID);

        return repository.DbAccess.Execute(db => db.Connection.Select(expression));
    }

    /// <summary>
    /// Deletes the attachments with their files (attachments stored in the file system, the rows only left the
    /// files on the server).
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="where">
    /// The attachments to delete.
    /// </param>
    public static void DeleteWithFiles(
        this IRepository<Attachment> repository,
        System.Linq.Expressions.Expression<Func<Attachment, bool>> where)
    {
        repository.Get(where).Select(a => a.ID).ToList().ForEach(repository.Delete);
    }

    /// <summary>
    /// The delete.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="attachmentId">
    /// The board id.
    /// </param>
    public static void Delete(this IRepository<Attachment> repository, int attachmentId)
    {
        var attachment = repository.GetById(attachmentId);

        if (attachment != null)
        {
            try
            {
                attachment.DeleteFile();
            }
            catch (Exception e)
            {
                // error deleting that file...
                BoardContext.Current.Get<ILoggerService>().Warn(e, "Error Deleting Attachment");
            }
        }

        repository.DeleteById(attachmentId);
    }

    /// <summary>
    /// Increments the download counter.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="attachmentId">The attachment identifier.</param>
    public static void IncrementDownloadCounter(this IRepository<Attachment> repository, int attachmentId)
    {
        repository.UpdateAdd(() => new Attachment { Downloads = 1 }, a => a.ID == attachmentId);
    }

    /// <summary>
    /// The save.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="bytes">The bytes.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="fileData">The file data.</param>
    /// <returns>Returns the new attachment identifier</returns>
    public static int Save(
        this IRepository<Attachment> repository,
        int userId,
        string fileName,
        int bytes,
        string contentType,
        byte[] fileData = null)
    {
        var entity = new Attachment
                         {
                             MessageID = 0,
                             Downloads = 0,
                             UserID = userId,
                             FileName = fileName,
                             Bytes = bytes,
                             ContentType = contentType,
                             FileData = fileData
                         };

        var newAttachmentId = repository.Insert(entity);

        repository.FireNew(entity);

        return newAttachmentId;
    }
}