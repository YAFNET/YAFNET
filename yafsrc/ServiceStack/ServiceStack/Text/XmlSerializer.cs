// ***********************************************************************
// <copyright file="XmlSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if !LITE
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace ServiceStack.Text
{
    /// <summary>
    /// Class XmlSerializer.
    /// </summary>
    public class XmlSerializer
    {
        /// <summary>
        /// The XML writer settings
        /// </summary>
        public static readonly XmlWriterSettings XmlWriterSettings = new();
        /// <summary>
        /// The XML reader settings
        /// </summary>
        public static readonly XmlReaderSettings XmlReaderSettings = new();

        /// <summary>
        /// The instance
        /// </summary>
        public static XmlSerializer Instance = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializer"/> class.
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
        /// Deserializes the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">DeserializeDataContract: Error converting type: " + ex.Message</exception>
        private static object Deserialize(string xml, Type type)
        {
            try
            {
                var stringReader = new StringReader(xml);
                using var reader = XmlReader.Create(stringReader, XmlReaderSettings);
                var serializer = new DataContractSerializer(type);
                return serializer.ReadObject(reader);
            }
            catch (Exception ex)
            {
                throw new SerializationException("DeserializeDataContract: Error converting type: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public static object DeserializeFromString(string xml, Type type)
        {
            return Deserialize(xml, type);
        }

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns>T.</returns>
        public static T DeserializeFromString<T>(string xml)
        {
            var type = typeof(T);
            return (T)Deserialize(xml, type);
        }

        /// <summary>
        /// Deserializes from reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The reader.</param>
        /// <returns>T.</returns>
        public static T DeserializeFromReader<T>(TextReader reader)
        {
            return DeserializeFromString<T>(reader.ReadToEnd());
        }

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>T.</returns>
        public static T DeserializeFromStream<T>(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(T));

            return (T)serializer.ReadObject(stream);
        }

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>System.Object.</returns>
        public static object DeserializeFromStream(Type type, Stream stream)
        {
            var serializer = new DataContractSerializer(type);
            return serializer.ReadObject(stream);
        }

        /// <summary>
        /// Serializes to string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from">From.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">Error serializing object of type {@from.GetType().FullName}</exception>
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
                throw new SerializationException($"Error serializing object of type {@from.GetType().FullName}", ex);
            }
        }

        /// <summary>
        /// Serializes to stream.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="stream">The stream.</param>
        public static void SerializeToStream(object obj, Stream stream)
        {
            if (obj == null) return;
            using var xw = XmlWriter.Create(stream, XmlWriterSettings);
            var serializer = new DataContractSerializer(obj.GetType());
            serializer.WriteObject(xw, obj);
        }
    }
}
#endif