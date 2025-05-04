// ***********************************************************************
// <copyright file="Diagnostics.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class Diagnostics.
/// </summary>
public class Diagnostics
{
    /// <summary>
    /// The instance
    /// </summary>
    private readonly static Diagnostics Instance = new();

    /// <summary>
    /// Prevents a default instance of the <see cref="Diagnostics"/> class from being created.
    /// </summary>
    private Diagnostics()
    {
    }

    /// <summary>
    /// The include stack trace
    /// </summary>
    private bool includeStackTrace;

    /// <summary>
    /// Gets or sets a value indicating whether [include stack trace].
    /// </summary>
    /// <value><c>true</c> if [include stack trace]; otherwise, <c>false</c>.</value>
    public static bool IncludeStackTrace
    {
        get => Instance.includeStackTrace;
        set => Instance.includeStackTrace = value;
    }

    /// <summary>
    /// Class Listeners.
    /// </summary>
    public static class Listeners
    {
        /// <summary>
        /// The service stack
        /// </summary>
        public const string ServiceStack = "ServiceStack";

        /// <summary>
        /// The orm lite
        /// </summary>
        public const string OrmLite = "ServiceStack.OrmLite";

        /// <summary>
        /// The redis
        /// </summary>
        public const string Redis = "ServiceStack.Redis";
    }

    /// <summary>
    /// Class Events.
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// Class ServiceStack.
        /// </summary>
        public static class ServiceStack
        {
            /// <summary>
            /// The prefix
            /// </summary>
            private const string Prefix = Listeners.ServiceStack + ".";

            /// <summary>
            /// The write request before
            /// </summary>
            public const string WriteRequestBefore = Prefix + nameof(WriteRequestBefore);

            /// <summary>
            /// The write request after
            /// </summary>
            public const string WriteRequestAfter = Prefix + nameof(WriteRequestAfter);

            /// <summary>
            /// The write request error
            /// </summary>
            public const string WriteRequestError = Prefix + nameof(WriteRequestError);

            /// <summary>
            /// The write gateway before
            /// </summary>
            public const string WriteGatewayBefore = Prefix + nameof(WriteGatewayBefore);

            /// <summary>
            /// The write gateway after
            /// </summary>
            public const string WriteGatewayAfter = Prefix + nameof(WriteGatewayAfter);

            /// <summary>
            /// The write gateway error
            /// </summary>
            public const string WriteGatewayError = Prefix + nameof(WriteGatewayError);
        }

        /// <summary>
        /// Class OrmLite.
        /// </summary>
        public static class OrmLite
        {
            /// <summary>
            /// The prefix
            /// </summary>
            private const string Prefix = Listeners.OrmLite + ".";

            /// <summary>
            /// The write command before
            /// </summary>
            public const string WriteCommandBefore = Prefix + nameof(WriteCommandBefore);

            /// <summary>
            /// The write command after
            /// </summary>
            public const string WriteCommandAfter = Prefix + nameof(WriteCommandAfter);

            /// <summary>
            /// The write command error
            /// </summary>
            public const string WriteCommandError = Prefix + nameof(WriteCommandError);

            /// <summary>
            /// The write connection open before
            /// </summary>
            public const string WriteConnectionOpenBefore = Prefix + nameof(WriteConnectionOpenBefore);

            /// <summary>
            /// The write connection open after
            /// </summary>
            public const string WriteConnectionOpenAfter = Prefix + nameof(WriteConnectionOpenAfter);

            /// <summary>
            /// The write connection open error
            /// </summary>
            public const string WriteConnectionOpenError = Prefix + nameof(WriteConnectionOpenError);

            /// <summary>
            /// The write connection close before
            /// </summary>
            public const string WriteConnectionCloseBefore = Prefix + nameof(WriteConnectionCloseBefore);

            /// <summary>
            /// The write connection close after
            /// </summary>
            public const string WriteConnectionCloseAfter = Prefix + nameof(WriteConnectionCloseAfter);

