// ***********************************************************************
// <copyright file="SavePoint.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Threading.Tasks;

using ServiceStack.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class SavePoint.
/// </summary>
public class SavePoint
{
    /// <summary>
    /// Gets the transaction.
    /// </summary>
    /// <value>The transaction.</value>
    private OrmLiteTransaction Transaction { get; }

    /// <summary>
    /// Gets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    private IOrmLiteDialectProvider DialectProvider { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>
    /// The did release
    /// </summary>
    private bool didRelease;

    /// <summary>
    /// The did rollback
    /// </summary>
    private bool didRollback;

    /// <summary>
    /// Initializes a new instance of the <see cref="SavePoint"/> class.
    /// </summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="name">The name.</param>
    public SavePoint(OrmLiteTransaction transaction, string name)
    {
        this.Transaction = transaction;
        this.Name = name;
        this.DialectProvider = this.Transaction.Db.GetDialectProvider();
    }

    /// <summary>
    /// Verifies the state of the valid.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">SAVEPOINT {Name} already RELEASED</exception>
    /// <exception cref="System.InvalidOperationException">SAVEPOINT {Name} already ROLLBACKED</exception>
    void VerifyValidState()
    {
        if (this.didRelease)
            throw new InvalidOperationException($"SAVEPOINT {this.Name} already RELEASED");
        if (this.didRollback)
            throw new InvalidOperationException($"SAVEPOINT {this.Name} already ROLLBACKED");
    }

    /// <summary>
    /// Saves this instance.
    /// </summary>
    public void Save()
    {
        this.VerifyValidState();

        var sql = this.DialectProvider.ToCreateSavePoint(this.Name);
        if (!string.IsNullOrEmpty(sql))
        {
            this.Transaction.Db.ExecuteSql(sql);
        }
    }

    /// <summary>
    /// Save as an asynchronous operation.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SaveAsync()
    {
        this.VerifyValidState();

        var sql = this.DialectProvider.ToCreateSavePoint(this.Name);
        if (!string.IsNullOrEmpty(sql))
        {
            await this.Transaction.Db.ExecuteSqlAsync(sql).ConfigAwait();
        }
    }

    /// <summary>
    /// Releases this instance.
    /// </summary>
    public void Release()
    {
        this.VerifyValidState();
        this.didRelease = true;

        var sql = this.DialectProvider.ToReleaseSavePoint(this.Name);
        if (!string.IsNullOrEmpty(sql))
        {
            this.Transaction.Db.ExecuteSql(sql);
        }
    }

    /// <summary>
    /// Release as an asynchronous operation.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task ReleaseAsync()
    {
        this.VerifyValidState();
        this.didRelease = true;

        var sql = this.DialectProvider.ToReleaseSavePoint(this.Name);
        if (!string.IsNullOrEmpty(sql))
        {
            await this.Transaction.Db.ExecuteSqlAsync(sql).ConfigAwait();
        }
    }

    /// <summary>
    /// Rollbacks this instance.
    /// </summary>
    public void Rollback()
    {
        this.VerifyValidState();
        this.didRollback = true;

        var sql = this.DialectProvider.ToRollbackSavePoint(this.Name);
        if (!string.IsNullOrEmpty(sql))
        {
            this.Transaction.Db.ExecuteSql(sql);
        }
    }

    /// <summary>
    /// Rollback as an asynchronous operation.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task RollbackAsync()
    {
        this.VerifyValidState();
        this.didRollback = true;

        var sql = this.DialectProvider.ToRollbackSavePoint(this.Name);
        if (!string.IsNullOrEmpty(sql))
        {
            await this.Transaction.Db.ExecuteSqlAsync(sql).ConfigAwait();
        }
    }
}