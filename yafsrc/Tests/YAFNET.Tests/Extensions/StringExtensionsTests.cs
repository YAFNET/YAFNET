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

/// <summary>
/// YAF.Types.Extensions StringExtensions Tests
/// </summary>
[TestFixture]
public class StringExtensionsTests
{
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("   ", false)]
    [TestCase("\t\n", false)]
    [TestCase("a", true)]
    [TestCase(" a ", true)]
    [Description("IsSet / IsNotSet treat null, empty and whitespace as not set")]
    public void IsSet_And_IsNotSet_Test(string input, bool expected)
    {
        input.IsSet().Should().Be(expected);
        input.IsNotSet().Should().Be(!expected);
    }

    [Test]
    [Description("ToJsString escapes backslashes, quotes and line breaks")]
    public void ToJsString_Escapes_Special_Characters_Test()
    {
        "a\\b'c\r\nd\"e".ToJsString().Should().Be(@"a\\b\'c\r\nd'e");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    [Description("ToJsString returns unset strings unchanged")]
    public void ToJsString_Returns_Unset_Input_Test(string input)
    {
        input.ToJsString().Should().Be(input);
    }

    [TestCase("hello world", "", 0)]
    [TestCase("hello world", "w", 6)]
    [TestCase("hello world", "wo", 6)]
    [TestCase("hello world", "world", 6)]
    [TestCase("hello world", "hello", 0)]
    [TestCase("hello world", "worlds", -1)]
    [TestCase("hello world", "xyz", -1)]
    [TestCase("abababc", "abc", 4)]
    [TestCase("aaab", "ab", 2)]
    [TestCase("ab", "abc", -1)]
    [TestCase("hello world", "d", 10)]
    [TestCase("hello world", "ld", 9)]
    [Description("FastIndexOf returns the same result as string.IndexOf (ordinal)")]
    public void FastIndexOf_Test(string input, string pattern, int expected)
    {
        input.FastIndexOf(pattern).Should().Be(expected);
        input.FastIndexOf(pattern).Should().Be(input.IndexOf(pattern, StringComparison.Ordinal));
    }

    [Test]
    [Description("ForEachChar visits every character in order")]
    public void ForEachChar_Test()
    {
        var chars = new List<char>();

        "abc".ForEachChar(chars.Add);

        chars.Should().Equal('a', 'b', 'c');
    }

    [TestCase("a  b\t\tc\r\n d", "a b c d")]
    [TestCase("single", "single")]
    [TestCase(null, "")]
    [TestCase("   ", "")]
    [Description("RemoveMultipleWhitespace collapses whitespace runs into one space")]
    public void RemoveMultipleWhitespace_Test(string input, string expected)
    {
        input.RemoveMultipleWhitespace().Should().Be(expected);
    }

    [Test]
    [Description("ToDelimitedString joins items with the delimiter")]
    public void ToDelimitedString_Test()
    {
        new[] { 1, 2, 3 }.ToDelimitedString(",").Should().Be("1,2,3");
        new[] { "a" }.ToDelimitedString(", ").Should().Be("a");
        Array.Empty<int>().ToDelimitedString(",").Should().BeEmpty();
    }

    [Test]
    [Description("ToDelimitedString throws on null")]
    public void ToDelimitedString_Null_Throws_Test()
    {
        IEnumerable<int> list = null;

        var act = () => list.ToDelimitedString(",");

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("ToRegExString escapes everything except letters, digits, whitespace and underscore")]
    public void ToRegExString_Test()
    {
        const string input = "a.b*c (d)_1?";

        var escaped = input.ToRegExString();

        escaped.Should().Be(@"a\.b\*c \(d\)_1\?");
        System.Text.RegularExpressions.Regex.IsMatch(input, $"^{escaped}$").Should().BeTrue();
    }

    [TestCase("The quick brown fox", 13, "...", "The quick...")]
    [TestCase("The quick brown fox", 12, "...", "The quick...")]
    [TestCase("Short", 20, "...", "Short")]
    [TestCase("Supercalifragilistic", 10, "...", "Superca...")]
    [TestCase("The quick brown fox", 11, "~", "The quick~")]
    [TestCase("abc", 2, "...", "abc")]
    [Description("Truncate cuts on word boundaries and appends the cut-off string")]
    public void Truncate_Test(string input, int limit, string cutOff, string expected)
    {
        input.Truncate(limit, cutOff).Should().Be(expected);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [Description("Truncate returns null for unset input")]
    public void Truncate_Unset_Returns_Null_Test(string input)
    {
        input.Truncate(10).Should().BeNull();
    }

    [TestCase("image.png", true)]
    [TestCase("image.PNG", true)]
    [TestCase("image.gif", true)]
    [TestCase("image.jpeg", true)]
    [TestCase("image.jpg", true)]
    [TestCase("image.bmp", true)]
    [TestCase("image.webp", true)]
    [TestCase("image.svg", true)]
    [TestCase("document.pdf", false)]
    [TestCase("archive.zip", false)]
    [Description("IsImageName detects image file extensions case-insensitively")]
    public void IsImageName_Test(string fileName, bool expected)
    {
        fileName.IsImageName().Should().Be(expected);
    }

    [Test]
    [Description("PersianNumberToEnglish converts Persian digits to ASCII digits")]
    public void PersianNumberToEnglish_Test()
    {
        "۰۱۲۳۴۵۶۷۸۹ abc".PersianNumberToEnglish().Should().Be("0123456789 abc");
    }

    [Test]
    [Description("ToGuid is deterministic and encodes the integer in the first bytes")]
    public void ToGuid_Test()
    {
        "1".ToGuid().Should().Be(new Guid("00000001-0000-0000-0000-000000000000"));
        "42".ToGuid().Should().Be("42".ToGuid());
        "42".ToGuid().Should().NotBe("43".ToGuid());
    }

    [Test]
    [Description("FormatWith behaves like string.Format")]
    public void FormatWith_Test()
    {
        "{0} + {1} = {2}".FormatWith(1, 2, 3).Should().Be("1 + 2 = 3");
    }

    [TestCase("HelloWorld", "hello", true)]
    [TestCase("HelloWorld", "HELLO", true)]
    [TestCase("HelloWorld", "world", false)]
    [TestCase(null, "hello", false)]
    [Description("StartsWithIgnoreCase is null-safe and case-insensitive")]
    public void StartsWithIgnoreCase_Test(string input, string prefix, bool expected)
    {
        input.StartsWithIgnoreCase(prefix).Should().Be(expected);
    }

    [TestCase("/forum", "topics", "/forum/topics")]
    [TestCase("/forum/", "topics", "/forum/topics")]
    [TestCase("/forum\\", "topics", "/forum/topics")]
    [TestCase("/forum", "/topics", "/forum/topics")]
    [TestCase("/forum", "", "/forum")]
    [TestCase("/forum", null, "/forum")]
    [TestCase(null, "topics", "/topics")]
    [TestCase("", "topics", "/topics")]
    [Description("CombineWith joins paths with exactly one slash")]
    public void CombineWith_Test(string path, string other, string expected)
    {
        path.CombineWith(other).Should().Be(expected);
    }

    [Test]
    [Description("StringToList splits and removes excluded entries")]
    public void StringToList_Test()
    {
        "a,b,c".StringToList(',').Should().Equal("a", "b", "c");
        "a,b,c".StringToList(',', ["b"]).Should().Equal("a", "c");
    }

    [Test]
    [Description("StringToList throws on null arguments")]
    public void StringToList_Null_Throws_Test()
    {
        string input = null;

        FluentActions.Invoking(() => input.StringToList(',')).Should().Throw<ArgumentNullException>();
        FluentActions.Invoking(() => "a".StringToList(',', null)).Should().Throw<ArgumentNullException>();
    }

    [TestCase("/forum", "/forum/")]
    [TestCase("/forum/", "/forum/")]
    [TestCase("", "/")]
    [Description("AppendTrailingSlash adds a slash only when missing")]
    public void AppendTrailingSlash_Test(string input, string expected)
    {
        input.AppendTrailingSlash().Should().Be(expected);
    }
}
