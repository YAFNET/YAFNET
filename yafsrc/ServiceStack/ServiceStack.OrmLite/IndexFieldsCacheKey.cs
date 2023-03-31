// ***********************************************************************
// <copyright file="IndexFieldsCacheKey.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class IndexFieldsCacheKey.
/// </summary>
public class IndexFieldsCacheKey
{
    /// <summary>
    /// The hash code
    /// </summary>
    readonly int hashCode;

    /// <summary>
    /// Gets the model definition.
    /// </summary>
    /// <value>The model definition.</value>
    public ModelDefinition ModelDefinition { get; private set; }

    /// <summary>
    /// Gets the dialect.
    /// </summary>
    /// <value>The dialect.</value>
    public IOrmLiteDialectProvider Dialect { get; private set; }

    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>The fields.</value>
    public List<string> Fields { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexFieldsCacheKey"/> class.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="modelDefinition">The model definition.</param>
    /// <param name="dialect">The dialect.</param>
    public IndexFieldsCacheKey(IDataReader reader, ModelDefinition modelDefinition, IOrmLiteDialectProvider dialect)
    {
        ModelDefinition = modelDefinition;
        Dialect = dialect;

        int startPos = 0;
        int endPos = reader.FieldCount;

        Fields = new List<string>(endPos - startPos);

        for (int i = startPos; i < endPos; i++)
            Fields.Add(reader.GetName(i));

        unchecked
        {
            hashCode = 17;
            hashCode = hashCode * 23 + ModelDefinition.GetHashCode();
            hashCode = hashCode * 23 + Dialect.GetHashCode();
            hashCode = hashCode * 23 + Fields.Count;
            for (int i = 0; i < Fields.Count; i++)
                hashCode = hashCode * 23 + Fields[i].Length;
        }
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        var that = obj as IndexFieldsCacheKey;

        if (obj == null) return false;

        return this.ModelDefinition == that.ModelDefinition
               && this.Dialect == that.Dialect
               && this.Fields.Count == that.Fields.Count
               && this.Fields.SequenceEqual(that.Fields);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return hashCode;
    }
}