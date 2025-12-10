// ***********************************************************************
// <copyright file="XmlSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Runtime.Serialization;
using System.Xml;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class XmlSerializer.
/// </summary>
public class XmlSerializer
{
    /// <summary>
    /// The XML writer settings
    /// </summary>
    public readonly static XmlWriterSettings XmlWriterSettings = new();
    /// <summary>
    /// The XML reader settings
    /// </summary>
    public readonly static XmlReaderSettings XmlReaderSettings = new();

    /// <summary>
    /// The instance
    /// </summary>
    public static XmlSerializer Instance { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializer" /> class.
    /// </summary>
    /// <param name="omitXmlDeclaration">if set to <c>true</c> [omit XML declaration].</param>
    /// <param name="maxCharsInDocument">The maximum chars in document.</param>
    public XmlSerializer(bool omitXmlDeclaration = false, int maxCharsInDocument = 1024 * 1024)
    {
        XmlWriterSettings.Encoding = PclExport.Instance.GetUTF8Encoding();
        XmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
        XmlReaderSettings.MaxCharactersInDocument = maxCharsInDocument;

        //Prevent XML bombs by default: https://msdn.microsoft.com/en-us/magazine/ee335713.aspx
        XmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from">From.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.Runtime.Serialization.SerializationException">Error serializing object of type {from.GetType().FullName}</exception>
    public static string SerializeToString<T>(T from)
    {
        try
        {
            using var ms = MemoryStreamFactory.GetStream();
            using var xw = XmlWriter.Create(ms, XmlWriterSettings);
            var serializer = new DataContractSerializer(from.GetType());
            serializer.WriteObject(xw, from);
            xw.Flush();
            return ms.ReadToEnd();
        }
        catch (Exception ex)
        {
            throw new SerializationException($"Error serializing object of type {from.GetType().FullName}", ex);
        }
    }
}