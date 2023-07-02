// ***********************************************************************
// <copyright file="UploadFile.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.IO;

namespace ServiceStack;

/// <summary>
/// Class UploadFile.
/// </summary>
public class UploadFile
{
    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    public string FileName { get; set; }
    /// <summary>
    /// Gets or sets the stream.
    /// </summary>
    /// <value>The stream.</value>
    public Stream Stream { get; set; }
    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public string FieldName { get; set; }
    /// <summary>
    /// Gets or sets the type of the content.
    /// </summary>
    /// <value>The type of the content.</value>
    public string ContentType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadFile" /> class.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public UploadFile(Stream stream)
        : this(null, stream, null, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadFile" /> class.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="stream">The stream.</param>
    public UploadFile(string fileName, Stream stream)
        : this(fileName, stream, null, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadFile" /> class.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="fieldName">Name of the field.</param>
    public UploadFile(string fileName, Stream stream, string fieldName)
        : this(fileName, stream, fieldName, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadFile" /> class.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="contentType">Type of the content.</param>
    public UploadFile(string fileName, Stream stream, string fieldName, string contentType)
    {
        FileName = fileName;
        Stream = stream;
        FieldName = fieldName;
        ContentType = contentType;
    }
}