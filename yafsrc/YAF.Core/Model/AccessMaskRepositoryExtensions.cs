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
    using System.Data;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    /// <summary>
    /// The access mask repository extensions.
    /// </summary>
    public static class AccessMaskRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="accessMaskID">
        /// The access mask id.
        /// </param>
        public static bool Delete(this IRepository<AccessMask> repository, int accessMaskID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var returnValue = (int)repository.DbFunction.Scalar.accessmask_delete(AccessMaskID: accessMaskID);

            if (returnValue != 0)
            {
                repository.FireDeleted(accessMaskID);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="accessMaskID">
        /// The access mask id.
        /// </param>
        /// <param name="excludeFlags">
        /// The exclude flags.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(this IRepository<AccessMask> repository, int? accessMaskID = null, int excludeFlags = 0, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.accessmask_list(
                BoardID: boardId ?? repository.BoardID, AccessMaskID: accessMaskID, ExcludeFlags: excludeFlags);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="accessMaskID">
        /// The access mask id.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="readAccess">
        /// The read access.
        /// </param>
        /// <param name="postAccess">
        /// The post access.
        /// </param>
        /// <param name="replyAccess">
        /// The reply access.
        /// </param>
        /// <param name="priorityAccess">
        /// The priority access.
        /// </param>
        /// <param name="pollAccess">
        /// The poll access.
        /// </param>
        /// <param name="voteAccess">
        /// The vote access.
        /// </param>
        /// <param name="moderatorAccess">
        /// The moderator access.
        /// </param>
        /// <param name="editAccess">
        /// The edit access.
        /// </param>
        /// <param name="deleteAccess">
        /// The delete access.
        /// </param>
        /// <param name="uploadAccess">
        /// The upload access.
        /// </param>
        /// <param name="downloadAccess">
        /// The download access.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
        public static void Save(
            this IRepository<AccessMask> repository, 
            int? accessMaskID, 
            string name, 
            bool readAccess, 
            bool postAccess, 
            bool replyAccess, 
            bool priorityAccess, 
            bool pollAccess, 
            bool voteAccess, 
            bool moderatorAccess, 
            bool editAccess, 
            bool deleteAccess, 
            bool uploadAccess, 
            bool downloadAccess, 
            short sortOrder,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.accessmask_save(
                AccessMaskID: accessMaskID,
                BoardID: boardId ?? repository.BoardID, 
                Name: name, 
                ReadAccess: readAccess, 
                PostAccess: postAccess, 
                ReplyAccess: replyAccess, 
                PriorityAccess: priorityAccess, 
                PollAccess: pollAccess, 
                VoteAccess: voteAccess, 
                ModeratorAccess: moderatorAccess, 
                EditAccess: editAccess, 
                DeleteAccess: deleteAccess, 
                UploadAccess: uploadAccess, 
                DownloadAccess: downloadAccess, 
                SortOrder: sortOrder);

            if (accessMaskID.HasValue)
            {
                repository.FireUpdated(accessMaskID);
            }
            else
            {
                repository.FireNew();
            }
        }

        #endregion
    }
}