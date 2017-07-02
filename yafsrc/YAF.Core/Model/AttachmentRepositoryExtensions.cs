/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2017 Ingo Herbote
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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Core.Data;
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
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="attachmentID">
        /// The board id.
        /// </param>
        public static void Delete(this IRepository<Attachment> repository, int attachmentID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var attachment = repository.ListTyped(attachmentID: attachmentID).FirstOrDefault();

            if (attachment != null)
            {
                try
                {
                    attachment.DeleteFile();
                }
                catch (Exception e)
                {
                    // error deleting that file... 
                    YafContext.Current.Get<ILogger>().Warn(e, "Error Deleting Attachment");
                }
            }

            repository.DeleteByID(attachmentID);
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
        /// Gets the Attachment list as Data Table
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageID">The message id.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="attachmentID">The attachment id.</param>
        /// <param name="boardId">The board Id.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>
        /// Returns the Attachment <see cref="DataTable" />
        /// </returns>
        public static DataTable List(
            this IRepository<Attachment> repository,
            int? messageID = null,
            int? userID = null,
            int? attachmentID = null,
            int? boardId = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.attachment_list(
                MessageID: messageID,
                userID: userID,
                AttachmentID: attachmentID,
                boardID: boardId,
                PageIndex: pageIndex,
                PageSize: pageSize);
        }

        /// <summary>
        /// Gets the Attachment list as IList
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageID">The message id.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="attachmentID">The attachment id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>
        /// Returns the Attachment list
        /// </returns>
        public static IList<Attachment> ListTyped(
            this IRepository<Attachment> repository,
            int? messageID = null,
            int? userID = null,
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
                            userID: userID,
                            AttachmentID: attachmentID,
                            boardID: boardId,
                            PageIndex: pageIndex,
                            PageSize: pageSize));
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageID">The message id.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="fileData">The file data.</param>
        /// <returns>Returns the new attachment identifier</returns>
        public static int Save(
            this IRepository<Attachment> repository,
            int messageID,
            int userID,
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
                        UserID = userID,
                        FileName = fileName,
                        Bytes = bytes,
                        ContentType = contentType,
                        FileData = fileData
                    }.ToMappedEntity<Attachment>();

            var attachmentID = repository.DbAccess.Insert(entity, selectIdentity: true).ToType<int>();

            repository.FireNew(entity);

            return attachmentID;
        }

        #endregion
    }
}