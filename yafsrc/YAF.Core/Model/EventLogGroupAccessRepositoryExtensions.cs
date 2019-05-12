/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    using System.Data;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The EventLogGroupAccess Repository Extensions
    /// </summary>
    public static class EventLogGroupAccessExtensions
    {
        #region Public Methods and Operators

        public static void Save(
            this IRepository<EventLogGroupAccess> repository,
            [NotNull] int groupId,
            [NotNull] int eventTypeId,
            [NotNull] string eventTypeName,
            [NotNull] bool deleteAccess)
        {
            var update = repository.GetSingle(e => e.GroupID == groupId && e.EventTypeName == eventTypeName);

            if (update == null)
            {
                var entity = new EventLogGroupAccess
                                 {
                                     GroupID = groupId,
                                     EventTypeID = eventTypeId,
                                     EventTypeName = eventTypeName,
                                     DeleteAccess = deleteAccess.ToType<int>()
                                 };

                repository.Insert(entity);
            }
            else
            {
                repository.UpdateOnly(
                    () => new EventLogGroupAccess { DeleteAccess = deleteAccess.ToType<int>() },
                    e => e.GroupID == groupId && e.EventTypeID == eventTypeId);
            }
        }

        /// <summary>
        /// Deletes event log access entries from table.
        /// </summary>
        /// <param name="groupID">
        /// The group Id.
        /// </param>
        /// <param name="eventTypeId">
        /// The event Type Id.
        /// </param>
        /// <param name="eventTypeName">
        /// The event Type Name.
        /// </param>
        public static void Delete(
            this IRepository<EventLogGroupAccess> repository,
            [NotNull] int groupID,
            [NotNull] int eventTypeId,
            [NotNull] string eventTypeName)
        {
            if (eventTypeName.IsSet())
            {
                repository.Delete(e => e.GroupID == groupID && e.EventTypeID == eventTypeId);
            }
            else
            {
                repository.Delete(e => e.GroupID == groupID);
            }

        }

        /// <summary>
        /// Returns a list of access entries for a group.
        /// </summary>
        /// <param name="groupId">
        /// The group Id.
        /// </param>
        /// <param name="eventTypeId">
        /// The event Type Id.
        /// </param>
        /// <returns>Returns a list of access entries for a group.</returns>
        public static DataTable ListAsTable(
            this IRepository<EventLogGroupAccess> repository,
            [NotNull] int groupId,
            [NotNull] int? eventTypeId)
        {
            return repository.DbFunction.GetData.eventloggroupaccess_list(GroupID: groupId, EventTypeID: eventTypeId);
        }

        #endregion
    }
}