/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
using System.IO;
using System.Linq;
using System.Web.Hosting;

using ServiceStack.OrmLite;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
///     The attachment repository extensions.
/// </summary>
public static class AttachmentRepositoryExtensions
{
    #region Public Methods and Operators

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
        [NotNull] this IRepository<T> repository,
        out int count,
        int boardId,
        int? pageIndex = 0,
        int? pageSize = 10000000)
    {
        CodeContracts.VerifyNotNull(repository);

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
        CodeContracts.VerifyNotNull(repository);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        expression.Join<Attachment>((m, a) => a.MessageID == m.ID);

        return repository.DbAccess.Execute(db => db.Connection.Select(expression));
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
    public static void Delete(this IRepository<Attachment> repository, [NotNull] int attachmentId)
    {
        CodeContracts.VerifyNotNull(repository);

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
    /// Deletes all Attachments by Message Id
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    public static void DeleteByMessageId(this IRepository<Attachment> repository, [NotNull] int messageId)
    {
        CodeContracts.VerifyNotNull(repository);

        var attachments = repository.Get(a => a.MessageID == messageId);

        var uploadDir = HostingEnvironment.MapPath(
            string.Concat(BaseUrlBuilder.ServerFileRoot, BoardContext.Current.Get<BoardFolders>().Uploads));

        attachments.ForEach(
            attachment =>
                {
                    try
                    {
                        var fileName = $"{uploadDir}/{messageId}.{attachment.FileName}.yafupload";

                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                    }
                    catch
                    {
                        // error deleting that file...
                    }
                });
    }

    /// <summary>
    /// Increments the download counter.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="attachmentId">The attachment identifier.</param>
    public static void IncrementDownloadCounter(this IRepository<Attachment> repository, [NotNull] int attachmentId)
    {
        CodeContracts.VerifyNotNull(repository);

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
        [NotNull] int userId,
        [NotNull] string fileName,
        [NotNull] int bytes,
        [NotNull] string contentType,
        [CanBeNull] byte[] fileData = null)
    {
        CodeContracts.VerifyNotNull(repository);

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

    #endregion
}