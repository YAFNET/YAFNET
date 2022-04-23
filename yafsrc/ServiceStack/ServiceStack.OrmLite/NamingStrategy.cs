// ***********************************************************************
// <copyright file="NamingStrategy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;

namespace ServiceStack.OrmLite;

using ServiceStack.Text;

/// <summary>
/// Class OrmLiteDefaultNamingStrategy.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
public class OrmLiteDefaultNamingStrategy : OrmLiteNamingStrategyBase { }
/// <summary>
/// Class AliasNamingStrategy.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
public class AliasNamingStrategy : OrmLiteNamingStrategyBase
{
    /// <summary>
    /// The table aliases
    /// </summary>
    public Dictionary<string, string> TableAliases = new();
    /// <summary>
    /// The column aliases
    /// </summary>
    public Dictionary<string, string> ColumnAliases = new();
    /// <summary>
    /// Gets or sets the use naming strategy.
    /// </summary>
    /// <value>The use naming strategy.</value>
    public INamingStrategy UseNamingStrategy { get; set; }

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetTableName(string name)
    {
        string alias;
        return UseNamingStrategy != null
                   ? UseNamingStrategy.GetTableName(TableAliases.TryGetValue(name, out alias) ? alias : name)
                   : base.GetTableName(TableAliases.TryGetValue(name, out alias) ? alias : name);
    }

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnName(string name)
    {
        string alias;
        return UseNamingStrategy != null
                   ? UseNamingStrategy.GetColumnName(ColumnAliases.TryGetValue(name, out alias) ? alias : name)
                   : base.GetColumnName(ColumnAliases.TryGetValue(name, out alias) ? alias : name);
    }
}

/// <summary>
/// Class LowercaseUnderscoreNamingStrategy.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
public class LowercaseUnderscoreNamingStrategy : OrmLiteNamingStrategyBase
{
    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetTableName(string name)
    {
        return name.ToLowercaseUnderscore();
    }

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnName(string name)
    {
        return name.ToLowercaseUnderscore();
    }
}

/// <summary>
/// Class UpperCaseNamingStrategy.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
public class UpperCaseNamingStrategy : OrmLiteNamingStrategyBase
{
    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetTableName(string name)
    {
        return name.ToUpper();
    }

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnName(string name)
    {
        return name.ToUpper();
    }
}
/// <summary>
/// Class PrefixNamingStrategy.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteNamingStrategyBase" />
public class PrefixNamingStrategy : OrmLiteNamingStrategyBase
{
    /// <summary>
    /// Gets or sets the table prefix.
    /// </summary>
    /// <value>The table prefix.</value>
    public string TablePrefix { get; set; }

    /// <summary>
    /// Gets or sets the column prefix.
    /// </summary>
    /// <value>The column prefix.</value>
    public string ColumnPrefix { get; set; }

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetTableName(string name)
    {
        return TablePrefix + name;
    }

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnName(string name)
    {
        return ColumnPrefix + name;
    }

}