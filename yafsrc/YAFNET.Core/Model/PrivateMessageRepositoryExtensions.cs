/* Yet Another Forum.NET
*Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Core.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

using YAF.Types.Models;

/// <summary>
/// The PrivateMessage Repository Extensions
/// </summary>
public static class PrivateMessageRepositoryExtensions
{
    /// <param name="repository">The repository.</param>
    extension(IRepository<PrivateMessage> repository)
    {
        /// <summary>
        /// Get conversation with user A (userId) and user B (toUserId)
        /// </summary>
        /// <param name="userId">The from user identifier.</param>
        /// <param name="toUserId">The to user identifier.</param>
        /// <returns>Returns the Conversation as List</returns>
        public async Task<List<PrivateMessage>> GetConversationAsync(int userId,
            int toUserId)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

            // Get conversations from From user to To User
            expression.Where(p => p.FromUserId == userId && p.ToUserId == toUserId && (p.Flags & 2) != 2)
                .Select<PrivateMessage>(p => p);

            var expression2 = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

            // Get conversations from To user to From User
            expression2.Where(p => p.ToUserId == userId && p.FromUserId == toUserId && (p.Flags & 4) != 4)
                .Select<PrivateMessage>(p => p);

            // clear lazy data.
            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

            // Mark all to messages as read
            await repository.DbAccess.ExecuteAsync(
                db =>
                {
                    var updateExpression = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

                    return db.ExecuteSqlAsync(
                        $" update {updateExpression.Table<PrivateMessage>()} set Flags = Flags | 1 where FromUserId = {toUserId} and ToUserId = {userId}");
                });

            var list = await repository.DbAccess.ExecuteAsync(
                db => db.SelectAsync<PrivateMessage>(
                    $"{expression.ToMergedParamsSelectStatement()} UNION ALL {expression2.ToMergedParamsSelectStatement()}"));

            return [.. list.OrderBy(x => x.ID)];
        }

        /// <summary>
        /// Delete the Conversation
        /// </summary>
        /// <param name="userId">The from user identifier.</param>
        /// <param name="toUserId">The to user identifier.</param>
        public async Task DeleteConversationAsync(int userId,
            int toUserId)
        {
            // Delete From UserId
            await repository.DbAccess.ExecuteAsync(
                db =>
                {
                    var updateExpression = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

                    return db.ExecuteSqlAsync(
                        $" update {updateExpression.Table<PrivateMessage>()} set Flags = Flags | 2 where FromUserId = {userId} and ToUserId = {toUserId}");
                });

            await repository.DeleteAsync(
                x => x.FromUserId == userId && x.ToUserId == toUserId && (x.Flags & 4) == 4 && (x.Flags & 2) == 2);

            // Delete to UserId
            await repository.DbAccess.ExecuteAsync(
                db =>
                {
                    var updateExpression = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

                    return db.ExecuteSqlAsync(
                        $" update {updateExpression.Table<PrivateMessage>()} set Flags = Flags | 4 where ToUserId = {userId} and FromUserId = {toUserId}");
                });

            await repository.DeleteAsync(
                x => x.ToUserId == userId && x.FromUserId == toUserId && (x.Flags & 4) == 4 && (x.Flags & 2) == 2);
        }

        /// <summary>
        /// Gets a user list with all users of existing conversations.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Returns the User List</returns>
        public async Task<List<User>> GetUserListAsync(int userId)
        {
            var list = await repository.DbAccess.ExecuteAsync(
                db =>
                {
                    var expression = db.From<PrivateMessage>(db.TableAlias("from"));

                    expression.Where(
                        from => Sql.TableAlias(from.FromUserId, "from") == userId
                                && (Sql.TableAlias(from.Flags, "from") & 2) != 2);

                    // Get from user friends
                    expression.Join<User>(
                            (from, u) => u.ID == Sql.TableAlias(from.ToUserId, "from") && u.ID != userId)
                        .Select<User>(u => u);

                    var expression2 = db.From<PrivateMessage>(db.TableAlias("to"));

                    expression2.Where(
                        to => Sql.TableAlias(to.ToUserId, "to") == userId
                              && (Sql.TableAlias(to.Flags, "to") & 4) != 4);

                    // Get from user friends
                    expression2.Join<User>(
                            (to, u) => u.ID == Sql.TableAlias(to.FromUserId, "to") && u.ID != userId)
                        .Select<User>(u => u);

                    return db.SelectAsync<User>(
                        $"{expression.ToMergedParamsSelectStatement()} UNION ALL {expression2.ToMergedParamsSelectStatement()}");
                });

            return [.. list.DistinctBy(x => x.ID).OrderBy(x => x.Name)];
        }

        /// <summary>
        /// Get latest conversation user as an asynchronous operation.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A Task&lt;User&gt; representing the asynchronous operation.</returns>
        public async Task<User> GetLatestConversationUserAsync(int userId)
        {
            var list = await repository.DbAccess.ExecuteAsync(db =>
            {
                var expression = db.From<PrivateMessage>(db.TableAlias("from"));

                expression.Where(from =>
                    Sql.TableAlias(from.ToUserId, "from") == userId && (Sql.TableAlias(from.Flags, "from") & 4) != 4 &&
                    (Sql.TableAlias(from.Flags, "from") & 1) != 1);

                expression.Join<User>((from, u) => u.ID == Sql.TableAlias(from.FromUserId, "from"));

                expression.Select<User>(u => u);

                return db.SelectAsync<User>(expression);
            });

            return list.LastOrDefault();
        }

        /// <summary>
        /// Delete all Private Messages (Chats) older than x days
        /// </summary>
        /// <param name="days">
        /// The days to delete
        /// </param>
        public Task<int> PruneAllAsync(int days)
        {
            // Delete Read Messages
            return repository.DbAccess.ExecuteAsync(
                db =>
                {
                    var q = db.From<PrivateMessage>();

                    q.Where(
                        $"{OrmLiteConfig.DialectProvider.DateDiffFunction("dd", q.Column<PrivateMessage>(b1 => b1.Created, true), OrmLiteConfig.DialectProvider.GetUtcDateFunction())} > {days}");

                    return db.DeleteAsync(q);
                });
        }
    }
}