            /// <summary>
            /// The write connection close error
            /// </summary>
            public const string WriteConnectionCloseError = Prefix + nameof(WriteConnectionCloseError);

            /// <summary>
            /// The write transaction open
            /// </summary>
            public const string WriteTransactionOpen = Prefix + nameof(WriteTransactionOpen);

            /// <summary>
            /// The write transaction commit before
            /// </summary>
            public const string WriteTransactionCommitBefore = Prefix + nameof(WriteTransactionCommitBefore);

            /// <summary>
            /// The write transaction commit after
            /// </summary>
            public const string WriteTransactionCommitAfter = Prefix + nameof(WriteTransactionCommitAfter);

            /// <summary>
            /// The write transaction commit error
            /// </summary>
            public const string WriteTransactionCommitError = Prefix + nameof(WriteTransactionCommitError);

            /// <summary>
            /// The write transaction rollback before
            /// </summary>
            public const string WriteTransactionRollbackBefore = Prefix + nameof(WriteTransactionRollbackBefore);

            /// <summary>
            /// The write transaction rollback after
            /// </summary>
            public const string WriteTransactionRollbackAfter = Prefix + nameof(WriteTransactionRollbackAfter);

            /// <summary>
            /// The write transaction rollback error
            /// </summary>
            public const string WriteTransactionRollbackError = Prefix + nameof(WriteTransactionRollbackError);
        }
    }

    /// <summary>
    /// Class Activity.
    /// </summary>
    public static class Activity
    {
        /// <summary>
        /// The HTTP begin
        /// </summary>
        public const string HttpBegin = nameof(HttpBegin);

        /// <summary>
        /// The HTTP end
        /// </summary>
        public const string HttpEnd = nameof(HttpEnd);

        /// <summary>
        /// The operation identifier
        /// </summary>
        public const string OperationId = nameof(OperationId);

        /// <summary>
        /// The user identifier
        /// </summary>
        public const string UserId = nameof(UserId);

        /// <summary>
        /// The tag
        /// </summary>
        public const string Tag = nameof(Tag);
    }

    /// <summary>
    /// Gets or sets the servicestack.
    /// </summary>
    /// <value>The servicestack.</value>
    private DiagnosticListener servicestack { get; } = new(Listeners.ServiceStack);

    /// <summary>
    /// Gets or sets the ormlite.
    /// </summary>
    /// <value>The ormlite.</value>
    private DiagnosticListener ormlite { get; } = new(Listeners.OrmLite);

    /// <summary>
    /// Gets or sets the redis.
    /// </summary>
    /// <value>The redis.</value>
    private DiagnosticListener redis { get; } = new(Listeners.Redis);

    /// <summary>
    /// Gets the service stack.
    /// </summary>
    /// <value>The service stack.</value>
    public static DiagnosticListener ServiceStack => Instance.servicestack;

    /// <summary>
    /// Gets the orm lite.
    /// </summary>
    /// <value>The orm lite.</value>
    public static DiagnosticListener OrmLite => Instance.ormlite;

    /// <summary>
    /// Gets the redis.
    /// </summary>
    /// <value>The redis.</value>
    public static DiagnosticListener Redis => Instance.redis;
}

/// <summary>
/// Enum ProfileSource
/// </summary>
[Flags]
public enum ProfileSource
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0,

    /// <summary>
    /// The service stack
    /// </summary>
    ServiceStack = 1 << 0,

    /// <summary>
    /// The redis
    /// </summary>
    Redis = 1 << 1,

    /// <summary>
    /// The orm lite
    /// </summary>
    OrmLite = 1 << 2,

    /// <summary>
    /// All
    /// </summary>
    All = ServiceStack | OrmLite | Redis
}

/// <summary>
/// Class DiagnosticEvent.
/// </summary>
public abstract class DiagnosticEvent
{
    /// <summary>
    /// Gets the source.
    /// </summary>
    /// <value>The source.</value>
    public virtual string Source => this.GetType().Name.Replace(nameof(DiagnosticEvent), "");

