/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Model
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;

    using ServiceStack.OrmLite;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The attachment repository extensions.
    /// </summary>
    public static class AttachmentRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the Attachment by ID (without the FileData)
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="attachId">
        /// The attach id.
        /// </param>
        /// <returns>
        /// The <see cref="Attachment"/>.
        /// </returns>
        public static Attachment GetSingleById(this IRepository<Attachment> repository, [NotNull] int attachId)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Attachment>();

            expression.Where(attach => attach.ID == attachId).Select(
                attach => new
                              {
                                  attach.ID,
                                  attach.Bytes,
                                  attach.ContentType,
                                  attach.Downloads,
                                  attach.FileName,
                                  attach.MessageID,
                                  attach.UserID
                              });

            return repository.DbAccess.Execute(db => db.Connection.Select(expression)).FirstOrDefault();
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
            CodeContracts.VerifyNotNull(repository, "repository");

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
                    BoardContext.Current.Get<ILogger>().Warn(e, "Error Deleting Attachment");
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
        public static void DeleteByMessageId(this IRepository<Attachment> repository, int messageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var attachments = repository.Get(a => a.MessageID == messageId);

            var uploadDir =
                HostingEnvironment.MapPath(
                    string.Concat(BaseUrlBuilder.ServerFileRoot, BoardFolders.Current.Uploads));

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
        public static void IncrementDownloadCounter(this IRepository<Attachment> repository, int attachmentId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateAdd(() => new Attachment { Downloads = 1 }, a => a.ID == attachmentId);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="fileData">The file data.</param>
        /// <returns>Returns the new attachment identifier</returns>
        public static int Save(
            this IRepository<Attachment> repository,
            int messageId,
            int userId,
            string fileName,
            int bytes,
            string contentType,
            byte[] fileData = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

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
}