// ***********************************************************************
// <copyright file="WrappedReader.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class DisposedReader. This class cannot be inherited.
/// Implements the <see cref="System.Data.Common.DbDataReader" />
/// </summary>
/// <seealso cref="System.Data.Common.DbDataReader" />
internal sealed class DisposedReader : DbDataReader
{
    /// <summary>
    /// The instance
    /// </summary>
    readonly static internal DisposedReader Instance = new DisposedReader();
    /// <summary>
    /// Prevents a default instance of the <see cref="DisposedReader" /> class from being created.
    /// </summary>
    private DisposedReader() { }
    /// <summary>
    /// Gets a value indicating the depth of nesting for the current row.
    /// </summary>
    /// <value>The depth.</value>
    public override int Depth => 0;
    /// <summary>
    /// Gets the number of columns in the current row.
    /// </summary>
    /// <value>The field count.</value>
    public override int FieldCount => 0;
    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader" /> is closed.
    /// </summary>
    /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
    public override bool IsClosed => true;
    /// <summary>
    /// Gets a value that indicates whether this <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows.
    /// </summary>
    /// <value><c>true</c> if this instance has rows; otherwise, <c>false</c>.</value>
    public override bool HasRows => false;
    /// <summary>
    /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
    /// </summary>
    /// <value>The records affected.</value>
    public override int RecordsAffected => -1;
    /// <summary>
    /// Gets the number of fields in the <see cref="T:System.Data.Common.DbDataReader" /> that are not hidden.
    /// </summary>
    /// <value>The visible field count.</value>
    public override int VisibleFieldCount => 0;

    /// <summary>
    /// Throws the disposed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    /// <exception cref="System.ObjectDisposedException">DbDataReader</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static T ThrowDisposed<T>()
    {
        throw new ObjectDisposedException(nameof(DbDataReader));
    }

    /// <summary>
    /// Throw disposed as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private async static Task<T> ThrowDisposedAsync<T>()
    {
        var result = ThrowDisposed<T>();
        await Task.Yield(); // will never hit this - already thrown and handled
        return result;
    }
    /// <summary>
    /// Closes the <see cref="T:System.Data.Common.DbDataReader" /> object.
    /// </summary>
    public override void Close() { }
    /// <summary>
    /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.Common.DbDataReader" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
    public override DataTable GetSchemaTable()
    {
        return ThrowDisposed<DataTable>();
    }

    /// <summary>
    /// Obtains a lifetime service object to control the lifetime policy for this instance.
    /// </summary>
    /// <returns>An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease" /> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" /> property.</returns>
    [Obsolete]
    public override object InitializeLifetimeService()
    {
        return ThrowDisposed<object>();
    }

    /// <summary>
    /// Releases the managed resources used by the <see cref="T:System.Data.Common.DbDataReader" /> and optionally releases the unmanaged resources.
    /// </summary>
    /// <param name="disposing"><see langword="true" /> to release managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    override protected void Dispose(bool disposing) { }
    /// <summary>
    /// Gets the value of the specified column as a Boolean.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override bool GetBoolean(int ordinal)
    {
        return ThrowDisposed<bool>();
    }

    /// <summary>
    /// Reads a stream of bytes from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by <paramref name="bufferOffset" />.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
    /// <param name="buffer">The buffer into which to copy the data.</param>
    /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
    /// <param name="length">The maximum number of characters to read.</param>
    /// <returns>The actual number of bytes read.</returns>
    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
    {
        return ThrowDisposed<long>();
    }

    /// <summary>
    /// Gets the value of the specified column as a single-precision floating point number.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override float GetFloat(int ordinal)
    {
        return ThrowDisposed<float>();
    }

    /// <summary>
    /// Gets the value of the specified column as a 16-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override short GetInt16(int ordinal)
    {
        return ThrowDisposed<short>();
    }

    /// <summary>
    /// Gets the value of the specified column as a byte.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override byte GetByte(int ordinal)
    {
        return ThrowDisposed<byte>();
    }

    /// <summary>
    /// Gets the value of the specified column as a single character.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override char GetChar(int ordinal)
    {
        return ThrowDisposed<char>();
    }

