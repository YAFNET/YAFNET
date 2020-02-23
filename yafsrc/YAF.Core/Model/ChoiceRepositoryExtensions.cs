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
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The Choice Repository Extensions
    /// </summary>
    public static class ChoiceRepositoryExtensions
    {
        #region Public Methods and Operators

        public static int AddChoice(
            this IRepository<Choice> repository,
            [NotNull] int pollId,
            [NotNull] string choice,
            [NotNull] string objectPath,
            [NotNull] string mimeType)
        {
            var entity = new Choice
                             {
                                 PollID = pollId,
                                 ChoiceName = choice,
                                 Votes = 0,
                                 ObjectPath = objectPath,
                                 MimeType = mimeType
                             };

            var newId = repository.Insert(entity);

            repository.FireNew(entity);

            return newId;
        }

        public static void UpdateChoice(this IRepository<Choice> repository,
                                        [NotNull] int choiceId,
                                        [NotNull] string choice,
                                        [NotNull] string objectPath,
                                        [NotNull] string mimeType)
        {
            repository.UpdateOnly(
                () => new Choice { ChoiceName = choice, ObjectPath = objectPath, MimeType = mimeType },
                c => c.ID == choiceId);
        }

        public static void Vote(this IRepository<Choice> repository,
                                        [NotNull] int choiceId)
        {
            repository.UpdateAdd(() => new Choice { Votes = 1 }, a => a.ID == choiceId);
        }

        #endregion
    }
}