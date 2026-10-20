// ***********************************************************************
// <copyright file="OrmLiteRetryReader.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// The rows of a query that's run again when reading its first row fails with a temporary error, which some
/// databases report after the query has started, e.g. a deadlock or a lost connection. Once a row has been read,
/// the query isn't run again as its rows have been returned.
/// </summary>
internal sealed class OrmLiteRetryReader : DbDataReader
{
    private DbDataReader reader;
    private readonly IDbCommand dbCmd;
    private readonly IDbCommand runCmd;
    private readonly IOrmLiteDialectProvider dialect;
    private readonly OrmLiteRetryPolicy policy;
    private readonly CommandBehavior behavior;
    private bool started;

    public OrmLiteRetryReader(DbDataReader reader, IDbCommand dbCmd, IOrmLiteDialectProvider dialect,
        OrmLiteRetryPolicy policy, CommandBehavior behavior)
    {
        this.reader = reader;
        this.dbCmd = dbCmd;
        this.runCmd = dbCmd.ToDbCommand(); // runs the query without retrying it again
        this.dialect = dialect;
        this.policy = policy;
        this.behavior = behavior;
    }

    public override bool Read()
    {
        if (this.started)
        {
            return this.reader.Read();
        }

        for (var retry = 1; ; retry++)
        {
            try
            {
                var ret = this.reader.Read();
                this.started = true;
                return ret;
            }
            catch (Exception ex) when (OrmLiteRetryExec.ShouldRetryStatement(this.policy, this.dialect, this.dbCmd, ex, retry, out var delay))
            {
                DisposeQuietly(this.reader);
                Thread.Sleep(delay);
            }
            OrmLiteRetryExec.Reopen(this.dbCmd);
            this.reader = (DbDataReader)this.runCmd.ExecuteReader(this.behavior);
        }
    }

    public async override Task<bool> ReadAsync(CancellationToken token)
    {
        if (this.started)
        {
            return await this.dialect.ReadAsync(this.reader, token).ConfigAwait();
        }

        for (var retry = 1; ; retry++)
        {
            TimeSpan delay;
            try
            {
                var ret = await this.dialect.ReadAsync(this.reader, token).ConfigAwait();
                this.started = true;
                return ret;
            }
            catch (Exception ex) when (OrmLiteRetryExec.ShouldRetryStatement(this.policy, this.dialect, this.dbCmd, ex, retry, out delay))
            {
                DisposeQuietly(this.reader);
            }
            await Task.Delay(delay, token).ConfigAwait();
            await OrmLiteRetryExec.ReopenAsync(this.dbCmd, token).ConfigAwait();
            this.reader = (DbDataReader)await this.dialect.ExecuteReaderAsync(this.runCmd, token).ConfigAwait();
        }
    }

    // The reader of a query that failed, whose connection may have been lost
    private static void DisposeQuietly(DbDataReader reader)
    {
        try
        {
            reader.Dispose();
        }
        catch (Exception e)
        {
            OrmLiteLog.Log.Warn("Failed to close the reader of a query before retrying it", e);
        }
    }

    public override bool NextResult()
    {
        this.started = true;
        return this.reader.NextResult();
    }

    public override Task<bool> NextResultAsync(CancellationToken token)
    {
        this.started = true;
        return this.reader.NextResultAsync(token);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.reader.Dispose();
        }

        base.Dispose(disposing);
    }

    public override void Close() => this.reader.Close();
    public override bool HasRows => this.reader.HasRows;
    public override DataTable? GetSchemaTable() => this.reader.GetSchemaTable();
    public override int Depth => this.reader.Depth;
    public override bool IsClosed => this.reader.IsClosed;
    public override int RecordsAffected => this.reader.RecordsAffected;
    public override int FieldCount => this.reader.FieldCount;
    public override int VisibleFieldCount => this.reader.VisibleFieldCount;
    public override bool GetBoolean(int i) => this.reader.GetBoolean(i);
    public override byte GetByte(int i) => this.reader.GetByte(i);
    public override long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferOffset, int length) =>
        this.reader.GetBytes(i, fieldOffset, buffer, bufferOffset, length);
    public override char GetChar(int i) => this.reader.GetChar(i);
    public override long GetChars(int i, long fieldOffset, char[]? buffer, int bufferOffset, int length) =>
        this.reader.GetChars(i, fieldOffset, buffer, bufferOffset, length);
    public override string GetDataTypeName(int i) => this.reader.GetDataTypeName(i);
    public override DateTime GetDateTime(int i) => this.reader.GetDateTime(i);
    public override decimal GetDecimal(int i) => this.reader.GetDecimal(i);
    public override double GetDouble(int i) => this.reader.GetDouble(i);
    public override Type GetFieldType(int i) => this.reader.GetFieldType(i);
    public override float GetFloat(int i) => this.reader.GetFloat(i);
    public override Guid GetGuid(int i) => this.reader.GetGuid(i);
    public override short GetInt16(int i) => this.reader.GetInt16(i);
    public override int GetInt32(int i) => this.reader.GetInt32(i);
    public override long GetInt64(int i) => this.reader.GetInt64(i);
    public override string GetName(int i) => this.reader.GetName(i);
    public override int GetOrdinal(string name) => this.reader.GetOrdinal(name);
    public override string GetString(int i) => this.reader.GetString(i);
    public override object GetValue(int i) => this.reader.GetValue(i);
    public override int GetValues(object[] values) => this.reader.GetValues(values);
    public override bool IsDBNull(int i) => this.reader.IsDBNull(i);
    public override object this[string name] => this.reader[name];
    public override object this[int i] => this.reader[i];
    public override T GetFieldValue<T>(int i) => this.reader.GetFieldValue<T>(i);
    public override Task<T> GetFieldValueAsync<T>(int i, CancellationToken token) => this.reader.GetFieldValueAsync<T>(i, token);
    public override Task<bool> IsDBNullAsync(int i, CancellationToken token) => this.reader.IsDBNullAsync(i, token);
    public override IEnumerator GetEnumerator() => this.reader.GetEnumerator();
    public override Type GetProviderSpecificFieldType(int i) => this.reader.GetProviderSpecificFieldType(i);
    public override object GetProviderSpecificValue(int i) => this.reader.GetProviderSpecificValue(i);
    public override int GetProviderSpecificValues(object[] values) => this.reader.GetProviderSpecificValues(values);
    public override Stream GetStream(int i) => this.reader.GetStream(i);
    public override TextReader GetTextReader(int i) => this.reader.GetTextReader(i);
    protected override DbDataReader GetDbDataReader(int i) =>
        ((IDataReader)this.reader).GetData(i) as DbDataReader ?? throw new NotSupportedException();
}
