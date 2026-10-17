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

using Autofac;

using Microsoft.Owin.Security.DataProtection;

using NSubstitute;

using YAF.Core.Context.Start;
using YAF.Core.Extensions;
using YAF.Core.Identity;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The admin actions for the not approved users of the start page: approve all and delete all.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AspNetUsersHelperTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IAspNetUsersHelper helper;

    private IDataProtectionProvider previousProvider;

    [SetUp]
    public void SetUp()
    {
        // the user manager creates its token provider from the OWIN data protection
        this.previousProvider = Startup.DataProtectionProvider;
        Startup.DataProtectionProvider = Substitute.For<IDataProtectionProvider>();
        Startup.DataProtectionProvider.Create(Arg.Any<string[]>()).Returns(Substitute.For<IDataProtector>());

        this.board = new TestBoardContext(
            b =>
            {
                b.Register(c => new AspNetUsersManager(new UserStore(c.Resolve<IServiceLocator>()))).AsSelf()
                    .SingleInstance();
                b.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>();
            },
            useDatabase: true);

        this.db = this.board.Database;

        // no user of the tests is the guest (a deleted guest would be skipped)
        this.board.PageLoad.GuestUserID = 9999;

        this.helper = this.board.Resolve<IAspNetUsersHelper>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Startup.DataProtectionProvider = this.previousProvider;
    }

    [Test]
    [Description("Approve all approves the not approved users and removes their e-mail check, disabled (deleted) accounts stay disabled")]
    public void Approve_All_Test()
    {
        var waiting = this.AddUser("waiting", UserFlags.Flags.None);
        var disabled = this.AddUser("disabled", UserFlags.Flags.IsDeleted);

        this.Repository<CheckEmail>().Insert(new CheckEmail { UserID = waiting, Email = "waiting@example.com", Created = DateTime.UtcNow, Hash = "x" });

        this.helper.ApproveAll();

        this.Repository<User>().GetById(waiting).UserFlags.IsApproved.Should().BeTrue();
        this.Identity("waiting").IsApproved.Should().BeTrue();
        this.Repository<CheckEmail>().Count(c => c.UserID == waiting).Should().Be(0);

        this.Repository<User>().GetById(disabled).UserFlags.IsApproved.Should().BeFalse("disabled accounts are not listed");
        this.Identity("disabled").IsApproved.Should().BeFalse();
    }

    [Test]
    [Description("Delete all deletes the not approved users created before the cutoff, not the newer and not the disabled (deleted) accounts")]
    public void Delete_All_Unapproved_Test()
    {
        var old = this.AddUser("old", UserFlags.Flags.None, DateTime.UtcNow.AddDays(-10));
        var recent = this.AddUser("recent", UserFlags.Flags.None, DateTime.UtcNow);
        var disabled = this.AddUser("disabled", UserFlags.Flags.IsDeleted, DateTime.UtcNow.AddDays(-10));

        this.helper.DeleteAllUnapproved(DateTime.UtcNow.AddDays(-5));

        this.Repository<User>().GetById(old).Should().BeNull();
        this.Repository<User>().GetById(recent).Should().NotBeNull();
        this.Repository<User>().GetById(disabled).Should().NotBeNull("disabled accounts are not listed");
    }

    private int AddUser(string name, UserFlags.Flags flags, DateTime? created = null)
    {
        var aspNetUser = new AspNetUsers
        {
            Id = $"{name}-key",
            UserName = name,
            LoweredUserName = name,
            Email = $"{name}@example.com",
            LoweredEmail = $"{name}@example.com",
            IsApproved = false,
            CreateDate = created ?? DateTime.UtcNow
        };

        this.Repository<AspNetUsers>().Insert(aspNetUser, false);

        return this.db.AddUser(
            name,
            u =>
            {
                u.ProviderUserKey = aspNetUser.Id;
                u.Flags = (int)flags;
            });
    }

    private AspNetUsers Identity(string name)
    {
        return this.Repository<AspNetUsers>().GetSingle(u => u.UserName == name);
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
