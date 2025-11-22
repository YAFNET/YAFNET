// ***********************************************************************
// <copyright file="OrmLiteDiagnostics.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>
/// Class OrmLiteDiagnostics.
/// </summary>
static internal class OrmLiteDiagnostics
{
    /// <param name="listener">The listener.</param>
    extension(DiagnosticListener listener)
    {
        /// <summary>
        /// Writes the command before.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>Guid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid WriteCommandBefore(IDbCommand dbCmd, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteCommandBefore))
            {
                var operationId = Guid.NewGuid();
                listener.Write(Diagnostics.Events.OrmLite.WriteCommandBefore, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteCommandBefore,
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = dbCmd.GetConnectionId(),
                    Tag = dbCmd.GetTag(),
                    Command = dbCmd
                }.Init(Activity.Current));

                return operationId;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Writes the command after.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteCommandAfter(Guid operationId, IDbCommand dbCmd, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteCommandAfter))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteCommandAfter, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteCommandAfter,
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = dbCmd.GetConnectionId(),
                    Tag = dbCmd.GetTag(),
                    Command = dbCmd
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the command error.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteCommandError(Guid operationId, IDbCommand dbCmd,
            Exception ex, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteCommandError))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteCommandError, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteCommandError,
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = dbCmd.GetConnectionId(),
                    Tag = dbCmd.GetTag(),
                    Command = dbCmd,
                    Exception = ex
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the connection open before.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>Guid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid WriteConnectionOpenBefore(IDbConnection dbConn, [CallerMemberName] string operation = "")
        {
            if (!listener.IsEnabled(Diagnostics.Events.OrmLite.WriteConnectionOpenBefore))
            {
                return Guid.Empty;
            }

            var operationId = Guid.NewGuid();
            listener.Write(
                Diagnostics.Events.OrmLite.WriteConnectionOpenBefore,
                new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteConnectionOpenBefore,
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    Connection = dbConn,
                    StackTrace = Diagnostics.IncludeStackTrace ? Environment.StackTrace : null
                }.Init(Activity.Current));

            return operationId;
        }

        /// <summary>
        /// Writes the transaction open.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>Guid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid WriteTransactionOpen(IsolationLevel isolationLevel,
            IDbConnection dbConn, [CallerMemberName] string operation = "")
        {
            if (!listener.IsEnabled(Diagnostics.Events.OrmLite.WriteTransactionOpen))
            {
                return Guid.Empty;
            }

            var operationId = Guid.NewGuid();
            listener.Write(
                Diagnostics.Events.OrmLite.WriteTransactionOpen,
                new OrmLiteDiagnosticEvent {
                    EventType = Diagnostics.Events.OrmLite.WriteTransactionOpen,
                    OperationId = operationId,
                    Operation = operation,
                    IsolationLevel = isolationLevel,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    Connection = dbConn
                }.Init(Activity.Current));
            return operationId;
        }

        /// <summary>
        /// Writes the connection open after.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteConnectionOpenAfter(Guid operationId, IDbConnection dbConn, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteConnectionOpenAfter))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteConnectionOpenAfter, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteConnectionOpenAfter,
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    Connection = dbConn
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the connection open error.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteConnectionOpenError(Guid operationId, IDbConnection dbConn,
            Exception ex, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteConnectionOpenError))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteConnectionOpenError, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteConnectionOpenError,
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    Connection = dbConn,
                    Exception = ex
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the connection close before.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>Guid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid WriteConnectionCloseBefore(IDbConnection dbConn, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteConnectionCloseBefore))
            {
                var operationId = Guid.NewGuid();
                listener.Write(Diagnostics.Events.OrmLite.WriteConnectionCloseBefore, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteConnectionCloseBefore,
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    Connection = dbConn,
                    StackTrace = Diagnostics.IncludeStackTrace ? Environment.StackTrace : null
                }.Init(Activity.Current));

                return operationId;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Writes the connection close after.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="clientConnectionId">The client connection identifier.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteConnectionCloseAfter(Guid operationId,
            Guid clientConnectionId, IDbConnection dbConn, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteConnectionCloseAfter))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteConnectionCloseAfter, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteConnectionCloseAfter,
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = clientConnectionId,
                    Tag = dbConn.GetTag(),
                    Connection = dbConn
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the connection close error.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="clientConnectionId">The client connection identifier.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteConnectionCloseError(Guid operationId,
            Guid clientConnectionId, IDbConnection dbConn, Exception ex, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteConnectionCloseError))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteConnectionCloseError, new OrmLiteDiagnosticEvent
                {
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = clientConnectionId,
                    Tag = dbConn.GetTag(),
                    Connection = dbConn,
                    Exception = ex
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the transaction commit before.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>Guid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid WriteTransactionCommitBefore(IsolationLevel isolationLevel,
            IDbConnection dbConn, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteTransactionCommitBefore))
            {
                var operationId = Guid.NewGuid();
                listener.Write(Diagnostics.Events.OrmLite.WriteTransactionCommitBefore, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteTransactionCommitBefore,
                    OperationId = operationId,
                    Operation = operation,
                    IsolationLevel = isolationLevel,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    Connection = dbConn
                }.Init(Activity.Current));

                return operationId;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Writes the transaction commit after.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTransactionCommitAfter(Guid operationId,
            IsolationLevel isolationLevel, IDbConnection dbConn, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteTransactionCommitAfter))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteTransactionCommitAfter, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteTransactionCommitAfter,
                    OperationId = operationId,
                    Operation = operation,
                    IsolationLevel = isolationLevel,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    Connection = dbConn
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the transaction commit error.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTransactionCommitError(Guid operationId,
            IsolationLevel isolationLevel, IDbConnection dbConn, Exception ex, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteTransactionCommitError))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteTransactionCommitError, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteTransactionCommitError,
                    OperationId = operationId,
                    Operation = operation,
                    IsolationLevel = isolationLevel,
                    Connection = dbConn,
                    Exception = ex
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the transaction rollback before.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="transactionName">Name of the transaction.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>Guid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid WriteTransactionRollbackBefore(IsolationLevel isolationLevel,
            IDbConnection dbConn, string transactionName, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteTransactionRollbackBefore))
            {
                var operationId = Guid.NewGuid();
                listener.Write(Diagnostics.Events.OrmLite.WriteTransactionRollbackBefore, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteTransactionRollbackBefore,
                    OperationId = operationId,
                    Operation = operation,
                    IsolationLevel = isolationLevel,
                    Connection = dbConn,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    TransactionName = transactionName
                }.Init(Activity.Current));

                return operationId;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Writes the transaction rollback after.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="transactionName">Name of the transaction.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTransactionRollbackAfter(Guid operationId,
            IsolationLevel isolationLevel, IDbConnection dbConn, string transactionName,
            [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteTransactionRollbackAfter))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteTransactionRollbackAfter, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteTransactionRollbackAfter,
                    OperationId = operationId,
                    Operation = operation,
                    IsolationLevel = isolationLevel,
                    Connection = dbConn, ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    TransactionName = transactionName
                }.Init(Activity.Current));
            }
        }

        /// <summary>
        /// Writes the transaction rollback error.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="transactionName">Name of the transaction.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="operation">The operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTransactionRollbackError(Guid operationId,
            IsolationLevel isolationLevel, IDbConnection dbConn, string transactionName, Exception ex,
            [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(Diagnostics.Events.OrmLite.WriteTransactionRollbackError))
            {
                listener.Write(Diagnostics.Events.OrmLite.WriteTransactionRollbackError, new OrmLiteDiagnosticEvent
                {
                    EventType = Diagnostics.Events.OrmLite.WriteTransactionRollbackError,
                    OperationId = operationId,
                    Operation = operation,
                    IsolationLevel = isolationLevel,
                    Connection = dbConn,
                    ConnectionId = dbConn.GetConnectionId(),
                    Tag = dbConn.GetTag(),
                    TransactionName = transactionName,
                    Exception = ex
                }.Init(Activity.Current));
            }
        }
    }
}