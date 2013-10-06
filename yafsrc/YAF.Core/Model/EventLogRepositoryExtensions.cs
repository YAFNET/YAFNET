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

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The event log repository extensions.
    /// </summary>
    public static class EventLogRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Create(this IRepository<EventLog> repository, int? userID, object source, string description, EventLogTypes logType = EventLogTypes.Information)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var returnValue =
                (int)
                repository.DbFunction.Scalar.eventlog_create(
                    UserID: userID, Source: source.GetType().ToString(), Description: description, Type: logType.ToInt(), UTCTIMESTAMP: DateTime.UtcNow);

            repository.FireNew();

            return returnValue;
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="eventLogID">
        /// The event log id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        public static void Delete(this IRepository<EventLog> repository, int? eventLogID, int userId, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.eventlog_delete(EventLogID: eventLogID, BoardID: boardId ?? repository.BoardID, PageUserID: userId);

            repository.FireDeleted(eventLogID);
        }

        /// <summary>
        /// The delete by user.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable DeleteByUser(this IRepository<EventLog> repository, int userId, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.eventlog_deletebyuser(BoardID: boardId ?? repository.BoardID, PageUserID: userId);
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="maxRows">
        /// The max rows.
        /// </param>
        /// <param name="maxDays">
        /// The max days.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="sinceDate">
        /// The since date.
        /// </param>
        /// <param name="toDate">
        /// The to date.
        /// </param>
        /// <param name="eventIDs">
        /// The event i ds.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(
            this IRepository<EventLog> repository, 
            int userId, 
            int maxRows, 
            int maxDays, 
            int pageIndex, 
            int pageSize, 
            DateTime sinceDate, 
            DateTime toDate, 
            string eventIDs, 
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.eventlog_list(
                BoardID: boardId ?? repository.BoardID, 
                PageUserID: userId, 
                MaxRows: maxRows, 
                MaxDays: maxDays, 
                PageIndex: pageIndex, 
                PageSize: pageSize, 
                SinceDate: sinceDate, 
                ToDate: toDate, 
                EventIDs: eventIDs, 
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        #endregion
    }
}