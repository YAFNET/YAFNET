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

namespace YAF.Tests.Utilities;

using System.Text;

using YAF.Core.Utilities.StringUtils;

/// <summary>
/// Tests for DiffMatchPatch (used by the message history diff view)
/// </summary>
[TestFixture]
public class DiffMatchPatchTests
{
    private DiffMatchPatch dmp;

    [SetUp]
    public void SetUp()
    {
        this.dmp = new DiffMatchPatch();
    }

    [TestCase("abc", "xyz", 0)]
    [TestCase("1234abcdef", "1234xyz", 4)]
    [TestCase("1234", "1234xyz", 4)]
    [TestCase("", "abc", 0)]
    [Description("CommonPrefix returns the length of the shared prefix")]
    public void CommonPrefix_Test(string text1, string text2, int expected)
    {
        DiffMatchPatch.CommonPrefix(text1, text2).Should().Be(expected);
    }

    [TestCase("abc", "xyz", 0)]
    [TestCase("abcdef1234", "xyz1234", 4)]
    [TestCase("1234", "xyz1234", 4)]
    [Description("CommonSuffix returns the length of the shared suffix")]
    public void CommonSuffix_Test(string text1, string text2, int expected)
    {
        DiffMatchPatch.CommonSuffix(text1, text2).Should().Be(expected);
    }

    [Test]
    [Description("Equal texts produce a single equality, empty texts produce nothing")]
    public void DiffMain_Equal_Test()
    {
        this.dmp.DiffMain("abc", "abc", false).Should().BeEquivalentTo([new Diff(Operation.Equal, "abc")]);
        this.dmp.DiffMain("", "", false).Should().BeEmpty();
    }

    [Test]
    [Description("Pure insertions and deletions are detected")]
    public void DiffMain_Insert_Delete_Test()
    {
        this.dmp.DiffMain("abc", "ab123c", false).Should().BeEquivalentTo(
            [new Diff(Operation.Equal, "ab"), new Diff(Operation.Insert, "123"), new Diff(Operation.Equal, "c")],
            o => o.WithStrictOrdering());

        this.dmp.DiffMain("a123bc", "abc", false).Should().BeEquivalentTo(
            [new Diff(Operation.Equal, "a"), new Diff(Operation.Delete, "123"), new Diff(Operation.Equal, "bc")],
            o => o.WithStrictOrdering());

        this.dmp.DiffMain("", "abc", false).Should().BeEquivalentTo([new Diff(Operation.Insert, "abc")]);
        this.dmp.DiffMain("abc", "", false).Should().BeEquivalentTo([new Diff(Operation.Delete, "abc")]);
    }

    [TestCase("The quick brown fox jumps over the lazy dog.", "The quick red fox leaps over the lazy cat!", false)]
    [TestCase("Lorem ipsum dolor sit amet", "Lorem dolor sit amet, consectetur", false)]
    [TestCase("abcdefghijklmnop", "ponmlkjihgfedcba", false)]
    [TestCase("line1\nline2\nline3\n", "line1\nchanged\nline3\nline4\n", true)]
    [Description("A diff always reconstructs both source and target text")]
    public void DiffMain_Reconstructs_Texts_Test(string text1, string text2, bool checkLines)
    {
        var diffs = this.dmp.DiffMain(text1, text2, checkLines);

        Source(diffs).Should().Be(text1);
        Target(diffs).Should().Be(text2);

        this.dmp.CleanupSemantic(diffs);

        Source(diffs).Should().Be(text1);
        Target(diffs).Should().Be(text2);
    }

    [Test]
    [Description("Line mode diffs of long texts still reconstruct both texts")]
    public void DiffMain_LineMode_Long_Text_Test()
    {
        var a = new StringBuilder();
        var b = new StringBuilder();

        for (var i = 0; i < 200; i++)
        {
            a.Append("Line number ").Append(i).Append('\n');
            b.Append(i % 7 == 0 ? "Changed line " : "Line number ").Append(i).Append('\n');
        }

        var diffs = this.dmp.DiffMain(a.ToString(), b.ToString(), true);

        Source(diffs).Should().Be(a.ToString());
        Target(diffs).Should().Be(b.ToString());
        diffs.Should().Contain(d => d.Operation == Operation.Insert);
        diffs.Should().Contain(d => d.Operation == Operation.Delete);
    }

    [Test]
    [Description("CleanupMerge merges neighbouring edits of the same kind")]
    public void CleanupMerge_Test()
    {
        var diffs = new List<Diff>
        {
            new(Operation.Equal, "a"), new(Operation.Equal, "b"), new(Operation.Insert, "c"),
            new(Operation.Insert, "d"), new(Operation.Delete, "")
        };

        DiffMatchPatch.CleanupMerge(diffs);

        diffs.Should().BeEquivalentTo(
            [new Diff(Operation.Equal, "ab"), new Diff(Operation.Insert, "cd")],
            o => o.WithStrictOrdering());
    }

    [Test]
    [Description("CleanupSemantic replaces trivial equalities with bigger edits")]
    public void CleanupSemantic_Test()
    {
        var diffs = new List<Diff>
        {
            new(Operation.Delete, "a"), new(Operation.Equal, "b"), new(Operation.Delete, "c")
        };

        this.dmp.CleanupSemantic(diffs);

        diffs.Should().BeEquivalentTo(
            [new Diff(Operation.Delete, "abc"), new Diff(Operation.Insert, "b")],
            o => o.WithStrictOrdering());
    }

    [Test]
    [Description("CleanupSemanticLossless aligns edits to word boundaries")]
    public void CleanupSemanticLossless_Test()
    {
        var diffs = new List<Diff>
        {
            new(Operation.Equal, "The c"), new(Operation.Insert, "ow and the c"), new(Operation.Equal, "at.")
        };

        this.dmp.CleanupSemanticLossless(diffs);

        diffs.Should().BeEquivalentTo(
            [new Diff(Operation.Equal, "The "), new Diff(Operation.Insert, "cow and the "), new Diff(Operation.Equal, "cat.")],
            o => o.WithStrictOrdering());
    }

    [Test]
    [Description("PrettyHtml renders escaped paragraphs per operation")]
    public void PrettyHtml_Test()
    {
        var html = DiffMatchPatch.PrettyHtml(
        [
            new Diff(Operation.Equal, "a\n"), new Diff(Operation.Delete, "<b>"), new Diff(Operation.Insert, "c&d")
        ]);

        html.Should().Be(
            "<p>a<br></p><p class=\"bg-danger text-light\">&lt;b&gt;</p><p class=\"bg-success text-light\">c&amp;d</p>");
    }

    private static string Source(IEnumerable<Diff> diffs)
    {
        return string.Concat(diffs.Where(d => d.Operation != Operation.Insert).Select(d => d.Text));
    }

    private static string Target(IEnumerable<Diff> diffs)
    {
        return string.Concat(diffs.Where(d => d.Operation != Operation.Delete).Select(d => d.Text));
    }
}
