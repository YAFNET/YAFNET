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

using YAF.Core.Controllers;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Objects;

/// <summary>
/// The multi-quote buttons of the posts (<see cref="MultiQuoteController"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class MultiQuoteControllerTests
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
    [Description("Ticking adds the post to the multi-quote list once, unticking removes it again")]
    public void Tick_And_Untick_Test()
    {
        var controller = new MultiQuoteController();

        controller.HandleMultiQuote(Button(5, true));
        controller.HandleMultiQuote(Button(5, true));
        controller.HandleMultiQuote(Button(6, true));

        var session = this.board.Current.Get<ISession>();

        session.MultiQuoteIds.Should().HaveCount(2);

        controller.HandleMultiQuote(Button(5, false));

        session.MultiQuoteIds.Should().ContainSingle().Which.MessageID.Should().Be(6);
    }

    private static MultiQuoteButton Button(int messageId, bool isChecked)
    {
        return new MultiQuoteButton
                   {
                       ButtonId = $"multiQuote{messageId}",
                       IsMultiQuoteButton = isChecked,
                       MessageId = messageId,
                       TopicId = 1,
                       ButtonCssClass = "btn-multiquote"
                   };
    }
}
