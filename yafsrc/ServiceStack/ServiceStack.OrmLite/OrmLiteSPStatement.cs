// ***********************************************************************
// <copyright file="OrmLiteSPStatement.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteSPStatement.
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="System.IDisposable" />
public class OrmLiteSPStatement : IDisposable
{
    /// <summary>
    /// The database
    /// </summary>
    private readonly IDbConnection db;
    /// <summary>
    /// The database command
    /// </summary>
    private readonly IDbCommand dbCmd;
    /// <summary>
    /// The dialect provider
    /// </summary>
    private readonly IOrmLiteDialectProvider dialectProvider;

    /// <summary>
    /// Tries the get parameter value.
    /// </summary>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetParameterValue(string parameterName, out object value)
    {
        try
        {
            value = ((IDataParameter)dbCmd.Parameters[parameterName]).Value;
            return true;
        }
        catch (Exception)
        {
            value = null;
            return false;
        }
    }

    /// <summary>
    /// Gets the return value.
    /// </summary>
    /// <value>The return value.</value>
    public int ReturnValue
    {
        get
        {
            var returnValue = ((IDataParameter)dbCmd.Parameters["__ReturnValue"]).Value;
            return (int)returnValue;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteSPStatement"/> class.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    public OrmLiteSPStatement(IDbCommand dbCmd)
        : this(null, dbCmd) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteSPStatement"/> class.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="dbCmd">The database command.</param>
    public OrmLiteSPStatement(IDbConnection db, IDbCommand dbCmd)
    {
        this.db = db;
        this.dbCmd = dbCmd;
        dialectProvider = dbCmd.GetDialectProvider();
    }

    /// <summary>
    /// Converts to list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>List&lt;T&gt;.</returns>
    /// <exception cref="System.Exception">Type " + typeof(T).Name + " is a primitive type. Use ConvertScalarToList function.</exception>
    public List<T> ConvertToList<T>()
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            throw new Exception("Type " + typeof(T).Name + " is a primitive type. Use ConvertScalarToList function.");

        IDataReader reader = null;
        try
        {
            reader = dbCmd.ExecuteReader();
            return reader.ConvertToList<T>(dialectProvider);
        }
        finally
        {
            reader?.Close();
        }
    }

    /// <summary>
    /// Converts to scalar list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>List&lt;T&gt;.</returns>
    /// <exception cref="System.Exception">Type " + typeof(T).Name + " is a non primitive type. Use ConvertToList function.</exception>
    public List<T> ConvertToScalarList<T>()
    {
        if (!(typeof(T).IsPrimitive || typeof(T).IsValueType || typeof(T) == typeof(string) || typeof(T) == typeof(String)))
            throw new Exception("Type " + typeof(T).Name + " is a non primitive type. Use ConvertToList function.");

        IDataReader reader = null;
        try
        {
            reader = dbCmd.ExecuteReader();
            return reader.Column<T>(dialectProvider);
        }
        finally
        {
            reader?.Close();
        }
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    /// <exception cref="System.Exception">Type " + typeof(T).Name + " is a primitive type. Use ConvertScalarTo function.</exception>
    public T ConvertTo<T>()
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            throw new Exception("Type " + typeof(T).Name + " is a primitive type. Use ConvertScalarTo function.");

        IDataReader reader = null;
        try
        {
            reader = dbCmd.ExecuteReader();
            return reader.ConvertTo<T>(dialectProvider);
        }
        finally
        {
            reader?.Close();
        }
    }

    /// <summary>
    /// Converts to scalar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    /// <exception cref="System.Exception">Type " + typeof(T).Name + " is a non primitive type. Use ConvertTo function.</exception>
    public T ConvertToScalar<T>()
    {
        if (!(typeof(T).IsPrimitive || typeof(T).IsValueType || typeof(T) == typeof(string) || typeof(T) == typeof(String)))
            throw new Exception("Type " + typeof(T).Name + " is a non primitive type. Use ConvertTo function.");

        IDataReader reader = null;
        try
        {
            reader = dbCmd.ExecuteReader();
            return reader.Scalar<T>(dialectProvider);
        }
        finally
        {
            reader?.Close();
        }
    }

    /// <summary>
    /// Converts the first column to list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>List&lt;T&gt;.</returns>
    /// <exception cref="System.Exception">Type " + typeof(T).Name + " is a non primitive type. Only primitive type can be used.</exception>
    public List<T> ConvertFirstColumnToList<T>()
    {
        if (!(typeof(T).IsPrimitive || typeof(T).IsValueType || typeof(T) == typeof(string) || typeof(T) == typeof(String)))
            throw new Exception("Type " + typeof(T).Name + " is a non primitive type. Only primitive type can be used.");

        IDataReader reader = null;
        try
        {
            reader = dbCmd.ExecuteReader();
            return reader.Column<T>(dialectProvider);
        }
        finally
        {
            reader?.Close();
        }
    }

    /// <summary>
    /// Converts the first column to list distinct.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>HashSet&lt;T&gt;.</returns>
    /// <exception cref="System.Exception">Type " + typeof(T).Name + " is a non primitive type. Only primitive type can be used.</exception>
    public HashSet<T> ConvertFirstColumnToListDistinct<T>()
    {
        if (!typeof(T).IsPrimitive || typeof(T).IsValueType || typeof(T) == typeof(string) || typeof(T) == typeof(String))
            throw new Exception("Type " + typeof(T).Name + " is a non primitive type. Only primitive type can be used.");

        IDataReader reader = null;
        try
        {
            reader = dbCmd.ExecuteReader();
            return reader.ColumnDistinct<T>(dialectProvider);
        }
        finally
        {
            reader?.Close();
        }
    }

    /// <summary>
    /// Executes the non query.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public int ExecuteNonQuery()
    {
        return dbCmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Determines whether this instance has result.
    /// </summary>
    /// <returns><c>true</c> if this instance has result; otherwise, <c>false</c>.</returns>
    public bool HasResult()
    {
        IDataReader reader = null;
        try
        {
            reader = dbCmd.ExecuteReader();
            return reader.Read();
        }
        finally
        {
            reader?.Close();
        }
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        dialectProvider.GetExecFilter().DisposeCommand(this.dbCmd, this.db);
    }
}