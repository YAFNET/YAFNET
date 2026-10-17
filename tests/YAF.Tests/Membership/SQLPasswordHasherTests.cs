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

namespace YAF.Tests.Membership;

using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNet.Identity;

using YAF.Core.Helpers;
using YAF.Core.Membership;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;

using PasswordVerificationResult = Microsoft.AspNet.Identity.PasswordVerificationResult;

/// <summary>
/// <see cref="SQLPasswordHasher"/>: current identity hashes and the passwords migrated from the
/// legacy ASP.NET membership ("password|format|salt").
/// </summary>
[TestFixture]
[NonParallelizable]
public class SQLPasswordHasherTests
{
    private const string Salt = "c2FsdHNhbHRzYWx0c2FsdA==";

    private readonly SQLPasswordHasher hasher = new();

    private TestAppSettings settings;

    [SetUp]
    public void SetUp()
    {
        // the defaults of the ASP.NET membership
        this.settings = TestAppSettings.Set("YAF.LegacyMembershipHashAlgorithmType", "SHA1")
            .And("YAF.LegacyMembershipHashCase", "None").And("YAF.LegacyMembershipHashHex", "false");
    }

    [TearDown]
    public void TearDown()
    {
        this.settings.Dispose();
    }

    [Test]
    [Description("Current identity hashes are verified by the default hasher, empty hashes always fail")]
    public void Identity_Hash_Test()
    {
        var hash = this.hasher.HashPassword("Secret1!");

        this.Verify(hash, "Secret1!").Should().Be(PasswordVerificationResult.Success);
        this.Verify(hash, "wrong").Should().Be(PasswordVerificationResult.Failed);
        this.Verify(null, "Secret1!").Should().Be(PasswordVerificationResult.Failed);
        this.Verify(string.Empty, string.Empty).Should().Be(PasswordVerificationResult.Failed);
    }

    [Test]
    [Description("Migrated clear text passwords: the right password is accepted (and rehashed), a wrong one is not")]
    public void Clear_Test()
    {
        this.Verify($"Secret1!|0|{Salt}", "Secret1!").Should().Be(PasswordVerificationResult.SuccessRehashNeeded);
        this.Verify($"Secret1!|0|{Salt}", "wrong").Should().Be(PasswordVerificationResult.Failed);
        this.Verify($"Secret1!|Clear|{Salt}", "Secret1!").Should().Be(PasswordVerificationResult.SuccessRehashNeeded);
    }

    [Test]
    [Description("Passwords are case sensitive, also migrated clear text passwords")]
    public void Clear_Case_Sensitive_Test()
    {
        this.Verify($"Secret1!|0|{Salt}", "SECRET1!").Should().Be(PasswordVerificationResult.Failed);
        this.Verify($"Secret1!|0|{Salt}", "secret1!").Should().Be(PasswordVerificationResult.Failed);
    }

    [Test]
    [Description("A migrated clear text password may contain the separator")]
    public void Clear_With_Separator_Test()
    {
        this.Invoking(t => t.Verify($"a|b|0|{Salt}", "a|b")).Should().NotThrow();
        this.Verify($"a|b|0|{Salt}", "a|b").Should().Be(PasswordVerificationResult.SuccessRehashNeeded);
    }

    [Test]
    [Description("Hashed passwords of the ASP.NET membership (SHA1 of salt + password, base64) can log in")]
    public void AspNet_Membership_Hash_Test()
    {
        var stored = Convert.ToBase64String(
            SHA1.Create().ComputeHash([.. Convert.FromBase64String(Salt), .. Encoding.Unicode.GetBytes("Secret1!")]));

        this.Verify($"{stored}|1|{Salt}", "Secret1!").Should().Be(PasswordVerificationResult.SuccessRehashNeeded);
        this.Verify($"{stored}|1|{Salt}", "wrong").Should().Be(PasswordVerificationResult.Failed);
    }

    [Test]
    [Description("Hashed passwords of the old YAF membership provider (not MS compliant) can log in")]
    public void Yaf_Provider_Hash_Test()
    {
        var stored = HashHelper.Hash("Secret1!", HashAlgorithmType.SHA1, Salt, false, HashCaseType.None, null, false);

        this.Verify($"{stored}|1|{Salt}", "Secret1!").Should().Be(PasswordVerificationResult.SuccessRehashNeeded);
        this.Verify($"{stored}|1|{Salt}", "wrong").Should().Be(PasswordVerificationResult.Failed);
    }

    [Test]
    [Description("Hex hashes are compared ignoring the case of the hex digits")]
    public void Hex_Hash_Test()
    {
        this.settings.And("YAF.LegacyMembershipHashHex", "true").And("YAF.LegacyMembershipHashCase", "Upper")
            .And("YAF.LegacyMembershipHashAlgorithmType", "MD5");

        var stored = HashHelper.Hash("Secret1!", HashAlgorithmType.MD5, Salt, true, HashCaseType.Lower, null, false);

        this.Verify($"{stored}|1|{Salt}", "Secret1!").Should().Be(PasswordVerificationResult.SuccessRehashNeeded);
        this.Verify($"{stored}|1|{Salt}", "wrong").Should().Be(PasswordVerificationResult.Failed);
    }

    [Test]
    [Description("Encrypted passwords that can't be decrypted with the machine key fail instead of causing an error")]
    public void Encrypted_Test()
    {
        this.Verify($"Secret1!|2|{Salt}", "Secret1!").Should().Be(PasswordVerificationResult.Failed);
    }

    [TestCase("x|abc|salt")]
    [TestCase("x|9|salt")]
    [Description("Unknown password formats fail instead of causing an error or being compared as clear text")]
    public void Unknown_Format_Test(string stored)
    {
        this.Invoking(t => t.Verify(stored, "x")).Should().NotThrow();
        this.Verify(stored, "x").Should().Be(PasswordVerificationResult.Failed);
    }

    private PasswordVerificationResult Verify(string stored, string password)
    {
        return this.hasher.VerifyHashedPassword(stored, password);
    }
}
