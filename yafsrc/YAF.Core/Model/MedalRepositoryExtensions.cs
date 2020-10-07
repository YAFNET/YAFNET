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
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The medal repository extensions.
    /// </summary>
    public static class MedalRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Lists users assigned to the medal
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static
            List<(int MedalID, string Name, string Message, string MedalURL, string RibbonURL, string SmallMedalURL,
                string SmallRibbonURL, byte SortOrder, bool Hide, bool OnlyRibbon, int Flags, DateTime DateAwarded)>
            ListUserMedals(this IRepository<Medal> repository, [NotNull] int userId)
        {
            var expressionUser = OrmLiteConfig.DialectProvider.SqlExpression<Medal>();

            expressionUser.Join<UserMedal>((a, b) => b.MedalID == a.ID).Where<UserMedal>(b => b.UserID == userId)
                .Select<Medal, UserMedal>(
                    (a, b) => new
                    {
                        MedalID = a.ID,
                        a.Name,
                        Message = b.Message != null ? b.Message : a.Message,
                        a.MedalURL,
                        a.RibbonURL,
                        a.SmallMedalURL,
                        SmallRibbonURL = a.SmallRibbonURL != null ? a.SmallRibbonURL : a.SmallMedalURL,
                        SortOrder = (a.Flags & 8) == 0 ? a.SortOrder : b.SortOrder,
                        Hide = (a.Flags & 4) == 0 ? false : b.Hide,
                        OnlyRibbon = (a.RibbonURL == null || (a.Flags & 2) == 0) ? false : b.OnlyRibbon,
                        a.Flags,
                        b.DateAwarded
                    });

            var userMedals = repository.DbAccess.Execute(
                db => db.Connection
                    .Select<(int MedalID, string Name, string Message, string MedalURL, string RibbonURL, string
                        SmallMedalURL, string SmallRibbonURL, byte SortOrder, bool Hide, bool OnlyRibbon, int Flags, DateTime DateAwarded)>(expressionUser));

            var expressionUserGroup = OrmLiteConfig.DialectProvider.SqlExpression<Medal>();

            expressionUserGroup.Join<GroupMedal>((a, b) => b.MedalID == a.ID)
                .Join<GroupMedal, UserGroup>((b, c) => c.GroupID == b.GroupID).Where<UserGroup>(c => c.UserID == userId)
                .Select<Medal, GroupMedal>(
                    (a, b) => new
                    {
                        MedalID = a.ID,
                        a.Name,
                        Message = b.Message != null ? b.Message : a.Message,
                        a.MedalURL,
                        a.RibbonURL,
                        a.SmallMedalURL,
                        SmallRibbonURL = a.SmallRibbonURL != null ? a.SmallRibbonURL : a.SmallMedalURL,
                        SortOrder = (a.Flags & 8) == 0 ? a.SortOrder : b.SortOrder,
                        Hide = (a.Flags & 4) == 0 ? false : b.Hide,
                        OnlyRibbon = (a.RibbonURL == null || (a.Flags & 2) == 0) ? false : b.OnlyRibbon,
                        a.Flags,
                        DateAwarded = default(DateTime)
                    });

            var userGroupMedals = repository.DbAccess.Execute(
                db => db.Connection
                    .Select<(int MedalID, string Name, string Message, string MedalURL, string RibbonURL, string
                        SmallMedalURL, string SmallRibbonURL, byte SortOrder, bool Hide, bool OnlyRibbon, int Flags,
                        DateTime DateAwarded)>(expressionUserGroup));

            return userMedals.Union(userGroupMedals).Distinct().ToList();
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="medalId">
        /// The medal Id.
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="description">
        /// The description. 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="category">
        /// The category. 
        /// </param>
        /// <param name="medalURL">
        /// The medal url. 
        /// </param>
        /// <param name="ribbonURL">
        /// The ribbon url. 
        /// </param>
        /// <param name="smallMedalURL">
        /// The small medal url. 
        /// </param>
        /// <param name="smallRibbonURL">
        /// The small ribbon url. 
        /// </param>
        /// <param name="flags">
        /// The flags. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public static void Save(
            this IRepository<Medal> repository, 
            int? medalId, 
            string name, 
            string description, 
            string message, 
            string category, 
            string medalURL, 
            string ribbonURL, 
            string smallMedalURL, 
            string smallRibbonURL, 
            int flags, 
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository);

            if (medalId.HasValue)
            {
                repository.UpdateOnly(
                    () => new Medal
                    {
                        BoardID = boardId ?? repository.BoardID,
                        Name = name,
                        Description = description,
                        Message = message,
                        Category = category,
                        MedalURL = medalURL,
                        RibbonURL = ribbonURL,
                        SmallMedalURL = smallMedalURL,
                        SmallRibbonURL = smallRibbonURL,
                        SortOrder = 0,
                        Flags = flags
                    },
                    medal => medal.ID == medalId.Value);
                
                repository.FireUpdated(medalId);
            }
            else
            {
               var newId = repository.Insert(
                    new Medal
                    {
                        BoardID = boardId ?? repository.BoardID,
                        Name = name,
                        Description = description,
                        Message = message,
                        Category = category,
                        MedalURL = medalURL,
                        RibbonURL = ribbonURL,
                        SmallMedalURL = smallMedalURL,
                        SmallRibbonURL = smallRibbonURL,
                        SortOrder = 0,
                        Flags = flags
                    });

                repository.FireNew(newId);
            }
        }

        #endregion
    }
}