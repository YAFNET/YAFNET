// ***********************************************************************
// <copyright file="TextConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Security.Cryptography;

namespace ServiceStack.Text;

public class TextConfig
{
    public static Func<SHA1> CreateSha { get; set; } = SHA1.Create;
}
