// ***********************************************************************
// <copyright file="IHasModel.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

namespace ServiceStack.Model;

public interface IHasName
{
    string? Name { get; set; }
}