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

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using YAF.Core.Extensions;
using YAF.Core.Logger;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Models;

/// <summary>
/// <see cref="DbLogger"/> writes the YAF logger extensions (Info, Error, Log, ...) to the event log table.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DbLoggerTests
{
    private TestBoardContext board;

    private ILogger logger;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);

        var provider = new DbLoggerProvider(this.board.Resolve<IInjectServices>());
        this.logger = provider.CreateLogger("YAF.Tests.Category");

        this.board.BoardSettings.LogInformation = true;
        this.board.BoardSettings.LogWarning = true;
        this.board.BoardSettings.LogError = true;

        this.board.HttpContext.Request.Path = "/Posts/5";
        this.board.HttpContext.Request.Headers.UserAgent = "TestAgent/1.0";
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Info, Warn and Error are written with the request details")]
    public void Info_Warn_Error_Test()
    {
        this.logger.Info("info message");
        this.logger.Warn("warning {0}", 42);
        this.logger.Error(new InvalidOperationException("boom"), "error message");

        var entries = this.Entries();
        entries.Select(e => e.EventType).Should().Equal(EventLogTypes.Information, EventLogTypes.Warning, EventLogTypes.Error);

        Description(entries[0])["Message"].ToString().Should().Be("info message");
        Description(entries[1])["Message"].ToString().Should().Be("warning 42");

        var error = Description(entries[2]);
        error["Message"].ToString().Should().Be("error message");
        error["ExceptionMessage"].ToString().Should().Be("boom");
        error["UserIP"].ToString().Should().Be("203.0.113.10");
        error["Url"].ToString().Should().Contain("forum.example.com/Posts/5");
        error["UserAgent"].ToString().Should().Be("TestAgent/1.0");

        entries.Should().OnlyContain(e => e.Source == "YAF.Tests.Category", "without a source the logger category is used");
    }

    [Test]
    [Description("Info, warnings and errors are only written when enabled in the board settings")]
    public void Disabled_Levels_Test()
    {
        this.board.BoardSettings.LogInformation = false;
        this.board.BoardSettings.LogWarning = false;
        this.board.BoardSettings.LogError = false;

        this.logger.Info("info");
        this.logger.Warn("warn");
        this.logger.Error(new InvalidOperationException(), "error");

        this.Entries().Should().BeEmpty();
    }

    [Test]
    [Description("Special events (user deleted, spam) are always written with their user and source")]
    public void Special_Events_Test()
    {
        this.board.BoardSettings.LogInformation = false;

        this.logger.UserDeleted(5, "user deleted");
        this.logger.SpamMessageDetected(6, "spam");
        this.logger.SpamBotDetected(7, "bot");

        var entries = this.Entries();
        entries.Select(e => (e.EventType, e.UserID, e.Source)).Should().Equal(
            (EventLogTypes.UserDeleted, 5, "User Deleted"),
            (EventLogTypes.SpamMessageDetected, 6, "Spam Message Detected"),
            (EventLogTypes.SpamBotDetected, 7, "Bot Detected"));
    }

    [Test]
    [Description("The scopes of the logging framework (external scope provider) carry the log entry too")]
    public void External_Scope_Provider_Test()
    {
        var provider = new DbLoggerProvider(this.board.Resolve<IInjectServices>());
        ((ISupportExternalScope)provider).SetScopeProvider(new LoggerExternalScopeProvider());

        provider.CreateLogger("YAF.Tests.Category").UserDeleted(5, "user deleted");

        this.Entries().Should().ContainSingle().Which.UserID.Should().Be(5);
    }

    [Test]
    [Description("The provider can be disposed more than once")]
    public void Dispose_Test()
    {
        var provider = new DbLoggerProvider(this.board.Resolve<IInjectServices>());

        provider.IsDisposed.Should().BeFalse();

        ((IDisposable)provider).Dispose();
        FluentActions.Invoking(() => ((IDisposable)provider).Dispose()).Should().NotThrow();

        provider.IsDisposed.Should().BeTrue();
    }

    [Test]
    [Description("Log with a source object uses its name, a Type its full name")]
    public void Log_Source_Test()
    {
        this.logger.Log(1, "Login Failure: bob", "failed", EventLogTypes.LoginFailure);
        this.logger.Log(1, typeof(DbLoggerTests), "from a type", EventLogTypes.LoginFailure);
        this.logger.Log(1, null, "no source", EventLogTypes.LoginFailure);

        this.Entries().Select(e => e.Source).Should().Equal(
            "Login Failure: bob",
            typeof(DbLoggerTests).FullName.Truncate(47),
            "unknown");
    }

    [Test]
    [Description("Long sources are truncated to fit the column, new lines in messages are removed")]
    public void Truncate_And_NewLines_Test()
    {
        this.logger.Log("line1\nline2\rline3", EventLogTypes.Information, source: new string('s', 80));

        var entry = this.Entries().Single();
        entry.Source.Length.Should().BeLessThanOrEqualTo(50);
        Description(entry)["Message"].ToString().Should().Be("line1_line2_line3");
    }

    [Test]
    [Description("Debug messages are never written to the database")]
    public void Debug_Not_Stored_Test()
    {
        this.logger.Debug("debug");

        this.Entries().Should().BeEmpty();
    }

    [Test]
    [Description("Plain ILogger calls without a YAF log scope are not written")]
    public void Plain_ILogger_Not_Stored_Test()
    {
        this.logger.LogError("framework error");
        this.logger.Log(LogLevel.None, "none");

        this.logger.IsEnabled(LogLevel.None).Should().BeFalse();
        this.logger.IsEnabled(LogLevel.Error).Should().BeTrue();
        this.Entries().Should().BeEmpty();
    }

    [Test]
    [Description("All event types are enabled except debug and trace in release builds")]
    public void IsLogTypeEnabled_Test()
    {
        var dbLogger = (DbLogger)this.logger;

        dbLogger.IsLogTypeEnabled(EventLogTypes.Error).Should().BeTrue();
        dbLogger.IsLogTypeEnabled(EventLogTypes.SpamBotDetected).Should().BeTrue();
    }

    private List<EventLog> Entries()
    {
        return [.. this.board.Current.GetRepository<EventLog>().Get(_ => true).OrderBy(e => e.ID)];
    }

    private static JObject Description(EventLog entry)
    {
        return JObject.Parse(entry.Description);
    }
}
