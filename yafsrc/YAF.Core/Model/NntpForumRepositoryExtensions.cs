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
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    ///     The NntpForum repository extensions.
    /// </summary>
    public static class NntpForumRepositoryExtensions
    {
        #region Public Methods and Operators
        /*
        /// <summary>
        ///  Get All the Thanks for the Message IDs which are in the
        ///   delimited string variable MessageIDs
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageIdsSeparatedWithColon">The message ids separated with colon.</param>
        /// <returns>
        /// Retuns All the Thanks for the Message IDs which are in the
        ///   delimited string variable MessageIDs
        /// </returns>
        public static IList<NntpForum> ListTyped(this IRepository<NntpForum> repository, int boardId, int? minutes, int? nntpForumId, bool? active)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var functionSession = repository.DbFunction.CreateSession())
            {
                return functionSession.GetTyped<NntpForum>(
                    r => r.message_getallthanks(
                        BoardID: boardId,
                        Minutes: minutes,
                        NntpForumID: nntpForumId,
                        Active: active,
                        UTCTIMESTAMP: DateTime.UtcNow));
            }
        }*/

        #endregion
    }
}