    /// <summary>
    /// Reads a stream of characters from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by <paramref name="bufferOffset" />.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
    /// <param name="buffer">The buffer into which to copy the data.</param>
    /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
    /// <param name="length">The maximum number of characters to read.</param>
    /// <returns>The actual number of characters read.</returns>
    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
    {
        return ThrowDisposed<long>();
    }

    /// <summary>
    /// Gets name of the data type of the specified column.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>A string representing the name of the data type.</returns>
    public override string GetDataTypeName(int ordinal)
    {
        return ThrowDisposed<string>();
    }

    /// <summary>
    /// Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override DateTime GetDateTime(int ordinal)
    {
        return ThrowDisposed<DateTime>();
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Common.DbDataReader" /> object for the requested column ordinal that can be overridden with a provider-specific implementation.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>A <see cref="T:System.Data.Common.DbDataReader" /> object.</returns>
    override protected DbDataReader GetDbDataReader(int ordinal)
    {
        return ThrowDisposed<DbDataReader>();
    }

    /// <summary>
    /// Gets the value of the specified column as a <see cref="T:System.Decimal" /> object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override decimal GetDecimal(int ordinal)
    {
        return ThrowDisposed<decimal>();
    }

    /// <summary>
    /// Gets the value of the specified column as a double-precision floating point number.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override double GetDouble(int ordinal)
    {
        return ThrowDisposed<double>();
    }

    /// <summary>
    /// Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
    public override IEnumerator GetEnumerator()
    {
        return ThrowDisposed<IEnumerator>();
    }

    /// <summary>
    /// Gets the data type of the specified column.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The data type of the specified column.</returns>
    public override Type GetFieldType(int ordinal)
    {
        return ThrowDisposed<Type>();
    }

    /// <summary>
    /// Synchronously gets the value of the specified column as a type.
    /// </summary>
    /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
    /// <param name="ordinal">The column to be retrieved.</param>
    /// <returns>The column to be retrieved.</returns>
    public override T GetFieldValue<T>(int ordinal)
    {
        return ThrowDisposed<T>();
    }

    /// <summary>
    /// Asynchronously gets the value of the specified column as a type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <param name="ordinal">The type of the value to be returned.</param>
    /// <param name="cancellationToken">The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of <see langword="CancellationToken.None" /> makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.GetFieldValueAsync``1(System.Int32)" />. The returned task must be marked as cancelled.</param>
    /// <returns>The type of the value to be returned.</returns>
    public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
    {
        return ThrowDisposedAsync<T>();
    }

    /// <summary>
    /// Gets the value of the specified column as a globally-unique identifier (GUID).
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override Guid GetGuid(int ordinal)
    {
        return ThrowDisposed<Guid>();
    }

    /// <summary>
    /// Gets the value of the specified column as a 32-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override int GetInt32(int ordinal)
    {
        return ThrowDisposed<int>();
    }

    /// <summary>
    /// Gets the value of the specified column as a 64-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override long GetInt64(int ordinal)
    {
        return ThrowDisposed<long>();
    }

    /// <summary>
    /// Gets the name of the column, given the zero-based column ordinal.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The name of the specified column.</returns>
    public override string GetName(int ordinal)
    {
        return ThrowDisposed<string>();
    }

    /// <summary>
    /// Gets the column ordinal given the name of the column.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The zero-based column ordinal.</returns>
    public override int GetOrdinal(string name)
    {
        return ThrowDisposed<int>();
    }

    /// <summary>
    /// Returns the provider-specific field type of the specified column.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The <see cref="T:System.Type" /> object that describes the data type of the specified column.</returns>
    public override Type GetProviderSpecificFieldType(int ordinal)
    {
        return ThrowDisposed<Type>();
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override object GetProviderSpecificValue(int ordinal)
    {
        return ThrowDisposed<object>();
    }

    /// <summary>
    /// Gets all provider-specific attribute columns in the collection for the current row.
    /// </summary>
    /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
    /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
    public override int GetProviderSpecificValues(object[] values)
    {
        return ThrowDisposed<int>();
    }

    /// <summary>
    /// Retrieves data as a <see cref="T:System.IO.Stream" />.
    /// </summary>
    /// <param name="ordinal">Retrieves data as a <see cref="T:System.IO.Stream" />.</param>
    /// <returns>The returned object.</returns>
    public override Stream GetStream(int ordinal)
    {
        return ThrowDisposed<Stream>();
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.String" />.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override string GetString(int ordinal)
    {
        return ThrowDisposed<string>();
    }

    /// <summary>
    /// Retrieves data as a <see cref="T:System.IO.TextReader" />.
    /// </summary>
    /// <param name="ordinal">Retrieves data as a <see cref="T:System.IO.TextReader" />.</param>
    /// <returns>The returned object.</returns>
    public override TextReader GetTextReader(int ordinal)
    {
        return ThrowDisposed<TextReader>();
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override object GetValue(int ordinal)
    {
        return ThrowDisposed<object>();
    }

    /// <summary>
    /// Populates an array of objects with the column values of the current row.
    /// </summary>
    /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
    /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
    public override int GetValues(object[] values)
    {
        return ThrowDisposed<int>();
    }

    /// <summary>
    /// Gets a value that indicates whether the column contains nonexistent or missing values.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns><see langword="true" /> if the specified column is equivalent to <see cref="T:System.DBNull" />; otherwise <see langword="false" />.</returns>
    public override bool IsDBNull(int ordinal)
    {
        return ThrowDisposed<bool>();
    }

    /// <summary>
    /// An asynchronous version of <see cref="M:System.Data.Common.DbDataReader.IsDBNull(System.Int32)" />, which gets a value that indicates whether the column contains non-existent or missing values. Optionally, sends a notification that operations should be cancelled.
    /// </summary>
    /// <param name="ordinal">The zero-based column to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of <see langword="CancellationToken.None" /> makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.IsDBNullAsync(System.Int32)" />. The returned task must be marked as cancelled.</param>
    /// <returns><see langword="true" /> if the specified column value is equivalent to <see langword="DBNull" /> otherwise <see langword="false" />.</returns>
    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
    {
        return ThrowDisposedAsync<bool>();
    }

    /// <summary>
    /// Advances the reader to the next result when reading the results of a batch of statements.
    /// </summary>
    /// <returns><see langword="true" /> if there are more result sets; otherwise <see langword="false" />.</returns>
    public override bool NextResult()
    {
        return ThrowDisposed<bool>();
    }

    /// <summary>
    /// Advances the reader to the next record in a result set.
    /// </summary>
    /// <returns><see langword="true" /> if there are more rows; otherwise <see langword="false" />.</returns>
    public override bool Read()
    {
        return ThrowDisposed<bool>();
    }

    /// <summary>
    /// This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.NextResult" />. Providers should override with an appropriate implementation. The <paramref name="cancellationToken" /> may optionally be ignored.
    /// The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.NextResult" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled <paramref name="cancellationToken" />. Exceptions thrown by <see cref="M:System.Data.Common.DbDataReader.NextResult" /> will be communicated via the returned Task Exception property.
    /// Other methods and properties of the DbDataReader object should not be invoked while the returned Task is not yet completed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
        return ThrowDisposedAsync<bool>();
    }

    /// <summary>
    /// This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.Read" />.  Providers should override with an appropriate implementation. The cancellationToken may optionally be ignored.
    /// The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.Read" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled cancellationToken.  Exceptions thrown by Read will be communicated via the returned Task Exception property.
    /// Do not invoke other methods and properties of the <see langword="DbDataReader" /> object until the returned Task is complete.
    /// </summary>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        return ThrowDisposedAsync<bool>();
    }

    /// <summary>
    /// Gets the <see cref="object" /> with the specified ordinal.
    /// </summary>
    /// <param name="ordinal">The ordinal.</param>
    /// <returns>System.Object.</returns>
    public override object this[int ordinal] => ThrowDisposed<object>();
    /// <summary>
    /// Gets the <see cref="object" /> with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.Object.</returns>
    public override object this[string name] => ThrowDisposed<object>();
}

/// <summary>
/// Class WrappedReader.
/// </summary>
static internal class WrappedReader
{
    // the purpose of wrapping here is to allow closing a reader to *also* close
    // the command, without having to explicitly hand the command back to the
    // caller; what that actually looks like depends on what we get: if we are
    // given a DbDataReader, we will surface a DbDataReader; if we are given
    // a raw IDataReader, we will surface that; and if null: null
    /// <summary>
    /// Creates the specified command.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="reader">The reader.</param>
    /// <returns>IDataReader.</returns>
    public static IDataReader Create(IDbCommand cmd, IDataReader reader)
    {
        if (cmd == null)
        {
            return reader; // no need to wrap if no command
        }

        if (reader is DbDataReader dbr)
        {
            return new DbWrappedReader(cmd, dbr);
        }

        if (reader != null)
        {
            return new BasicWrappedReader(cmd, reader);
        }

        cmd.Dispose();
        return null; // GIGO
    }
    /// <summary>
    /// Creates the specified command.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="reader">The reader.</param>
    /// <returns>DbDataReader.</returns>
    public static DbDataReader Create(IDbCommand cmd, DbDataReader reader)
    {
        if (cmd == null)
        {
            return reader; // no need to wrap if no command
        }

        if (reader != null)
        {
            return new DbWrappedReader(cmd, reader);
        }

        cmd.Dispose();
        return null; // GIGO
    }
}
/// <summary>
/// Class DbWrappedReader. This class cannot be inherited.
/// Implements the <see cref="System.Data.Common.DbDataReader" />
/// Implements the <see cref="ServiceStack.OrmLite.Dapper.IWrappedDataReader" />
/// </summary>
/// <seealso cref="System.Data.Common.DbDataReader" />
/// <seealso cref="ServiceStack.OrmLite.Dapper.IWrappedDataReader" />
internal sealed class DbWrappedReader : DbDataReader, IWrappedDataReader
{
    /// <summary>
    /// The reader
    /// </summary>
    private DbDataReader _reader;
    /// <summary>
    /// The command
    /// </summary>
    private IDbCommand _cmd;

    /// <summary>
    /// Obtain the underlying reader
    /// </summary>
    /// <value>The reader.</value>
    IDataReader IWrappedDataReader.Reader => this._reader;

    /// <summary>
    /// Obtain the underlying command
    /// </summary>
    /// <value>The command.</value>
    IDbCommand IWrappedDataReader.Command => this._cmd;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbWrappedReader" /> class.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="reader">The reader.</param>
    public DbWrappedReader(IDbCommand cmd, DbDataReader reader)
    {
        this._cmd = cmd;
        this._reader = reader;
    }

    /// <summary>
    /// Gets a value that indicates whether this <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows.
    /// </summary>
    /// <value><c>true</c> if this instance has rows; otherwise, <c>false</c>.</value>
    public override bool HasRows => this._reader.HasRows;

    /// <summary>
    /// Closes the <see cref="T:System.Data.Common.DbDataReader" /> object.
    /// </summary>
    public override void Close()
    {
        this._reader.Close();
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.Common.DbDataReader" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
    public override DataTable GetSchemaTable()
    {
        return this._reader.GetSchemaTable();
    }

    /// <summary>
    /// Obtains a lifetime service object to control the lifetime policy for this instance.
    /// </summary>
    /// <returns>An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease" /> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" /> property.</returns>
    [Obsolete] 
    public override object InitializeLifetimeService()
    {
        return this._reader.InitializeLifetimeService();
    }

    /// <summary>
    /// Gets a value indicating the depth of nesting for the current row.
    /// </summary>
    /// <value>The depth.</value>
    public override int Depth => this._reader.Depth;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader" /> is closed.
    /// </summary>
    /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
    public override bool IsClosed => this._reader.IsClosed;

    /// <summary>
    /// Advances the reader to the next result when reading the results of a batch of statements.
    /// </summary>
    /// <returns><see langword="true" /> if there are more result sets; otherwise <see langword="false" />.</returns>
    public override bool NextResult()
    {
        return this._reader.NextResult();
    }

    /// <summary>
    /// Advances the reader to the next record in a result set.
    /// </summary>
    /// <returns><see langword="true" /> if there are more rows; otherwise <see langword="false" />.</returns>
    public override bool Read()
    {
        return this._reader.Read();
    }

    /// <summary>
    /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
    /// </summary>
    /// <value>The records affected.</value>
    public override int RecordsAffected => this._reader.RecordsAffected;

    /// <summary>
    /// Releases the managed resources used by the <see cref="T:System.Data.Common.DbDataReader" /> and optionally releases the unmanaged resources.
    /// </summary>
    /// <param name="disposing"><see langword="true" /> to release managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    override protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._reader.Close();
            this._reader.Dispose();
            this._reader = DisposedReader.Instance; // all future ops are no-ops
            this._cmd?.Dispose();
            this._cmd = null;
        }
    }

    /// <summary>
    /// Gets the number of columns in the current row.
    /// </summary>
    /// <value>The field count.</value>
    public override int FieldCount => this._reader.FieldCount;

    /// <summary>
    /// Gets the value of the specified column as a Boolean.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override bool GetBoolean(int i)
    {
        return this._reader.GetBoolean(i);
    }

    /// <summary>
    /// Gets the 8-bit unsigned integer value of the specified column.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The 8-bit unsigned integer value of the specified column.</returns>
    public override byte GetByte(int i)
    {
        return this._reader.GetByte(i);
    }

    /// <summary>
    /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
    /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
    /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The actual number of bytes read.</returns>
    public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
        return this._reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
    }

