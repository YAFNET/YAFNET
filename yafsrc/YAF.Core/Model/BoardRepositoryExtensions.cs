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
    using System.Collections.Generic;
    using System.Data;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The board repository extensions.
    /// </summary>
    public static class BoardRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardName">
        /// The board name.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="languageFile">
        /// The language file.
        /// </param>
        /// <param name="membershipAppName">
        /// The membership app name.
        /// </param>
        /// <param name="rolesAppName">
        /// The roles app name.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="userEmail">
        /// The user email.
        /// </param>
        /// <param name="userKey">
        /// The user key.
        /// </param>
        /// <param name="isHostAdmin">
        /// The is host admin.
        /// </param>
        /// <param name="rolePrefix">
        /// The role prefix.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Create(
            this IRepository<Board> repository, 
            string boardName, 
            string culture, 
            string languageFile, 
            string membershipAppName, 
            string rolesAppName, 
            string userName, 
            string userEmail, 
            string userKey, 
            bool isHostAdmin, 
            string rolePrefix)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var boardId =
                (int)
                repository.DbFunction.Scalar.board_create(
                    BoardName: boardName, 
                    Culture: culture, 
                    LanguageFile: languageFile, 
                    MembershipAppName: membershipAppName, 
                    RolesAppName: rolesAppName, 
                    UserName: userName, 
                    UserEmail: userEmail, 
                    UserKey: userKey, 
                    IsHostAdmin: isHostAdmin, 
                    RolePrefix: rolePrefix, 
                    UTCTIMESTAMP: DateTime.UtcNow);

            repository.FireNew(boardId);

            return boardId;
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        public static void Delete(this IRepository<Board> repository, int boardID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.board_delete(BoardID: boardID);

            repository.FireDeleted(boardID);
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
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(this IRepository<Board> repository, int? boardID = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.board_list(BoardID: boardID);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<Board> ListTyped(this IRepository<Board> repository, int? boardID = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return boardID.HasValue
                ? new List<Board> { repository.GetByID(boardID.Value) }
                : repository.DbAccess.Execute(cmd => cmd.Select<Board>());
        }

        /// <summary>
        /// The poststats.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="boardID">The board id.</param>
        /// <param name="styledNicks">The styled nicks.</param>
        /// <param name="showNoCountPosts">The show no count posts.</param>
        /// <returns>
        /// The <see cref="DataTable" />.
        /// </returns>
        public static DataRow Poststats(this IRepository<Board> repository, int boardID, bool styledNicks, bool showNoCountPosts)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var dt =
                (DataTable)
                repository.DbFunction.GetData.board_poststats(
                    BoardID: boardID, StyledNicks: styledNicks, ShowNoCountPosts: showNoCountPosts, GetDefaults: 0);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }

            dt =
                (DataTable)
                repository.DbFunction.GetData.board_poststats(
                    BoardID: boardID, StyledNicks: styledNicks, ShowNoCountPosts: showNoCountPosts, GetDefaults: 1);

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /// <summary>
        /// The resync.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        public static void Resync(this IRepository<Board> repository, int? boardID = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.board_resync(BoardID: boardID);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="boardID">The board id.</param>
        /// <param name="name">The name.</param>
        /// <param name="languageFile">The language file.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="allowThreaded">The allow threaded.</param>
        public static void Save(
            this IRepository<Board> repository,
            int boardID,
            string name,
            string languageFile,
            string culture,
            bool allowThreaded)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.board_save(
                BoardID: boardID,
                Name: name,
                LanguageFile: languageFile,
                Culture: culture,
                AllowThreaded: allowThreaded);

            repository.FireUpdated(boardID);
        }

        /// <summary>
        /// The stats.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataRow Stats(this IRepository<Board> repository, int? boardID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return ((DataTable)repository.DbFunction.GetData.board_stats(BoardID: boardID)).Rows[0];
        }

        /// <summary>
        /// The userstats.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataRow Userstats(this IRepository<Board> repository, int? boardID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return ((DataTable)repository.DbFunction.GetData.board_userstats(BoardID: boardID)).Rows[0];
        }

        #endregion
    }
}