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

namespace YAF.Tests.Services;

using System.Text.RegularExpressions;

using YAF.Core.Services;
using YAF.Core.Utilities;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Objects;

/// <summary>
/// Tests for BadWordReplace, SpamWordCheck and MimeTypes. The word lists are pre-filled in the
/// cache so the services never fall through to their repositories.
/// </summary>
[TestFixture]
[NonParallelizable]
public class WordFilterTests
{
    private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
    }

    [TearDown]
    public void TearDown()
    {
        this.board.Dispose();
    }

    [Test]
    [Description("BadWordReplace replaces every active bad word, case-insensitively")]
    public void BadWordReplace_Replace_Test()
    {
        this.board.Cache[Constants.Cache.ReplaceWords] = new List<BadWordReplaceItem>
        {
            new("****", "darn", Options),
            new("kitten", @"\bcat\b", Options)
        };

        var service = this.CreateBadWordReplace();

        service.Replace("Darn, the cat ate my catalog!").Should().Be("****, the kitten ate my catalog!");
    }

    [Test]
    [Description("BadWordReplace encodes the good words in html, in text they stay unchanged (the text is encoded later)")]
    public void BadWordReplace_ReplaceHtml_Test()
    {
        this.board.Cache[Constants.Cache.ReplaceWords] = new List<BadWordReplaceItem>
        {
            new("<i>nice</i> & \"good\"", "darn", Options)
        };

        var service = this.CreateBadWordReplace();

        service.ReplaceHtml("Oh <b>darn</b>").Should().Be("Oh <b>&lt;i&gt;nice&lt;/i&gt; &amp; &quot;good&quot;</b>");
        service.Replace("Oh darn").Should().Be("Oh <i>nice</i> & \"good\"");
        service.ReplaceHtml(null).Should().BeNull();
    }

    [Test]
    [Description("BadWordReplace skips inactive and invalid expressions")]
    public void BadWordReplace_Skips_Inactive_And_Invalid_Test()
    {
        var inactive = new BadWordReplaceItem("x", "darn", Options) { Active = false };
        var invalid = new BadWordReplaceItem("x", "[unclosed", Options);

        this.board.Cache[Constants.Cache.ReplaceWords] = new List<BadWordReplaceItem> { inactive, invalid };

        invalid.BadWordRegEx.Should().BeNull();
        this.CreateBadWordReplace().Replace("darn it").Should().Be("darn it");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    [Description("BadWordReplace returns unset input without touching the word list")]
    public void BadWordReplace_Unset_Input_Test(string input)
    {
        // nothing in the cache: reading the word list would hit the (unregistered) repository
        this.CreateBadWordReplace().Replace(input).Should().Be(input);
    }

    [Test]
    [Description("SpamWordCheck reports the first matching spam word")]
    public void SpamWordCheck_Match_Test()
    {
        this.board.Cache[Constants.Cache.SpamWords] = new List<SpamWordCheckItem>
        {
            new(@"\bviagra\b", Options),
            new(@"casino\w*", Options)
        };

        var service = this.CreateSpamWordCheck();

        service.CheckForSpamWord("Visit our CASINOS today", out var spamWord).Should().BeTrue();
        spamWord.Should().Be("CASINOS");

        service.CheckForSpamWord("A perfectly normal post", out spamWord).Should().BeFalse();
        spamWord.Should().BeEmpty();
    }

    [Test]
    [Description("SpamWordCheck ignores inactive and invalid expressions")]
    public void SpamWordCheck_Skips_Inactive_And_Invalid_Test()
    {
        this.board.Cache[Constants.Cache.SpamWords] = new List<SpamWordCheckItem>
        {
            new("spam", Options) { Active = false },
            new("(unclosed", Options)
        };

        this.CreateSpamWordCheck().CheckForSpamWord("spam spam spam", out var spamWord).Should().BeFalse();
        spamWord.Should().BeEmpty();
    }

    [TestCase(null)]
    [TestCase("")]
    [Description("SpamWordCheck treats unset input as clean")]
    public void SpamWordCheck_Unset_Input_Test(string input)
    {
        this.CreateSpamWordCheck().CheckForSpamWord(input, out var spamWord).Should().BeFalse();
        spamWord.Should().BeEmpty();
    }

    [TestCase("photo.jpg", "image/jpeg", true)]
    [TestCase("PHOTO.JPG", "image/jpeg", true)]
    [TestCase("archive.zip", "application/zip", true)]
    [TestCase("photo.jpg", "application/zip", false)]
    [TestCase("script.exe", "application/octet-stream", false)]
    [TestCase("noextension", "image/jpeg", false)]
    [Description("FileMatchContentType only accepts known extension / content type pairs")]
    public void MimeTypes_FileMatchContentType_Test(string fileName, string contentType, bool expected)
    {
        // MimeTypes keeps the list in a static field after the first call, so every test uses the same list
        this.board.Cache["MimeTypes"] = new List<MimeType>
        {
            new() { Extension = "jpg", Type = "image/jpeg" },
            new() { Extension = "zip", Type = "application/zip" }
        };

        MimeTypes.FileMatchContentType(fileName, contentType).Should().Be(expected);
    }

    private BadWordReplace CreateBadWordReplace()
    {
        return new BadWordReplace(
            this.board.Cache,
            this.board.Logger,
            this.board.Resolve<IServiceLocator>());
    }

    private SpamWordCheck CreateSpamWordCheck()
    {
        return new SpamWordCheck(
            this.board.Cache,
            this.board.Logger,
            this.board.Resolve<IServiceLocator>());
    }
}
