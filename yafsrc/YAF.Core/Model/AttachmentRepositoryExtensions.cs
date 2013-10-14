/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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

            repository.DbFunction.GetData.attachment_download(AttachmentID: attachmentID);
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