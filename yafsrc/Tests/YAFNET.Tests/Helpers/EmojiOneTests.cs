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

using System.Reflection;

using YAF.Core.Utilities.StringUtils;

/// <summary>
/// YAF.Utils.Helpers EmojiOne Tests
/// </summary>
[TestFixture]
public class EmojiOneTests
{
    /// <summary>
    /// The ASCII to Unicode test.
    /// </summary>
    [Test]
    [Description("Ascii To Unicode Test")]
    public void AsciiToUnicode_Test()
    {
        // single smiley
        const string text = ":D";
        const string expected = "😃";
        var actual = EmojiOne.AsciiToUnicode(EmojiOne.ShortNameToUnicode(text, true));

        actual.Should().Be(expected);
    }

    [Test]
    [Description("Emoji made of several characters (e.g. a person with a skin tone) keep all of them")]
    public void Skin_Tone_Emoji_Test()
    {
        // 26f9 (person bouncing ball, basic plane) + 1f3fb (light skin tone)
        EmojiOne.ShortNameToUnicode("[basketball_player_tone1]").Should().Be("⛹🏻");

        // two characters outside the basic plane
        EmojiOne.ShortNameToUnicode("[grinning]").Should().Be("😀");

        EmojiOne.ShortNameToUnicode("no [unknown_emoji] here").Should().Be("no [unknown_emoji] here");
    }

    [Test]
    [Description("Every short name is converted to its characters, and those characters back to the same code")]
    public void All_Short_Names_Round_Trip_Test()
    {
        var table = (Dictionary<string, string>)typeof(EmojiOne)
            .GetField("SHORTNAME_TO_CODEPOINT", BindingFlags.NonPublic | BindingFlags.Static)!
            .GetValue(null);

        var toUnicode = typeof(EmojiOne).GetMethod("ToUnicode", BindingFlags.NonPublic | BindingFlags.Static)!;
        var toCodePoint = typeof(EmojiOne).GetMethod("ToCodePoint", BindingFlags.NonPublic | BindingFlags.Static)!;

        table.Should().HaveCountGreaterThan(2000);

        var broken = table
            .Where(e => (string)toCodePoint.Invoke(null, [toUnicode.Invoke(null, [e.Value])]) != e.Value.ToLowerInvariant())
            .Select(e => $"{e.Key} = {e.Value}")
            .ToList();

        broken.Should().BeEmpty();
    }
}
