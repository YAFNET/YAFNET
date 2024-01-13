// ***********************************************************************
// <copyright file="DbString.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// This class represents a SQL string, it can be used if you need to denote your parameter is a Char vs VarChar vs nVarChar vs nChar
/// </summary>
public sealed class DbString : SqlMapper.ICustomQueryParameter
{
    /// <summary>
    /// Default value for IsAnsi.
    /// </summary>
    /// <value><c>true</c> if this instance is ANSI default; otherwise, <c>false</c>.</value>
    public static bool IsAnsiDefault { get; set; }

    /// <summary>
    /// A value to set the default value of strings
    /// going through Dapper. Default is 4000, any value larger than this
    /// field will not have the default value applied.
    /// </summary>
    public const int DefaultLength = 4000;

    /// <summary>
    /// Create a new DbString
    /// </summary>
    public DbString()
    {
        this.Length = -1;
        this.IsAnsi = IsAnsiDefault;
    }
    /// <summary>
    /// Ansi vs Unicode
    /// </summary>
    /// <value><c>true</c> if this instance is ANSI; otherwise, <c>false</c>.</value>
    public bool IsAnsi { get; set; }
    /// <summary>
    /// Fixed length
    /// </summary>
    /// <value><c>true</c> if this instance is fixed length; otherwise, <c>false</c>.</value>
    public bool IsFixedLength { get; set; }
    /// <summary>
    /// Length of the string -1 for max
    /// </summary>
    /// <value>The length.</value>
    public int Length { get; set; }
    /// <summary>
    /// The value of the string
    /// </summary>
    /// <value>The value.</value>
    public string Value { get; set; }
    /// <summary>
    /// Add the parameter to the command... internal use only
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="name">The name.</param>
    /// <exception cref="InvalidOperationException">If specifying IsFixedLength,  a Length must also be specified</exception>
    /// <exception cref="System.InvalidOperationException">If specifying IsFixedLength,  a Length must also be specified</exception>
    public void AddParameter(IDbCommand command, string name)
    {
        if (this.IsFixedLength && this.Length == -1)
        {
            throw new InvalidOperationException("If specifying IsFixedLength,  a Length must also be specified");
        }
        var add = !command.Parameters.Contains(name);
        IDbDataParameter param;
        if (add)
        {
            param = command.CreateParameter();
            param.ParameterName = name;
        }
        else
        {
            param = (IDbDataParameter)command.Parameters[name];
        }
#pragma warning disable 0618
        param.Value = SqlMapper.SanitizeParameterValue(this.Value);
#pragma warning restore 0618
        if (this.Length == -1 && this.Value is {Length: <= DefaultLength})
        {
            param.Size = DefaultLength;
        }
        else
        {
            param.Size = this.Length;
        }
        param.DbType = this.IsAnsi ? this.IsFixedLength ? DbType.AnsiStringFixedLength : DbType.AnsiString : this.IsFixedLength ? DbType.StringFixedLength : DbType.String;
        if (add)
        {
            command.Parameters.Add(param);
        }
    }
}