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
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    using YAF.Core.Data;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    ///     The Poll repository extensions.
    /// </summary>
    public static class PollRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets a typed poll group list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [NotNull]
        public static IEnumerable<TypedPollGroup> PollGroupList(
            this IRepository<Poll> repository,
            int userID,
            int? forumId,
            int boardId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction
                .GetAsDataTable(cdb => cdb.pollgroup_list(UserID: userID, ForumID: forumId, BoardID: boardId))
                .SelectTypedList(t => new TypedPollGroup(t));
        }

        /// <summary>
        /// The poll_remove.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="pollGroupID">
        /// The poll group id. The parameter should always be present.
        /// </param>
        /// <param name="pollID">
        /// The poll id. If null all polls in a group a deleted.
        /// </param>
        /// <param name="boardId">
        /// The BoardID id.
        /// </param>
        /// <param name="removeCompletely">
        /// The RemoveCompletely. If true and pollID is null , all polls in a group are deleted completely,
        ///   else only one poll is deleted completely.
        /// </param>
        public static void Remove(
            this IRepository<Poll> repository,
            [NotNull] object pollGroupID,
            [NotNull] object pollID,
            [NotNull] object boardId,
            bool removeCompletely)
        {
            repository.DbFunction.Scalar.poll_remove(
                PollGroupID: pollGroupID,
                PollID: pollID,
                BoardID: boardId,
                RemoveCompletely: removeCompletely);
        }

        /// <summary>
        /// The poll_stats.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="pollId">
        /// The poll id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable StatsAsDataTable(this IRepository<Poll> repository, int? pollId)
        {
            return repository.DbFunction.GetData.poll_stats(PollID: pollId);
        }

        /// <summary>
        /// The poll_update.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="pollID">
        /// The poll id.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="closes">
        /// The closes.
        /// </param>
        /// <param name="isBounded">
        /// The is bounded.
        /// </param>
        /// <param name="isClosedBounded">
        /// The is closed bounded.
        /// </param>
        /// <param name="allowMultipleChoices">
        /// The allow Multiple Choices option.
        /// </param>
        /// <param name="showVoters">
        /// The show Voters.
        /// </param>
        /// <param name="allowSkipVote">
        /// The allow Skip Vote.
        /// </param>
        /// <param name="questionPath">
        /// The question file path.
        /// </param>
        /// <param name="questionMime">
        /// The question file mime type.
        /// </param>
        public static void Update(
            this IRepository<Poll> repository,
            [NotNull] object pollID,
            [NotNull] object question,
            [NotNull] object closes,
            [NotNull] object isBounded,
            bool isClosedBounded,
            bool allowMultipleChoices,
            bool showVoters,
            bool allowSkipVote,
            [NotNull] object questionPath,
            [NotNull] object questionMime)
        {
            repository.DbFunction.Scalar.poll_update(
                PollID: pollID,
                Question: question,
                Closes: closes,
                QuestionObjectPath: questionPath,
                QuestionMimeType: questionMime,
                IsBounded: isBounded,
                IsClosedBounded: isClosedBounded,
                AllowMultipleChoices: allowMultipleChoices,
                ShowVoters: showVoters,
                AllowSkipVote: allowSkipVote);
        }

        /// <summary>
        /// The pollgroup_attach.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="pollGroupId">
        /// The poll group id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The pollgroup_attach.
        /// </returns>
        public static int PollGroupAttach(
            this IRepository<Poll> repository,
            int? pollGroupId,
            int? topicId,
            int? forumId,
            int? categoryId,
            int? boardId)
        {
            return repository.DbFunction.GetData.pollgroup_attach(
                PollGroupID: pollGroupId,
                TopicID: topicId,
                ForumID: forumId,
                CategoryID: categoryId,
                BoardID: boardId);
        }

        /// <summary>
        /// The poll_remove.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="pollGroupID">
        /// The poll group id. The parameter should always be present.
        /// </param>
        /// <param name="topicId">
        /// The topic identifier.
        /// </param>
        /// <param name="forumId">
        /// The forum identifier.
        /// </param>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="boardId">
        /// The BoardID id.
        /// </param>
        /// <param name="removeCompletely">
        /// The RemoveCompletely. If true and pollID is null , all polls in a group are deleted completely,
        /// else only one poll is deleted completely.
        /// </param>
        /// <param name="removeEverywhere">
        /// if set to <c>true</c> [remove everywhere].
        /// </param>
        public static void PollGroupRemove(
            this IRepository<Poll> repository,
            [NotNull] object pollGroupID,
            [NotNull] object topicId,
            [NotNull] object forumId,
            [NotNull] object categoryId,
            [NotNull] object boardId,
            bool removeCompletely,
            bool removeEverywhere)
        {
            repository.DbFunction.Scalar.pollgroup_remove(
                PollGroupID: pollGroupID,
                TopicID: topicId,
                ForumID: forumId,
                CategoryID: categoryId,
                BoardID: boardId,
                RemoveCompletely: removeCompletely,
                RemoveEverywhere: removeEverywhere);
        }

        /// <summary>
        /// The pollgroup_stats.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="pollGroupId">
        /// The poll group id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable PollGroupStatsAsDataTable(this IRepository<Poll> repository, int? pollGroupId)
        {
            return repository.DbFunction.GetData.pollgroup_stats(PollGroupID: pollGroupId);
        }

        /// <summary>
        /// Checks for a vote in the database
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="pollGroupId">
        /// The pollGroupid.
        /// </param>
        /// <param name="userId">
        /// The userid.
        /// </param>
        /// <param name="remoteIp">
        /// The remoteip.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable PollGroupVotecheckAsDataTable(
            this IRepository<Poll> repository,
            [NotNull] object pollGroupId,
            [NotNull] object userId,
            [NotNull] object remoteIp)
        {
            return repository.DbFunction.GetData.pollgroup_votecheck(
                PollGroupID: pollGroupId,
                UserID: userId,
                RemoteIP: remoteIp);
        }

        /// <summary>
        /// The method saves many questions and answers to them in a single transaction
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="pollList">
        /// List to hold all polls data
        /// </param>
        /// <returns>
        /// Last saved poll id.
        /// </returns>
        public static int? Save(this IRepository<Poll> repository, [NotNull] List<PollSaveList> pollList)
        {
            foreach (var question in pollList)
            {
                var sb = new StringBuilder();

                // Check if the group already exists
                if (question.TopicId > 0)
                {
                    sb.Append("select @PollGroupID = PollID  from ");
                    sb.Append(CommandTextHelpers.GetObjectName("Topic"));
                    sb.Append(" WHERE TopicID = @TopicID; ");
                }
                else if (question.ForumId > 0)
                {
                    sb.Append("select @PollGroupID = PollGroupID  from ");
                    sb.Append(CommandTextHelpers.GetObjectName("Forum"));
                    sb.Append(" WHERE ForumID = @ForumID");
                }
                else if (question.CategoryId > 0)
                {
                    sb.Append("select @PollGroupID = PollGroupID  from ");
                    sb.Append(CommandTextHelpers.GetObjectName("Category"));
                    sb.Append(" WHERE CategoryID = @CategoryID");
                }

                // the group doesn't exists, create a new one
                sb.Append("IF @PollGroupID IS NULL BEGIN INSERT INTO ");
                sb.Append(CommandTextHelpers.GetObjectName("PollGroupCluster"));
                sb.Append("(UserID,Flags ) VALUES(@UserID, @Flags) SET @NewPollGroupID = SCOPE_IDENTITY(); END; ");

                sb.Append("INSERT INTO ");
                sb.Append(CommandTextHelpers.GetObjectName("Poll"));

                sb.Append(
                    question.Closes > DateTime.MinValue
                        ? "(Question,Closes, UserID,PollGroupID,ObjectPath,MimeType,Flags) "
                        : "(Question,UserID, PollGroupID, ObjectPath, MimeType,Flags) ");

                sb.Append(" VALUES(");
                sb.Append("@Question");

                if (question.Closes > DateTime.MinValue)
                {
                    sb.Append(",@Closes");
                }

                sb.Append(
                    ",@UserID, (CASE WHEN  @NewPollGroupID IS NULL THEN @PollGroupID ELSE @NewPollGroupID END), @QuestionObjectPath,@QuestionMimeType,@PollFlags");
                sb.Append("); ");
                sb.Append("SET @PollID = SCOPE_IDENTITY(); ");

                // The cycle through question reply choices
                for (uint choiceCount = 0; choiceCount < question.Choice.GetUpperBound(1) + 1; choiceCount++)
                {
                    if (question.Choice[0, choiceCount].IsNotSet())
                    {
                        continue;
                    }

                    sb.Append("INSERT INTO ");
                    sb.Append(CommandTextHelpers.GetObjectName("Choice"));
                    sb.Append("(PollID,Choice,Votes,ObjectPath,MimeType) VALUES (");
                    sb.AppendFormat(
                        "@PollID,@Choice{0},@Votes{0},@ChoiceObjectPath{0}, @ChoiceMimeType{0}",
                        choiceCount);
                    sb.Append("); ");
                }

                // we don't update if no new group is created
                sb.Append("IF  @PollGroupID IS NULL BEGIN  ");

                // fill a pollgroup field - double work if a poll exists
                if (question.TopicId > 0)
                {
                    sb.Append("UPDATE ");
                    sb.Append(CommandTextHelpers.GetObjectName("Topic"));
                    sb.Append(" SET PollID = @NewPollGroupID WHERE TopicID = @TopicID; ");
                }

                // fill a pollgroup field in Forum Table if the call comes from a forum's topic list
                if (question.ForumId > 0)
                {
                    sb.Append("UPDATE ");
                    sb.Append(CommandTextHelpers.GetObjectName("Forum"));
                    sb.Append(" SET PollGroupID= @NewPollGroupID WHERE ForumID= @ForumID; ");
                }

                // fill a pollgroup field in Category Table if the call comes from a category's topic list
                if (question.CategoryId > 0)
                {
                    sb.Append("UPDATE ");
                    sb.Append(CommandTextHelpers.GetObjectName("Category"));
                    sb.Append(" SET PollGroupID= @NewPollGroupID WHERE CategoryID= @CategoryID; ");
                }

                // fill a pollgroup field in Board Table if the call comes from the main page poll
                sb.Append("END;  ");

                using (var cmd = repository.DbAccess.GetCommand(sb.ToString(), CommandType.Text))
                {
                    var ret = new SqlParameter
                                  {
                                      ParameterName = "@PollID",
                                      SqlDbType = SqlDbType.Int,
                                      Direction = ParameterDirection.Output
                                  };
                    cmd.Parameters.Add(ret);

                    var ret2 = new SqlParameter
                                   {
                                       ParameterName = "@PollGroupID",
                                       SqlDbType = SqlDbType.Int,
                                       Direction = ParameterDirection.Output
                                   };
                    cmd.Parameters.Add(ret2);

                    var ret3 = new SqlParameter
                                   {
                                       ParameterName = "@NewPollGroupID",
                                       SqlDbType = SqlDbType.Int,
                                       Direction = ParameterDirection.Output
                                   };
                    cmd.Parameters.Add(ret3);

                    cmd.AddParam("@Question", question.Question);

                    if (question.Closes > DateTime.MinValue)
                    {
                        cmd.AddParam("@Closes", question.Closes);
                    }

                    // set poll group flags
                    var groupFlags = 0;
                    if (question.IsBound)
                    {
                        groupFlags = groupFlags | 2;
                    }

                    cmd.AddParam("@UserID", question.UserId);
                    cmd.AddParam("@Flags", groupFlags);
                    cmd.AddParam(
                        "@QuestionObjectPath",
                        question.QuestionObjectPath.IsNotSet() ? string.Empty : question.QuestionObjectPath);
                    cmd.AddParam(
                        "@QuestionMimeType",
                        question.QuestionMimeType.IsNotSet() ? string.Empty : question.QuestionMimeType);

                    var pollFlags = question.IsClosedBound ? 0 | 4 : 0;
                    pollFlags = question.AllowMultipleChoices ? pollFlags | 8 : pollFlags;
                    pollFlags = question.ShowVoters ? pollFlags | 16 : pollFlags;
                    pollFlags = question.AllowSkipVote ? pollFlags | 32 : pollFlags;

                    cmd.AddParam("@PollFlags", pollFlags);

                    for (uint choiceCount1 = 0; choiceCount1 < question.Choice.GetUpperBound(1) + 1; choiceCount1++)
                    {
                        if (question.Choice[0, choiceCount1].IsNotSet())
                        {
                            continue;
                        }

                        cmd.AddParam($"@Choice{choiceCount1}", question.Choice[0, choiceCount1]);
                        cmd.AddParam($"@Votes{choiceCount1}", 0);

                        cmd.AddParam(
                            $"@ChoiceObjectPath{choiceCount1}",
                            question.Choice[1, choiceCount1].IsNotSet()
                                ? string.Empty
                                : question.Choice[1, choiceCount1]);
                        cmd.AddParam(
                            $"@ChoiceMimeType{choiceCount1}",
                            question.Choice[2, choiceCount1].IsNotSet()
                                ? string.Empty
                                : question.Choice[2, choiceCount1]);
                    }

                    if (question.TopicId > 0)
                    {
                        cmd.AddParam("@TopicID", question.TopicId);
                    }

                    if (question.ForumId > 0)
                    {
                        cmd.AddParam("@ForumID", question.ForumId);
                    }

                    if (question.CategoryId > 0)
                    {
                        cmd.AddParam("@CategoryID", question.CategoryId);
                    }

                    repository.DbAccess.ExecuteNonQuery(cmd, true);

                    if (ret.Value != DBNull.Value)
                    {
                        return (int?)ret.Value;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}