// ***********************************************************************
// <copyright file="ProfiledDbTransaction.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data.Common;
using System.Data;
using ServiceStack.Data;

#pragma warning disable 1591 // xml doc comments warnings

namespace ServiceStack.MiniProfiler.Data;

/// <summary>
/// Class ProfiledDbTransaction.
/// Implements the <see cref="System.Data.Common.DbTransaction" />
/// Implements the <see cref="ServiceStack.Data.IHasDbTransaction" />
/// </summary>
/// <seealso cref="System.Data.Common.DbTransaction" />
/// <seealso cref="ServiceStack.Data.IHasDbTransaction" />
public class ProfiledDbTransaction : DbTransaction, IHasDbTransaction
{
    /// <summary>
    /// The database
    /// </summary>
    private ProfiledConnection db;
    /// <summary>
    /// The trans
    /// </summary>
    private DbTransaction trans;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfiledDbTransaction"/> class.
    /// </summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="connection">The connection.</param>
    /// <exception cref="System.ArgumentNullException">transaction</exception>
    /// <exception cref="System.ArgumentNullException">connection</exception>
    public ProfiledDbTransaction(DbTransaction transaction, ProfiledConnection connection)
    {
        this.trans = transaction ?? throw new ArgumentNullException(nameof(transaction));
        this.db = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    /// <summary>
    /// Specifies the <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.
    /// </summary>
    /// <value>The database connection.</value>
    protected override DbConnection DbConnection => db;

    /// <summary>
    /// Gets the database transaction.
    /// </summary>
    /// <value>The database transaction.</value>
    public IDbTransaction DbTransaction => trans;

    /// <summary>
    /// Specifies the <see cref="T:System.Data.IsolationLevel" /> for this transaction.
    /// </summary>
    /// <value>The isolation level.</value>
    public override IsolationLevel IsolationLevel => trans.IsolationLevel;

    /// <summary>
    /// Commits the database transaction.
    /// </summary>
    public override void Commit()
    {
        trans.Commit();
    }

    /// <summary>
    /// Rolls back a transaction from a pending state.
    /// </summary>
    public override void Rollback()
    {
        trans.Rollback();
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.Data.Common.DbTransaction" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">If <see langword="true" />, this method releases all resources held by any managed objects that this <see cref="T:System.Data.Common.DbTransaction" /> references.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            trans?.Dispose();
        }
        trans = null;
        db = null;
        base.Dispose(disposing);
    }
}

#pragma warning restore 1591 // xml doc comments warnings