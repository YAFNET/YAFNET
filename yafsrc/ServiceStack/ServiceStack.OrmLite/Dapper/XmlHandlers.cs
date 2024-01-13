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
    override protected XmlDocument Parse(string xml)
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
    override protected string Format(XmlDocument xml)
    {
        return xml.OuterXml;
    }
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
    override protected XDocument Parse(string xml)
    {
        return XDocument.Parse(xml);
    }

    /// <summary>
    /// Format an instace into a string (the instance will never be null)
    /// </summary>
    /// <param name="xml">The string to format.</param>
    /// <returns>System.String.</returns>
    override protected string Format(XDocument xml)
    {
        return xml.ToString();
    }
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
    override protected XElement Parse(string xml)
    {
        return XElement.Parse(xml);
    }

    /// <summary>
    /// Formats the specified XML.
    /// </summary>
    /// <param name="xml">The XML.</param>
    /// <returns>System.String.</returns>
    override protected string Format(XElement xml)
    {
        return xml.ToString();
    }
}