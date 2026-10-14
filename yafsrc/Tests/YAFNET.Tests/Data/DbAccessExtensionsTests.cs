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

namespace YAF.Tests.Data;

using System.Data;
using System.Dynamic;
using System.Threading.Tasks;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// The <see cref="IDbAccess"/> helpers (updates, existence checks, maintenance and script execution)
/// and the command parameter helper.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DbAccessExtensionsTests
{
    private TestBoardContext board;

    private IDbAccess db;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database.DbAccess;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Update, upsert, update only / add and the existence checks")]
    public async Task Update_And_Exists_Test()
    {
        var id = this.board.Database.AddUser("Alice");
        var user = await this.board.Current.GetRepository<User>().GetByIdAsync(id);

        user.DisplayName = "Alice A";
        (await this.db.UpdateAsync(user)).Should().Be(1);

        user.DisplayName = "Alice B";
        (await this.db.UpdateAsync(user)).Should().Be(1);

        (await this.db.UpdateOnlyAsync(() => new User { NumPosts = 5 }, u => u.ID == id)).Should().Be(1);
        (await this.db.UpdateAddAsync(() => new User { NumPosts = 2 }, u => u.ID == id)).Should().Be(1);
        (await this.db.UpdateAddAsync(() => new User { NumPosts = 1 }, u => u.ID == id)).Should().Be(1);
        (await this.db.UpdateOnlyAsync(() => new User { Points = 9 }, u => u.ID == id)).Should().Be(1);

        var stored = await this.board.Current.GetRepository<User>().GetByIdAsync(id);
        stored.DisplayName.Should().Be("Alice B");
        stored.NumPosts.Should().Be(8);
        stored.Points.Should().Be(9);

        this.db.Upsert(new Registry { Name = "upsert", Value = "1" }, r => r.Name == "upsert");
        this.db.Upsert(new Registry { Name = "upsert", Value = "2" }, r => r.Name == "upsert");

        (await this.db.ExistsAsync<Registry>(r => r.Name == "upsert" && r.Value == "2")).Should().BeTrue();
        (await this.db.ExistsAsync<Registry>(r => r.Name == "nope")).Should().BeFalse();
        (await this.db.TableExistsAsync<User>()).Should().BeTrue();
        (await this.db.TableExistsAsync<User>()).Should().BeTrue();
    }

    [Test]
    [Description("Table and column checks find the tables of the board (with the table prefix), used by the migrations")]
    public void Table_And_Column_Exist_Test()
    {
        this.db.Execute(c => c.Connection.TableExists<User>()).Should().BeTrue();
        this.db.Execute(c => c.Connection.ColumnExists<User>(u => u.DisplayName)).Should().BeTrue();
        this.db.Execute(c => c.Connection.TableExists(new TableRef("NoSuchTable"))).Should().BeFalse();
    }

    [Test]
    [Description("Database information: version, size and fragmentation")]
    public async Task Database_Info_Test()
    {
        (await this.db.GetSqlVersionAsync()).Should().StartWith("SQLite");
        (await this.db.GetDatabaseSizeAsync()).Should().BeGreaterThanOrEqualTo(0);
        (await this.db.GetDatabaseFragmentationInfoAsync()).Should().NotBeNull();
        await this.db.Invoking(d => d.ReIndexDatabaseAsync("yaf_")).Should().NotThrowAsync();
    }

    [Test]
    [Description("Changing the recovery mode runs the recovery mode statement, not a re-index")]
    public async Task Change_Recovery_Mode_Test()
    {
        var statements = new List<string>();
        var previous = OrmLiteConfig.BeforeExecFilter;
        OrmLiteConfig.BeforeExecFilter = cmd => statements.Add(cmd.CommandText);

        try
        {
            await this.db.ChangeRecoveryModeAsync("FULL");
        }
        finally
        {
            OrmLiteConfig.BeforeExecFilter = previous;
        }

        statements.Should().NotContain(s => s.Contains("auto_vacuum"), "that is the re-index statement");
    }

    [Test]
    [Description("RunSql returns the results of the statement")]
    public void Run_Sql_Test()
    {
        this.db.RunSql("select 42 as Answer", 30).Should().Contain("42");
    }

    [Test]
    [Description("Install scripts run statement by statement (GO separated) in one transaction")]
    public void Execute_Scripts_Test()
    {
        this.db.SystemInitializeExecuteScripts(
            "create table script_test (id int)\nGO\ninsert into script_test values (1)\nGO\n",
            "test.sql",
            30);

        this.db.Execute(c => c.Connection.Scalar<int>("select count(*) from script_test")).Should().Be(1);

        this.db.Invoking(
                d => d.SystemInitializeExecuteScripts(
                    "insert into script_test values (2)\nGO\nthis is not sql\nGO\n",
                    "broken.sql",
                    30))
            .Should().Throw<FormatException>().WithMessage("*broken.sql*");

        this.db.Execute(c => c.Connection.Scalar<int>("select count(*) from script_test"))
            .Should().Be(1, "the failing script is rolled back");
    }

    [Test]
    [Description("Command parameters: null, strings, guids and expando values")]
    public void Add_Param_Test()
    {
        using var connection = this.board.Database.Open();
        using var cmd = connection.CreateCommand();

        cmd.AddParam("empty", null);
        cmd.AddParam("short", "abc");
        cmd.AddParam("long", new string('x', 5000));
        var guid = Guid.NewGuid();
        cmd.AddParam("guid", guid);
        dynamic expando = new ExpandoObject();
        expando.Value = 7;
        cmd.AddParam("expando", (object)expando);
        cmd.AddParam(new KeyValuePair<string, object>(null, 3));

        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToList();

        parameters[0].Value.Should().Be(DBNull.Value);
        parameters[1].Size.Should().Be(4000);
        parameters[2].Size.Should().Be(-1);
        parameters[3].Value.Should().Be(guid.ToString());
        parameters[4].Value.Should().Be(7);
        parameters[5].ParameterName.Should().Be("5");
    }
}
