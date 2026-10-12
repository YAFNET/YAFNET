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

namespace YAF.Tests.Controllers;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAF.Core.BBCode;
using YAF.Core.BBCode.ReplaceRules;
using YAF.Core.Controllers.Modals;
using YAF.Core.Extensions;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The admin profile definition dialog and the simple (non regex) BBCode replace rule.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ProfileDefinitionAndSimpleRuleTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.board.LoginAs(this.board.Database.AddUser("Admin"));
        this.board.PageLoad.IsAdmin = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("New definitions are added to the board, existing ones are updated")]
    public async Task Add_And_Edit_Test()
    {
        var controller = this.board.CreateController<ProfileDefinitionController>();

        (await controller.Edit(Modal(0, "Hobby"))).Should().BeOfType<OkResult>();

        var added = this.Definitions.GetSingle(p => p.Name == "Hobby");
        added.BoardID.Should().Be(TestDatabase.BoardId);
        added.DataType.Should().Be("Text");
        added.Length.Should().Be(50);
        added.ShowInUserInfo.Should().BeTrue();

        (await controller.Edit(Modal(added.ID, "Hobbies", required: true))).Should().BeOfType<OkResult>();

        var edited = this.Definitions.GetById(added.ID);
        edited.Name.Should().Be("Hobbies");
        edited.Required.Should().BeTrue();
        this.Definitions.Count(_ => true).Should().Be(1);
    }

    [Test]
    [Description("The dialog form leaves out empty fields: a definition without a default value is saved, an unknown type becomes text, a missing name is refused")]
    public async Task Add_Without_Optional_Fields_Test()
    {
        var controller = this.board.CreateController<ProfileDefinitionController>();

        (await controller.Edit(new EditProfileDefinitionModal { Id = 0, Name = " Age ", DataType = "Number", Length = 0 }))
            .Should().BeOfType<OkResult>();

        var added = this.Definitions.GetSingle(p => p.Name == "Age");
        added.DefaultValue.Should().BeEmpty();
        added.DataType.Should().Be("Number");

        (await controller.Edit(new EditProfileDefinitionModal { Id = 0, Name = "Color", DataType = "<b>", Length = -5 }))
            .Should().BeOfType<OkResult>();

        var color = this.Definitions.GetSingle(p => p.Name == "Color");
        color.DataType.Should().Be("Text");
        color.Length.Should().Be(0);

        var result = await controller.Edit(new EditProfileDefinitionModal { Id = 0, DataType = "Text", Length = 10 });

        ((result as OkObjectResult)?.Value as MessageModalNotification).Should().NotBeNull("the name is required");
        this.Definitions.Count(_ => true).Should().Be(2);
    }

    [Test]
    [Description("Definitions of another board can not be changed")]
    public async Task Edit_Other_Board_Test()
    {
        var other = this.Definitions.Insert(
            new ProfileDefinition { BoardID = 2, Name = "Other", DataType = "Text", Length = 10, DefaultValue = string.Empty },
            true);

        var result = await this.board.CreateController<ProfileDefinitionController>().Edit(Modal((int)other, "Hijacked"));

        result.Should().BeOfType<NotFoundResult>();
        var unchanged = this.Definitions.GetById((int)other);
        unchanged.Name.Should().Be("Other");
        unchanged.BoardID.Should().Be(2);
    }

    [Test]
    [Description("The simple rule replaces every occurrence, its html is protected from later rules")]
    public async Task Simple_Replace_Rule_Test()
    {
        var rule = new SimpleReplaceRule(":-)", "<img alt=\":-)\">");
        rule.RuleRank.Should().Be(100);
        rule.RuleDescription.Should().Be("Find = \":-)\"");

        var engine = new ProcessReplaceRules();
        engine.AddRule(rule);

        (await engine.ProcessAsync("a :-) b :-)")).Should().Be("a <img alt=\":-)\"> b <img alt=\":-)\">");
        (await engine.ProcessAsync("nothing")).Should().Be("nothing");
    }

    [Test]
    [Description("A rule with an empty search text leaves the text alone instead of looping forever")]
    public void Simple_Replace_Rule_Empty_Find_Test()
    {
        var text = "text";

        new SimpleReplaceRule(string.Empty, "x").Replace(ref text, new ReplaceBlocksCollection());

        text.Should().Be("text");
    }

    private IRepository<ProfileDefinition> Definitions => this.board.Current.GetRepository<ProfileDefinition>();

    private static EditProfileDefinitionModal Modal(int id, string name, bool required = false)
    {
        return new EditProfileDefinitionModal
        {
            Id = id,
            Name = name,
            DataType = "Text",
            Length = 50,
            Required = required,
            ShowInUserInfo = true,
            ShowOnRegisterPage = false,
            DefaultValue = string.Empty
        };
    }
}
