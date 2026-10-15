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
using System.Threading.Tasks;

using YAF.Core.BBCode;
using YAF.Core.BBCode.ReplaceRules;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;

/// <summary>
/// The rule engine of the BBCodes: order of the rules, copies of the engine and rules that time out.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ProcessReplaceRulesTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Empty texts are returned unchanged")]
    public async Task Empty_Text_Test()
    {
        var engine = new ProcessReplaceRules();
        engine.AddRule(new SimpleReplaceRule("a", "b"));

        (await engine.ProcessAsync(string.Empty)).Should().BeEmpty();
        (await engine.ProcessAsync(null)).Should().BeNull();
    }

    [Test]
    [Description("A copy of the engine has the same rules, rules added to the copy are not in the original")]
    public async Task Clone_Test()
    {
        var engine = new ProcessReplaceRules();
        engine.AddRule(new SimpleReplaceRule("a", "b"));

        var copy = (ProcessReplaceRules)engine.Clone();
        copy.AddRule(new SimpleReplaceRule("c", "d"));

        (await copy.ProcessAsync("ac")).Should().Be("bd");
        (await engine.ProcessAsync("ac")).Should().Be("bc");
        copy.HasRules.Should().BeTrue();
        new ProcessReplaceRules().HasRules.Should().BeFalse();
    }

    [Test]
    [Description("A rule that times out is skipped, the other rules are applied")]
    public async Task Rule_Timeout_Test()
    {
        var engine = new ProcessReplaceRules();
        engine.AddRule(new SimpleReplaceRule("a", "b") { RuleRank = 1 });
        engine.AddRule(new TimeoutRule { RuleRank = 2 });
        engine.AddRule(new SimpleReplaceRule("c", "d") { RuleRank = 3 });

        (await engine.ProcessAsync("ac")).Should().Be("bd");
    }

    [Test]
    [Description("Rules are applied by rank, rules of the same rank in the order they were added")]
    public async Task Rule_Order_Test()
    {
        var engine = new ProcessReplaceRules();
        engine.AddRule(new SimpleReplaceRule("x", "late") { RuleRank = 60 });
        engine.AddRule(new SimpleReplaceRule("a", "x") { RuleRank = 10 });
        engine.AddRule(new SimpleReplaceRule("b", "first") { RuleRank = 10 });
        engine.AddRule(new SimpleReplaceRule("first", "second") { RuleRank = 10 });

        // the replaced html of a rule is protected, so the order is visible in the result
        (await engine.ProcessAsync("a b")).Should().Be("x first");
    }

    private sealed class TimeoutRule : BaseReplaceRule
    {
        public override void Replace(ref string text, IReplaceBlocks replacement)
        {
            throw new RegexMatchTimeoutException("c", "pattern", TimeSpan.FromMilliseconds(1));
        }
    }
}