    /// <summary>
    /// Gets the character value of the specified column.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The character value of the specified column.</returns>
    public override char GetChar(int i)
    {
        return this._reader.GetChar(i);
    }

    /// <summary>
    /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
    /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
    /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The actual number of characters read.</returns>
    public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
        return this._reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
    }

    /// <summary>
    /// Gets the data type information for the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The data type information for the specified field.</returns>
    public override string GetDataTypeName(int i)
    {
        return this._reader.GetDataTypeName(i);
    }

    /// <summary>
    /// Gets the date and time data value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The date and time data value of the specified field.</returns>
    public override DateTime GetDateTime(int i)
    {
        return this._reader.GetDateTime(i);
    }

    /// <summary>
    /// Gets the fixed-position numeric value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The fixed-position numeric value of the specified field.</returns>
    public override decimal GetDecimal(int i)
    {
        return this._reader.GetDecimal(i);
    }

    /// <summary>
    /// Gets the double-precision floating point number of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The double-precision floating point number of the specified field.</returns>
    public override double GetDouble(int i)
    {
        return this._reader.GetDouble(i);
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.</returns>
    public override Type GetFieldType(int i)
    {
        return this._reader.GetFieldType(i);
    }

    /// <summary>
    /// Gets the single-precision floating point number of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The single-precision floating point number of the specified field.</returns>
    public override float GetFloat(int i)
    {
        return this._reader.GetFloat(i);
    }

    /// <summary>
    /// Returns the GUID value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The GUID value of the specified field.</returns>
    public override Guid GetGuid(int i)
    {
        return this._reader.GetGuid(i);
    }

    /// <summary>
    /// Gets the 16-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The 16-bit signed integer value of the specified field.</returns>
    public override short GetInt16(int i)
    {
        return this._reader.GetInt16(i);
    }

    /// <summary>
    /// Gets the 32-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The 32-bit signed integer value of the specified field.</returns>
    public override int GetInt32(int i)
    {
        return this._reader.GetInt32(i);
    }

    /// <summary>
    /// Gets the 64-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The 64-bit signed integer value of the specified field.</returns>
    public override long GetInt64(int i)
    {
        return this._reader.GetInt64(i);
    }

    /// <summary>
    /// Gets the name for the field to find.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The name of the field or the empty string (""), if there is no value to return.</returns>
    public override string GetName(int i)
    {
        return this._reader.GetName(i);
    }

    /// <summary>
    /// Gets the column ordinal given the name of the column.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The zero-based column ordinal.</returns>
    public override int GetOrdinal(string name)
    {
        return this._reader.GetOrdinal(name);
    }

    /// <summary>
    /// Gets the string value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The string value of the specified field.</returns>
    public override string GetString(int i)
    {
        return this._reader.GetString(i);
    }

    /// <summary>
    /// Return the value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The <see cref="T:System.Object" /> which will contain the field value upon return.</returns>
    public override object GetValue(int i)
    {
        return this._reader.GetValue(i);
    }

    /// <summary>
    /// Populates an array of objects with the column values of the current row.
    /// </summary>
    /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
    /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
    public override int GetValues(object[] values)
    {
        return this._reader.GetValues(values);
    }

    /// <summary>
    /// Return whether the specified field is set to null.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns><see langword="true" /> if the specified field is set to null; otherwise, <see langword="false" />.</returns>
    public override bool IsDBNull(int i)
    {
        return this._reader.IsDBNull(i);
    }

    /// <summary>
    /// Gets the <see cref="object" /> with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.Object.</returns>
    public override object this[string name] => this._reader[name];

    /// <summary>
    /// Gets the <see cref="object" /> with the specified i.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <returns>System.Object.</returns>
    public override object this[int i] => this._reader[i];

    /// <summary>
    /// Synchronously gets the value of the specified column as a type.
    /// </summary>
    /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
    /// <param name="ordinal">The column to be retrieved.</param>
    /// <returns>The column to be retrieved.</returns>
    public override T GetFieldValue<T>(int ordinal)
    {
        return this._reader.GetFieldValue<T>(ordinal);
    }

    /// <summary>
    /// Asynchronously gets the value of the specified column as a type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <param name="ordinal">The type of the value to be returned.</param>
    /// <param name="cancellationToken">The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of <see langword="CancellationToken.None" /> makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.GetFieldValueAsync``1(System.Int32)" />. The returned task must be marked as cancelled.</param>
    /// <returns>The type of the value to be returned.</returns>
    public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
    {
        return this._reader.GetFieldValueAsync<T>(ordinal, cancellationToken);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
    public override IEnumerator GetEnumerator()
    {
        return this._reader.GetEnumerator();
    }

    /// <summary>
    /// Returns the provider-specific field type of the specified column.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The <see cref="T:System.Type" /> object that describes the data type of the specified column.</returns>
    public override Type GetProviderSpecificFieldType(int ordinal)
    {
        return this._reader.GetProviderSpecificFieldType(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override object GetProviderSpecificValue(int ordinal)
    {
        return this._reader.GetProviderSpecificValue(ordinal);
    }

    /// <summary>
    /// Gets all provider-specific attribute columns in the collection for the current row.
    /// </summary>
    /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
    /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
    public override int GetProviderSpecificValues(object[] values)
    {
        return this._reader.GetProviderSpecificValues(values);
    }

    /// <summary>
    /// Retrieves data as a <see cref="T:System.IO.Stream" />.
    /// </summary>
    /// <param name="ordinal">Retrieves data as a <see cref="T:System.IO.Stream" />.</param>
    /// <returns>The returned object.</returns>
    public override Stream GetStream(int ordinal)
    {
        return this._reader.GetStream(ordinal);
    }

    /// <summary>
    /// Retrieves data as a <see cref="T:System.IO.TextReader" />.
    /// </summary>
    /// <param name="ordinal">Retrieves data as a <see cref="T:System.IO.TextReader" />.</param>
    /// <returns>The returned object.</returns>
    public override TextReader GetTextReader(int ordinal)
    {
        return this._reader.GetTextReader(ordinal);
    }

    /// <summary>
    /// An asynchronous version of <see cref="M:System.Data.Common.DbDataReader.IsDBNull(System.Int32)" />, which gets a value that indicates whether the column contains non-existent or missing values. Optionally, sends a notification that operations should be cancelled.
    /// </summary>
    /// <param name="ordinal">The zero-based column to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of <see langword="CancellationToken.None" /> makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.IsDBNullAsync(System.Int32)" />. The returned task must be marked as cancelled.</param>
    /// <returns><see langword="true" /> if the specified column value is equivalent to <see langword="DBNull" /> otherwise <see langword="false" />.</returns>
    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
    {
        return this._reader.IsDBNullAsync(ordinal, cancellationToken);
    }

    /// <summary>
    /// This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.NextResult" />. Providers should override with an appropriate implementation. The <paramref name="cancellationToken" /> may optionally be ignored.
    /// The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.NextResult" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled <paramref name="cancellationToken" />. Exceptions thrown by <see cref="M:System.Data.Common.DbDataReader.NextResult" /> will be communicated via the returned Task Exception property.
    /// Other methods and properties of the DbDataReader object should not be invoked while the returned Task is not yet completed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
        return this._reader.NextResultAsync(cancellationToken);
    }

    /// <summary>
    /// This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.Read" />.  Providers should override with an appropriate implementation. The cancellationToken may optionally be ignored.
    /// The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.Read" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled cancellationToken.  Exceptions thrown by Read will be communicated via the returned Task Exception property.
    /// Do not invoke other methods and properties of the <see langword="DbDataReader" /> object until the returned Task is complete.
    /// </summary>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        return this._reader.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the number of fields in the <see cref="T:System.Data.Common.DbDataReader" /> that are not hidden.
    /// </summary>
    /// <value>The visible field count.</value>
    public override int VisibleFieldCount => this._reader.VisibleFieldCount;
    /// <summary>
    /// Returns a <see cref="T:System.Data.Common.DbDataReader" /> object for the requested column ordinal that can be overridden with a provider-specific implementation.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>A <see cref="T:System.Data.Common.DbDataReader" /> object.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    override protected DbDataReader GetDbDataReader(int ordinal)
    {
        return ((IDataReader)this._reader).GetData(ordinal) as DbDataReader ?? throw new NotSupportedException();
    }
}

/// <summary>
/// Class BasicWrappedReader.
/// Implements the <see cref="ServiceStack.OrmLite.Dapper.IWrappedDataReader" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Dapper.IWrappedDataReader" />
internal class BasicWrappedReader : IWrappedDataReader
{
    /// <summary>
    /// The reader
    /// </summary>
    private IDataReader _reader;
    /// <summary>
    /// The command
    /// </summary>
    private IDbCommand _cmd;

    /// <summary>
    /// Obtain the underlying reader
    /// </summary>
    /// <value>The reader.</value>
    IDataReader IWrappedDataReader.Reader => this._reader;

    /// <summary>
    /// Obtain the underlying command
    /// </summary>
    /// <value>The command.</value>
    IDbCommand IWrappedDataReader.Command => this._cmd;

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicWrappedReader" /> class.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="reader">The reader.</param>
    public BasicWrappedReader(IDbCommand cmd, IDataReader reader)
    {
        this._cmd = cmd;
        this._reader = reader;
    }

    /// <summary>
    /// Closes the <see cref="T:System.Data.IDataReader" /> Object.
    /// </summary>
    void IDataReader.Close()
    {
        this._reader.Close();
    }

    /// <summary>
    /// Gets a value indicating the depth of nesting for the current row.
    /// </summary>
    /// <value>The depth.</value>
    int IDataReader.Depth => this._reader.Depth;

    /// <summary>
    /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
    DataTable IDataReader.GetSchemaTable()
    {
        return this._reader.GetSchemaTable();
    }

    /// <summary>
    /// Gets a value indicating whether the data reader is closed.
    /// </summary>
    /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
    bool IDataReader.IsClosed => this._reader.IsClosed;

    /// <summary>
    /// Advances the data reader to the next result, when reading the results of batch SQL statements.
    /// </summary>
    /// <returns><see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
    bool IDataReader.NextResult()
    {
        return this._reader.NextResult();
    }

    /// <summary>
    /// Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
    /// </summary>
    /// <returns><see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
    bool IDataReader.Read()
    {
        return this._reader.Read();
    }

    /// <summary>
    /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
    /// </summary>
    /// <value>The records affected.</value>
    int IDataReader.RecordsAffected => this._reader.RecordsAffected;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    void IDisposable.Dispose()
    {
        this._reader.Close();
        this._reader.Dispose();
        this._reader = DisposedReader.Instance;
        this._cmd?.Dispose();
        this._cmd = null;
    }

    /// <summary>
    /// Gets the number of columns in the current row.
    /// </summary>
    /// <value>The field count.</value>
    int IDataRecord.FieldCount => this._reader.FieldCount;

    /// <summary>
    /// Gets the value of the specified column as a Boolean.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    bool IDataRecord.GetBoolean(int i)
    {
        return this._reader.GetBoolean(i);
    }

    /// <summary>
    /// Gets the 8-bit unsigned integer value of the specified column.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The 8-bit unsigned integer value of the specified column.</returns>
    byte IDataRecord.GetByte(int i)
    {
        return this._reader.GetByte(i);
    }

    /// <summary>
    /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
    /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
    /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The actual number of bytes read.</returns>
    long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
        return this._reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
    }

    /// <summary>
    /// Gets the character value of the specified column.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The character value of the specified column.</returns>
    char IDataRecord.GetChar(int i)
    {
        return this._reader.GetChar(i);
    }

    /// <summary>
    /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
    /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
    /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The actual number of characters read.</returns>
    long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
        return this._reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.</returns>
    IDataReader IDataRecord.GetData(int i)
    {
        return this._reader.GetData(i);
    }

    /// <summary>
    /// Gets the data type information for the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The data type information for the specified field.</returns>
    string IDataRecord.GetDataTypeName(int i)
    {
        return this._reader.GetDataTypeName(i);
    }

    /// <summary>
    /// Gets the date and time data value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The date and time data value of the specified field.</returns>
    DateTime IDataRecord.GetDateTime(int i)
    {
        return this._reader.GetDateTime(i);
    }

    /// <summary>
    /// Gets the fixed-position numeric value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The fixed-position numeric value of the specified field.</returns>
    decimal IDataRecord.GetDecimal(int i)
    {
        return this._reader.GetDecimal(i);
    }

    /// <summary>
    /// Gets the double-precision floating point number of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The double-precision floating point number of the specified field.</returns>
    double IDataRecord.GetDouble(int i)
    {
        return this._reader.GetDouble(i);
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.</returns>
    Type IDataRecord.GetFieldType(int i)
    {
        return this._reader.GetFieldType(i);
    }

    /// <summary>
    /// Gets the single-precision floating point number of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The single-precision floating point number of the specified field.</returns>
    float IDataRecord.GetFloat(int i)
    {
        return this._reader.GetFloat(i);
    }

    /// <summary>
    /// Returns the GUID value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The GUID value of the specified field.</returns>
    Guid IDataRecord.GetGuid(int i)
    {
        return this._reader.GetGuid(i);
    }

    /// <summary>
    /// Gets the 16-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The 16-bit signed integer value of the specified field.</returns>
    short IDataRecord.GetInt16(int i)
    {
        return this._reader.GetInt16(i);
    }

    /// <summary>
    /// Gets the 32-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The 32-bit signed integer value of the specified field.</returns>
    int IDataRecord.GetInt32(int i)
    {
        return this._reader.GetInt32(i);
    }

    /// <summary>
    /// Gets the 64-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The 64-bit signed integer value of the specified field.</returns>
    long IDataRecord.GetInt64(int i)
    {
        return this._reader.GetInt64(i);
    }

    /// <summary>
    /// Gets the name for the field to find.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The name of the field or the empty string (""), if there is no value to return.</returns>
    string IDataRecord.GetName(int i)
    {
        return this._reader.GetName(i);
    }

    /// <summary>
    /// Return the index of the named field.
    /// </summary>
    /// <param name="name">The name of the field to find.</param>
    /// <returns>The index of the named field.</returns>
    int IDataRecord.GetOrdinal(string name)
    {
        return this._reader.GetOrdinal(name);
    }

    /// <summary>
    /// Gets the string value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The string value of the specified field.</returns>
    string IDataRecord.GetString(int i)
    {
        return this._reader.GetString(i);
    }

    /// <summary>
    /// Return the value of the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The <see cref="T:System.Object" /> which will contain the field value upon return.</returns>
    object IDataRecord.GetValue(int i)
    {
        return this._reader.GetValue(i);
    }

    /// <summary>
    /// Populates an array of objects with the column values of the current record.
    /// </summary>
    /// <param name="values">An array of <see cref="T:System.Object" /> to copy the attribute fields into.</param>
    /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
    int IDataRecord.GetValues(object[] values)
    {
        return this._reader.GetValues(values);
    }

    /// <summary>
    /// Return whether the specified field is set to null.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns><see langword="true" /> if the specified field is set to null; otherwise, <see langword="false" />.</returns>
    bool IDataRecord.IsDBNull(int i)
    {
        return this._reader.IsDBNull(i);
    }

    /// <summary>
    /// Gets the <see cref="object" /> with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.Object.</returns>
    object IDataRecord.this[string name] => this._reader[name];

    /// <summary>
    /// Gets the <see cref="object" /> with the specified i.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <returns>System.Object.</returns>
    object IDataRecord.this[int i] => this._reader[i];
}