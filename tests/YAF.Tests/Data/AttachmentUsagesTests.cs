/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Data;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// The posts that show an attachment (the usages on the attachments page of the admin).
/// </summary>
[TestFixture]
[NonParallelizable]
public class AttachmentUsagesTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");

        var forum = this.db.AddForum("General");

        this.topic = this.db.AddTopic(forum, this.alice, "Files");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A post shows an attachment with [attach]id[/attach] in any case, the number alone or a longer id is no usage")]
    public void Usages_Of_The_Attach_Tags_Test()
    {
        var first = this.db.AddMessage(this.topic, this.alice, "see [attach]5[/attach]");
        var second = this.db.AddMessage(this.topic, this.alice, "[ATTACH]5[/Attach] and [attach] 7 [/attach] and [attach]5[/attach]");

        // the number in the text, "]5[" (the search of the usages) and a longer id
        this.db.AddMessage(this.topic, this.alice, "the number 5 and ]5[ and [attach]55[/attach]");

        var usages = this.Repository<Attachment>().GetUsages([5, 7, 9]);

        usages[5].Select(u => u.MessageId).Should().BeEquivalentTo([first, second]);
        usages[7].Select(u => u.MessageId).Should().Equal(second);
        usages[9].Should().BeEmpty();
        usages[5].Should().OnlyContain(u => u.TopicId == this.topic && u.TopicName == "Files");
    }

    [Test]
    [Description("A deleted post is no usage")]
    public void Deleted_Post_Is_No_Usage_Test()
    {
        this.db.AddMessage(this.topic, this.alice, "[attach]8[/attach]", configure: m => m.Flags |= 8);

        this.Repository<Attachment>().GetUsages([8])[8].Should().BeEmpty();
    }

    [Test]
    [Description("Without attachments there is no query")]
    public void No_Attachments_Test()
    {
        this.Repository<Attachment>().GetUsages([]).Should().BeEmpty();
    }

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }
}
