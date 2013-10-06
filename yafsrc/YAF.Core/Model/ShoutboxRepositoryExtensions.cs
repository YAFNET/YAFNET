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

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    public static class ShoutboxRepositoryExtensions
    {
        #region Public Methods and Operators

        public static void ClearMessages(this IRepository<ShoutboxMessage> repository, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.shoutbox_clearmessages(BoardId: boardId ?? repository.BoardID, UTCTIMESTAMP: DateTime.UtcNow);
            repository.FireDeleted();
        }

        public static DataTable GetMessages(
            this IRepository<ShoutboxMessage> repository, int numberOfMessages, bool styledNicks, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.shoutbox_getmessages(
                BoardId: boardId ?? repository.BoardID,
                NumberOfMessages: numberOfMessages,
                StyledNicks: styledNicks);
        }

        public static IList<ShoutboxMessage> GetMessagesTyped(
            this IRepository<ShoutboxMessage> repository, int numberOfMessages, bool styledNicks, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return
                    session.GetTyped<ShoutboxMessage>(
                        r =>
                        r.shoutbox_getmessages(BoardId: boardId ?? repository.BoardID, NumberOfMessages: numberOfMessages, StyledNicks: styledNicks));
            }
        }

        public static void SaveMessage(
            this IRepository<ShoutboxMessage> repository, string message, string userName, int userID, string ip, int? boardId = null, DateTime? date = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.shoutbox_savemessage(
                UserName: userName,
                BoardId: boardId ?? repository.BoardID,
                UserID: userID,
                Message: message,
                Date: date,
                IP: ip,
                UTCTIMESTAMP: DateTime.UtcNow);

            repository.FireNew();
        }

        #endregion
    }
}