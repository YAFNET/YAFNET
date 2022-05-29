// ***********************************************************************
// <copyright file="UrnId.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

using ServiceStack.Text;

namespace ServiceStack;

/// <summary>
/// Creates a Unified Resource Name (URN) with the following formats:
/// - urn:{TypeName}:{IdFieldValue}						e.g. urn:UserSession:1
/// - urn:{TypeName}:{IdFieldName}:{IdFieldValue}		e.g. urn:UserSession:UserId:1
/// </summary>
public class UrnId
{
    /// <summary>
    /// The field seperator
    /// </summary>
    private const char FieldSeperator = ':';
    /// <summary>
    /// The field parts seperator
    /// </summary>
    private const char FieldPartsSeperator = '/';
    /// <summary>
    /// Gets the name of the type.
    /// </summary>
    /// <value>The name of the type.</value>
    public string TypeName { get; private set; }
    /// <summary>
    /// Gets the identifier field value.
    /// </summary>
    /// <value>The identifier field value.</value>
    public string IdFieldValue { get; private set; }
    /// <summary>
    /// Gets the name of the identifier field.
    /// </summary>
    /// <value>The name of the identifier field.</value>
    public string IdFieldName { get; private set; }

    /// <summary>
    /// The has no identifier field name
    /// </summary>
    const int HasNoIdFieldName = 3;
    /// <summary>
    /// The has identifier field name
    /// </summary>
    const int HasIdFieldName = 4;

    /// <summary>
    /// Prevents a default instance of the <see cref="UrnId"/> class from being created.
    /// </summary>
    private UrnId() { }

    /// <summary>
    /// Parses the specified urn identifier.
    /// </summary>
    /// <param name="urnId">The urn identifier.</param>
    /// <returns>UrnId.</returns>
    /// <exception cref="System.ArgumentException">Cannot parse invalid urn: '{0}'</exception>
    public static UrnId Parse(string urnId)
    {
        var urnParts = urnId.Split(FieldSeperator);
        if (urnParts.Length == HasNoIdFieldName)
            return new UrnId { TypeName = urnParts[1], IdFieldValue = urnParts[2] };

        if (urnParts.Length == HasIdFieldName)
            return new UrnId { TypeName = urnParts[1], IdFieldName = urnParts[2], IdFieldValue = urnParts[3] };

        throw new ArgumentException("Cannot parse invalid urn: '{0}'", urnId);
    }

    /// <summary>
    /// Creates the specified object type name.
    /// </summary>
    /// <param name="objectTypeName">Name of the object type.</param>
    /// <param name="idFieldValue">The identifier field value.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentException">objectTypeName cannot have the illegal characters: ':' - objectTypeName</exception>
    /// <exception cref="System.ArgumentException">idFieldValue cannot have the illegal characters: ':' - idFieldValue</exception>
    public static string Create(string objectTypeName, string idFieldValue)
    {
        if (objectTypeName.Contains(FieldSeperator.ToString()))
            throw new ArgumentException("objectTypeName cannot have the illegal characters: ':'", nameof(objectTypeName));

        if (idFieldValue.Contains(FieldSeperator.ToString()))
            throw new ArgumentException("idFieldValue cannot have the illegal characters: ':'", nameof(idFieldValue));

        return $"urn:{objectTypeName}:{idFieldValue}";
    }

    /// <summary>
    /// Creates the with parts.
    /// </summary>
    /// <param name="objectTypeName">Name of the object type.</param>
    /// <param name="keyParts">The key parts.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentException">objectTypeName cannot have the illegal characters: ':' - objectTypeName</exception>
    public static string CreateWithParts(string objectTypeName, params string[] keyParts)
    {
        if (objectTypeName.Contains(FieldSeperator.ToString()))
            throw new ArgumentException("objectTypeName cannot have the illegal characters: ':'", nameof(objectTypeName));

        var sb = StringBuilderCache.Allocate();
        foreach (var keyPart in keyParts)
        {
            if (sb.Length > 0)
                sb.Append(FieldPartsSeperator);
            sb.Append(keyPart);
        }

        return $"urn:{objectTypeName}:{StringBuilderCache.ReturnAndFree(sb)}";
    }

    /// <summary>
    /// Creates the with parts.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keyParts">The key parts.</param>
    /// <returns>System.String.</returns>
    public static string CreateWithParts<T>(params string[] keyParts)
    {
        return CreateWithParts(typeof(T).Name, keyParts);
    }

    /// <summary>
    /// Creates the specified identifier field value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="idFieldValue">The identifier field value.</param>
    /// <returns>System.String.</returns>
    public static string Create<T>(string idFieldValue)
    {
        return Create(typeof(T), idFieldValue);
    }

    /// <summary>
    /// Creates the specified identifier field value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="idFieldValue">The identifier field value.</param>
    /// <returns>System.String.</returns>
    public static string Create<T>(object idFieldValue)
    {
        return Create(typeof(T), idFieldValue.ToString());
    }

    /// <summary>
    /// Creates the specified object type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="idFieldValue">The identifier field value.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentException">idFieldValue cannot have the illegal characters: ':' - idFieldValue</exception>
    public static string Create(Type objectType, string idFieldValue)
    {
        if (idFieldValue.Contains(FieldSeperator.ToString()))
            throw new ArgumentException("idFieldValue cannot have the illegal characters: ':'", nameof(idFieldValue));

        return $"urn:{objectType.Name}:{idFieldValue}";
    }

    /// <summary>
    /// Creates the specified identifier field name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="idFieldName">Name of the identifier field.</param>
    /// <param name="idFieldValue">The identifier field value.</param>
    /// <returns>System.String.</returns>
    public static string Create<T>(string idFieldName, string idFieldValue)
    {
        return Create(typeof(T), idFieldName, idFieldValue);
    }

    /// <summary>
    /// Creates the specified object type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="idFieldName">Name of the identifier field.</param>
    /// <param name="idFieldValue">The identifier field value.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentException">idFieldValue cannot have the illegal characters: ':' - idFieldValue</exception>
    /// <exception cref="System.ArgumentException">idFieldName cannot have the illegal characters: ':' - idFieldName</exception>
    public static string Create(Type objectType, string idFieldName, string idFieldValue)
    {
        if (idFieldValue.Contains(FieldSeperator.ToString()))
            throw new ArgumentException("idFieldValue cannot have the illegal characters: ':'", nameof(idFieldValue));

        if (idFieldName.Contains(FieldSeperator.ToString()))
            throw new ArgumentException("idFieldName cannot have the illegal characters: ':'", nameof(idFieldName));

        return $"urn:{objectType.Name}:{idFieldName}:{idFieldValue}";
    }

    /// <summary>
    /// Gets the string identifier.
    /// </summary>
    /// <param name="urn">The urn.</param>
    /// <returns>System.String.</returns>
    public static string GetStringId(string urn)
    {
        return Parse(urn).IdFieldValue;
    }

    /// <summary>
    /// Gets the unique identifier identifier.
    /// </summary>
    /// <param name="urn">The urn.</param>
    /// <returns>Guid.</returns>
    public static Guid GetGuidId(string urn)
    {
        return new Guid(Parse(urn).IdFieldValue);
    }

    /// <summary>
    /// Gets the long identifier.
    /// </summary>
    /// <param name="urn">The urn.</param>
    /// <returns>System.Int64.</returns>
    public static long GetLongId(string urn)
    {
        return long.Parse(Parse(urn).IdFieldValue);
    }
}