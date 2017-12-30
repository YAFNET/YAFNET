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

namespace YAF.Core.Helpers
{
    using System.Linq;

    using YAF.Core.Data;
    using YAF.Types.Interfaces.Data;

    public static class DbInformationHelper
    {
        /// <summary>
        /// Build connection string with userid and password...
        /// </summary>
        /// <param name="information"></param>
        /// <param name="parameters"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string BuildConnectionString(this IDbInformation information, IDbConnectionParam[] parameters, string userId, string password)
        {
            var paramList = parameters.ToList();

            paramList.Add(new DbConnectionParam(100, "UserID", userId));
            paramList.Add(new DbConnectionParam(101, "Password", password));

            return information.BuildConnectionString(paramList);
        }
    }
}