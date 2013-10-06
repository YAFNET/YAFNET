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

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    public static class WatchTopicRepositoryExtensions
    {
        public static void Add(this IRepository<WatchTopic> repository, int userID, int topicID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.watchtopic_add(UserID: userID, TopicID: topicID, UTCTIMESTAMP: DateTime.UtcNow);

            repository.FireNew();
        }

        public static int? Check(this IRepository<WatchTopic> repository, int userID, int topicID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return (int?)repository.DbFunction.Scalar.watchtopic_check(UserID: userID, TopicID: topicID);
        }

        public static DataTable List(this IRepository<WatchTopic> repository, int userID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.watchtopic_list(UserID: userID);
        }

        public static IList<WatchTopic> ListTyped(this IRepository<WatchTopic> repository, int userID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return session.GetTyped<WatchTopic>(r => r.watchtopic_list(UserID: userID));
            }
        }
    }
}