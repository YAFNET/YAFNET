// ***********************************************************************
// <copyright file="CsvStreamExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;
using System.IO;

namespace ServiceStack.Text;

/// <summary>
/// Class CsvStreamExtensions.
/// </summary>
public static class CsvStreamExtensions
{
    /// <summary>
    /// Writes the CSV.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="outputStream">The output stream.</param>
    /// <param name="records">The records.</param>
    public static void WriteCsv<T>(this Stream outputStream, IEnumerable<T> records)
    {
        using var textWriter = new StreamWriter(outputStream);
        textWriter.WriteCsv(records);
    }

    /// <summary>
    /// Writes the CSV.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer">The writer.</param>
    /// <param name="records">The records.</param>
    public static void WriteCsv<T>(this TextWriter writer, IEnumerable<T> records)
    {
        CsvWriter<T>.Write(writer, records);
    }

}