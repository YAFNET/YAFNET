// ***********************************************************************
// <copyright file="SqlMapper.DapperRow.Descriptor.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class SqlMapper.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// Class DapperRow. This class cannot be inherited.
    /// Implements the <see cref="object" />
    /// Implements the <see cref="System.Dynamic.IDynamicMetaObjectProvider" />
    /// </summary>
    /// <seealso cref="object" />
    /// <seealso cref="System.Dynamic.IDynamicMetaObjectProvider" />
    [TypeDescriptionProvider(typeof(DapperRowTypeDescriptionProvider))]
    private sealed partial class DapperRow
    {
        /// <summary>
        /// Class DapperRowTypeDescriptionProvider. This class cannot be inherited.
        /// Implements the <see cref="System.ComponentModel.TypeDescriptionProvider" />
        /// </summary>
        /// <seealso cref="System.ComponentModel.TypeDescriptionProvider" />
        private sealed class DapperRowTypeDescriptionProvider : TypeDescriptionProvider
        {
            /// <summary>
            /// Gets an extended custom type descriptor for the given object.
            /// </summary>
            /// <param name="instance">The object for which to get the extended type descriptor.</param>
            /// <returns>An <see cref="T:System.ComponentModel.ICustomTypeDescriptor" /> that can provide extended metadata for the object.</returns>
            public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
                => new DapperRowTypeDescriptor(instance);
            /// <summary>
            /// Gets a custom type descriptor for the given type and object.
            /// </summary>
            /// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
            /// <param name="instance">An instance of the type. Can be <see langword="null" /> if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor" />.</param>
            /// <returns>An <see cref="T:System.ComponentModel.ICustomTypeDescriptor" /> that can provide metadata for the type.</returns>
            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
                => new DapperRowTypeDescriptor(instance);
        }

        //// in theory we could implement this for zero-length results to bind; would require
        //// additional changes, though, to capture a table even when no rows - so not currently provided
        //internal sealed class DapperRowList : List<DapperRow>, ITypedList
        //{
        //    private readonly DapperTable _table;
        //    public DapperRowList(DapperTable table) { _table = table; }
        //    PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        //    {
        //        if (listAccessors != null && listAccessors.Length != 0) return PropertyDescriptorCollection.Empty;

        //        return DapperRowTypeDescriptor.GetProperties(_table);
        //    }

        //    string ITypedList.GetListName(PropertyDescriptor[] listAccessors) => null;
        //}

        /// <summary>
        /// Class DapperRowTypeDescriptor. This class cannot be inherited.
        /// Implements the <see cref="System.ComponentModel.ICustomTypeDescriptor" />
        /// </summary>
        /// <seealso cref="System.ComponentModel.ICustomTypeDescriptor" />
        private sealed class DapperRowTypeDescriptor : ICustomTypeDescriptor
        {
            /// <summary>
            /// The row
            /// </summary>
            private readonly DapperRow _row;
            /// <summary>
            /// Initializes a new instance of the <see cref="DapperRowTypeDescriptor"/> class.
            /// </summary>
            /// <param name="instance">The instance.</param>
            public DapperRowTypeDescriptor(object instance)
            {
                _row = (DapperRow)instance;
            }

            /// <summary>
            /// Returns a collection of custom attributes for this instance of a component.
            /// </summary>
            /// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> containing the attributes for this object.</returns>
            AttributeCollection ICustomTypeDescriptor.GetAttributes()
                => AttributeCollection.Empty;

            /// <summary>
            /// Returns the class name of this instance of a component.
            /// </summary>
            /// <returns>The class name of the object, or <see langword="null" /> if the class does not have a name.</returns>
            string ICustomTypeDescriptor.GetClassName() => typeof(DapperRow).FullName;

            /// <summary>
            /// Returns the name of this instance of a component.
            /// </summary>
            /// <returns>The name of the object, or <see langword="null" /> if the object does not have a name.</returns>
            string ICustomTypeDescriptor.GetComponentName() => null;

            /// <summary>
            /// The s converter
            /// </summary>
            private static readonly TypeConverter s_converter = new ExpandableObjectConverter();
            /// <summary>
            /// Returns a type converter for this instance of a component.
            /// </summary>
            /// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> that is the converter for this object, or <see langword="null" /> if there is no <see cref="T:System.ComponentModel.TypeConverter" /> for this object.</returns>
            TypeConverter ICustomTypeDescriptor.GetConverter() => s_converter;

            /// <summary>
            /// Returns the default event for this instance of a component.
            /// </summary>
            /// <returns>An <see cref="T:System.ComponentModel.EventDescriptor" /> that represents the default event for this object, or <see langword="null" /> if this object does not have events.</returns>
            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => null;

            /// <summary>
            /// Returns the default property for this instance of a component.
            /// </summary>
            /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the default property for this object, or <see langword="null" /> if this object does not have properties.</returns>
            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => null;

            /// <summary>
            /// Returns an editor of the specified type for this instance of a component.
            /// </summary>
            /// <param name="editorBaseType">A <see cref="T:System.Type" /> that represents the editor for this object.</param>
            /// <returns>An <see cref="T:System.Object" /> of the specified type that is the editor for this object, or <see langword="null" /> if the editor cannot be found.</returns>
            object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => null;

            /// <summary>
            /// Returns the events for this instance of a component.
            /// </summary>
            /// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the events for this component instance.</returns>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => EventDescriptorCollection.Empty;

            /// <summary>
            /// Returns the events for this instance of a component using the specified attribute array as a filter.
            /// </summary>
            /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
            /// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the filtered events for this component instance.</returns>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => EventDescriptorCollection.Empty;

            /// <summary>
            /// Gets the properties.
            /// </summary>
            /// <param name="row">The row.</param>
            /// <returns>PropertyDescriptorCollection.</returns>
            internal static PropertyDescriptorCollection GetProperties(DapperRow row) => GetProperties(row?.table, row);
            /// <summary>
            /// Gets the properties.
            /// </summary>
            /// <param name="table">The table.</param>
            /// <param name="row">The row.</param>
            /// <returns>PropertyDescriptorCollection.</returns>
            internal static PropertyDescriptorCollection GetProperties(DapperTable table, IDictionary<string, object> row = null)
            {
                string[] names = table?.FieldNames;
                if (names == null || names.Length == 0) return PropertyDescriptorCollection.Empty;
                var arr = new PropertyDescriptor[names.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    var type = row != null && row.TryGetValue(names[i], out var value) && value != null
                                   ? value.GetType() : typeof(object);
                    arr[i] = new RowBoundPropertyDescriptor(type, names[i], i);
                }
                return new PropertyDescriptorCollection(arr, true);
            }
            /// <summary>
            /// Returns the properties for this instance of a component.
            /// </summary>
            /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the properties for this component instance.</returns>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => GetProperties(_row);

            /// <summary>
            /// Returns the properties for this instance of a component using the attribute array as a filter.
            /// </summary>
            /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
            /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the filtered properties for this component instance.</returns>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) => GetProperties(_row);

            /// <summary>
            /// Returns an object that contains the property described by the specified property descriptor.
            /// </summary>
            /// <param name="pd">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the property whose owner is to be found.</param>
            /// <returns>An <see cref="T:System.Object" /> that represents the owner of the specified property.</returns>
            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => _row;
        }

        /// <summary>
        /// Class RowBoundPropertyDescriptor. This class cannot be inherited.
        /// Implements the <see cref="System.ComponentModel.PropertyDescriptor" />
        /// </summary>
        /// <seealso cref="System.ComponentModel.PropertyDescriptor" />
        private sealed class RowBoundPropertyDescriptor : PropertyDescriptor
        {
            /// <summary>
            /// The type
            /// </summary>
            private readonly Type _type;
            /// <summary>
            /// The index
            /// </summary>
            private readonly int _index;
            /// <summary>
            /// Initializes a new instance of the <see cref="RowBoundPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="name">The name.</param>
            /// <param name="index">The index.</param>
            public RowBoundPropertyDescriptor(Type type, string name, int index) : base(name, null)
            {
                _type = type;
                _index = index;
            }
            /// <summary>
            /// When overridden in a derived class, returns whether resetting an object changes its value.
            /// </summary>
            /// <param name="component">The component to test for reset capability.</param>
            /// <returns><see langword="true" /> if resetting the component changes its value; otherwise, <see langword="false" />.</returns>
            public override bool CanResetValue(object component) => true;
            /// <summary>
            /// When overridden in a derived class, resets the value for this property of the component to the default value.
            /// </summary>
            /// <param name="component">The component with the property value that is to be reset to the default value.</param>
            public override void ResetValue(object component) => ((DapperRow)component).Remove(_index);
            /// <summary>
            /// When overridden in a derived class, gets a value indicating whether this property is read-only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public override bool IsReadOnly => false;
            /// <summary>
            /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
            /// </summary>
            /// <param name="component">The component with the property to be examined for persistence.</param>
            /// <returns><see langword="true" /> if the property should be persisted; otherwise, <see langword="false" />.</returns>
            public override bool ShouldSerializeValue(object component) => ((DapperRow)component).TryGetValue(_index, out _);
            /// <summary>
            /// When overridden in a derived class, gets the type of the component this property is bound to.
            /// </summary>
            /// <value>The type of the component.</value>
            public override Type ComponentType => typeof(DapperRow);
            /// <summary>
            /// When overridden in a derived class, gets the type of the property.
            /// </summary>
            /// <value>The type of the property.</value>
            public override Type PropertyType => _type;
            /// <summary>
            /// When overridden in a derived class, gets the current value of the property on a component.
            /// </summary>
            /// <param name="component">The component with the property for which to retrieve the value.</param>
            /// <returns>The value of a property for a given component.</returns>
            public override object GetValue(object component)
                => ((DapperRow)component).TryGetValue(_index, out var val) ? val ?? DBNull.Value : DBNull.Value;
            /// <summary>
            /// Sets the value.
            /// </summary>
            /// <param name="component">The component.</param>
            /// <param name="value">The value.</param>
            public override void SetValue(object component, object value)
                => ((DapperRow)component).SetValue(_index, value is DBNull ? null : value);
        }
    }
}