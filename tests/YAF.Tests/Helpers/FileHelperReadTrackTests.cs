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

namespace YAF.Tests.Helpers;

using System.IO;

using NSubstitute;

using YAF.Core.Helpers;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Objects;

/// <summary>
/// Small helpers: file names and the image lists of the admin pages and the read tracking of forums and topics.
/// </summary>
[TestFixture]
public class FileHelperReadTrackTests
{
    [TestCase("photo.png", true)]
    [TestCase("my photo (1).jpg", true)]
    [TestCase("a/b.png", false)]
    [TestCase("a\0b.png", false)]
    [TestCase("", false)]
    [Description("File names without invalid characters are valid")]
    public void Validate_File_Name_Test(string fileName, bool expected)
    {
        FileHelper.ValidateFileName(fileName).Should().Be(expected);
    }

    [Test]
    [Description("Invalid characters are removed from file names")]
    public void Clean_File_Name_Test()
    {
        FileHelper.CleanFileName("a/b\0c|.png").Should().NotContainAny("/", "\0").And.EndWith("c.png").And.StartWith("a");
        FileHelper.CleanFileName("photo.png").Should().Be("photo.png");
    }

    [Test]
    [Description("Only images are added to the image lists, with the address of the file")]
    public void Add_Image_Files_Test()
    {
        var folder = Path.Combine(Path.GetTempPath(), $"yaf-images-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);

        try
        {
            var files = new[] { "logo.png", "photo.JPG", "notes.txt", "script.js" }
                .Select(name => new FileInfo(Path.Combine(folder, name))).ToList();

            var list = new List<NamedParameter> { new("None", string.Empty) };

            list.AddImageFiles(files, "images/logos");

            list.Select(i => i.Name).Should().Equal("None", "logo.png", "photo.JPG");
            list.Skip(1).Select(i => (string)i.Value).Should().AllSatisfy(v => v.Should().Contain("images/logos/"));
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    [Test]
    [Description("A topic counts as read at the later time of the topic and its forum")]
    public void Forum_Topic_Read_Test()
    {
        var readTrack = Substitute.For<IReadTrackCurrentUser>();
        var forumRead = new DateTime(2026, 1, 2);
        var topicRead = new DateTime(2026, 1, 1);

        readTrack.GetForumRead(5, null).Returns(forumRead);
        readTrack.GetTopicRead(7, null).Returns(topicRead);

        readTrack.GetForumTopicRead(5, 7, null, null).Should().Be(forumRead, "the whole forum was read later");

        readTrack.GetTopicRead(7, null).Returns(forumRead.AddDays(1));

        readTrack.GetForumTopicRead(5, 7, null, null).Should().Be(forumRead.AddDays(1));
    }

    [Test]
    [Description("Several forums and topics are marked as read at once")]
    public void Set_Read_Lists_Test()
    {
        var readTrack = Substitute.For<IReadTrackCurrentUser>();

        readTrack.SetForumRead([1, 2]);
        readTrack.SetTopicRead([3, 4]);

        readTrack.Received(1).SetForumRead(1);
        readTrack.Received(1).SetForumRead(2);
        readTrack.Received(1).SetTopicRead(3);
        readTrack.Received(1).SetTopicRead(4);
    }
}
