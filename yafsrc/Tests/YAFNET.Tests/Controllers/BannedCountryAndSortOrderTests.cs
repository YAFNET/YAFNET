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

using Microsoft.AspNetCore.Mvc;

using YAF.Core.Controllers.Modals;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The admin banned countries dialog and the optional sort order of modules.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BannedCountryAndSortOrderTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.board.LoginAs(this.board.Database.AddUser("Admin"));
        this.board.PageLoad.IsAdmin = true;
        this.board.Localization.Add("ADMIN_BANNED_COUNTRIES", "MSG_EXIST", "exists");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A country is banned once per board, the dialog reports a duplicate")]
    public void Add_Country_Test()
    {
        var controller = this.board.CreateController<BannedCountryController>();

        controller.Edit(new BannedCountryModal { Id = 0, Country = " DE " }).Should().BeOfType<OkResult>();
        Notification(controller.Edit(new BannedCountryModal { Country = "DE" })).Message.Should().Be("exists");

        this.Countries.Get(_ => true).Select(c => c.Country).Should().Equal("DE");
        this.Countries.GetSingle(c => c.Country == "DE").BoardID.Should().Be(TestDatabase.BoardId);
    }

    [Test]
    [Description("A ban can be changed, but not into a country that is banned already")]
    public void Edit_Country_Test()
    {
        var controller = this.board.CreateController<BannedCountryController>();
        controller.Edit(new BannedCountryModal { Country = "DE" });
        controller.Edit(new BannedCountryModal { Country = "AT" });

        var austria = this.Countries.GetSingle(c => c.Country == "AT");

        Notification(controller.Edit(new BannedCountryModal { Id = austria.ID, Country = "DE" })).Message.Should().Be("exists");
        this.Countries.Get(_ => true).Select(c => c.Country).Should().BeEquivalentTo(["DE", "AT"], "no duplicate is created");

        controller.Edit(new BannedCountryModal { Id = austria.ID, Country = "CH" }).Should().BeOfType<OkResult>();
        this.Countries.GetById(austria.ID).Country.Should().Be("CH");

        // saving an entry unchanged is fine
        controller.Edit(new BannedCountryModal { Id = austria.ID, Country = "CH" }).Should().BeOfType<OkResult>();
    }

    [Test]
    [Description("Countries of other boards do not count as duplicates and can not be changed")]
    public void Other_Board_Test()
    {
        var other = (int)this.Countries.Insert(new BannedCountry { BoardID = 2, Country = "DE" }, true);

        this.Countries.Save(null, "DE").Should().BeTrue("the ban of the other board is separate");
        this.Countries.Save(other, "FR");

        this.Countries.GetById(other).Country.Should().Be("DE");
    }

    [Test]
    [Description("Items with a sort order come first (highest first), others get the default sort order")]
    public void Optional_Sort_Order_Test()
    {
        object[] items = [new Sorted("low", 1), "plain", new Sorted("high", 5000), new Sorted("middle", 2000)];

        items.ByOptionalSortOrder().Select(i => i is Sorted s ? s.Name : (string)i).Should()
            .Equal("high", "middle", "plain", "low");

        items.ByOptionalSortOrder(0).Select(i => i is Sorted s ? s.Name : (string)i).Last().Should().Be("plain");
    }

    private IRepository<BannedCountry> Countries => this.board.Current.GetRepository<BannedCountry>();

    private static MessageModalNotification Notification(IActionResult result)
    {
        return (MessageModalNotification)((OkObjectResult)result).Value;
    }

    private sealed record Sorted(string Name, int SortOrder) : IHaveSortOrder;
}
