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

namespace YAF.Tests.CoreTests.Extensions;

using System.Collections.Specialized;
using System.IO;
using System.Text;

/// <summary>
/// Tests for the enumerable, list, dictionary, NameValueCollection, stream and bit extensions in YAF.Types.Extensions
/// </summary>
[TestFixture]
public class CollectionExtensionsTests
{
    [Test]
    [Description("ForEach, ForEachFirst and ForEachIndex visit every item")]
    public void ForEach_Variants_Test()
    {
        var items = new[] { "a", "b", "c" };

        var visited = new List<string>();
        items.ForEach(visited.Add);
        visited.Should().Equal(items);

        var firsts = new List<bool>();
        items.ForEachFirst((_, isFirst) => firsts.Add(isFirst));
        firsts.Should().Equal(true, false, false);

        var indexes = new List<int>();
        items.ForEachIndex((_, i) => indexes.Add(i));
        indexes.Should().Equal(0, 1, 2);
    }

    [Test]
    [Description("IfNullEmpty and NullOrEmpty handle null sequences")]
    public void IfNullEmpty_And_NullOrEmpty_Test()
    {
        IEnumerable<int> nullList = null;

        nullList.IfNullEmpty().Should().BeEmpty();
        nullList.NullOrEmpty().Should().BeTrue();
        Array.Empty<int>().NullOrEmpty().Should().BeTrue();
        new[] { 1 }.NullOrEmpty().Should().BeFalse();
        new[] { 1 }.IfNullEmpty().Should().Equal(1);
    }

    [Test]
    [Description("Array Exists and Find delegate to System.Array")]
    public void Array_Exists_And_Find_Test()
    {
        var array = new[] { 1, 2, 3 };

        array.Exists(x => x == 2).Should().BeTrue();
        array.Exists(x => x == 5).Should().BeFalse();
        array.Find(x => x > 1).Should().Be(2);
        array.Find(x => x > 5).Should().Be(0);
    }

    [Test]
    [Description("HasItems reports whether a list contains items")]
    public void HasItems_Test()
    {
        new List<int>().HasItems().Should().BeFalse();
        new List<int> { 1 }.HasItems().Should().BeTrue();
    }

    [Test]
    [Description("AddOrUpdate and AddRange overwrite existing keys")]
    public void Dictionary_AddOrUpdate_And_AddRange_Test()
    {
        var dictionary = new Dictionary<string, int> { ["a"] = 1 };

        dictionary.AddOrUpdate("a", 2);
        dictionary.AddOrUpdate("b", 3);
        dictionary.AddRange(new Dictionary<string, int> { ["b"] = 4, ["c"] = 5 });

        dictionary.Should().BeEquivalentTo(new Dictionary<string, int> { ["a"] = 2, ["b"] = 4, ["c"] = 5 });
    }

    [Test]
    [Description("NameValueCollection helpers split comma separated values")]
    public void NameValueCollection_Test()
    {
        var collection = new NameValueCollection { { "ids", "1" }, { "ids", "2" }, { "name", "yaf" } };

        collection.Exists("ids").Should().BeTrue();
        collection.Exists("missing").Should().BeFalse();
        collection.GetValueList("ids").Should().Equal("1", "2");
        collection.GetValueList("missing").Should().BeEmpty();
        collection.GetFirstOrDefault("ids").Should().Be("1");
        collection.GetFirstOrDefault("missing").Should().BeNull();
    }

    [Test]
    [Description("Stream AsString and ToArray read the whole stream")]
    public void Stream_Test()
    {
        var bytes = Encoding.UTF8.GetBytes("hello yaf");

        using (var stream = new MemoryStream(bytes))
        {
            stream.AsString().Should().Be("hello yaf");
        }

        using (var stream = new MemoryStream(bytes))
        {
            stream.Position = 3;
            stream.ToArray().Should().Equal(bytes);
        }
    }

    [Test]
    [Description("ToHexString renders upper case hex")]
    public void ToHexString_Test()
    {
        new byte[] { 0x00, 0x0F, 0xAB, 0xFF }.ToHexString().Should().Be("000FABFF");
        Array.Empty<byte>().ToHexString().Should().BeEmpty();
    }

    [Test]
    [Description("Bit helpers read, set and clear single bits")]
    public void BitBool_Test()
    {
        BitBoolExtensions.GetBitAsBool(0b0101, 0).Should().BeTrue();
        BitBoolExtensions.GetBitAsBool(0b0101, 1).Should().BeFalse();
        BitBoolExtensions.GetBitAsBool(0b0101, 2).Should().BeTrue();

        BitBoolExtensions.SetBitFromBool(0, 3, true).Should().Be(8);
        BitBoolExtensions.SetBitFromBool(8, 3, true).Should().Be(8);
        BitBoolExtensions.SetBitFromBool(9, 3, false).Should().Be(1);
        BitBoolExtensions.SetBitFromBool(1, 3, false).Should().Be(1);
    }

    [Test]
    [Description("ToIntOfBits packs booleans least significant bit first")]
    public void ToIntOfBits_Test()
    {
        new[] { true, false, true }.ToIntOfBits().Should().Be(5);
        new[] { false, false, false, true }.ToIntOfBits().Should().Be(8);
        Array.Empty<bool>().ToIntOfBits().Should().Be(0);
    }
}
