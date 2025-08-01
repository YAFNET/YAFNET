// ***********************************************************************
// <copyright file="FieldDefinition.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections;
using System.Reflection;

/// <summary>
/// Class FieldDefinition.
/// </summary>
public class FieldDefinition
{
    /// <summary>
    /// Gets or sets the model def.
    /// </summary>
    /// <value>The model definition.</value>
    public ModelDefinition ModelDef { get; set; }

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
    /// Gets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public string FieldName => this.Alias ?? this.Name;

    /// <summary>
    /// Gets or sets the type of the field.
    /// </summary>
    /// <value>The type of the field.</value>
    public Type FieldType { get; set; }

    /// <summary>
    /// Gets or sets the field type default value.
    /// </summary>
    /// <value>The field type default value.</value>
    public object FieldTypeDefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the type of the treat as.
    /// </summary>
    /// <value>The type of the treat as.</value>
    public Type TreatAsType { get; set; }

    /// <summary>
    /// Gets the type of the column.
    /// </summary>
    /// <value>The type of the column.</value>
    public Type ColumnType => this.TreatAsType ?? this.FieldType;

    /// <summary>
    /// Gets or sets the property information.
    /// </summary>
    /// <value>The property information.</value>
    public PropertyInfo PropertyInfo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is primary key.
    /// </summary>
    /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic increment].
    /// </summary>
    /// <value><c>true</c> if [automatic increment]; otherwise, <c>false</c>.</value>
    public bool AutoIncrement { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic identifier].
    /// </summary>
    /// <value><c>true</c> if [automatic identifier]; otherwise, <c>false</c>.</value>
    public bool AutoId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is nullable.
    /// </summary>
    /// <value><c>true</c> if this instance is nullable; otherwise, <c>false</c>.</value>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is indexed.
    /// </summary>
    /// <value><c>true</c> if this instance is indexed; otherwise, <c>false</c>.</value>
    public bool IsIndexed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is unique index.
    /// </summary>
    /// <value><c>true</c> if this instance is unique index; otherwise, <c>false</c>.</value>
    public bool IsUniqueIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is clustered.
    /// </summary>
    /// <value><c>true</c> if this instance is clustered; otherwise, <c>false</c>.</value>
    public bool IsClustered { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is non clustered.
    /// </summary>
    /// <value><c>true</c> if this instance is non clustered; otherwise, <c>false</c>.</value>
    public bool IsNonClustered { get; set; }

    /// <summary>
    /// Gets or sets the name of the index.
    /// </summary>
    /// <value>The name of the index.</value>
    public string IndexName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is row version.
    /// </summary>
    /// <value><c>true</c> if this instance is row version; otherwise, <c>false</c>.</value>
    public bool IsRowVersion { get; set; }

    /// <summary>
    /// Gets or sets the length of the field.
    /// </summary>
    /// <value>The length of the field.</value>
    public int? FieldLength { get; set; }  // Precision for Decimal Type

    /// <summary>
    /// Gets or sets the scale.
    /// </summary>
    /// <value>The scale.</value>
    public int? Scale { get; set; }  //  for decimal type

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public string DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the default value constraint.
    /// </summary>
    /// <value>The default value constraint.</value>
    public string DefaultValueConstraint { get; set; }

    /// <summary>
    /// Gets or sets the check constraint.
    /// </summary>
    /// <value>The check constraint.</value>
    public string CheckConstraint { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is unique constraint.
    /// </summary>
    /// <value><c>true</c> if this instance is unique constraint; otherwise, <c>false</c>.</value>
    public bool IsUniqueConstraint { get; set; }

    /// <summary>
    /// Gets or sets the order.
    /// </summary>
    /// <value>The order.</value>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the foreign key.
    /// </summary>
    /// <value>The foreign key.</value>
    public ForeignKeyConstraint ForeignKey { get; set; }

    /// <summary>
    /// Gets or sets the get value function.
    /// </summary>
    /// <value>The get value function.</value>
    public GetMemberDelegate GetValueFn { get; set; }

    /// <summary>
    /// Gets or sets the set value function.
    /// </summary>
    /// <value>The set value function.</value>
    public SetMemberDelegate SetValueFn { get; set; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public object GetValue(object instance)
    {
        var type = instance.GetType();
        if (this.PropertyInfo.DeclaringType?.IsAssignableFrom(type) != true)
        {
            if (instance is IDictionary d)
            {
                return d[this.Name];
            }

            var accessor = TypeProperties.Get(type).GetAccessor(this.Name);
            return accessor?.PublicGetter(instance);
        }

        return this.GetValueFn?.Invoke(instance);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="value">The value.</param>
    public void SetValue(object instance, object value)
    {
        if (instance is IDictionary d)
        {
            d[this.Name] = value;
            return;
        }

        this.SetValueFn?.Invoke(instance, value);
    }

    /// <summary>
    /// Gets the name of the quoted.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>System.String.</returns>
    public string GetQuotedName(IOrmLiteDialectProvider dialectProvider)
    {
        return this.IsRowVersion
                   ? dialectProvider.GetRowVersionSelectColumn(this).ToString()
                   : dialectProvider.GetQuotedColumnName(this.FieldName);
    }

    /// <summary>
    /// Gets the quoted value.
    /// </summary>
    /// <param name="fromInstance">From instance.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>System.String.</returns>
    public string GetQuotedValue(object fromInstance, IOrmLiteDialectProvider dialect = null)
    {
        var value = this.GetValue(fromInstance);
        return (dialect ?? OrmLiteConfig.DialectProvider).GetQuotedValue(value, this.ColumnType);
    }

    /// <summary>
    /// Gets or sets the sequence.
    /// </summary>
    /// <value>The sequence.</value>
    public string Sequence { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is computed.
    /// </summary>
    /// <value><c>true</c> if this instance is computed; otherwise, <c>false</c>.</value>
    public bool IsComputed { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is persisted.
    /// </summary>
    /// <value><c>true</c> if this instance is persisted; otherwise, <c>false</c>.</value>
    public bool IsPersisted { get; set; }

    /// <summary>
    /// Gets or sets the compute expression.
    /// </summary>
    /// <value>The compute expression.</value>
    public string ComputeExpression { get; set; }

    /// <summary>
    /// Gets or sets the custom select.
    /// </summary>
    /// <value>The custom select.</value>
    public string CustomSelect { get; set; }
    /// <summary>
    /// Gets or sets the custom insert.
    /// </summary>
    /// <value>The custom insert.</value>
    public string CustomInsert { get; set; }
    /// <summary>
    /// Gets or sets the custom update.
    /// </summary>
    /// <value>The custom update.</value>
    public string CustomUpdate { get; set; }

    /// <summary>
    /// Gets a value indicating whether [requires alias].
    /// </summary>
    /// <value><c>true</c> if [requires alias]; otherwise, <c>false</c>.</value>
    public bool RequiresAlias => this.Alias != null || this.CustomSelect != null;

    /// <summary>
    /// Gets or sets the name of the belong to model.
    /// </summary>
    /// <value>The name of the belong to model.</value>
    public string BelongToModelName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is reference.
    /// </summary>
    /// <value><c>true</c> if this instance is reference; otherwise, <c>false</c>.</value>
    public bool IsReference { get; set; }

    /// <summary>
    /// Whether the PK for the Reference Table is a field on the same table
    /// </summary>
    public string ReferenceSelfId { get; set; }

    /// <summary>
    /// The PK to use for the Reference Table (e.g. what ReferenceSelfId references)
    /// </summary>
    public string ReferenceRefId { get; set; }

    /// <summary>
    /// References a Field on another Table
    /// [ReferenceField(typeof(Target), nameof(TargetId))]
    /// public TargetFieldType TargetFieldName { get; set; }
    /// </summary>
    public FieldReference FieldReference { get; set; }

    /// <summary>
    /// Gets or sets the custom field definition.
    /// </summary>
    /// <value>The custom field definition.</value>
    public string CustomFieldDefinition { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is reference type.
    /// </summary>
    /// <value><c>true</c> if this instance is reference type; otherwise, <c>false</c>.</value>
    public bool IsRefType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [ignore on update].
    /// </summary>
    /// <value><c>true</c> if [ignore on update]; otherwise, <c>false</c>.</value>
    public bool IgnoreOnUpdate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [ignore on insert].
    /// </summary>
    /// <value><c>true</c> if [ignore on insert]; otherwise, <c>false</c>.</value>
    public bool IgnoreOnInsert { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [return on insert].
    /// </summary>
    /// <value><c>true</c> if [return on insert]; otherwise, <c>false</c>.</value>
    public bool ReturnOnInsert { get; set; }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return this.Name;
    }

    /// <summary>
    /// Shoulds the skip insert.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ShouldSkipInsert()
    {
        return this.IgnoreOnInsert || this.AutoIncrement || this.IsComputed && !this.IsPersisted || this.IsRowVersion;
    }

    /// <summary>
    /// Shoulds the skip update.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ShouldSkipUpdate()
    {
        return this.IgnoreOnUpdate || this.IsComputed && !this.IsPersisted;
    }

    /// <summary>
    /// Shoulds the skip delete.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ShouldSkipDelete()
    {
        return this.IsComputed && !this.IsPersisted;
    }

    /// <summary>
    /// Determines whether [is self reference field] [the specified field definition].
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if [is self reference field] [the specified field definition]; otherwise, <c>false</c>.</returns>
    public bool IsSelfRefField(FieldDefinition fieldDef)
    {
        return fieldDef.Alias != null && this.IsSelfRefField(fieldDef.Alias)
               || this.IsSelfRefField(fieldDef.Name);
    }

    /// <summary>
    /// Determines whether [is self reference field] [the specified name].
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if [is self reference field] [the specified name]; otherwise, <c>false</c>.</returns>
    public bool IsSelfRefField(string name)
    {
        return this.Alias != null && this.Alias + "Id" == name
               || this.Name + "Id" == name;
    }

    /// <summary>
    /// Clones the specified modifier.
    /// </summary>
    /// <param name="modifier">The modifier.</param>
    /// <returns>FieldDefinition.</returns>
    public FieldDefinition Clone(Action<FieldDefinition> modifier = null)
    {
        var fieldDef = new FieldDefinition
        {
            Name = this.Name,
            Alias = this.Alias,
            FieldType = this.FieldType,
            FieldTypeDefaultValue = this.FieldTypeDefaultValue,
            TreatAsType = this.TreatAsType,
            PropertyInfo = this.PropertyInfo,
            IsPrimaryKey = this.IsPrimaryKey,
            AutoIncrement = this.AutoIncrement,
            AutoId = this.AutoId,
            IsNullable = this.IsNullable,
            IsIndexed = this.IsIndexed,
            IsUniqueIndex = this.IsUniqueIndex,
            IsClustered = this.IsClustered,
            IsNonClustered = this.IsNonClustered,
            IsRowVersion = this.IsRowVersion,
            FieldLength = this.FieldLength,
            Scale = this.Scale,
            DefaultValue = this.DefaultValue,
            CheckConstraint = this.CheckConstraint,
            IsUniqueConstraint = this.IsUniqueConstraint,
            ForeignKey = this.ForeignKey,
            GetValueFn = this.GetValueFn,
            SetValueFn = this.SetValueFn,
            Sequence = this.Sequence,
            IsComputed = this.IsComputed,
            IsPersisted = this.IsPersisted,
            ComputeExpression = this.ComputeExpression,
            CustomSelect = this.CustomSelect,
            BelongToModelName = this.BelongToModelName,
            IsReference = this.IsReference,
            ReferenceRefId = this.ReferenceRefId,
            ReferenceSelfId = this.ReferenceSelfId,
            FieldReference = this.FieldReference,
            CustomFieldDefinition = this.CustomFieldDefinition,
            IsRefType = this.IsRefType
        };

        modifier?.Invoke(fieldDef);
        return fieldDef;
    }
}