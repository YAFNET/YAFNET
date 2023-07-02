// ***********************************************************************
// <copyright file="AppTasks.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using ServiceStack.Logging;
using ServiceStack.Text;

namespace ServiceStack;

/// <summary>
/// Interface IAppTask
/// </summary>
public interface IAppTask
{
    /// <summary>
    /// Gets or sets the log.
    /// </summary>
    /// <value>The log.</value>
    public string? Log { get; set; }

    /// <summary>
    /// Gets or sets the started at.
    /// </summary>
    /// <value>The started at.</value>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the completed date.
    /// </summary>
    /// <value>The completed date.</value>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    /// <value>The error.</value>
    public Exception? Error { get; set; }
}

/// <summary>
/// Class AppTaskResult.
/// </summary>
public class AppTaskResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppTaskResult" /> class.
    /// </summary>
    /// <param name="tasksRun">The tasks run.</param>
    public AppTaskResult(List<IAppTask> tasksRun)
    {
        TasksRun = tasksRun;
        TypesCompleted = tasksRun.Where(x => x.Error == null).Map(x => x.GetType());
    }

    /// <summary>
    /// Gets the logs.
    /// </summary>
    /// <returns>System.String.</returns>
    public string GetLogs()
    {
        var sb = StringBuilderCache.Allocate();
        foreach (var instance in TasksRun)
        {
            var migrationType = instance.GetType();
            var descFmt = AppTasks.GetDescFmt(migrationType);
            sb.AppendLine($"# {migrationType.Name}{descFmt}");
            sb.AppendLine(instance.Log);
            sb.AppendLine();
        }
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    /// <value>The error.</value>
    public Exception? Error { get; set; }
    /// <summary>
    /// Gets the types completed.
    /// </summary>
    /// <value>The types completed.</value>
    public List<Type> TypesCompleted { get; }
    /// <summary>
    /// Gets the tasks run.
    /// </summary>
    /// <value>The tasks run.</value>
    public List<IAppTask> TasksRun { get; }
    /// <summary>
    /// Gets a value indicating whether this <see cref="AppTaskResult" /> is succeeded.
    /// </summary>
    /// <value><c>true</c> if succeeded; otherwise, <c>false</c>.</value>
    public bool Succeeded => Error == null && TasksRun.All(x => x.Error == null);
}

/// <summary>
/// Class AppTasks.
/// </summary>
public class AppTasks
{
    /// <summary>
    /// Gets or sets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static AppTasks Instance { get; set; } = new();
    /// <summary>
    /// Gets or sets the log.
    /// </summary>
    /// <value>The log.</value>
    public ILog Log { get; set; } = new ConsoleLogger(typeof(AppTasks));
    /// <summary>
    /// Gets the tasks.
    /// </summary>
    /// <value>The tasks.</value>
    public Dictionary<string, Action<string[]>> Tasks { get; } = new();

    /// <summary>
    /// Register Task to run in APP_TASKS=task1;task2
    /// </summary>
    /// <param name="taskName">Name of the task.</param>
    /// <param name="appTask">The application task.</param>
    public static void Register(string taskName, Action<string[]> appTask)
    {
        Instance.Tasks[taskName] = appTask;
    }

    /// <summary>
    /// Gets the application task commands.
    /// </summary>
    /// <returns>System.Nullable&lt;System.String&gt;.</returns>
    public static string? GetAppTaskCommands() => GetAppTaskCommands(Environment.GetCommandLineArgs());

    /// <summary>
    /// Gets the application task commands.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Nullable&lt;System.String&gt;.</returns>
    public static string? GetAppTaskCommands(string[] args)
    {
        foreach (var arg in args)
        {
            if (arg.IndexOf('=') == -1)
                continue;
            var key = arg.LeftPart('=').TrimPrefixes("/", "--");
            if (key == nameof(AppTasks))
                return arg.RightPart('=');
        }

        return null;
    }

    /// <summary>
    /// Determines whether [is run as application task].
    /// </summary>
    /// <returns><c>true</c> if [is run as application task]; otherwise, <c>false</c>.</returns>
    public static bool IsRunAsAppTask() => GetAppTaskCommands() != null;

    /// <summary>
    /// Rans as task.
    /// </summary>
    /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
    public static int? RanAsTask()
    {
        var appTasksStr = GetAppTaskCommands();
        if (appTasksStr != null)
        {
            var tasks = Instance.Tasks;
            if (tasks.Count > 0)
            {
                var appTasks = appTasksStr.Split(';');
                for (var i = 0; i < appTasks.Length; i++)
                {
                    var appTaskWithArgs = appTasks[i];
                    var appTask = appTaskWithArgs.LeftPart(':');
                    var args = appTaskWithArgs.IndexOf(':') >= 0
                        ? appTaskWithArgs.RightPart(':').Split(',')
                        : Array.Empty<string>();

                    if (!tasks.TryGetValue(appTask, out var taskFn))
                    {
                        Instance.Log.Warn($"Unknown AppTask '{appTask}' was not registered with this App, ignoring...");
                        continue;
                    }

                    var exitCode = 0;
                    try
                    {
                        Instance.Log.Info($"Running AppTask '{appTask}'...");
                        taskFn(args);
                    }
                    catch (Exception e)
                    {
                        exitCode = i + 1; // return 1-based index of AppTask that failed
                        Instance.Log.Error($"Failed to run AppTask '{appTask}'", e);
                    }
                    return exitCode;
                }
            }
            else
            {
                Instance.Log.Info("No AppTasks to run, exiting...");
            }
            return 0;
        }
        return null;
    }

    /// <summary>
    /// Runs the specified on exit.
    /// </summary>
    /// <param name="onExit">The on exit.</param>
    public static void Run(Action? onExit = null)
    {
        var exitCode = RanAsTask();
        if (exitCode != null)
        {
            onExit?.Invoke();
            Environment.Exit(exitCode.Value);
            // Trying to Stop Application before app.Run() throws Unhandled exception. System.OperationCanceledException
            // var appLifetime = ApplicationServices.Resolve<IHostApplicationLifetime>();
            // Environment.ExitCode = exitCode;
            // appLifetime.StopApplication();
        }
    }

    /// <summary>
    /// Gets the desc FMT.
    /// </summary>
    /// <param name="nextRun">The next run.</param>
    /// <returns>System.String.</returns>
    public static string GetDescFmt(Type nextRun)
    {
        var desc = GetDesc(nextRun);
        return desc != null ? " '" + desc + "'" : "";
    }

    /// <summary>
    /// Gets the desc.
    /// </summary>
    /// <param name="nextRun">The next run.</param>
    /// <returns>System.Nullable&lt;System.String&gt;.</returns>
    public static string? GetDesc(Type nextRun)
    {
        var desc = nextRun.GetDescription() ?? nextRun.FirstAttribute<NotesAttribute>()?.Notes;
        return desc;
    }

}