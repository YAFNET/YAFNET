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

    [Test]
    [Description("Random edits: every diff (char and line mode, with the semantic cleanup) reconstructs both texts")]
    public void Random_Edits_Reconstruct_Texts_Test()
    {
        var random = new Random(4711);
        const string Alphabet = "ab c\nxy.";

        for (var i = 0; i < 500; i++)
        {
            var text1 = RandomText(random, Alphabet, random.Next(0, 150));
            var text2 = Mutate(random, text1, Alphabet);
            var checkLines = i % 2 == 0;

            var diffs = this.dmp.DiffMain(text1, text2, checkLines);

            Source(diffs).Should().Be(text1, $"case {i}");
            Target(diffs).Should().Be(text2, $"case {i}");

            this.dmp.CleanupSemantic(diffs);

            Source(diffs).Should().Be(text1, $"case {i} after the cleanup");
            Target(diffs).Should().Be(text2, $"case {i} after the cleanup");
            diffs.Should().NotContain(d => d.Text.Length == 0, $"case {i}");
        }
    }

    [Test]
    [Description("Without a time limit the diff is complete")]
    public void No_Timeout_Test()
    {
        this.dmp.DiffTimeout = 0;

        var diffs = this.dmp.DiffMain("the quick brown fox", "the quick red fox jumps", false);

        Source(diffs).Should().Be("the quick brown fox");
        Target(diffs).Should().Be("the quick red fox jumps");
    }

    [TestCaseSource(nameof(CleanupMergeCases))]
    [Description("CleanupMerge (cases of the original library): common prefixes and suffixes, sliding edits")]
    public void CleanupMerge_Cases_Test(List<Diff> diffs, List<Diff> expected)
    {
        DiffMatchPatch.CleanupMerge(diffs);

        diffs.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    [TestCaseSource(nameof(CleanupSemanticCases))]
    [Description("CleanupSemantic (cases of the original library): overlaps, word boundaries, back pass")]
    public void CleanupSemantic_Cases_Test(List<Diff> diffs, List<Diff> expected)
    {
        this.dmp.CleanupSemantic(diffs);

        diffs.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    [TestCaseSource(nameof(CleanupSemanticLosslessCases))]
    [Description("CleanupSemanticLossless (cases of the original library): blank lines, lines, words, start and end")]
    public void CleanupSemanticLossless_Cases_Test(List<Diff> diffs, List<Diff> expected)
    {
        this.dmp.CleanupSemanticLossless(diffs);

        diffs.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    private static IEnumerable<TestCaseData> CleanupMergeCases()
    {
        yield return Case("prefix and suffix", [Del("a"), Ins("abc"), Del("dc")], [Eq("a"), Del("d"), Ins("b"), Eq("c")]);
        yield return Case(
            "prefix and suffix with equalities",
            [Eq("x"), Del("a"), Ins("abc"), Del("dc"), Eq("y")],
            [Eq("xa"), Del("d"), Ins("b"), Eq("cy")]);
        yield return Case("slide edit left", [Eq("a"), Ins("ba"), Eq("c")], [Ins("ab"), Eq("ac")]);
        yield return Case("slide edit right", [Eq("c"), Ins("ab"), Eq("a")], [Eq("ca"), Ins("ba")]);
        yield return Case(
            "slide edit left recursive",
            [Eq("a"), Del("b"), Eq("c"), Del("ac"), Eq("x")],
            [Del("abc"), Eq("acx")]);
        yield return Case(
            "slide edit right recursive",
            [Eq("x"), Del("ca"), Eq("c"), Del("b"), Eq("a")],
            [Eq("xca"), Del("cba")]);
        yield return Case("empty merge", [Del("b"), Ins("ab"), Eq("c")], [Ins("a"), Eq("bc")]);
        yield return Case("empty equality", [Eq(string.Empty), Ins("a"), Eq("b")], [Ins("a"), Eq("b")]);
    }

    private static IEnumerable<TestCaseData> CleanupSemanticCases()
    {
        yield return Case("no overlap elimination", [Del("abcxx"), Ins("xxdef")], [Del("abcxx"), Ins("xxdef")]);
        yield return Case("overlap elimination", [Del("abcxxx"), Ins("xxxdef")], [Del("abc"), Eq("xxx"), Ins("def")]);
        yield return Case(
            "reverse overlap elimination",
            [Del("xxxabc"), Ins("defxxx")],
            [Ins("def"), Eq("xxx"), Del("abc")]);
        yield return Case(
            "two overlap eliminations",
            [Del("abcd1212"), Ins("1212efghi"), Eq("----"), Del("A3"), Ins("3BC")],
            [Del("abcd"), Eq("1212"), Ins("efghi"), Eq("----"), Del("A"), Eq("3"), Ins("BC")]);
        yield return Case(
            "multiple eliminations",
            [Ins("1"), Eq("A"), Del("B"), Ins("2"), Eq("_"), Ins("1"), Eq("A"), Del("B"), Ins("2")],
            [Del("AB_AB"), Ins("1A2_1A2")]);
        yield return Case(
            "word boundaries",
            [Eq("The c"), Del("ow and the c"), Eq("at.")],
            [Eq("The "), Del("cow and the "), Eq("cat.")]);
        yield return Case(
            "back pass elimination",
            [Del("ab"), Eq("cd"), Del("e"), Eq("f"), Ins("g")],
            [Del("abcdef"), Ins("cdfg")]);
    }

    private static IEnumerable<TestCaseData> CleanupSemanticLosslessCases()
    {
        yield return Case(
            "blank lines",
            [Eq("AAA\r\n\r\nBBB"), Ins("\r\nDDD\r\n\r\nBBB"), Eq("\r\nEEE")],
            [Eq("AAA\r\n\r\n"), Ins("BBB\r\nDDD\r\n\r\n"), Eq("BBB\r\nEEE")]);
        yield return Case(
            "line boundaries",
            [Eq("AAA\r\nBBB"), Ins(" DDD\r\nBBB"), Eq(" EEE")],
            [Eq("AAA\r\n"), Ins("BBB DDD\r\n"), Eq("BBB EEE")]);
        yield return Case(
            "alphanumeric boundaries",
            [Eq("The-c"), Ins("ow-and-the-c"), Eq("at.")],
            [Eq("The-"), Ins("cow-and-the-"), Eq("cat.")]);
        yield return Case("hitting the start", [Eq("a"), Del("a"), Eq("ax")], [Del("a"), Eq("aax")]);
        yield return Case("hitting the end", [Eq("xa"), Del("a"), Eq("a")], [Eq("xaa"), Del("a")]);
        yield return Case(
            "sentence boundaries",
            [Eq("The xxx. The "), Ins("zzz. The "), Eq("yyy.")],
            [Eq("The xxx."), Ins(" The zzz."), Eq(" The yyy.")]);
    }

    private static TestCaseData Case(string name, List<Diff> diffs, List<Diff> expected)
    {
        return new TestCaseData(diffs, expected).SetArgDisplayNames(name);
    }

    private static Diff Eq(string text) => new(Operation.Equal, text);

    private static Diff Ins(string text) => new(Operation.Insert, text);

    private static Diff Del(string text) => new(Operation.Delete, text);

    private static string RandomText(Random random, string alphabet, int length)
    {
        return new string(Enumerable.Range(0, length).Select(_ => alphabet[random.Next(alphabet.Length)]).ToArray());
    }

    /// <summary>
    /// Inserts, deletes and replaces some parts of the text.
    /// </summary>
    private static string Mutate(Random random, string text, string alphabet)
    {
        var result = new System.Text.StringBuilder(text);

        for (var edits = random.Next(0, 6); edits > 0; edits--)
        {
            var position = random.Next(0, result.Length + 1);
            var length = Math.Min(random.Next(0, 10), result.Length - position);

            switch (random.Next(3))
            {
                case 0:
                    result.Insert(position, RandomText(random, alphabet, random.Next(1, 10)));
                    break;
                case 1:
                    result.Remove(position, length);
                    break;
                default:
                    result.Remove(position, length).Insert(position, RandomText(random, alphabet, length));
                    break;
            }
        }

        return result.ToString();
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
