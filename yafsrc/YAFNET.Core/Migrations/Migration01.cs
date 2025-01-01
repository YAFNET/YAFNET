/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Migrations;

using ServiceStack.DataAnnotations;

using YAF.Types.Models;

/// <summary>
/// Version 01 Migrations
/// </summary>
[Description("Create Initial Database")]
public class Migration01 : MigrationBase
{
    /// <summary>
    /// Migrations
    /// </summary>
    public override void Up()
    {
        this.Db.CreateTable<Board>();
        this.Db.CreateTable<Rank>();
        this.Db.CreateTable<User>();
        this.Db.CreateTable<Category>();
        this.Db.CreateTable<Forum>();
        this.Db.CreateTable<Topic>();
        this.Db.CreateTable<Message>();
        this.Db.CreateTable<Thanks>();
        this.Db.CreateTable<Buddy>();
        this.Db.CreateTable<UserAlbum>();
        this.Db.CreateTable<UserAlbumImage>();
        this.Db.CreateTable<Active>();
        this.Db.CreateTable<ActiveAccess>();
        this.Db.CreateTable<Activity>();
        this.Db.CreateTable<AdminPageUserAccess>();
        this.Db.CreateTable<Group>();
        this.Db.CreateTable<BannedIP>();
        this.Db.CreateTable<BannedName>();
        this.Db.CreateTable<BannedEmail>();
        this.Db.CreateTable<BannedUserAgent>();
        this.Db.CreateTable<CheckEmail>();
        this.Db.CreateTable<Poll>();
        this.Db.CreateTable<Choice>();
        this.Db.CreateTable<PollVote>();
        this.Db.CreateTable<AccessMask>();
        this.Db.CreateTable<ForumAccess>();
        this.Db.CreateTable<MessageHistory>();
        this.Db.CreateTable<MessageReported>();
        this.Db.CreateTable<MessageReportedAudit>();
        this.Db.CreateTable<WatchForum>();
        this.Db.CreateTable<WatchTopic>();
        this.Db.CreateTable<Attachment>();
        this.Db.CreateTable<UserGroup>();
        this.Db.CreateTable<UserForum>();
        this.Db.CreateTable<PrivateMessage>();
        this.Db.CreateTable<ReplaceWords>();
        this.Db.CreateTable<SpamWords>();
        this.Db.CreateTable<Registry>();
        this.Db.CreateTable<EventLog>();
        this.Db.CreateTable<BBCode>();
        this.Db.CreateTable<Medal>();
        this.Db.CreateTable<GroupMedal>();
        this.Db.CreateTable<UserMedal>();
        this.Db.CreateTable<IgnoreUser>();
        this.Db.CreateTable<TopicReadTracking>();
        this.Db.CreateTable<ForumReadTracking>();
        this.Db.CreateTable<ReputationVote>();
        this.Db.CreateTable<Tag>();
        this.Db.CreateTable<TopicTag>();
        this.Db.CreateTable<ProfileDefinition>();
        this.Db.CreateTable<ProfileCustom>();

        // Create Identity tables
        this.Db.CreateTable<AspNetUsers>();
        this.Db.CreateTable<AspNetRoles>();
        this.Db.CreateTable<AspNetUserClaims>();
        this.Db.CreateTable<AspNetUserLogins>();
        this.Db.CreateTable<AspNetUserRoles>();
    }
}