// ***********************************************************************
// <copyright file="JsDelegates.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;

namespace ServiceStack.Text.Common
{
    /// <summary>
    /// Delegate WriteListDelegate
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oList">The o list.</param>
    /// <param name="toStringFn">To string function.</param>
    internal delegate void WriteListDelegate(TextWriter writer, object oList, WriteObjectDelegate toStringFn);

    /// <summary>
    /// Delegate WriteDelegate
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    internal delegate void WriteDelegate(TextWriter writer, object value);

    /// <summary>
    /// Delegate ParseFactoryDelegate
    /// </summary>
    /// <returns>ParseStringSpanDelegate.</returns>
    internal delegate ParseStringSpanDelegate ParseFactoryDelegate();

    /// <summary>
    /// Delegate DeserializeStringSpanDelegate
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="source">The source.</param>
    /// <returns>System.Object.</returns>
    public delegate object DeserializeStringSpanDelegate(Type type, ReadOnlySpan<char> source);

    /// <summary>
    /// Delegate WriteObjectDelegate
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">The object.</param>
    public delegate void WriteObjectDelegate(TextWriter writer, object obj);

    /// <summary>
    /// Delegate ParseStringDelegate
    /// </summary>
    /// <param name="stringValue">The string value.</param>
    /// <returns>System.Object.</returns>
    public delegate object ParseStringDelegate(string stringValue);

    /// <summary>
    /// Delegate ParseStringSpanDelegate
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public delegate object ParseStringSpanDelegate(ReadOnlySpan<char> value);

    /// <summary>
    /// Delegate ConvertObjectDelegate
    /// </summary>
    /// <param name="fromObject">From object.</param>
    /// <returns>System.Object.</returns>
    public delegate object ConvertObjectDelegate(object fromObject);

    /// <summary>
    /// Delegate ConvertInstanceDelegate
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public delegate object ConvertInstanceDelegate(object obj, Type type);

    /// <summary>
    /// Delegate DeserializationErrorDelegate
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="propertyType">Type of the property.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="propertyValueStr">The property value string.</param>
    /// <param name="ex">The ex.</param>
    public delegate void DeserializationErrorDelegate(object instance, Type propertyType, string propertyName, string propertyValueStr, Exception ex);
}
