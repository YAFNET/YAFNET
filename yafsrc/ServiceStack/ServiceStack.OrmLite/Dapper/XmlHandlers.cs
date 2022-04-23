// ***********************************************************************
// <copyright file="XmlHandlers.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class XmlTypeHandler.
/// Implements the <see cref="ServiceStack.OrmLite.Dapper.SqlMapper.StringTypeHandler{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.Dapper.SqlMapper.StringTypeHandler{T}" />
internal abstract class XmlTypeHandler<T> : SqlMapper.StringTypeHandler<T>
{
    /// <summary>
    /// Assign the value of a parameter before a command executes
    /// </summary>
    /// <param name="parameter">The parameter to configure</param>
    /// <param name="value">Parameter value</param>
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        base.SetValue(parameter, value);
        parameter.DbType = DbType.Xml;
    }
}

/// <summary>
/// Class XmlDocumentHandler. This class cannot be inherited.
/// </summary>
internal sealed class XmlDocumentHandler : XmlTypeHandler<XmlDocument>
{
    /// <summary>
    /// Parse a string into the expected type (the string will never be null)
    /// </summary>
    /// <param name="xml">The string to parse.</param>
    /// <returns>T.</returns>
    protected override XmlDocument Parse(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc;
    }

    /// <summary>
    /// Format an instace into a string (the instance will never be null)
    /// </summary>
    /// <param name="xml">The string to format.</param>
    /// <returns>System.String.</returns>
    protected override string Format(XmlDocument xml) => xml.OuterXml;
}

/// <summary>
/// Class XDocumentHandler. This class cannot be inherited.
/// </summary>
internal sealed class XDocumentHandler : XmlTypeHandler<XDocument>
{
    /// <summary>
    /// Parse a string into the expected type (the string will never be null)
    /// </summary>
    /// <param name="xml">The string to parse.</param>
    /// <returns>T.</returns>
    protected override XDocument Parse(string xml) => XDocument.Parse(xml);
    /// <summary>
    /// Format an instace into a string (the instance will never be null)
    /// </summary>
    /// <param name="xml">The string to format.</param>
    /// <returns>System.String.</returns>
    protected override string Format(XDocument xml) => xml.ToString();
}

/// <summary>
/// Class XElementHandler. This class cannot be inherited.
/// </summary>
internal sealed class XElementHandler : XmlTypeHandler<XElement>
{
    /// <summary>
    /// Parse a string into the expected type (the string will never be null)
    /// </summary>
    /// <param name="xml">The string to parse.</param>
    /// <returns>T.</returns>
    protected override XElement Parse(string xml) => XElement.Parse(xml);
    /// <summary>
    /// Formats the specified XML.
    /// </summary>
    /// <param name="xml">The XML.</param>
    /// <returns>System.String.</returns>
    protected override string Format(XElement xml) => xml.ToString();
}