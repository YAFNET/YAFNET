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

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Abstractions;

using YAF.Core.Events;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Tasks;

/// <summary>
/// The background task base classes (start delay / run period, run once, errors, dispose) and the
/// <see cref="AppInitTaskManager"/> that starts and tracks them.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BackgroundTaskTests
{
    private TestBoardContext board;

    private readonly List<IBackgroundTask> started = [];

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
    }

    [TearDown]
    public void TearDown()
    {
        this.started.ForEach(t => t.Dispose());
        this.started.Clear();

        var manager = this.NewManager();

        foreach (var name in manager.TaskManagerInstances.Where(n => n.StartsWith("test-") || n == nameof(CleanUpTask)))
        {
            if (manager.TryGetTask(name, out var task))
            {
                task.Dispose();
            }

            manager.TryRemoveTask(name);
        }

        this.board?.Dispose();
    }

    [Test]
    [Description("An intermittent task waits the start delay (ms) before the first run and then runs every period (ms)")]
    public async Task Start_Delay_And_Period_Test()
    {
        var task = this.Start(new CountingTask { StartDelayMs = 300, RunPeriodMs = 20 });

        task.IsRunning.Should().BeTrue();
        task.Runs.Should().Be(0, "the first run waits for the start delay");

        await WaitUntil(() => task.Runs >= 3);

        task.Started.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Test]
    [Description("Starting a running task again does nothing")]
    public async Task Run_Twice_Test()
    {
        var task = this.Start(new CountingTask { StartDelayMs = 0, RunPeriodMs = 1000 });
        task.Run();

        await WaitUntil(() => task.Runs >= 1);
        await Task.Delay(200);

        task.Runs.Should().Be(1, "only one loop runs");
    }

    [Test]
    [Description("Without a period the task runs once and is then finished")]
    public async Task Run_Once_Test()
    {
        var task = this.Start(new CountingTask { StartDelayMs = 0, RunPeriodMs = Timeout.Infinite });

        await WaitUntil(() => !task.IsRunning);
        await Task.Delay(100);

        task.Runs.Should().Be(1);
    }

    [Test]
    [Description("A failing run is logged and the task keeps running")]
    public async Task Failing_Run_Test()
    {
        var task = this.Start(new CountingTask { StartDelayMs = 0, RunPeriodMs = 10, FailFirstRuns = 2 });

        await WaitUntil(() => task.Runs >= 4);

        task.IsRunning.Should().BeTrue();
    }

    [Test]
    [Description("A disposed task stops running")]
    public async Task Dispose_Stops_Task_Test()
    {
        var task = this.Start(new CountingTask { StartDelayMs = 0, RunPeriodMs = 10 });
        await WaitUntil(() => task.Runs >= 2);

        task.Dispose();
        task.IsRunning.Should().BeFalse();

        await Task.Delay(100);
        var runs = task.Runs;
        await Task.Delay(200);

        task.Runs.Should().Be(runs);
    }

    [Test]
    [Description("A long task runs once in the background and is only finished when its work is done")]
    public async Task Long_Task_Test()
    {
        var release = new TaskCompletionSource();
        var task = this.Start(new LongTask(release.Task));

        task.IsRunning.Should().BeTrue();
        await Task.Delay(100);
        task.IsRunning.Should().BeTrue("the work is still running");

        release.SetResult();

        await WaitUntil(() => !task.IsRunning);
        task.Runs.Should().Be(1);
    }

    [Test]
    [Description("A long task waits the start delay before it runs (e.g. the search index update 30 seconds after the first request)")]
    public async Task Long_Task_Start_Delay_Test()
    {
        var task = this.Start(new LongTask(Task.CompletedTask) { StartDelayMs = 300 });

        await Task.Delay(100);
        task.Runs.Should().Be(0, "the start delay has not passed");
        task.IsRunning.Should().BeTrue("the task is waiting for its run");

        await WaitUntil(() => !task.IsRunning);
        task.Runs.Should().Be(1);
    }

    [Test]
    [Description("The task manager only starts tasks after the application started, and every name only once")]
    public async Task Task_Manager_Test()
    {
        var manager = this.NewManager();

        manager.StartTask("test-before", () => new CountingTask()).Should().BeFalse("the application has not started");

        manager.Handle(new HttpContextInitEvent(this.board.HttpContext));

        manager.TaskExists(nameof(CleanUpTask)).Should().BeTrue("the clean up task is started with the application");

        var release = new TaskCompletionSource();
        manager.StartTask("test-long", () => new LongTask(release.Task)).Should().BeTrue();
        manager.StartTask("test-long", () => new LongTask(release.Task)).Should().BeFalse("the task exists already");

        manager.TaskExists("test-long").Should().BeTrue();
        manager.IsTaskRunning("test-long").Should().BeTrue();
        manager.AreTasksRunning(["test-unknown", "test-long"]).Should().BeTrue();
        manager.AreTasksRunning(["test-unknown"]).Should().BeFalse();
        manager.TaskManagerInstances.Should().Contain("test-long");
        manager.TaskManagerSnapshot.Should().ContainKey("test-long");
        manager.TaskCount.Should().BeGreaterThanOrEqualTo(2);

        // the clean up task removes finished tasks
        release.SetResult();
        await WaitUntil(() => !manager.TaskExists("test-long"));

        manager.TryRemoveTask("test-unknown").Should().BeFalse();
        FluentActions.Invoking(() => manager.StartTask(null, () => new CountingTask())).Should().Throw<ArgumentNullException>();
    }

    private AppInitTaskManager NewManager()
    {
        return new AppInitTaskManager(this.board.Resolve<IServiceLocator>(), NullLogger<AppInitTaskManager>.Instance);
    }

    [Test]
    [Description("A task started by a request does not get the execution context of the request (the HttpContext of IHttpContextAccessor is an AsyncLocal), it could use the BoardContext of the request concurrently")]
    public async Task No_Execution_Context_Of_Caller_Test()
    {
        Caller.Value = "request";

        var intermittent = this.Start(new ContextTask { StartDelayMs = 0, RunPeriodMs = Timeout.Infinite });
        var longTask = this.Start(new LongContextTask());

        Caller.Value = null;

        await WaitUntil(() => intermittent.Seen != ContextTask.NotRun && longTask.Seen != ContextTask.NotRun);

        intermittent.Seen.Should().BeNull();
        longTask.Seen.Should().BeNull();
    }

    private T Start<T>(T task)
        where T : IBackgroundTask
    {
        this.started.Add(task);
        task.Run();

        return task;
    }

    private async static Task WaitUntil(Func<bool> condition)
    {
        var watch = Stopwatch.StartNew();

        while (!condition())
        {
            if (watch.Elapsed > TimeSpan.FromSeconds(10))
            {
                Assert.Fail("The condition was not met in time.");
            }

            await Task.Delay(10);
        }
    }

    private readonly static AsyncLocal<string> Caller = new();

    private class ContextTask : IntermittentBackgroundTask
    {
        public const string NotRun = "not run";

        public string Seen { get; private set; } = NotRun;

        public override string TaskName => "test-context";

        public override Task RunOnceAsync()
        {
            this.Seen = Caller.Value;

            return Task.CompletedTask;
        }
    }

    private class LongContextTask : LongBackgroundTask
    {
        public string Seen { get; private set; } = ContextTask.NotRun;

        public override Task RunOnceAsync()
        {
            this.Seen = Caller.Value;

            return Task.CompletedTask;
        }
    }

    private class CountingTask : IntermittentBackgroundTask
    {
        private int runs;

        public int FailFirstRuns { get; set; }

        public int Runs => Volatile.Read(ref this.runs);

        public override string TaskName => "test-counting";

        public override Task RunOnceAsync()
        {
            var run = Interlocked.Increment(ref this.runs);

            if (run <= this.FailFirstRuns)
            {
                throw new InvalidOperationException("failing run");
            }

            return Task.CompletedTask;
        }
    }

    private class LongTask(Task work) : LongBackgroundTask
    {
        public int Runs { get; private set; }

        public async override Task RunOnceAsync()
        {
            await work;
            this.Runs++;
        }
    }
}
