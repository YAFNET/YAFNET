// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMutId.cs" company="ServiceStack, Inc.">
//   Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>
//   Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceStack.Model;

public interface IMutId<T>
{
    T Id { get; set; }
}

public interface IMutLongId : IMutId<long>
{
}

public interface IMutIntId : IMutId<int>
{
}

public interface IMutStringId : IMutId<string>
{
}