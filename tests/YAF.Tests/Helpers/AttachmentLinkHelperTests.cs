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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Helpers;

using YAF.Core.Helpers;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Models;

/// <summary>
/// The signed download links of attachments (<see cref="AttachmentLinkHelper"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class AttachmentLinkHelperTests
{
    private TestBoardContext board;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);

        this.alice = this.board.Database.AddUser("Alice");
        this.bob = this.board.Database.AddUser("Bob");

        this.board.LoginAs(this.bob);
        this.board.PageLoad.DownloadAccess = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A signature only belongs to its attachment, the key stays the same")]
    public void Sign_And_Validate_Test()
    {
        var signature = AttachmentLinkHelper.Sign(5);

        signature.Should().NotBeNullOrEmpty().And.NotContain("+").And.NotContain("/").And.NotContain("=");
        AttachmentLinkHelper.Sign(5).Should().Be(signature);

        AttachmentLinkHelper.IsValid(5, signature).Should().BeTrue();
        AttachmentLinkHelper.IsValid(6, signature).Should().BeFalse();
        AttachmentLinkHelper.IsValid(5, signature.Substring(1)).Should().BeFalse();
        AttachmentLinkHelper.IsValid(5, null).Should().BeFalse();
        AttachmentLinkHelper.IsValid(5, new string('a', 100)).Should().BeFalse();
    }

    [Test]
    [Description("An attachment added with [attach] (no post) is downloaded with a signed link of the forum, by its uploader, never without download permission")]
    public void CanDownload_Attachment_Without_Post_Test()
    {
        var attachment = new Attachment { ID = 7, UserID = this.alice, MessageID = 0, FileName = "a.zip" };

        // another member: only with the signed link (the attachments of the posts were not shown to anybody else)
        AttachmentLinkHelper.CanDownload(attachment, null).Should().BeFalse();
        AttachmentLinkHelper.CanDownload(attachment, AttachmentLinkHelper.Sign(8)).Should().BeFalse();
        AttachmentLinkHelper.CanDownload(attachment, AttachmentLinkHelper.Sign(7)).Should().BeTrue();

        // the uploader without a signature
        this.board.LoginAs(this.alice);
        this.board.PageLoad.DownloadAccess = true;
        AttachmentLinkHelper.CanDownload(attachment, null).Should().BeTrue();

        // the download permission of the groups still applies
        this.board.PageLoad.DownloadAccess = false;
        AttachmentLinkHelper.CanDownload(attachment, AttachmentLinkHelper.Sign(7)).Should().BeFalse();
    }
}
