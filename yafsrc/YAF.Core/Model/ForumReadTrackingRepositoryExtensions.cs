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
    using System;
    using System.Data;

    using ServiceStack.OrmLite;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    public static class ForumReadTrackingRepositoryExtensions
    {
        #region Public Methods and Operators

        public static void AddOrUpdate(this IRepository<ForumReadTracking> repository, int userID, int forumID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.readforum_addorupdate(UserID: userID, ForumID: forumID, UTCTIMESTAMP: DateTime.UtcNow);
        }

        public static bool Delete(this IRepository<ForumReadTracking> repository, int userID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = repository.DbAccess.Execute(db => db.Delete<ForumReadTracking>(x => x.UserID == userID)) == 1;

            if (success)
            {
                repository.FireDeleted();
            }

            return success;
        }

        public static DateTime? Lastread(this IRepository<ForumReadTracking> repository, int userID, int forumID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.Scalar.readforum_lastread(UserID: userID, ForumID: forumID);
        }

        #endregion
    }
}