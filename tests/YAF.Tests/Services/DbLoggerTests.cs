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

using Newtonsoft.Json.Linq;

using YAF.Core.Extensions;
using YAF.Core.Services.Logger;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The <see cref="DbLogger"/>: the event log entries in the database, the board settings of the log levels,
/// the request details and the source of the entries.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DbLoggerTests
{
    private TestBoardContext board;

    private ILoggerService logger;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);

        this.logger = new DbLoggerProvider(this.board.Resolve<IInjectServices>()).Create(typeof(DbLoggerTests));

        this.board.BoardSettings.LogInformation = true;
        this.board.BoardSettings.LogWarning = true;
        this.board.BoardSettings.LogError = true;
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
        error["Url"].ToString().Should().Contain("forum.example.com");

        entries.Should().OnlyContain(
            e => e.Source == typeof(DbLoggerTests).FullName,
            "without a source the type of the logger is used");
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
    [Description("Log with a source object uses its name, a Type its full name")]
    public void Log_Source_Test()
    {
        this.logger.Log(1, "Login Failure: bob", "failed", EventLogTypes.LoginFailure);
        this.logger.Log(1, typeof(DbLoggerTests), "from a type", EventLogTypes.LoginFailure);
        this.logger.Log(1, null, "no source", EventLogTypes.LoginFailure);

        this.Entries().Select(e => e.Source).Should().Equal(
            "Login Failure: bob",
            typeof(DbLoggerTests).FullName,
            "unknown");
    }

    [Test]
    [Description("Long sources are truncated to fit the column")]
    public void Truncate_Source_Test()
    {
        this.logger.Log("message", EventLogTypes.Information, source: new string('s', 80));

        this.Entries().Single().Source.Length.Should().BeLessThanOrEqualTo(50);
    }

    [Test]
    [Description("Debug and trace messages are never written to the database")]
    public void Debug_Not_Stored_Test()
    {
        this.logger.Debug("debug");
        this.logger.Trace("trace {0}", 1);

        this.Entries().Should().BeEmpty();
    }

    [Test]
    [Description("All event types are enabled, debug and trace only in debug builds")]
    public void IsLogTypeEnabled_Test()
    {
        var dbLogger = (DbLogger)this.logger;

        dbLogger.IsLogTypeEnabled(EventLogTypes.Error).Should().BeTrue();
        dbLogger.IsLogTypeEnabled(EventLogTypes.SpamBotDetected).Should().BeTrue();
        dbLogger.IsLogTypeEnabled(EventLogTypes.UserDeleted).Should().BeTrue();
    }

    private static JObject Description(EventLog entry)
    {
        return JObject.Parse(entry.Description);
    }

    private List<EventLog> Entries()
    {
        return [.. this.board.Current.GetRepository<EventLog>().Get(_ => true).OrderBy(e => e.ID)];
    }
}
