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

namespace YAF.Tests.Extensions;

using YAF.Types.Flags;

/// <summary>
/// Tests for the YAF.Types.Flags bit flag classes
/// </summary>
[TestFixture]
public class FlagsTests
{
    [Test]
    [Description("TopicFlags properties map to the documented bit values")]
    public void TopicFlags_Properties_Test()
    {
        var flags = new TopicFlags { IsLocked = true, IsDeleted = true, IsPersistent = true };

        flags.BitValue.Should().Be(1 + 8 + 512);

        flags.IsDeleted = false;

        flags.BitValue.Should().Be(1 + 512);
        flags.IsLocked.Should().BeTrue();
        flags.IsDeleted.Should().BeFalse();
        flags.IsPersistent.Should().BeTrue();
    }

    [Test]
    [Description("TopicFlags constructors agree with each other")]
    public void TopicFlags_Constructors_Test()
    {
        new TopicFlags(TopicFlags.Flags.IsLocked | TopicFlags.Flags.IsDeleted).BitValue.Should().Be(9);
        new TopicFlags((object)9).BitValue.Should().Be(9);
        new TopicFlags(9).IsDeleted.Should().BeTrue();
        new TopicFlags().BitValue.Should().Be(0);
        new TopicFlags(true, false, false, true).BitValue.Should().Be(9);
    }

    [Test]
    [Description("The indexer reads and writes individual bits")]
    public void Indexer_Test()
    {
        var flags = new TopicFlags(0);

        flags[5] = true;

        flags[5].Should().BeTrue();
        flags.BitValue.Should().Be(32);

        flags[5] = false;

        flags.BitValue.Should().Be(0);
    }

    [Test]
    [Description("MessageFlags round trip through their bit value")]
    public void MessageFlags_RoundTrip_Test()
    {
        var flags = new MessageFlags { IsApproved = true, IsHtml = true, IsBBCode = true };

        var copy = new MessageFlags(flags.BitValue);

        copy.IsApproved.Should().BeTrue();
        copy.IsHtml.Should().BeTrue();
        copy.IsBBCode.Should().BeTrue();
        copy.IsLocked.Should().BeFalse();
        copy.IsReported.Should().BeFalse();
    }

    [TestCase(TopicFlags.Flags.IsLocked, 0)]
    [TestCase(TopicFlags.Flags.IsDeleted, 3)]
    [TestCase(TopicFlags.Flags.IsPersistent, 9)]
    [Description("EnumToIndex returns the bit index of a single-bit flag")]
    public void EnumToIndex_Test(TopicFlags.Flags flag, int expectedIndex)
    {
        var index = new TopicFlags().EnumToIndex(flag);

        index.Should().Be(expectedIndex);
        new TopicFlags(flag)[index].Should().BeTrue();
    }
}
