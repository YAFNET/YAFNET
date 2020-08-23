/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

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
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The UserPMessage Repository Extensions
    /// </summary>
    public static class UserPMessageRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The mark as read.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userPMessageId">
        /// The user p message id.
        /// </param>
        public static void MarkAsRead(this IRepository<UserPMessage> repository, [NotNull] int userPMessageId)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.DbFunction.Scalar.pmessage_markread(UserPMessageID: userPMessageId);
        }

        #endregion
    }
}