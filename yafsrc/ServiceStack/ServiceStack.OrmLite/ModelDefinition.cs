// ***********************************************************************
// <copyright file="ModelDefinition.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.DataAnnotations;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class ModelDefinition.
/// </summary>
public class ModelDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelDefinition" /> class.
    /// </summary>
    public ModelDefinition()
    {
        this.FieldDefinitions = [];
        this.IgnoredFieldDefinitions = [];
        this.CompositeIndexes = [];
        this.CompositePrimaryKeys = [];
        this.UniqueConstraints = [];
    }

    /// <summary>
    /// The row version name
    /// </summary>
    public const string RowVersionName = "RowVersion";

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the alias.
    /// </summary>
    /// <value>The alias.</value>
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets the schema.
    /// </summary>
    /// <value>The schema.</value>
    public string Schema { get; set; }

    /// <summary>
    /// Gets or sets the pre create table SQL.
    /// </summary>
    /// <value>The pre create table SQL.</value>
    public string PreCreateTableSql { get; set; }

    /// <summary>
    /// Gets or sets the post create table SQL.
    /// </summary>
    /// <value>The post create table SQL.</value>
    public string PostCreateTableSql { get; set; }

    /// <summary>
    /// Gets or sets the pre drop table SQL.
    /// </summary>
    /// <value>The pre drop table SQL.</value>
    public string PreDropTableSql { get; set; }

    /// <summary>
    /// Gets or sets the post drop table SQL.
    /// </summary>
    /// <value>The post drop table SQL.</value>
    public string PostDropTableSql { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is in schema.
    /// </summary>
    /// <value><c>true</c> if this instance is in schema; otherwise, <c>false</c>.</value>
    public bool IsInSchema => this.Schema != null;

    /// <summary>
    /// Gets a value indicating whether this instance has automatic increment identifier.
    /// </summary>
    /// <value><c>true</c> if this instance has automatic increment identifier; otherwise, <c>false</c>.</value>
    public bool HasAutoIncrementId => this.PrimaryKey is { AutoIncrement: true };

    /// <summary>
    /// Gets a value indicating whether this instance has sequence attribute.
    /// </summary>
    /// <value><c>true</c> if this instance has sequence attribute; otherwise, <c>false</c>.</value>
    public bool HasSequenceAttribute => this.FieldDefinitions.Any(x => !x.Sequence.IsNullOrEmpty());

    /// <summary>
    /// Gets or sets the row version.
    /// </summary>
    /// <value>The row version.</value>
    public FieldDefinition RowVersion { get; set; }

    /// <summary>
    /// Gets the name of the model.
    /// </summary>
    /// <value>The name of the model.</value>
    public string ModelName => this.Alias ?? this.Name;

    /// <summary>
    /// Gets or sets the type of the model.
    /// </summary>
    /// <value>The type of the model.</value>
    public Type ModelType { get; set; }

    /// <summary>
    /// Gets the primary key.
    /// </summary>
    /// <value>The primary key.</value>
    public FieldDefinition PrimaryKey => this.FieldDefinitions.First(x => x.IsPrimaryKey);

    /// <summary>
    /// Gets the primary key.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public object GetPrimaryKey(object instance)
    {
        var pk = this.PrimaryKey;
        return pk != null
                   ? pk.GetValue(instance)
                   : instance.GetId();
    }

    /// <summary>
    /// Gets or sets the field definitions.
    /// </summary>
    /// <value>The field definitions.</value>
    public List<FieldDefinition> FieldDefinitions { get; set; }

    /// <summary>
    /// Gets the field definitions array.
    /// </summary>
    /// <value>The field definitions array.</value>
    public FieldDefinition[] FieldDefinitionsArray { get; private set; }

    /// <summary>
    /// Gets the field definitions with aliases.
    /// </summary>
    /// <value>The field definitions with aliases.</value>
    public FieldDefinition[] FieldDefinitionsWithAliases { get; private set; }

    /// <summary>
    /// Gets or sets the ignored field definitions.
    /// </summary>
    /// <value>The ignored field definitions.</value>
    public List<FieldDefinition> IgnoredFieldDefinitions { get; set; }

    /// <summary>
    /// Gets the ignored field definitions array.
    /// </summary>
    /// <value>The ignored field definitions array.</value>
    public FieldDefinition[] IgnoredFieldDefinitionsArray { get; private set; }

    /// <summary>
    /// Gets all field definitions array.
    /// </summary>
    /// <value>All field definitions array.</value>
    public FieldDefinition[] AllFieldDefinitionsArray { get; private set; }
    /// <summary>
    /// Gets the reference field definitions array.
    /// </summary>
    /// <value>The reference field definitions array.</value>
    public FieldDefinition[] ReferenceFieldDefinitionsArray { get; private set; }
    /// <summary>
    /// Gets the reference field names.
    /// </summary>
    /// <value>The reference field names.</value>
    public HashSet<string> ReferenceFieldNames { get; private set; }

    /// <summary>
    /// The field definition lock
    /// </summary>
    private readonly Lock fieldDefLock = new();
    /// <summary>
    /// The field definition map
    /// </summary>
    private Dictionary<string, FieldDefinition> fieldDefinitionMap;
    /// <summary>
    /// The field name sanitizer
    /// </summary>
    private Func<string, string> fieldNameSanitizer;

    /// <summary>
    /// Gets the automatic identifier fields.
    /// </summary>
    /// <value>The automatic identifier fields.</value>
    public FieldDefinition[] AutoIdFields { get; private set; }

    /// <summary>
    /// Gets the automatic identifier field definitions.
    /// </summary>
    /// <returns>List&lt;FieldDefinition&gt;.</returns>
    public List<FieldDefinition> GetAutoIdFieldDefinitions()
    {
        var to = new List<FieldDefinition>();
        foreach (var fieldDef in this.FieldDefinitionsArray)
        {
            if (fieldDef.AutoId)
            {
                to.Add(fieldDef);
            }
        }
        return to;
    }

    /// <summary>
    /// Gets the ordered field definitions.
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <param name="sanitizeFieldName">Name of the sanitize field.</param>
    /// <returns>FieldDefinition[].</returns>
    /// <exception cref="System.ArgumentNullException">fieldNames</exception>
    public FieldDefinition[] GetOrderedFieldDefinitions(ICollection<string> fieldNames, Func<string, string> sanitizeFieldName = null)
    {
        ArgumentNullException.ThrowIfNull(fieldNames);

        var fieldDefs = new FieldDefinition[fieldNames.Count];

        var i = 0;
        foreach (var fieldName in fieldNames)
        {
            var fieldDef = sanitizeFieldName != null
                               ? this.AssertFieldDefinition(fieldName, sanitizeFieldName)
                               : this.AssertFieldDefinition(fieldName);
            fieldDefs[i++] = fieldDef;
        }

        return fieldDefs;
    }

    /// <summary>
    /// Gets the field definition map.
    /// </summary>
    /// <param name="sanitizeFieldName">Name of the sanitize field.</param>
    /// <returns>Dictionary&lt;System.String, FieldDefinition&gt;.</returns>
    public Dictionary<string, FieldDefinition> GetFieldDefinitionMap(Func<string, string> sanitizeFieldName)
    {
        lock (this.fieldDefLock)
        {
            if (this.fieldDefinitionMap != null && this.fieldNameSanitizer == sanitizeFieldName)
            {
                return this.fieldDefinitionMap;
            }

            this.fieldDefinitionMap = new Dictionary<string, FieldDefinition>(StringComparer.OrdinalIgnoreCase);
            this.fieldNameSanitizer = sanitizeFieldName;
            foreach (var fieldDef in this.FieldDefinitionsArray)
            {
                this.fieldDefinitionMap[sanitizeFieldName(fieldDef.FieldName)] = fieldDef;
            }
            return this.fieldDefinitionMap;
        }
    }

    /// <summary>
    /// Gets or sets the composite primary keys.
    /// </summary>
    /// <value>The composite primary keys.</value>
    public List<CompositePrimaryKeyAttribute> CompositePrimaryKeys { get; set; }

    /// <summary>
    /// Gets or sets the composite indexes.
    /// </summary>
    /// <value>The composite indexes.</value>
    public List<CompositeIndexAttribute> CompositeIndexes { get; set; }

    /// <summary>
    /// Gets or sets the unique constraints.
    /// </summary>
    /// <value>The unique constraints.</value>
    public List<UniqueConstraintAttribute> UniqueConstraints { get; set; }

    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field">The field.</param>
    /// <returns>FieldDefinition.</returns>
    public FieldDefinition GetFieldDefinition<T>(Expression<Func<T, object>> field)
    {
        return this.GetFieldDefinition(ExpressionUtils.GetMemberName(field));
    }

    /// <summary>
    /// Throws the no field exception.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <exception cref="System.NotSupportedException">'{fieldName}' is not a property of '{Name}'</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowNoFieldException(string fieldName)
    {
        throw new NotSupportedException($"'{fieldName}' is not a property of '{this.Name}'");
    }

    /// <summary>
    /// Asserts the field definition.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>FieldDefinition.</returns>
    public FieldDefinition AssertFieldDefinition(string fieldName)
    {
        var fieldDef = this.GetFieldDefinition(fieldName);
        if (fieldDef == null)
        {
            this.ThrowNoFieldException(fieldName);
        }

        return fieldDef;
    }

    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>FieldDefinition.</returns>
    public FieldDefinition GetFieldDefinition(string fieldName)
    {
        if (fieldName != null)
        {
            foreach (var f in this.FieldDefinitionsWithAliases)
            {
                if (f.Alias == fieldName)
                {
                    return f;
                }
            }
            foreach (var f in this.FieldDefinitionsArray)
            {
                if (f.Name == fieldName)
                {
                    return f;
                }
            }
            foreach (var f in this.FieldDefinitionsWithAliases)
            {
                if (string.Equals(f.Alias, fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return f;
                }
            }
            foreach (var f in this.FieldDefinitionsArray)
            {
                if (string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return f;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Asserts the field definition.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="sanitizeFieldName">Name of the sanitize field.</param>
    /// <returns>FieldDefinition.</returns>
    public FieldDefinition AssertFieldDefinition(string fieldName, Func<string, string> sanitizeFieldName)
    {
        var fieldDef = this.GetFieldDefinition(fieldName, sanitizeFieldName);
        if (fieldDef == null)
        {
            this.ThrowNoFieldException(fieldName);
        }

        return fieldDef;
    }

    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="sanitizeFieldName">Name of the sanitize field.</param>
    /// <returns>FieldDefinition.</returns>
    public FieldDefinition GetFieldDefinition(string fieldName, Func<string, string> sanitizeFieldName)
    {
        if (fieldName != null)
        {
            foreach (var f in this.FieldDefinitionsWithAliases)
            {
                if (f.Alias == fieldName || sanitizeFieldName(f.Alias) == fieldName)
                {
                    return f;
                }
            }
            foreach (var f in this.FieldDefinitionsArray)
            {
                if (f.Name == fieldName || sanitizeFieldName(f.Name) == fieldName)
                {
                    return f;
                }
            }
            foreach (var f in this.FieldDefinitionsWithAliases)
            {
                if (string.Equals(f.Alias, fieldName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(sanitizeFieldName(f.Alias), fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return f;
                }
            }
            foreach (var f in this.FieldDefinitionsArray)
            {
                if (string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(sanitizeFieldName(f.Name), fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return f;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the name of the quoted.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>System.String.</returns>
    public string GetQuotedName(string fieldName, IOrmLiteDialectProvider dialectProvider)
    {
        return this.GetFieldDefinition(fieldName)?.GetQuotedName(dialectProvider);
    }

    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>FieldDefinition.</returns>
    public FieldDefinition GetFieldDefinition(Func<string, bool> predicate)
    {
        foreach (var f in this.FieldDefinitionsWithAliases)
        {
            if (predicate(f.Alias))
            {
                return f;
            }
        }
        foreach (var f in this.FieldDefinitionsArray)
        {
            if (predicate(f.Name))
            {
                return f;
            }
        }

        return null;
    }

    /// <summary>
    /// Afters the initialize.
    /// </summary>
    public void AfterInit()
    {
        this.FieldDefinitionsArray = [.. this.FieldDefinitions];
        this.FieldDefinitionsWithAliases = [.. this.FieldDefinitions.Where(x => x.Alias != null)];

        this.IgnoredFieldDefinitionsArray = [.. this.IgnoredFieldDefinitions];

        var allItems = new List<FieldDefinition>(this.FieldDefinitions);
        allItems.AddRange(this.IgnoredFieldDefinitions);
        this.AllFieldDefinitionsArray = [.. allItems];
        this.ReferenceFieldDefinitionsArray = [.. allItems.Where(x => x.IsReference)];
        this.ReferenceFieldNames = new HashSet<string>(this.ReferenceFieldDefinitionsArray.Select(x => x.Name), StringComparer.OrdinalIgnoreCase);

        this.AutoIdFields = [.. this.GetAutoIdFieldDefinitions()];

        OrmLiteConfig.OnModelDefinitionInit?.Invoke(this);
    }

    /// <summary>
    /// Determines whether [is reference field] [the specified field definition].
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if [is reference field] [the specified field definition]; otherwise, <c>false</c>.</returns>
    public bool IsRefField(FieldDefinition fieldDef)
    {
        return fieldDef.Alias != null && this.IsRefField(fieldDef.Alias)
               || this.IsRefField(fieldDef.Name);
    }

    /// <summary>
    /// Determines whether [is reference field] [the specified name].
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if [is reference field] [the specified name]; otherwise, <c>false</c>.</returns>
    private bool IsRefField(string name)
    {
        return this.Alias != null && this.Alias + "Id" == name
               || this.Name + "Id" == name;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return this.Name;
    }

    /// <summary>
    /// Determines whether the specified field name is reference.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns><c>true</c> if the specified field name is reference; otherwise, <c>false</c>.</returns>
    public bool IsReference(string fieldName)
    {
        return this.ReferenceFieldNames.Contains(fieldName);
    }

    /// <summary>
    /// Determines whether [has any references] [the specified field names].
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <returns><c>true</c> if [has any references] [the specified field names]; otherwise, <c>false</c>.</returns>
    public bool HasAnyReferences(IEnumerable<string> fieldNames)
    {
        foreach (var fieldName in fieldNames)
        {
            if (this.ReferenceFieldNames.Contains(fieldName))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Fors the specified model type.
    /// </summary>
    /// <param name="modelType">Type of the model.</param>
    /// <returns>ModelDefinition.</returns>
    public static ModelDefinition For(Type modelType)
    {
        return modelType.GetModelDefinition();
    }
}


/// <summary>
/// Class ModelDefinition.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class ModelDefinition<T>
{
    /// <summary>
    /// The definition
    /// </summary>
    private static ModelDefinition definition;
    /// <summary>
    /// Gets the definition.
    /// </summary>
    /// <value>The definition.</value>
    public static ModelDefinition Definition => definition ??= typeof(T).GetModelDefinition();

    /// <summary>
    /// The primary key name
    /// </summary>
    private static string primaryKeyName;
    /// <summary>
    /// Gets the name of the primary key.
    /// </summary>
    /// <value>The name of the primary key.</value>
    public static string PrimaryKeyName => primaryKeyName ??= Definition.PrimaryKey.FieldName;
}