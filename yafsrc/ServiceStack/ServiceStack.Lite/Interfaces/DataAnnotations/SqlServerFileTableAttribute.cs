// ***********************************************************************
// <copyright file="SqlServerFileTableAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Class SqlServerFileTableAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class SqlServerFileTableAttribute : AttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerFileTableAttribute"/> class.
    /// </summary>
    public SqlServerFileTableAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerFileTableAttribute"/> class.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="collateFileName">Name of the collate file.</param>
    public SqlServerFileTableAttribute(string directory, string collateFileName = null)
    {
        FileTableDirectory = directory;
        FileTableCollateFileName = collateFileName;
    }

    /// <summary>
    /// Gets the file table directory.
    /// </summary>
    /// <value>The file table directory.</value>
    public string FileTableDirectory { get; internal set; }

    /// <summary>
    /// Gets the name of the file table collate file.
    /// </summary>
    /// <value>The name of the file table collate file.</value>
    public string FileTableCollateFileName { get; internal set; }
}