// ***********************************************************************
// <copyright file="NotesAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System;

namespace ServiceStack;

/// <summary>
/// Document a longer form description about a Type
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class NotesAttribute : AttributeBase
{
    /// <summary>
    /// Get or sets a Label
    /// </summary>
    /// <value>The notes.</value>
    public string Notes { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotesAttribute"/> class.
    /// </summary>
    /// <param name="notes">The notes.</param>
    public NotesAttribute(string notes)
    {
        this.Notes = notes;
    }
}