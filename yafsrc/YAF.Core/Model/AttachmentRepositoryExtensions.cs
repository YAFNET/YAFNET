/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Model
{
    using System.Collections.Generic;
    using System.Data;

    using YAF.Core.Data;
    using YAF.Types;
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
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="attachmentID">
        /// The attachment id. 
        /// </param>
        public static void Delete(this IRepository<Attachment> repository, int attachmentID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.attachment_delete(AttachmentID: attachmentID);
            repository.FireDeleted(attachmentID);
        }

        /// <summary>
        /// Increments the download counter.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="attachmentID">The attachment id.</param>
        public static void IncrementDownloadCounter(this IRepository<Attachment> repository, int attachmentID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.attachment_download(AttachmentID: attachmentID);
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="messageID">
        /// The message id. 
        /// </param>
        /// <param name="attachmentID">
        /// The attachment id. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index. 
        /// </param>
        /// <param name="pageSize">
        /// The page size. 
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable List(
            this IRepository<Attachment> repository,
            int? messageID = null,
            int? attachmentID = null,
            int? boardId = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.attachment_list(
                MessageID: messageID,
                AttachmentID: attachmentID,
                boardID: boardId,
                PageIndex: pageIndex,
                PageSize: pageSize);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <param name="attachmentID">
        /// The attachment id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<Attachment> ListTyped(
            this IRepository<Attachment> repository,
            int? messageID = null,
            int? attachmentID = null,
            int? boardId = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return
                    session.GetTyped<Attachment>(
                        r =>
                        r.attachment_list(
                            MessageID: messageID,
                            AttachmentID: attachmentID,
                            boardID: boardId,
                            PageIndex: pageIndex,
                            PageSize: pageSize));
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="messageID">
        /// The message id. 
        /// </param>
        /// <param name="fileName">
        /// The file name. 
        /// </param>
        /// <param name="bytes">
        /// The bytes. 
        /// </param>
        /// <param name="contentType">
        /// The content type. 
        /// </param>
        /// <param name="fileData">
        /// The file data. 
        /// </param>
        public static void Save(
            this IRepository<Attachment> repository,
            int messageID,
            string fileName,
            int bytes,
            string contentType,
            byte[] fileData = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var entity =
                new
                    {
                        MessageID = messageID,
                        FileName = fileName,
                        Bytes = bytes,
                        ContentType = contentType,
                        FileData = fileData
                    }.ToMappedEntity<Attachment>();

            repository.DbAccess.Insert(entity);

            repository.FireNew(entity);
        }

        #endregion
    }
}