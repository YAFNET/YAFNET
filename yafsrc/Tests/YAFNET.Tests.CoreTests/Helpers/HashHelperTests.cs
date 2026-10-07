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

namespace YAF.Tests.CoreTests.Helpers;

using YAF.Types.Constants;

/// <summary>
/// YAF.Core.Helpers HashHelper Tests
/// </summary>
[TestFixture]
public class HashHelperTests
{
    [TestCase(HashAlgorithmType.MD5, "900150983CD24FB0D6963F7D28E17F72")]
    [TestCase(HashAlgorithmType.SHA1, "A9993E364706816ABA3E25717850C26C9CD0D89D")]
    [TestCase(HashAlgorithmType.SHA256, "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD")]
    [Description("Hash without salt matches the well known test vectors for 'abc'")]
    public void Hash_Known_Vectors_Test(HashAlgorithmType algorithm, string expected)
    {
        HashHelper.Hash("abc", algorithm).Should().Be(expected);
    }

    [TestCase(HashAlgorithmType.SHA384, 96)]
    [TestCase(HashAlgorithmType.SHA512, 128)]
    [Description("Hash produces hex strings of the right length for the larger algorithms")]
    public void Hash_Length_Test(HashAlgorithmType algorithm, int expectedLength)
    {
        HashHelper.Hash("abc", algorithm).Should().HaveLength(expectedLength).And.MatchRegex("^[0-9A-F]+$");
    }

    [Test]
    [Description("Hash honours the case, base64 and remove-chars options")]
    public void Hash_Options_Test()
    {
        HashHelper.Hash("abc", HashAlgorithmType.MD5, hashCaseType: HashCaseType.Lower)
            .Should().Be("900150983cd24fb0d6963f7d28e17f72");

        HashHelper.Hash("abc", HashAlgorithmType.MD5, hashHex: false, hashCaseType: HashCaseType.None)
            .Should().Be("kAFQmDzST7DWlj99KOF/cg==");

        HashHelper.Hash("abc", HashAlgorithmType.MD5, hashRemoveChars: "09")
            .Should().Be("1583CD24FBD663F7D28E17F72");
    }

    [Test]
    [Description("Salted hashes are deterministic and depend on the salt")]
    public void Hash_With_Salt_Test()
    {
        var saltA = Convert.ToBase64String("salt-a"u8.ToArray());
        var saltB = Convert.ToBase64String("salt-b"u8.ToArray());

        var hashA = HashHelper.Hash("password", HashAlgorithmType.SHA256, saltA);

        hashA.Should().Be(HashHelper.Hash("password", HashAlgorithmType.SHA256, saltA));
        hashA.Should().NotBe(HashHelper.Hash("password", HashAlgorithmType.SHA256, saltB));
        hashA.Should().NotBe(HashHelper.Hash("password", HashAlgorithmType.SHA256));
    }

    [Test]
    [Description("GeneratePasswordBuffer puts the salt first in ASP.NET Membership compatible mode")]
    public void GeneratePasswordBuffer_Standard_Test()
    {
        var saltBytes = new byte[] { 1, 2, 3 };

        var buffer = HashHelper.GeneratePasswordBuffer(Convert.ToBase64String(saltBytes), "ab", true);

        // salt followed by the UTF-16 bytes of "ab"
        buffer.Should().Equal(1, 2, 3, (byte)'a', 0, (byte)'b', 0);
    }

    [Test]
    [Description("GeneratePasswordBuffer keeps the legacy non-standard layout")]
    public void GeneratePasswordBuffer_NonStandard_Test()
    {
        var saltBytes = new byte[] { 1, 2, 3 };

        var buffer = HashHelper.GeneratePasswordBuffer(Convert.ToBase64String(saltBytes), "ab", false);

        // legacy behaviour: the salt overwrites the last password byte, the final byte stays 0
        buffer.Should().Equal((byte)'a', 0, (byte)'b', 1, 2, 3, 0);
    }

    [Test]
    [Description("Hash throws on null input")]
    public void Hash_Null_Throws_Test()
    {
        FluentActions.Invoking(() => HashHelper.Hash(null)).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("Hash throws on unknown algorithms")]
    public void Hash_Unknown_Algorithm_Throws_Test()
    {
        FluentActions.Invoking(() => HashHelper.Hash("abc", (HashAlgorithmType)999))
            .Should().Throw<ArgumentOutOfRangeException>();
    }
}
