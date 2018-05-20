/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The Shoutbox Repository Extensions
    /// </summary>
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