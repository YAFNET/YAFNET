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

 * https://www.apache.org/licenses/LICENSE-2.0

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

/// <summary>
/// The <see cref="SimpleReplaceRule"/>.
/// </summary>
[TestFixture]
public class SimpleReplaceRuleTests
{
    [Test]
    [Description("Every occurrence of the search text is replaced")]
    public void Replace_All_Test()
    {
        var replacement = new ReplaceBlocksCollection();
        var text = "a [hr] b [hr] c";

        new SimpleReplaceRule("[hr]", "<hr />").Replace(ref text, replacement);

        replacement.Reconstruct(ref text);

        text.Should().Be("a <hr /> b <hr /> c");
    }

    [Test]
    [Timeout(5000)]
    [Description("An empty search text leaves the text unchanged (the loop never finished)")]
    public void Empty_Search_Text_Test()
    {
        var text = "some text";

        new SimpleReplaceRule(string.Empty, "x").Replace(ref text, new ReplaceBlocksCollection());

        text.Should().Be("some text");
    }
}
