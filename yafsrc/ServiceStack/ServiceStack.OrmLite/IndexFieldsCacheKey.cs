// ***********************************************************************
// <copyright file="IndexFieldsCacheKey.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite;

/// <summary>
/// Class IndexFieldsCacheKey.
/// </summary>
public class IndexFieldsCacheKey
{
    /// <summary>
    /// The hash code
    /// </summary>
    private readonly int hashCode;

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
    public string Fields { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexFieldsCacheKey"/> class.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <param name="modelDefinition">The model definition.</param>
    /// <param name="dialect">The dialect.</param>
    public IndexFieldsCacheKey(string fields, ModelDefinition modelDefinition, IOrmLiteDialectProvider dialect)
    {
        this.Fields = fields;
        this.ModelDefinition = modelDefinition;
        this.Dialect = dialect;

        unchecked
        {
            this.hashCode = 17;
            this.hashCode = this.hashCode * 23 + this.ModelDefinition.GetHashCode();
            this.hashCode = this.hashCode * 23 + this.Dialect.GetHashCode();
            hashCode = hashCode * 23 + Fields.GetHashCode();
        }
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        var that = obj as IndexFieldsCacheKey;

        if (obj == null)
        {
            return false;
        }

        return this.ModelDefinition == that.ModelDefinition
               && this.Dialect == that.Dialect
               && this.Fields == that.Fields;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return this.hashCode;
    }
}