// ***********************************************************************
// <copyright file="SqlMapper.DapperTable.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class SqlMapper.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// Class DapperTable. This class cannot be inherited.
    /// </summary>
    private sealed class DapperTable
    {
        /// <summary>
        /// The field names
        /// </summary>
        private string[] fieldNames;
        /// <summary>
        /// The field name lookup
        /// </summary>
        private readonly Dictionary<string, int> fieldNameLookup;

        /// <summary>
        /// Gets the field names.
        /// </summary>
        /// <value>The field names.</value>
        internal string[] FieldNames => fieldNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="DapperTable"/> class.
        /// </summary>
        /// <param name="fieldNames">The field names.</param>
        /// <exception cref="System.ArgumentNullException">fieldNames</exception>
        public DapperTable(string[] fieldNames)
        {
            this.fieldNames = fieldNames ?? throw new ArgumentNullException(nameof(fieldNames));

            fieldNameLookup = new Dictionary<string, int>(fieldNames.Length, StringComparer.Ordinal);
            // if there are dups, we want the **first** key to be the "winner" - so iterate backwards
            for (int i = fieldNames.Length - 1; i >= 0; i--)
            {
                string key = fieldNames[i];
                if (key != null) fieldNameLookup[key] = i;
            }
        }

        /// <summary>
        /// Indexes the name of the of.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        internal int IndexOfName(string name)
        {
            return name != null && fieldNameLookup.TryGetValue(name, out int result) ? result : -1;
        }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        /// <exception cref="System.InvalidOperationException">Field already exists: " + name</exception>
        internal int AddField(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (fieldNameLookup.ContainsKey(name)) throw new InvalidOperationException("Field already exists: " + name);
            int oldLen = fieldNames.Length;
            Array.Resize(ref fieldNames, oldLen + 1); // yes, this is sub-optimal, but this is not the expected common case
            fieldNames[oldLen] = name;
            fieldNameLookup[name] = oldLen;
            return oldLen;
        }

        /// <summary>
        /// Fields the exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool FieldExists(string key) => key != null && fieldNameLookup.ContainsKey(key);

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>The field count.</value>
        public int FieldCount => fieldNames.Length;
    }
}