    /// <summary>
    /// Gets or sets the type of the event.
    /// </summary>
    /// <value>The type of the event.</value>
    public string? EventType { get; set; }

    /// <summary>
    /// Gets or sets the operation identifier.
    /// </summary>
    /// <value>The operation identifier.</value>
    public Guid OperationId { get; set; }

    /// <summary>
    /// Gets or sets the operation.
    /// </summary>
    /// <value>The operation.</value>
    public string? Operation { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier.
    /// </summary>
    /// <value>The trace identifier.</value>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the user authentication identifier.
    /// </summary>
    /// <value>The user authentication identifier.</value>
    public string? UserAuthId { get; set; }

    /// <summary>
    /// Gets or sets the exception.
    /// </summary>
    /// <value>The exception.</value>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    /// <value>The timestamp.</value>
    public long Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>The tag.</value>
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets the stack trace.
    /// </summary>
    /// <value>The stack trace.</value>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the meta.
    /// </summary>
    /// <value>The meta.</value>
    public Dictionary<string, string>? Meta { get; set; }
}

/// <summary>
/// Class OrmLiteDiagnosticEvent.
/// Implements the <see cref="DiagnosticEvent" />
/// </summary>
/// <seealso cref="DiagnosticEvent" />
public class OrmLiteDiagnosticEvent : DiagnosticEvent
{
    /// <summary>
    /// Gets the source.
    /// </summary>
    /// <value>The source.</value>
    public override string Source => "OrmLite";

    /// <summary>
    /// Gets or sets the connection identifier.
    /// </summary>
    /// <value>The connection identifier.</value>
    public Guid? ConnectionId { get; set; }

    /// <summary>
    /// Gets or sets the connection.
    /// </summary>
    /// <value>The connection.</value>
    public IDbConnection? Connection { get; set; }

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    public IDbCommand? Command { get; set; }

    /// <summary>
    /// Gets or sets the isolation level.
    /// </summary>
    /// <value>The isolation level.</value>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// Gets or sets the name of the transaction.
    /// </summary>
    /// <value>The name of the transaction.</value>
    public string? TransactionName { get; set; }
}

/// <summary>
/// Class DiagnosticsUtils.
/// </summary>
public static class DiagnosticsUtils
{
    /// <summary>
    /// Gets the root.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>System.Nullable&lt;Activity&gt;.</returns>
    public static Activity? GetRoot(this Activity? activity)
    {
        if (activity == null)
        {
            return null;
        }

        while (activity.Parent != null)
        {
            activity = activity.Parent;
        }

        return activity;
    }

    /// <summary>
    /// Gets the trace identifier.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>System.Nullable&lt;System.String&gt;.</returns>
    public static string? GetTraceId(this Activity? activity)
    {
        return GetRoot(activity)?.ParentId;
    }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>System.Nullable&lt;System.String&gt;.</returns>
    public static string? GetUserId(this Activity? activity)
    {
        return GetRoot(activity)?.GetTagItem(Diagnostics.Activity.UserId) as string;
    }

    /// <summary>
    /// Gets the tag.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>System.Nullable&lt;System.String&gt;.</returns>
    public static string? GetTag(this Activity? activity)
    {
        return GetRoot(activity)?.GetTagItem(Diagnostics.Activity.Tag) as string;
    }

    /// <summary>
    /// Initializes the specified activity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="evt">The evt.</param>
    /// <param name="activity">The activity.</param>
    /// <returns>T.</returns>
    public static T Init<T>(this T evt, Activity? activity)
        where T : DiagnosticEvent
    {
        var rootActivity = GetRoot(activity);
        if (rootActivity != null)
        {
            evt.TraceId ??= rootActivity.GetTraceId();
            evt.UserAuthId ??= rootActivity.GetUserId();
            evt.Tag ??= rootActivity.GetTag();
        }

        evt.Timestamp = Stopwatch.GetTimestamp();
        return evt;
    }
}