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
    public void Empty_Text_Test()
    {
        var engine = new ProcessReplaceRules();
        engine.AddRule(new SimpleReplaceRule("a", "b"));

        (Process(engine, string.Empty)).Should().BeEmpty();
        (Process(engine, null)).Should().BeNull();
    }

    [Test]
    [Description("A copy of the engine has the same rules, rules added to the copy are not in the original")]
    public void Clone_Test()
    {
        var engine = new ProcessReplaceRules();
        engine.AddRule(new SimpleReplaceRule("a", "b"));

        var copy = (ProcessReplaceRules)engine.Clone();
        copy.AddRule(new SimpleReplaceRule("c", "d"));

        (Process(copy, "ac")).Should().Be("bd");
        (Process(engine, "ac")).Should().Be("bc");
        copy.HasRules.Should().BeTrue();
        new ProcessReplaceRules().HasRules.Should().BeFalse();
    }

    [Test]
    [Description("Rules are applied by rank, rules of the same rank in the order they were added")]
    public void Rule_Order_Test()
    {
        var engine = new ProcessReplaceRules();
        engine.AddRule(new SimpleReplaceRule("x", "late") { RuleRank = 60 });
        engine.AddRule(new SimpleReplaceRule("a", "x") { RuleRank = 10 });
        engine.AddRule(new SimpleReplaceRule("b", "first") { RuleRank = 10 });
        engine.AddRule(new SimpleReplaceRule("first", "second") { RuleRank = 10 });

        // the replaced html of a rule is protected, so the order is visible in the result
        (Process(engine, "a b")).Should().Be("x first");
    }

    private static string Process(ProcessReplaceRules engine, string text)
    {
        engine.Process(ref text);

        return text;
    }
}
